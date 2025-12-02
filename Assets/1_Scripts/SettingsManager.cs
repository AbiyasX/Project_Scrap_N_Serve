using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public SoundSettings soundSettings;

    [Header("UI Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject closeButton;

    void Start()
    {
        masterSlider.value = soundSettings.masterVolume;
        musicSlider.value = soundSettings.musicVolume;
        sfxSlider.value = soundSettings.sfxVolume;

        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }

    void OnMasterChanged(float value)
    {
        soundSettings.masterVolume = value;
    }

    void OnMusicChanged(float value)
    {
        soundSettings.musicVolume = value;
    }

    void OnSFXChanged(float value)
    {
        soundSettings.sfxVolume = value;
    }

    public void SettingsButtonClicked()
    {
        settingsPanel.SetActive(true);
    }

    public void OnApplicationPause()
    {
        pausePanel.SetActive(true);
    }

    public void CloseButtonClicked()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(false);
    }
}
