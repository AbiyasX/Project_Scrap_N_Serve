using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CustomerOrderManager : MonoBehaviour
{
    [Header("Order Setup")]
    public List<CustomerData> customers;
    public Transform orderUIParent;
    public GameObject orderUIPrefab;
    public float baseOrderTime = 5f;
    public int maxOrders = 5;

    [Header("Player + Delivery")]
    public Transform deliveryZone;
    public float deliveryRadius = 2f;
    public Transform player;
    public Transform playerHoldPoint;

    [Header("Currency & Reputation")]
    public int currentReputation = 30;
    
    public CurrencyManager currencyManager;

    private List<CustomerOrder> activeOrders = new List<CustomerOrder>();
    private Dictionary<CustomerOrder, GameObject> orderUIObjects = new Dictionary<CustomerOrder, GameObject>();

    void Start()
    {
        GenerateOrders();
    }

    void Update()
    {
        UpdateOrders();
        CheckDelivery();
    }

    void GenerateOrders()
    {
        for (int i = 0; i < maxOrders; i++)
        {
            Debug.Log("Getting Customer by Reputation");

            CustomerData customer = GetCustomerByReputation();
            if (customer == null) continue;

            ItemData item = customer.possibleOrders[Random.Range(0, customer.possibleOrders.Length)];
            int payment = Random.Range(customer.minPayment, customer.maxPayment + 1);

            CustomerOrder newOrder = new CustomerOrder(
                System.Guid.NewGuid().ToString(),
                item,
                Random.Range(1, 3),
                payment,
                baseOrderTime
            );

            Debug.Log("Generating Orderss...");

            activeOrders.Add(newOrder);
            CreateOrderUI(newOrder, customer);
        }
    }

    void CreateOrderUI(CustomerOrder order, CustomerData customer)
    {
        Debug.Log("Generating orderPrefab");

        GameObject ui = Instantiate(orderUIPrefab, orderUIParent);
        ui.transform.localScale = Vector3.one;

        Image icon = ui.transform.Find("Icon").GetComponentInChildren<Image>();
        TMP_Text qtyText = ui.GetComponentInChildren<TMP_Text>();
       
        icon.sprite = order.orderedItem.materialIcon;
        qtyText.text = $"{order.quantity}x {order.orderedItem.materialName}";
       

        orderUIObjects.Add(order, ui);

        Debug.Log("OrderPrefab generarted!");
    }

   
    void UpdateOrders()
    {
        foreach (CustomerOrder order in new List<CustomerOrder>(activeOrders))
        {
            if (order.isCompleted) continue;

            order.timeRemaining -= Time.deltaTime;

            if (orderUIObjects.ContainsKey(order))
            {
                var ui = orderUIObjects[order];
               
            }

            if (order.timeRemaining <= 0)
            {
                FailOrder(order);
            }
        }
    }

    void CheckDelivery()
    {
        
        float distance = Vector3.Distance(player.position, deliveryZone.position);
        if (distance > deliveryRadius) return;

       
        if (playerHoldPoint.childCount == 0)
        {
            Debug.LogWarning(" You are not holding any item to deliver!");
            return;
        }

        GameObject heldItem = playerHoldPoint.GetChild(0).gameObject;

        foreach (CustomerOrder order in new List<CustomerOrder>(activeOrders))
        {
            if (order.isCompleted) continue;

           
            if (order.orderedItem.materialPrefab.name == heldItem.name.Replace("(Clone)", "").Trim())
            {
                CompleteOrder(order);
                Destroy(heldItem); 
                Debug.Log($"Delivered {order.orderedItem.materialName} successfully!");
                return; 
            }
        }

        Debug.LogWarning("Wrong item! No matching order found for what you're holding.");
    }

    void CompleteOrder(CustomerOrder order)
    {
        order.isCompleted = true;
        Debug.Log("Add Currency; Add Reputaion");
        currentReputation += 3;

        Destroy(orderUIObjects[order]);
        orderUIObjects.Remove(order);
        activeOrders.Remove(order);

        Debug.Log($"Completed Order: {order.orderedItem.materialName}! +{order.payment} Cogs | +Reputation");
    }

    void FailOrder(CustomerOrder order)
    {
        currentReputation -= 5;
        Destroy(orderUIObjects[order]);
        orderUIObjects.Remove(order);
        activeOrders.Remove(order);

        Debug.Log($"Order failed: {order.orderedItem.materialName}. Reputation -5");
    }

    CustomerData GetCustomerByReputation()
    {
        Debug.Log($"Checking customers... Reputation = {currentReputation}");
        foreach (var c in customers)
        {
            Debug.Log($"Customer: {c.name} | Min: {c.reputationRequiredMin}, Max: {c.reputationRequiredMax}");
        }

        List<CustomerData> possible = customers.FindAll(c =>
            currentReputation >= c.reputationRequiredMin &&
            currentReputation <= c.reputationRequiredMax);

        if (possible.Count == 0) return null;
        return possible[Random.Range(0, possible.Count)];
    }
}
