using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Play button clicked!");
        SceneManager.LoadScene("WorkScene");
    }

    public void ExitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit(); //For Built version of game 

        // Stops Playmode in Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
