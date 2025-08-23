# AudioManager para Unity

Um sistema de gerenciamento de áudio robusto, centralizado e de fácil utilização para projetos na Unity. Este `AudioManager` utiliza um padrão Singleton para ser acessível de qualquer lugar do código, permitindo tocar músicas, efeitos sonoros (SFX) e sons de interface (UI) com uma única linha de código.

---

## ⚙️ Requisitos

- **Unity 2020.3 LTS** ou superior.
- Conhecimento básico de C# e do editor da Unity.

---

## 🚀 Guia de Instalação

### Passo 1: Importar o AudioManager

- Faça download do repositório, importe para seu projeto no Unity.

### Passo 2: Adicionar o Prefab na Cena

- Na hierarquia da cena inicial do seu projeto (ex: menu principal), arraste o prefab `AudioManager` para a cena.
- O prefab já vem configurado com:
  - AudioMixer configurado e referenciado.
  - AudioSources para música (dois para crossfade), SFX e UI.
  - Todos os componentes vinculados corretamente.

### Passo 3: Usar o AudioManager via Código

- Basta chamar as funções estáticas do `AudioManager` de qualquer script no seu projeto, ex:

// Tocar música com crossfade de 2 segundos
AudioManager.PlayMusic(minhaMusica, crossfadeDuration: 2f);

// Tocar efeito sonoro com volume e pitch padrão
AudioManager.PlaySFX(somExplosao);

// Tocar som de UI
AudioManager.PlayUI(somCliqueBotao);

---

## 🔧 Explicação das Funções Principais

| Função                         | Descrição                                                                                      |
|-------------------------------|------------------------------------------------------------------------------------------------|
| `PlayMusic(AudioClip clip, float volume = 1, float pitch = 1, bool loop = true, float crossfadeDuration = 0)` | Toca música com opção de crossfade suave. Se `crossfadeDuration` for 0, troca imediatamente.  |
| `PlaySFX(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)`                   | Toca efeito sonoro na camada SFX usando `PlayOneShot`. Loop não aplicado.                       |
| `PlayUI(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)`                    | Toca som de interface na camada UI usando `PlayOneShot`. Loop não aplicado.                     |
| `StopMusic()`                                   | Para todas as músicas que estiverem tocando (ambos os sources da música).                      |
| `StopSFX()`                                     | Para o AudioSource de SFX.                                                                      |
| `StopUI()`                                      | Para o AudioSource de UI.                                                                       |
| `StopAll()`                                     | Para todas as fontes de áudio (música, SFX e UI).                                             |
| `PauseMusic()`, `PauseSFX()`, `PauseUI()`, `PauseAll()` | Pausa as respectivas fontes de áudio.                                                        |
| `SetMusicVolume(float volume)`                   | Ajusta o volume do grupo Music do AudioMixer (0 a 1, calculado em decibéis internamente).     |
| `SetSFXVolume(float volume)`                     | Ajusta o volume do grupo SFX do AudioMixer.                                                   |
| `SetUIVolume(float volume)`                      | Ajusta o volume do grupo UI do AudioMixer.                                                    |
| `SetMasterVolume(float volume)`                  | Ajusta o volume master do AudioMixer.                                                         |

---

## 💡 Boas Práticas

- Utilize o crossfade para garantir transições suaves entre as músicas.
- Aproveite o uso do `PlayOneShot` para efeitos sonoros e sons UI, otimizando o desempenho e simplicidade.
- Ajuste volumes por grupos via AudioMixer para que o jogador tenha configuração global fácil.
- Para sons menos comuns ou específicos, você pode passar clipes dinamicamente pelo parâmetro do método.
- Usar o prefab permite integração rápida e reduz erros de configuração em cenas diferentes.

---

Se desejar, posso ajudar a preparar exemplos práticos, scripts para menus de configuração de áudio, ou outras customizações que precisar.

---
