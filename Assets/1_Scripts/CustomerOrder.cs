using UnityEngine;

[System.Serializable]
public class CustomerOrder
{
    public string orderID;
    public ItemData orderedItem;
    public int quantity;
    public int payment;
    public bool isCompleted;
    public float totalTime;

    public float timeRemaining; // NEW: order timer in seconds

    public CustomerOrder(string id, ItemData item, int qty, int pay, float totalTime)
    {
        orderID = id;
        orderedItem = item;
        quantity = qty;
        payment = pay;
        this.totalTime = totalTime;
        timeRemaining = totalTime; // start full time
        isCompleted = false;
    }
}
