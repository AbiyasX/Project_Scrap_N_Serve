using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblySystem : MonoBehaviour
{
    public static AssemblySystem Instance; 

    [Header("Recipes")]
    public AssemblyRecipeData[] productRecipes;

    [Header("Spawn Settings")]
    public Transform productSpawnPoint;

    [Header("Detection Settings")]
    public string itemLayerName = "Item";
    [SerializeField]
    private List<GameObject> itemsOnTable = new List<GameObject>();
    private int itemLayer;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        itemLayer = LayerMask.NameToLayer(itemLayerName); 
    }

    private static GameObject GetItemRoot(Collider other)
    {
        if (other != null && other.attachedRigidbody != null)
            return other.attachedRigidbody.gameObject;

        return other != null ? other.transform.root.gameObject : null;
    }

    private static string CleanName(GameObject go)
    {
        return go == null ? "" : go.name.Replace("(Clone)", "").Trim();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other == null || other.gameObject.layer != itemLayer) return;

        var root = GetItemRoot(other);
        if (root == null) return;

        if (!itemsOnTable.Contains(root))
        {
            itemsOnTable.Add(root);
            CheckForRecipeMatch();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null || other.gameObject.layer != itemLayer) return;

        var root = GetItemRoot(other);
        if (root == null) return;

        var rb = root.GetComponent<Rigidbody>();
        if (rb != null && rb.isKinematic) return;

        if (itemsOnTable.Remove(root))
        {
        }
    }

    private Dictionary<string, int> BuildCounts()
    {
        var counts = new Dictionary<string, int>();
        for (int i = itemsOnTable.Count - 1; i >= 0; i--)
        {
            if (itemsOnTable[i] == null) itemsOnTable.RemoveAt(i);
        }

        foreach (var go in itemsOnTable)
        {
            var name = CleanName(go);
            if (string.IsNullOrEmpty(name)) continue;
            counts.TryGetValue(name, out int c);
            counts[name] = c + 1;
        }
        return counts;
    }

    private void CheckForRecipeMatch()
    {
        var counts = BuildCounts();
        if (counts.Count == 0) return;

        AssemblyRecipeData bestRecipe = null;
        int bestScore = 0;

        foreach (var recipe in productRecipes)
        {
            int score = GetRecipeMatchScore(recipe, counts);
            if (score > 0 && HasAllIngredients(recipe, counts))
            {
                if (score > bestScore)
                {
                    bestScore = score;
                    bestRecipe = recipe;
                }
            }
        }

        if (bestRecipe != null)
            StartCoroutine(ProduceProduct(bestRecipe));
    }

    private int GetRecipeMatchScore(AssemblyRecipeData recipe, Dictionary<string, int> counts)
    {
        int score = 0;
        foreach (var entry in recipe.itemDatas)
        {
            var key = entry.item.materialName;
            if (counts.TryGetValue(key, out int have))
                score += Mathf.Min(have, entry.Quantity);
        }
        return score;
    }

    private bool HasAllIngredients(AssemblyRecipeData recipe, Dictionary<string, int> counts)
    {
        foreach (var entry in recipe.itemDatas)
        {
            var key = entry.item.materialName;
            if (!counts.TryGetValue(key, out int have) || have < entry.Quantity)
                return false;
        }
        return true;
    }

    private IEnumerator ProduceProduct(AssemblyRecipeData recipe)
    {
        foreach (var entry in recipe.itemDatas)
        {
            int needed = entry.Quantity;

            for (int i = itemsOnTable.Count - 1; i >= 0 && needed > 0; i--)
            {
                var go = itemsOnTable[i];
                if (go == null) { itemsOnTable.RemoveAt(i); continue; }

                if (CleanName(go).Equals(entry.item.materialName, System.StringComparison.OrdinalIgnoreCase))
                {
                    itemsOnTable.RemoveAt(i);
                    Destroy(go);
                    needed--;
                }
            }
        }

        yield return null;
        GameObject Product = Instantiate(recipe.product.materialPrefab, productSpawnPoint.position, Quaternion.identity);
        Product.name = recipe.product.materialName;
        yield return null;
        CheckForRecipeMatch();
    }

    public void RemoveItemManually(GameObject pickedUp)
    {
        if (pickedUp == null) return;

        var root = pickedUp.GetComponent<Rigidbody>() != null
            ? pickedUp.GetComponent<Rigidbody>().gameObject
            : pickedUp.transform.root.gameObject;

        if (itemsOnTable.Remove(root))
        {
        }
        else
        {
            string key = CleanName(pickedUp);
            int removed = itemsOnTable.RemoveAll(go => CleanName(go).Equals(key, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}