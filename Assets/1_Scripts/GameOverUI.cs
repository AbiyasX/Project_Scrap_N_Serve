using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using NUnit.Framework.Internal;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    public CurrencyManager currencyManager;
    public ShiftManager shiftManager;
    public CustomerOrderManager customerOrderManager;

    [Header("Stats Text")]
    public TextMeshProUGUI totalDaystxt;
    public TextMeshProUGUI totalCogstxt;
    public TextMeshProUGUI totalReptxt;
    public TextMeshProUGUI ordFultxt;
    public TextMeshProUGUI ordFailtxt;

    private void Start()
    {
        DisplayStats();
    }

    void DisplayStats()
    {
        totalDaystxt.text = "" + shiftManager.dayCount.ToString();
        totalCogstxt.text = "" + currencyManager.Cog_currentCurrency.ToString("N0");
        totalReptxt.text = "" + customerOrderManager.currentReputation.ToString();
        ordFultxt.text = "" + customerOrderManager.totalOrdersFulfilled.ToString();
        ordFailtxt.text = "" + customerOrderManager.totalOrdersFailed.ToString();
    }

    public void PlayAgain()
    {
        Debug.Log("Play button clicked!");
        SceneManager.LoadScene("Main_Menu");
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
