using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkstationShopManager : MonoBehaviour
{
    [Header("Shop Setup")]
    public WorkstationsData[] workstations;
    public Transform shopUIParent;
    public GameObject shopButtonPrefab;
    public GameObject shopUI;

    [Header("Currency")]
    public CurrencyManager currencyManager;
    private WorkstationSlotScript currentWorkshopSlot;

    public static WorkstationShopManager instance;

    private void Awake()
    {    
        instance = this;

        foreach (var workstation in workstations) 
        {
            workstation.isPurchased = false;
        }
    }

    public void WorkstationOpenShop(WorkstationSlotScript space)
    {
        currentWorkshopSlot = space;
        shopUI.SetActive(true);
        PopulateShop();
    }

    private void PopulateShop()
    {
        foreach (Transform child in shopUIParent)
        {
            Destroy(child.gameObject);
        }

        foreach (WorkstationsData workstation in workstations)
        {
            GameObject buttonObj = Instantiate(shopButtonPrefab, shopUIParent);
            TextMeshProUGUI[] texts = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();
            buttonObj.transform.Find("WorkshopIcon").GetComponent<Image>().sprite = workstation.workstationIcon;
         
            if (texts.Length >= 2)
            {
                texts[0].text = workstation.workstationName;
                texts[1].text = workstation.isPurchased ? "Purchased" : $"Cost: {workstation.workstationCost:N0}";
            }

            Button button = buttonObj.GetComponent<Button>();
            button.interactable = !workstation.isPurchased;

            WorkstationsData localWorkstation = workstation;
            Button localButton = button;
            TextMeshProUGUI costText = texts.Length >= 2 ? texts[1] : null;

            button.onClick.AddListener(() =>
            {
                BuyWorkstation(localWorkstation, localButton, costText);
            });
        }

        Debug.Log($"Opening shop... UI: {shopUI}, Parent: {shopUIParent}, Prefab: {shopButtonPrefab}");

    }

    private void BuyWorkstation(WorkstationsData workstation, Button button, TextMeshProUGUI costText)
    {
        if (currencyManager.HasEnough(workstation.workstationCost))
        {
            currencyManager.SpendMoney(workstation.workstationCost);
            currentWorkshopSlot.isPurchaseLot(true, workstation);
            workstation.isPurchased = true;
            button.interactable = false;
            costText.text = "Purchased";
            shopUI.SetActive(false);

            
            GameObject newStation = Instantiate(workstation.workstationModel, currentWorkshopSlot.transform.position, currentWorkshopSlot.transform.rotation);
            newStation.transform.SetParent(currentWorkshopSlot.transform);

            Debug.Log($"{workstation.workstationName} purchased successfully!");
        }
        else
        {
            Debug.Log("Not enough cogs to buy this workstation!");
        }
    }


    public void CloseShop()
    {
        shopUI.SetActive(false);
    }
}
