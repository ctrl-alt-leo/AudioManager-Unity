# AudioManager para Unity

Um sistema de gerenciamento de áudio robusto, centralizado e de fácil utilização para projetos na Unity. Este `AudioManager` utiliza um padrão Singleton para ser acessível de qualquer lugar do código, permitindo tocar músicas, efeitos sonoros (SFX) e sons de interface (UI) com uma única linha de código.

## ✨ Principais Funcionalidades

- **Gerenciamento Centralizado**: Controle músicas, SFX e sons de UI em um único lugar.
- **API Estática Simples**: Chame métodos como `AudioManager.PlaySfx("Explosion")` de qualquer script, sem precisar de referências diretas.
- **Padrão Singleton**: Garante que exista apenas uma instância do `AudioManager` que persiste entre as cenas (`DontDestroyOnLoad`).
- **Integração com AudioMixer**: Permite o controle de volume em canais separados (Master, Music, SFX, UI) para criar menus de configurações de áudio facilmente.
- **Configuração via Inspector**: Configure todas as referências e clipes de áudio diretamente no editor da Unity, sem precisar mexer no código para adicionar novos sons.
- **Performance**: Utiliza dicionários para busca de clipes de áudio em tempo O(1), evitando lentidão mesmo com centenas de sons.
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

![Expondo Parâmetros do Mixer](httpsa://i.imgur.com/G5gR8fT.png)

### Passo 2: Adicionar o Script

1.  Crie um novo script C# no seu projeto chamado `AudioManager.cs`.
2.  Copie e cole o código do `AudioManager` fornecido no arquivo.

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
5.  **Adicione seus Clipes de Áudio**:
    -   Expanda as listas `Music Tracks`, `Sfx Clips` e `Ui Clips`.
    -   Defina o tamanho da lista (o número de sons que você quer adicionar).
    -   Para cada entrada, defina um **Name** (um nome único que você usará no código, ex: "PlayerJump") e arraste o **AudioClip** correspondente para o campo `Clip`.

![Configuração no Inspector](httpsa://i.imgur.com/8aVw1aE.png)

Pronto! O `AudioManager` está configurado e pronto para ser usado.

## 🎧 Como Usar

Para tocar um som de qualquer outro script, basta chamar os métodos estáticos do `AudioManager`.

### Tocar Música de Fundo
A música tocará em loop. Se a mesma música já estiver tocando, ela não será reiniciada.
```csharp
void Start()
{
    AudioManager.PlayMusic("MainMenuTheme");
}
```

### Tocar Efeitos Sonoros (SFX)
Ideal para ações no jogo como pulos, tiros, explosões, etc. Vários SFX podem tocar simultaneamente.
```csharp
void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.tag == "Obstacle")
    {
        AudioManager.PlaySfx("Explosion");
    }
}
```

### Tocar Sons de Interface (UI)
Usado para feedback de botões, abas e outras interações de UI.
```csharp
public void OnMyButtonClick()
{
    AudioManager.PlayUI("ButtonClick");
}
```

### Parar a Música
```csharp
public void GoToGameplayScene()
{
    AudioManager.StopMusic();
    SceneManager.LoadScene("Level1");
}
```

### Controlar o Volume
Você pode conectar esses métodos a Sliders em um menu de configurações. O valor de `level` deve ser entre `0.0` (mudo) e `1.0` (máximo).

```csharp
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;

    void Start()
    {
        // Carrega os valores salvos para exibir nos sliders
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
    }
    
    public void OnMasterVolumeChanged(float value)
    {
        AudioManager.SetMasterVolume(value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        AudioManager.SetMusicVolume(value);
    }
}
```

## 📜 Licença

Este projeto está sob a licença MIT. Sinta-se à vontade para usar, modificar e distribuir este código em seus projetos. Veja o arquivo `LICENSE` para mais detalhes.
