# AudioManager para Unity

Um sistema de gerenciamento de áudio robusto, centralizado e de fácil utilização para projetos na Unity. Este `AudioManager` utiliza um padrão Singleton para ser acessível de qualquer lugar do código, permitindo tocar músicas, efeitos sonoros (SFX) e sons de interface (UI) com uma única linha de código.

## ✨ Principais Funcionalidades

- **Gerenciamento Centralizado**: Controle músicas, SFX e sons de UI em um único lugar.
- **API Estática Simples**: Chame métodos como `AudioManager.PlaySfx("Explosion")` de qualquer script, sem precisar de referências diretas.
- **API Flexível**: Suporta tocar tanto sons pré-configurados (por nome) quanto sons dinâmicos (passando o `AudioClip` diretamente como parâmetro).
- **Controle de Volume por Chamada**: Opção de ajustar o volume de um efeito sonoro específico no momento em que ele é tocado.
- **Padrão Singleton**: Garante que exista apenas uma instância do `AudioManager` que persiste entre as cenas (`DontDestroyOnLoad`).
- **Integração com AudioMixer**: Permite o controle de volume em canais separados (Master, Music, SFX, UI) para criar menus de configurações de áudio facilmente.
- **Configuração via Inspector**: Configure todas as referências e clipes de áudio globais diretamente no editor da Unity.
- **Performance**: Utiliza dicionários para busca de clipes de áudio pré-configurados em tempo O(1).
- **Persistência de Volume**: Salva as configurações de volume do jogador entre as sessões de jogo usando `PlayerPrefs`.

## ⚙️ Requisitos

- **Unity 2020.3 LTS** ou superior.
- Conhecimento básico de C# e do editor da Unity.

## 🚀 Guia de Instalação e Configuração

Siga estes passos para integrar o `AudioManager` ao seu projeto.

### Passo 1: Criar o AudioMixer

O `AudioMixer` é essencial para controlar os diferentes canais de áudio.

1.  Na janela **Project**, clique com o botão direito e vá em **Create > Audio Mixer**. Dê um nome, por exemplo, `MainMixer`.
2.  Abra a janela do **Audio Mixer** (`Window > Audio > Audio Mixer`) e selecione o `MainMixer`.
3.  Crie os grupos de áudio. No grupo "Master", clique no ícone `+` para adicionar grupos filhos. Crie três grupos: `Music`, `SFX` e `UI`.
4.  **Exponha os parâmetros de volume**:
    -   Selecione o grupo **Master**. No **Inspector**, clique com o botão direito em **Volume** e selecione "Expose 'Volume' to script".
    -   Renomeie o parâmetro exposto para `MasterVolume` na janela do Audio Mixer (canto superior direito).
    -   Repita o processo para os grupos `Music`, `SFX` e `UI`, nomeando os parâmetros como `MusicVolume`, `SFXVolume` e `UIVolume`, respectivamente.

