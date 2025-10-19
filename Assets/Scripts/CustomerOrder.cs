using UnityEngine;

[System.Serializable]
public class CustomerOrder
{
    public string orderID;
    public ItemData requestedItem;
    public int quantity;
    public int payment;
    public bool isCompleted;

    public float timeRemaining; // NEW: order timer in seconds

    public CustomerOrder(string id, ItemData item, int qty, int pay, float time)
    {
        orderID = id;
        requestedItem = item;
        quantity = qty;
        payment = pay;
        timeRemaining = time;
        isCompleted = false;
    }
}
