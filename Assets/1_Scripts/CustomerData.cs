using UnityEngine;

public enum CustomerTier
{
    Low,
    Mid,
    High
}

[CreateAssetMenu(fileName = "CustomerData", menuName = "Economy/Customer Data")]
public class CustomerData : ScriptableObject
{
    [Header("Customer Tier Settings")]
    public CustomerTier customerTier;
    public int minPayment;
    public int maxPayment;

    [Header("Reputation Requirements")]
    public int reputationRequiredMin;
    public int reputationRequiredMax;

    [Header("Possible Order Items")]
    public AssemblyRecipeData[] possibleOrders;
}
