using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    public GameObject pauseBtn;
    public GameObject playBtn;

    private bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        playBtn.SetActive(false);
    }

    public void PausePressed()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;
            isPaused = true;

            pauseBtn.SetActive(false);
            playBtn.SetActive(true);

            pausePanel.SetActive(true);
        }
        Debug.Log("PausePressed!");
    }

    public void ResumePressed()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;

            playBtn.SetActive(false);
            pauseBtn.SetActive(true);

            pausePanel.SetActive(false);
            settingsPanel.SetActive(false);
        }
        Debug.Log("Resume Pressed!");
    }

    public void SettingsPressed()
    {
        settingsPanel.SetActive(true);
        pausePanel.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;

        Debug.Log("Settings Pressed!");
    }

    public void CloseButtonPressed()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
