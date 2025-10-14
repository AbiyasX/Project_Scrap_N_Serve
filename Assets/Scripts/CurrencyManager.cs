using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private float currentCurrency;
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
        return currentCurrency >= amount;
    }

    public void AddMoney(float amount)
    {
        currentCurrency += amount;
        UpdateCurrencyUI();
    }

    public void SpendMoney(float amount)
    {
        if (currentCurrency >= amount)
        {
            currentCurrency -= amount;
            UpdateCurrencyUI();
        }
        else
        {
            Debug.Log("Not enough cogs!");
        }
    }

    public float GetCurrentCurrency()
    {
        return currentCurrency;
    }
    public void AddMoneyButton(float amount)
    {
        AddMoney(amount); 
    }

    private void UpdateCurrencyUI()
    {
        cogsUIText.text = "Cogs: " + currentCurrency.ToString("N0");
    }
}