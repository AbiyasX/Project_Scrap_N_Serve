using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "Economy/Customer Data")]
public class CustomerData : ScriptableObject
{
    public string customerTier; // Low, Mid, High
    public int minPayment;
    public int maxPayment;

    [Header("Possible Order Items")]
    public ItemData[] possibleOrders; // Items the customer can request
}
