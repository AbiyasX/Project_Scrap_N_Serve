using TMPro;
using UnityEngine;

public class factoryLotScript : MonoBehaviour, Iinteract
{
    [SerializeField] private float facilityHealth = 100f;
    [SerializeField] private float facilityMaxHealth = 100f;
    [SerializeField] private GameObject buyTextUI;

    [Header("Option References")]
    [SerializeField] private GameObject optionTextUI;

    [SerializeField] public TextMeshProUGUI healthtext;
    [SerializeField] public TextMeshProUGUI upgradeText;
    [SerializeField] public TextMeshProUGUI repairText;
    [SerializeField] bool isFullyUpgrade = false;
    private bool playerNearby = false;
    private Renderer rend;
    private bool purchased = false;
    [SerializeField] private FacilityShopManager shopManager;

    private FacilitiesData builtFacility;
    private float currentUpgradeCost;
    int currentLevel = 1;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (buyTextUI != null)
            buyTextUI.SetActive(false);
    }

    private void Update()
    {
        if (purchased)
        {
            UpdateHealthUI();
            buyTextUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            rend.material.EnableKeyword("_EMISSION");
            if (!purchased)
            {
                buyTextUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            buyTextUI.SetActive(false);
            optionTextUI.SetActive(false);
            rend.material.DisableKeyword("_EMISSION");
        }
    }

    public void Interact()
    {
        if (playerNearby)
        {
            if (!purchased && shopManager != null)
            {
                shopManager.OpenShop(this);
                Debug.Log("Opened Facility Shop for this lot");
            }
            else if (purchased)
            {
                optionTextUI.SetActive(true);
                FacilityFunction();
            }
        }
    }

    public void isPurchaseLot(bool isActive, FacilitiesData facility = null)
    {
        purchased = isActive;
       
        if (purchased && facility != null)
        {
            currentLevel = 1;
            builtFacility = facility;
            currentUpgradeCost = builtFacility.facilityUpgrade;
            FacilityFunction();
        }
    }

    private void FacilityFunction()
    {
        repairText.text = $"Repair: {builtFacility.facilityRepairCost}";
        UpdateHealthUI();
    }

    public void Repair()
    {
        float repairCost = builtFacility.facilityRepairCost;

        if (facilityHealth >= facilityMaxHealth)
        {
            Debug.Log("Facility is already fully repaired!");
        }
        else if (CurrencyManager.instance.HasEnough(repairCost))
        {
            CurrencyManager.instance.SpendMoney(repairCost);
            facilityHealth = facilityMaxHealth;
            UpdateHealthUI();
            Debug.Log("Facility repaired successfully!");
        }
        else
        {
            //Add UI text <<
            Debug.Log("Not enough currency to repair the facility!");
        }
    }

    public void Upgrade()
    {
        if (currentLevel < 3)
        {
            if (CurrencyManager.instance.HasEnough(currentUpgradeCost))
            {
                CurrencyManager.instance.SpendMoney(currentUpgradeCost);

                currentLevel++;
                currentUpgradeCost *= 1.8f;

                if (currentLevel >= 3)
                {
                    isFullyUpgrade = true;
                    
                }
                UpdateHealthUI();
            }
            else
            {
                // Add UI Text Here <<
                Debug.Log("Not enough currency to upgrade!");
            }
        }
        else
        {
            isFullyUpgrade = true;
            Debug.Log("Facility is already fully upgraded!");
        }
    }

    public void addHealth(float amount)
    {
        facilityHealth = Mathf.Min(facilityHealth + amount, facilityMaxHealth);
        UpdateHealthUI();
    }
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    public void minusHealth(float amount)
    {
        facilityHealth = Mathf.Max(facilityHealth - amount, 0);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        upgradeText.text = isFullyUpgrade?"Max Upgraded" : $"Upgrade: {Mathf.RoundToInt(currentUpgradeCost)}";
        healthtext.text = $"Durability: {Mathf.RoundToInt(facilityHealth)}%";
    }
}