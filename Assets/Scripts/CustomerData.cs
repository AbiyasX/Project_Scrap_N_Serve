using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "Economy/Customer Data")]
public class CustomerData : ScriptableObject
{
    [Header("Customer Tier Settings")]
    public string customerTier; // "Low", "Mid", "High"
    public int minPayment;
    public int maxPayment;

    [Header("Reputation Requirements")]
    public int reputationRequiredMin;
    public int reputationRequiredMax;

    [Header("Possible Order Items")]
    public ItemData[] possibleOrders; // Items the customer can request
}
