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

    public float ToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }

    public void ApplySettings()
    {
        mixer.SetFloat("MasterVolume", ToDecibel(soundSettings.masterVolume));
        mixer.SetFloat("MusicVolume", ToDecibel(soundSettings.musicVolume));
        mixer.SetFloat("SFXVolume", ToDecibel(soundSettings.sfxVolume));
    }

}
