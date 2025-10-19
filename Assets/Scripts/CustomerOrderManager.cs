using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CustomerOrderManager : MonoBehaviour
{
    [Header("Customer Types")]
    public CustomerData lowRepCustomer;
    public CustomerData midRepCustomer;
    public CustomerData highRepCustomer;

    [Header("Settings")]
    public int maxOrders = 5;
    public float orderInterval = 15f;
    private float timer;

    [Header("References")]
    public Transform orderUIParent;
    public GameObject orderUIPrefab;
    public TMP_Text debugText;
    public PlayerInventory playerInventory; // reference to your inventory script

    private List<CustomerOrder> activeOrders = new List<CustomerOrder>();
    private float playerReputation = 50f;
    private int playerMoney = 0;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= orderInterval && activeOrders.Count < maxOrders)
        {
            GenerateNewOrder();
            timer = 0f;
        }
    }

    private void GenerateNewOrder()
    {
        CustomerData customer = GetCustomerTypeByReputation();
        if (customer == null || customer.possibleOrders.Length == 0) return;

        ItemData item = customer.possibleOrders[Random.Range(0, customer.possibleOrders.Length)];
        int payment = Random.Range(customer.minPayment, customer.maxPayment + 1);

        CustomerOrder newOrder = new CustomerOrder(
            System.Guid.NewGuid().ToString(),
            item,
            Random.Range(1, 3),
            payment
        );

        activeOrders.Add(newOrder);
        CreateOrderUI(newOrder);

        Debug.Log($"New {customer.customerTier} order: {item.itemName} x{newOrder.quantity} for {payment} cogs.");
    }

    private CustomerData GetCustomerTypeByReputation()
    {
        if (playerReputation >= 80)
            return highRepCustomer;
        else if (playerReputation >= 41)
            return midRepCustomer;
        else
            return lowRepCustomer;
    }

    private void CreateOrderUI(CustomerOrder order)
    {
        GameObject uiObj = Instantiate(orderUIPrefab, orderUIParent);
        TMP_Text[] texts = uiObj.GetComponentsInChildren<TMP_Text>();
        Button deliverButton = uiObj.GetComponentInChildren<Button>();

        if (texts.Length >= 2)
        {
            texts[0].text = order.requestedItem.itemName;
            texts[1].text = $"x{order.quantity} | 💰 {order.payment}";
        }

        deliverButton.onClick.AddListener(() => TryDeliverOrder(order, uiObj));
    }

    private void TryDeliverOrder(CustomerOrder order, GameObject uiObj)
    {
        // Check if player has enough items
        if (playerInventory.HasItem(order.requestedItem, order.quantity))
        {
            // Remove items from inventory
            playerInventory.RemoveItem(order.requestedItem, order.quantity);

            // Give rewards
            playerMoney += order.payment;
            playerReputation += 5; // reputation boost
            order.isCompleted = true;

            // Clean up
            Destroy(uiObj);
            activeOrders.Remove(order);

            Debug.Log($"Delivered {order.requestedItem.itemName}! Earned {order.payment} cogs and +5 reputation.");
        }
        else
        {
            Debug.Log($"Not enough {order.requestedItem.itemName} to deliver!");
        }
    }
}
