using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintSystem : MonoBehaviour
{
    [Header("Recipe Database")]
    public AssemblyRecipeData[] assemblyRecipeDatas;

    [Header("UI Setup")]
    public Transform uiParent;

    public GameObject blueprintUIPrefab;

    [Header("Debug")]
    [SerializeField] private List<string> storedIngredients = new List<string>();

    private Dictionary<string, GameObject> activeUI = new Dictionary<string, GameObject>();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Blueprint"))
            return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            UpdateUI(collision.gameObject.name);
            storedIngredients.Add(collision.gameObject.name);
            Destroy(collision.gameObject);
            CheckRecipes();
        }
    }

    private void UpdateUI(string itemName)
    {      
        int count = 1;
        foreach (var name in storedIngredients)
            if (name == itemName) count++;

        Sprite icon = FindIconByName(itemName);

        if (activeUI.ContainsKey(itemName))
        {
            GameObject uiObject = activeUI[itemName];

            Image iconImage = uiObject.GetComponent<Image>();
            TMP_Text quantityText = uiObject.GetComponentInChildren<TMP_Text>();

            if (iconImage != null && icon != null)
                iconImage.sprite = icon;

            if (quantityText != null)
                quantityText.text = $"{count}x";
        }
        else
        {
   
            GameObject newUI = Instantiate(blueprintUIPrefab, uiParent);
            newUI.name = blueprintUIPrefab.name;
            newUI.GetComponent<Image>().sprite = icon;
            TMP_Text quantityText = newUI.GetComponentInChildren<TMP_Text>();

           
              
            quantityText.text = $"{count}x";

            activeUI.Add(itemName, newUI);
        }
    }

    private Sprite FindIconByName(string itemName)
    {
        ItemData item = ItemDataBase.Instance.GetItemByName(itemName);
        return item.materialIcon; 
    }

    private void CheckRecipes()
    {
        foreach (var recipe in assemblyRecipeDatas)
        {
            if (HasAllIngredients(recipe))
            {
                SpawnProduct(recipe);
                storedIngredients.Clear();
                break;
            }
        }
    }

    private bool HasAllIngredients(AssemblyRecipeData recipe)
    {
        Dictionary<string, int> plateCount = new Dictionary<string, int>();
        foreach (var name in storedIngredients)
        {
            if (plateCount.ContainsKey(name))
                plateCount[name]++;
            else
                plateCount[name] = 1;
        }

        Dictionary<string, int> required = new Dictionary<string, int>();
        foreach (var ingredient in recipe.itemDatas)
        {
            required[ingredient.item.materialName] = ingredient.Quantity;
        }

        if (plateCount.Count != required.Count)
            return false;

        foreach (var quantity in required)
        {
            string name = quantity.Key;
            int neededAmount = quantity.Value;

            if (!plateCount.ContainsKey(name))
                return false;

            int currentAmount = plateCount[name];
            if (currentAmount != neededAmount)
                return false;
        }

        return true;
    }

    private void SpawnProduct(AssemblyRecipeData recipe)
    {
        if (recipe.product != null && recipe.product.materialPrefab != null)
        {
            GameObject product = Instantiate(recipe.product.materialPrefab, this.transform.position, Quaternion.identity);
            product.name = recipe.product.materialName;
            Destroy(gameObject);
        }
    }
}