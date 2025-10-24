using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerOrderManager : MonoBehaviour
{
    [Header("Order Setup")]
    public List<CustomerData> customers;

    public Transform orderUIParent;
    public GameObject orderUIPrefab;
    public GameObject requiredItemUIPrefab;
    public float baseOrderTime = 10f;
    public float orderInterval = 5f;
    public int completedOrders = 0;

    private bool canGenerateOrders = true;
    private Coroutine generateRoutine;

    [Header("Delivery")]
    public Transform deliveryZone;

    [Header("Currency & Reputation")]
    public int currentReputation = 30;
    public int dayEarnings = 0;

    [Header("UI References")]
    [SerializeField] TMP_Text dayEarningText;

    public CurrencyManager currencyManager;
    public ShiftManager shiftManager;

    private List<CustomerOrder> activeOrders = new List<CustomerOrder>();
    private Dictionary<CustomerOrder, GameObject> orderUIObjects = new Dictionary<CustomerOrder, GameObject>();

    public static CustomerOrderManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (shiftManager != null && !shiftManager.isNight)
        {
            generateRoutine = StartCoroutine(GenerateOrders());
        }
    }

    private void Update()
    {
        if (shiftManager != null && shiftManager.isNight)
            return;

        UpdateOrders();
    }

    private IEnumerator GenerateOrders()
    {
        while (!shiftManager.isNight)
        {
            if (shiftManager.isNight)
            {
                Debug.Log("Night detected — stopping order generation.");
                yield break;
            }

            while (!canGenerateOrders)
                yield return null;

            CustomerData customer = GetCustomerByReputation();
            if (customer == null)
            {
                Debug.LogWarning("No suitable customer found. Skipping...");
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            AssemblyRecipeData item = customer.possibleOrders[Random.Range(0, customer.possibleOrders.Length)];
            int payment = Random.Range(customer.minPayment, customer.maxPayment + 1);

            CustomerOrder newOrder = new CustomerOrder(
                System.Guid.NewGuid().ToString(),
                item.product,
                Random.Range(1, 3),
                payment,
                baseOrderTime
            );

            activeOrders.Add(newOrder);
            CreateOrderUI(newOrder, customer);

            yield return new WaitForSeconds(orderInterval);
        }

        Debug.Log("All orders generated!");
    }

    private void CreateOrderUI(CustomerOrder order, CustomerData customer)
    {
        Debug.Log("Generating Order UI...");

        GameObject ui = Instantiate(orderUIPrefab, orderUIParent);
        ui.transform.localScale = Vector3.one;

        // Safely find main UI components
        Image[] allImages = ui.GetComponentsInChildren<Image>(true);
        Slider orderMeter = ui.GetComponentInChildren<Slider>(true);

        // Try to detect item icon — the first Image not part of required items
        Image icon = null;
        foreach (var img in allImages)
        {
            if (img.transform.name.ToLower().Contains("itemicon"))
            {
                icon = img;
                break;
            }
        }

        // Try to find ingredient parent
        Transform reqUIParent = ui.transform.Find("RecipeRequirmentUI");

        icon.sprite = order.orderedItem.materialIcon;
        //qtyText.text = $"{order.quantity}x {order.orderedItem.materialName}";
        orderMeter.value = 0f;

        // Spawn ingredient icons (bonus tip)
        AssemblyRecipeData recipeData = FindRecipeForProduct(order.orderedItem);

        foreach (var ingredient in recipeData.itemDatas)
        {
            GameObject reqIcon = Instantiate(requiredItemUIPrefab, reqUIParent);
            Image reqImg = reqIcon.transform.Find("RequirdItemImage").GetComponent<Image>();
            TMP_Text reqText = reqIcon.GetComponentInChildren<TMP_Text>();

            reqImg.sprite = ingredient.item.materialIcon;

            reqText.text = "x" + ingredient.Quantity;
        }

        order.timeRemaining = order.totalTime;
        orderUIObjects.Add(order, ui);

        Debug.Log("Order UI generated successfully!");
    }

    private void UpdateOrders()
    {
        foreach (CustomerOrder order in new List<CustomerOrder>(activeOrders))
        {
            if (order.isCompleted) continue;

            order.timeRemaining -= Time.deltaTime;

            if (orderUIObjects.ContainsKey(order))
            {
                var ui = orderUIObjects[order];
                Slider orderMeter = ui.transform.Find("OrderMeter").GetComponent<Slider>();

                if (order.totalTime > 0f)
                {
                    float progress = Mathf.Clamp01((order.totalTime - order.timeRemaining) / order.totalTime);
                    orderMeter.value = Mathf.Lerp(orderMeter.value, progress, Time.deltaTime * 5f);
                }
                else
                {
                    orderMeter.value = 1f;
                }
            }

            if (order.timeRemaining <= 0)
            {
                FailOrder(order);
            }
        }
    }

    public void CheckDelivery(GameObject item)
    {
<<<<<<< Updated upstream
=======
        float distance = Vector3.Distance(player.position, deliveryZone.position);
        if (distance > deliveryRadius) return;


        if (playerHoldPoint.childCount == 0)
        {
            Debug.LogWarning("No item held — cannot deliver.");
            return;
        }

        GameObject heldItem = playerHoldPoint.GetChild(0).gameObject;
        ItemComponent heldItemComponent = heldItem.GetComponent<ItemComponent>();

>>>>>>> Stashed changes
        foreach (CustomerOrder order in new List<CustomerOrder>(activeOrders))
        {
            if (order.isCompleted) continue;

<<<<<<< Updated upstream
            if (order.orderedItem.materialName == item.name)
=======
            if (order.orderedItem == null)
            {
                Debug.LogError("Order has NO orderedItem assigned!");
                continue;
            }

            if (order.orderedItem == heldItemComponent.itemData)
>>>>>>> Stashed changes
            {
                CompleteOrder(order);
                Destroy(item);
                return;
            }
        }

        Debug.LogWarning("Wrong item! No matching order found.");
    }

    private void CompleteOrder(CustomerOrder order)
    {
        order.isCompleted = true;
        currencyManager.AddMoney(order.payment);

        dayEarnings += order.payment;
        if (dayEarningText != null)
            dayEarningText.text = $"Day Earnings: {dayEarnings}";

        currentReputation += 3;
        completedOrders++;

        Destroy(orderUIObjects[order]);
        orderUIObjects.Remove(order);
        activeOrders.Remove(order);

        Debug.Log($"Completed Order: {order.orderedItem.materialName}! +{order.payment} Cogs | +Reputation");
    }

    private void FailOrder(CustomerOrder order)
    {
        currentReputation -= 5;
        Destroy(orderUIObjects[order]);
        orderUIObjects.Remove(order);
        activeOrders.Remove(order);
        order.isCompleted = true;

        Debug.Log($"Order failed: {order.orderedItem.materialName}. Reputation -5");
    }

    private CustomerData GetCustomerByReputation()
    {
        List<CustomerData> possible = customers.FindAll(c =>
            currentReputation >= c.reputationRequiredMin &&
            currentReputation <= c.reputationRequiredMax);

        if (possible.Count == 0) return null;
        return possible[Random.Range(0, possible.Count)];
    }

    private AssemblyRecipeData FindRecipeForProduct(ItemData product)
    {
        foreach (CustomerData c in customers)
        {
            foreach (var recipe in c.possibleOrders)
            {
                if (recipe.product == product)
                    return recipe;
            }
        }
        return null;
    }

    public void StopOrders()
    {
        canGenerateOrders = false;

        if (generateRoutine != null)
            StopCoroutine(generateRoutine);

        foreach (Transform child in orderUIParent)
            Destroy(child.gameObject);

        activeOrders.Clear();
        orderUIObjects.Clear();

        Debug.Log("Orders stopped for the night.");
    }

    public void ResumeOrders()
    {
        if (generateRoutine != null)
            StopCoroutine(generateRoutine);

        canGenerateOrders = true;
        completedOrders = 0;

        Debug.Log("[OrderManager] Orders resumed for the new day.");
        generateRoutine = StartCoroutine(GenerateOrders());
    }
}