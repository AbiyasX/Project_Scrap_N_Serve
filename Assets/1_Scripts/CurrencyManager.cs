using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private float Cog_currentCurrency;
    [SerializeField] private TextMeshProUGUI cogsUIText;

    public static CurrencyManager instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        UpdateCurrencyUI();
    }
    public bool HasEnough(float amount)
    {
        return Cog_currentCurrency >= amount;
    }

    public void AddMoney(float amount)
    {
        Cog_currentCurrency += amount;
        UpdateCurrencyUI();
    }

    public void SpendMoney(float amount)
    {
        if (Cog_currentCurrency >= amount)
        {
            Cog_currentCurrency -= amount;
            UpdateCurrencyUI();
        }
        else
        {
            Debug.Log("Not enough cogs!");
        }
    }

    public float GetCurrentCurrency()
    {
        return Cog_currentCurrency;
    }
    public void AddMoneyButton(float amount)
    {
        AddMoney(amount); 
    }

    private void UpdateCurrencyUI()
    {
        cogsUIText.text = "" + Cog_currentCurrency.ToString("N0");
    }
}