using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class CustomerOrderManager : MonoBehaviour
{
    [Header("Order Setup")]
    public List<CustomerData> customers;
    public Transform orderUIParent;
    public GameObject orderUIPrefab;
    public float baseOrderTime = 5f;
    public float orderInterval = 5f;
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
        StartCoroutine(GenerateOrders());
    }

    void Update()
    {
        UpdateOrders();
        CheckDelivery();
    }

    IEnumerator GenerateOrders()
    {
        for (int i = 0; i < maxOrders; i++)
        {
            Debug.Log("Getting Customer by Reputation");

            CustomerData customer = GetCustomerByReputation();
            if (customer == null)
            {
                Debug.LogWarning("No suitable customer found. Skipping...");
                yield return new WaitForSeconds(0.5f); 
                continue;
            }

            ItemData item = customer.possibleOrders[Random.Range(0, customer.possibleOrders.Length)];
            int payment = Random.Range(customer.minPayment, customer.maxPayment + 1);

            CustomerOrder newOrder = new CustomerOrder(
                System.Guid.NewGuid().ToString(),
                item,
                Random.Range(1, 3),
                payment,
                baseOrderTime
            );

            Debug.Log($"Order {i + 1}/{maxOrders} generated");

            activeOrders.Add(newOrder);
            CreateOrderUI(newOrder, customer);

            yield return new WaitForSeconds(orderInterval);
            //yield return new WaitForSeconds(orderInterval - (GameManager.currentDay * 0.2f));
        }

        Debug.Log("All orders generated!");
    }


    void CreateOrderUI(CustomerOrder order, CustomerData customer)
    {
        Debug.Log("Generating orderPrefab");

        GameObject ui = Instantiate(orderUIPrefab, orderUIParent);
        ui.transform.localScale = Vector3.one;

        Image icon = ui.transform.Find("Icon").GetComponentInChildren<Image>();
        Image fill = ui.transform.Find("Icon/IconFill").GetComponent<Image>();
        TMP_Text qtyText = ui.GetComponentInChildren<TMP_Text>();
       
        icon.sprite = order.orderedItem.materialIcon;
        qtyText.text = $"{order.quantity}x {order.orderedItem.materialName}";

        fill.fillAmount = 0f;

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
        Debug.Log($"Player distance: {distance}, Delivery radius: {deliveryRadius}");

        if (distance > deliveryRadius) return;

        if (playerHoldPoint.childCount == 0)
        {
            Debug.LogWarning("No item held — cannot deliver.");
            return;
        }

        GameObject heldItem = playerHoldPoint.GetChild(0).gameObject;
        ItemComponent heldItemComponent = heldItem.GetComponent<ItemComponent>();


        Debug.Log($"Held item: {heldItem.name}");
        Debug.Log($"Held item data: {heldItemComponent?.itemData?.materialName}");
        Debug.Log($"Orders waiting: {activeOrders.Count}");

        foreach (CustomerOrder order in new List<CustomerOrder>(activeOrders))
        {
            if (order.isCompleted) continue;

            Debug.Log($"Comparing held item '{heldItemComponent?.itemData?.materialName}' with order '{order.orderedItem.materialName}'");
            if (order.orderedItem == heldItemComponent.itemData)
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
        currencyManager.AddMoney(order.payment);
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
        order.isCompleted = true;

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
