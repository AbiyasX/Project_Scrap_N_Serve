using UnityEngine;

public class AdminScript : MonoBehaviour
{
    [Header("Player Currency")]
    [SerializeField] float addMoney;
    [Header("Facility Modifier")]
    [Range(0,100)]
    [SerializeField] float damageFacility;
    [Range(0, 100)]
    [SerializeField] float repairFacility;

    [ContextMenu("Apply Admin Changes")]
    private void ApplyChanges()
    {
        var currency = GetComponent<CurrencyManager>();
        if (currency != null)
            currency.AddMoney(addMoney);

        var allLots = FindObjectsByType<factoryLotScript>(FindObjectsSortMode.None);

        foreach (var lot in allLots)
        {
            lot.minusHealth(damageFacility);
            lot.addHealth(repairFacility);
        }

        addMoney = 0;
        damageFacility = 0;
        repairFacility = 0;
    }
}
