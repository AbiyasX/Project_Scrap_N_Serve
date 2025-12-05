using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; 

    public SoundSettings soundSettings;
    public AudioMixer mixer;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource; 

    private void Awake()
    {
        // Singleton Setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ApplySettings();
    }

    public float ToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }

    public void ApplySettings()
    {
        if (mixer == null) return;

        mixer.SetFloat("MasterVolume", ToDecibel(soundSettings.masterVolume));
        mixer.SetFloat("MusicVolume", ToDecibel(soundSettings.musicVolume));
        mixer.SetFloat("SFXVolume", ToDecibel(soundSettings.sfxVolume));
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PauseMusic()
    {

        if (mixer != null)
        {
            mixer.SetFloat("MusicVolume", ToDecibel(0.0001f));
        }
    }

    public void ResumeMusic()
    {
        ApplySettings();
    }
}