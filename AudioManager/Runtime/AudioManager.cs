using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
// ReSharper disable All

/// <summary>
/// AudioManager singleton para gerenciar todo áudio do jogo.
/// Acesso estático: AudioManager.PlayMusic(...), etc.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer; // Defina no Inspector com grupos: Master, Music, SFX, UI

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSourceA; // Defina no Inspector
    [SerializeField] private AudioSource musicSourceB; // Defina no Inspector
    [SerializeField] private AudioSource sfxSource;    // Defina no Inspector
    [SerializeField] private AudioSource uiSource;     // Defina no Inspector

    // Controle do crossfade para música
    private static bool _usingMusicSourceA = true;
    
    #region Singleton Pattern
    // Singleton Instance
    private static AudioManager _instance;

    private void Awake()
    {
        // Configura Singleton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Verifica se as referências foram atribuídas no Inspector
            if (musicSourceA == null || musicSourceB == null || sfxSource == null || uiSource == null)
            {
                Debug.LogError("AudioManager: Algum AudioSource não foi atribuído no Inspector!");
            }

            if (audioMixer == null)
            {
                Debug.LogError("AudioManager: AudioMixer não foi atribuído no Inspector!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    // =========================== PLAY MUSIC ===========================

    /// <summary>
    /// Toca música no canal Music, com opcional crossfade entre músicas.
    /// crossfadeDuration = 0 significa que não haverá crossfade.
    /// </summary>
    public static void PlayMusic(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = true, float crossfadeDuration = 0f)
    {
        if (clip == null) return;

        if (crossfadeDuration <= 0f)
        {
            // Sem crossfade, toca direto
            AudioSource current = _usingMusicSourceA ? _instance.musicSourceA : _instance.musicSourceB;
            AudioSource other = _usingMusicSourceA ? _instance.musicSourceB : _instance.musicSourceA;

            other.Stop();
            current.clip = clip;
            current.volume = volume;
            current.pitch = pitch;
            current.loop = loop;
            current.Play();
        }
        else
        {
            // Com crossfade entre os dois AudioSources
            AudioSource from = _usingMusicSourceA ? _instance.musicSourceA : _instance.musicSourceB;
            AudioSource to = _usingMusicSourceA ? _instance.musicSourceB : _instance.musicSourceA;

            to.clip = clip;
            to.volume = 0f;
            to.pitch = pitch;
            to.loop = loop;
            to.Play();

            _instance.StartCoroutine(_instance.CrossfadeMusic(from, to, volume, crossfadeDuration));

            _usingMusicSourceA = !_usingMusicSourceA;
        }
    }

    private IEnumerator CrossfadeMusic(AudioSource from, AudioSource to, float targetVolume, float duration)
    {
        float time = 0f;
        float fromStartVolume = from.volume;
        float toStartVolume = to.volume;

        while (time < duration)
        {
            float t = time / duration;
            from.volume = Mathf.Lerp(fromStartVolume, 0f, t);
            to.volume = Mathf.Lerp(toStartVolume, targetVolume, t);
            time += Time.deltaTime;
            yield return null;
        }

        from.volume = 0f;
        from.Stop();
        to.volume = targetVolume;
    }

    // =========================== PLAY SFX ===========================

    /// <summary>
    /// Toca efeito sonoro em SFX usando PlayOneShot.
    /// volume e pitch opcionais.
    /// loop não se aplica aqui.
    /// </summary>
    public static void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        if (_instance.sfxSource == null || clip == null) return;

        _instance.sfxSource.pitch = pitch;
        _instance.sfxSource.PlayOneShot(clip, volume);
    }

    // =========================== PLAY UI ===========================

    /// <summary>
    /// Toca áudio de UI usando PlayOneShot.
    /// volume e pitch opcionais.
    /// loop não se aplica aqui.
    /// </summary>
    public static void PlayUI(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (_instance.uiSource == null || clip == null) return;

        _instance.uiSource.pitch = pitch;
        _instance.uiSource.PlayOneShot(clip, volume);
    }

    // =========================== STOP FUNCTIONS ===========================

    public static void StopMusic()
    {
        if (_instance.musicSourceA != null) _instance.musicSourceA.Stop();
        if (_instance.musicSourceB != null) _instance.musicSourceB.Stop();
    }

    public static void StopSFX()
    {
        if (_instance.sfxSource != null) _instance.sfxSource.Stop();
    }

    public static void StopUI()
    {
        if (_instance.uiSource != null) _instance.uiSource.Stop();
    }

    public static void StopAll()
    {
        StopMusic();
        StopSFX();
        StopUI();
    }

    // =========================== PAUSE FUNCTIONS ===========================

    public static void PauseMusic()
    {
        if (_instance.musicSourceA != null && _instance.musicSourceA.isPlaying) _instance.musicSourceA.Pause();
        if (_instance.musicSourceB != null && _instance.musicSourceB.isPlaying) _instance.musicSourceB.Pause();
    }

    public static void PauseSFX()
    {
        if (_instance.sfxSource != null && _instance.sfxSource.isPlaying) _instance.sfxSource.Pause();
    }

    public static void PauseUI()
    {
        if (_instance.uiSource != null && _instance.uiSource.isPlaying) _instance.uiSource.Pause();
    }

    public static void PauseAll()
    {
        PauseMusic();
        PauseSFX();
        PauseUI();
    }

    // =========================== VOLUME SETTERS ===========================

    /// <summary>
    /// Define o volume do grupo Music no AudioMixer (valores de 0 a 1).
    /// </summary>
    public static void SetMusicVolume(float volume)
    {
        if (_instance.audioMixer != null)
            _instance.audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    /// <summary>
    /// Define o volume do grupo SFX no AudioMixer (valores de 0 a 1).
    /// </summary>
    public static void SetSFXVolume(float volume)
    {
        if (_instance.audioMixer != null)
            _instance.audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    /// <summary>
    /// Define o volume do grupo UI no AudioMixer (valores de 0 a 1).
    /// </summary>
    public static void SetUIVolume(float volume)
    {
        if (_instance.audioMixer != null)
            _instance.audioMixer.SetFloat("UIVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    /// <summary>
    /// Define o volume master no AudioMixer (valores de 0 a 1).
    /// </summary>
    public static void SetMasterVolume(float volume)
    {
        if (_instance.audioMixer != null)
            _instance.audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }
}
