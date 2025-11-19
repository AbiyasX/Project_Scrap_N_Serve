using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject gameOverPanel;

    private bool isPaused = false;

    public void PausePressed()
    {
        Time.timeScale = 0f;         
        isPaused = true;
    }

    public void ResumePressed()
    {
        Time.timeScale = 1f;         
        isPaused = false;
    }

    public void SettingsPressed()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;         
        isPaused = true;
    }

    public void ClosePressed()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;         
        isPaused = false;
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;         
    }
}
