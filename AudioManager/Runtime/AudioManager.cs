using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    // Instância estática para o Singleton
    private static AudioManager _instance;

    // Mixer e Audio Sources
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    // Listas de sons para fácil acesso no Inspector
    [SerializeField] private List<Sound> musicTracks;
    [SerializeField] private List<Sound> sfxClips;
    [SerializeField] private List<Sound> uiClips;

    // Dicionários para acesso rápido aos clipes pelo nome
    private Dictionary<string, AudioClip> musicDict;
    private Dictionary<string, AudioClip> sfxDict;
    private Dictionary<string, AudioClip> uiDict;

    private void Awake()
    {
        // Implementação do Singleton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Converte as listas em dicionários para busca otimizada O(1)
        InitializeDictionaries();

        // Carrega as configurações de volume salvas (implementação omitida para brevidade)
        // LoadVolumeValues(); 
    }

    private void InitializeDictionaries()
    {
        musicDict = new Dictionary<string, AudioClip>();
        foreach (var track in musicTracks) musicDict[track.name] = track.clip;

        sfxDict = new Dictionary<string, AudioClip>();
        foreach (var sfx in sfxClips) sfxDict[sfx.name] = sfx.clip;

        uiDict = new Dictionary<string, AudioClip>();
        foreach (var uiSound in uiClips) uiDict[uiSound.name] = uiSound.clip;
    }

    // --- MÉTODOS ESTÁTICOS PÚBLICOS (WRAPPERS) ---
    // A partir de agora, você chamará estes métodos de outros scripts.

    public static void PlayMusic(string clipName)
    {
        if (_instance == null) return;
        _instance.PlayMusic_Internal(clipName);
    }

    public static void PlaySfx(string clipName)
    {
        if (_instance == null) return;
        _instance.PlaySfx_Internal(clipName);
    }

    public static void PlayUI(string clipName)
    {
        if (_instance == null) return;
        _instance.PlayUI_Internal(clipName);
    }

    public static void StopMusic()
    {
        if (_instance == null) return;
        _instance.StopMusic_Internal();
    }
    
    // --- MÉTODOS PÚBLICOS ESTÁTICOS PARA CONTROLE DE VOLUME ---

    public static void SetMasterVolume(float level)
    {
        if (_instance == null) return;
        _instance.SetMasterVolume_Internal(level);
    }

    public static void SetMusicVolume(float level)
    {
        if (_instance == null) return;
        _instance.SetMusicVolume_Internal(level);
    }

    public static void SetSfxVolume(float level)
    {
        if (_instance == null) return;
        _instance.SetSfxVolume_Internal(level);
    }

    public static void SetUIVolume(float level)
    {
        if (_instance == null) return;
        _instance.SetUIVolume_Internal(level);
    }


    // --- IMPLEMENTAÇÃO INTERNA (MÉTODOS DE INSTÂNCIA) ---
    // Estes métodos contêm a lógica real e são chamados pelos wrappers estáticos.

    private void PlayMusic_Internal(string clipName)
    {
        if (musicDict.TryGetValue(clipName, out var clip))
        {
            if (musicSource.clip == clip && musicSource.isPlaying)
                return; // Não reinicia a música se já estiver tocando

            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Música '{clipName}' não encontrada!");
        }
    }

    private void PlaySfx_Internal(string clipName)
    {
        if (sfxDict.TryGetValue(clipName, out var clip))
            // PlayOneShot permite tocar múltiplos sons sobrepostos, ideal para SFX
            sfxSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"SFX '{clipName}' não encontrado!");
    }

    private void PlayUI_Internal(string clipName)
    {
        if (uiDict.TryGetValue(clipName, out var clip))
            uiSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"Som de UI '{clipName}' não encontrado!");
    }

    private void StopMusic_Internal()
    {
        musicSource.Stop();
    }
    
    private void SetMasterVolume_Internal(float level)
    {
        // O volume do Mixer é em decibéis (logarítmico), então convertemos
        // Garante que o level não seja zero para evitar -Infinity no Log10
        float dbVolume = level > 0.001f ? Mathf.Log10(level) * 20 : -80f;
        mainMixer.SetFloat("MasterVolume", dbVolume);
        PlayerPrefs.SetFloat("MasterVolume", level);
    }

    private void SetMusicVolume_Internal(float level)
    {
        float dbVolume = level > 0.001f ? Mathf.Log10(level) * 20 : -80f;
        mainMixer.SetFloat("MusicVolume", dbVolume);
        PlayerPrefs.SetFloat("MusicVolume", level);
    }

    private void SetSfxVolume_Internal(float level)
    {
        float dbVolume = level > 0.001f ? Mathf.Log10(level) * 20 : -80f;
        mainMixer.SetFloat("SFXVolume", dbVolume);
        PlayerPrefs.SetFloat("SFXVolume", level);
    }

    private void SetUIVolume_Internal(float level)
    {
        float dbVolume = level > 0.001f ? Mathf.Log10(level) * 20 : -80f;
        mainMixer.SetFloat("UIVolume", dbVolume);
        PlayerPrefs.SetFloat("UIVolume", level);
    }
}