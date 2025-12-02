using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public SoundSettings soundSettings;
    public AudioMixer mixer;

    void Start()
    {
        ApplySettings();
    }

    public void ApplySettings()
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(soundSettings.masterVolume) * 20);
        mixer.SetFloat("MusicVolume", Mathf.Log10(soundSettings.musicVolume) * 20);
        mixer.SetFloat("SFXVolume", Mathf.Log10(soundSettings.sfxVolume) * 20);
    }
}