![Expondo Parâmetros do Mixer](https://i.imgur.com/qe4yhbD.png)

### Passo 2: Adicionar o Script

1.  Crie um novo script C# no seu projeto chamado `AudioManager.cs`.
2.  Copie e cole o código-fonte do `AudioManager` mais recente no arquivo.

### Passo 3: Configurar o GameObject na Cena

O `AudioManager` precisa existir na sua cena inicial (ex: a cena do menu principal) para ser carregado e persistir.

1.  Crie um novo `GameObject` vazio na sua cena e nomeie-o como `AudioManager`.
2.  Arraste o script `AudioManager.cs` para este `GameObject`.
3.  Crie três `GameObject`s filhos do `AudioManager` e nomeie-os como `MusicSource`, `SFXSource` e `UISource`.
4.  Adicione um componente **Audio Source** a cada um desses três filhos.
    -   **Importante**: Na `MusicSource`, habilite a opção `Loop`. Nos outros, deixe desabilitada.

Sua hierarquia deve ficar assim:
```
- AudioManager (com o script AudioManager.cs)
  - MusicSource (com o componente AudioSource)
  - SFXSource (com o componente AudioSource)
  - UISource (com o componente AudioSource)
```

### Passo 4: Conectar as Referências no Inspector

1.  Selecione o `GameObject` **AudioManager**. Você verá os campos públicos do script no Inspector.
2.  **Main Mixer**: Arraste o seu asset `MainMixer` para este campo.
3.  **Sources**: Arraste os `GameObject`s `MusicSource`, `SFXSource` e `UISource` para seus respectivos campos.
4.  **Conecte os Audio Sources aos Grupos do Mixer**:
    -   Selecione o `MusicSource`. No componente Audio Source, clique no campo `Output` e escolha o grupo `Music` do seu `MainMixer`.
    -   Faça o mesmo para `SFXSource` (conectando ao grupo `SFX`) e `UISource` (conectando ao grupo `UI`).
5.  **(Opcional) Adicione seus Clipes de Áudio Globais**:
    -   Expanda as listas `Music Tracks`, `Sfx Clips` e `Ui Clips`.
    -   Defina o tamanho da lista (o número de sons que você quer adicionar).
    -   Para cada entrada, defina um **Name** (um nome único que você usará no código, ex: "PlayerJump") e arraste o **AudioClip** correspondente para o campo `Clip`.

![Configuração no Inspector](https://i.imgur.com/qe4yhbD.png)

Pronto! O `AudioManager` está configurado e pronto para ser usado.

## 🎧 Como Usar

O `AudioManager` agora oferece dois métodos principais para tocar sons, oferecendo máxima flexibilidade.

### Método 1: Tocar Sons Pré-Configurados (por Nome)

Esta é a abordagem ideal para sons globais e frequentemente usados, como música de fundo, cliques de botões e sons genéricos de feedback.

**Exemplo:**
```csharp
void Start()
{
    // Toca a música tema do menu, que foi adicionada à lista no Inspector
    AudioManager.PlayMusic("MainMenuTheme"); 
}

public void OnAnyButtonClick()
{
    // Toca um som de clique padrão da UI
    AudioManager.PlayUI("ButtonClick");
}
```

### Método 2: Tocar um `AudioClip` Diretamente (Método Flexível)

Esta abordagem é perfeita para sons que são específicos de um objeto, prefab, ou que precisam de um volume dinâmico. Você não precisa adicionar o som à lista no Inspector do `AudioManager`.

Basta passar a referência do `AudioClip` e, opcionalmente, um `volumeScale` (de `0.0` a `1.0`).

**Exemplo em um script `PlayerController.cs`:**
```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Arraste os clipes de áudio aqui pelo Inspector
    public AudioClip jumpSound;
    public AudioClip footstepSound;

    void Update()
    {
        // Toca o som de pulo com volume máximo (padrão 1.0)
        if (Input.GetButtonDown("Jump"))
        {
            AudioManager.PlaySfx(jumpSound);
        }

        // Toca o som de passo com 40% do volume para ser mais sutil
        if (IsWalking()) // Supondo que IsWalking() seja sua lógica de movimento
        {
            // (Nota: você precisaria de um timer para não tocar a cada frame)
            AudioManager.PlaySfx(footstepSound, 0.4f);
        }
    }
}
```

### Qual Método Usar? (Boas Práticas)

Use uma abordagem híbrida para obter o melhor dos dois mundos:

-   ✅ **Use o método por Nome (Listas no Inspector)** para:
    -   Músicas de fundo.
    -   Sons de UI genéricos (cliques, hovers).
    -   Efeitos sonoros muito comuns e globais (ex: som de dano do jogador).

-   ✅ **Use o método por `AudioClip` direto** para:
    -   Sons específicos de um prefab (tiro de uma arma, morte de um inimigo).
    -   Sons que precisam ter seu volume ajustado dinamicamente.
    -   Itens colecionáveis com sons únicos.
    -   Qualquer som que não precise ser acessado globalmente por um nome fixo.

### Controlar o Volume Global

O controle de volume dos canais do Mixer continua o mesmo e afeta todos os sons que passam por aquele canal, independentemente de como foram tocados.

```csharp
// Exemplo para um slider de volume em um menu de opções
public void OnMasterVolumeChanged(float value)
{
    // 'value' deve ser de 0.0 a 1.0
    AudioManager.SetMasterVolume(value);
}

public void OnMusicVolumeChanged(float value)
{
    AudioManager.SetMusicVolume(value);
}
```

## 📜 Licença

Este projeto está sob a licença MIT. Sinta-se à vontade para usar, modificar e distribuir este código em seus projetos.
