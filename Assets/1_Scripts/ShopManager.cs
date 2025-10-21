using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject FacilityShopUI;

    public static ShopManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void UI_FacilityShop(bool isActive)
    {
        FacilityShopUI.SetActive(isActive);
    }
}
