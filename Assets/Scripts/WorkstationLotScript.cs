using TMPro;
using UnityEngine;

public class WorkstationLotScript : MonoBehaviour, Iinteract
{
    [SerializeField] private GameObject buyTextUI;

    private bool playerNearby = false;
    private Renderer rend;
    private bool purchased = false;
    [SerializeField] private WorkstationShopManager workstationShopManager;

    private FacilitiesData builtFacility;
   
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
            rend.material.DisableKeyword("_EMISSION");
        }
    }

    public void Interact()
    {
        if (playerNearby)
        {
            if (!purchased && workstationShopManager != null)
            {
                workstationShopManager.WorkstationOpenShop(this);
                Debug.Log("Opened Workstation Shop for this space");
            }
            else if (purchased)
            {
                Debug.Log("Item already purchased");
            }
        }
    }

    public void isPurchaseLot(bool isActive, FacilitiesData facility = null)
    {
        purchased = isActive;

        if (purchased && facility != null)
        {
            builtFacility = facility;
        }
    }

}