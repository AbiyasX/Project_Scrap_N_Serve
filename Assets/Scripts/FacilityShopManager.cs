using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FacilityShopManager : MonoBehaviour
{
    [Header("Shop Setup")]
    public FacilitiesData[] facilities;
    public Transform shopUIParent;
    public GameObject shopButtonPrefab;
    public GameObject shopUI;
    [Header("Currency")]
    public CurrencyManager currencyManager;
    public OrderMaterialSystem orderMenuManager;
    private factoryLotScript currentLot;

    public void OpenShop(factoryLotScript lot)
    {
        currentLot = lot;

        shopUI.SetActive(true);
        PopulateShop();
    }

    private void PopulateShop()
    {
        foreach (Transform child in shopUIParent)
        {
            Destroy(child.gameObject);
        }

        foreach (FacilitiesData facility in facilities)
        {
            GameObject buttonObj = Instantiate(shopButtonPrefab, shopUIParent);

            TextMeshProUGUI[] texts = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 2)
            {
                texts[0].text = facility.facilityName;
                texts[1].text = facility.isPurchased ? "Purchased" : $"Cost: {facility.facilityCost:N0}";
            }
       
            Button button = buttonObj.GetComponent<Button>();
            
            button.interactable = !facility.isPurchased;

            Button localButton = button;
            TextMeshProUGUI costText = texts.Length >= 2 ? texts[1] : null;
            FacilitiesData localFacility = facility;

            button.onClick.AddListener(() =>
            {
                BuyFacility(localFacility, localButton, costText);
            });
            
        }
    }

    private void BuyFacility(FacilitiesData facility, Button button, TextMeshProUGUI costText)
    {
       
        if (currencyManager.HasEnough(facility.facilityCost))
        {
            orderMenuManager.AddMaterialToMenu(facility.productionMaterial);
            currentLot.isPurchaseLot(true, facility);
            currencyManager.SpendMoney(facility.facilityCost);
            facility.isPurchased = true;
            button.interactable = false;
            costText.text = "Purchased";
            shopUI.SetActive(false);
            Vector3 spawnPos = currentLot.transform.position + new Vector3(0, -1f, 0);
            GameObject factory = Instantiate(facility.facilityModel, spawnPos, Quaternion.identity);
            factory.transform.SetParent(currentLot.transform);
            
        }
        else
        {
            Debug.Log("Not enough cogs!");
        }
    }
}
