using System.Collections.Generic;
using UnityEngine;
using bob; 

public class Crafter : MonoBehaviour
{
    [System.Serializable] //Used for listing
    public class Recipe
    {
        public GameObject itemA;
        public GameObject itemB;
        public GameObject result;
    }

    [Header("Crafting Settings")]
    public List<Recipe> recipes = new List<Recipe>(); //holds current recipe list. 
    public GameObject trashPrefab;
    public Transform outputPoint;

    private List<GameObject> itemsInZone = new List<GameObject>();

    private void OnTriggerEnter(Collider other) 
    {
        if (other.GetComponent<ItemPickup>() != null && !itemsInZone.Contains(other.gameObject))
        {
            itemsInZone.Add(other.gameObject);
            CheckForCraft();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (itemsInZone.Contains(other.gameObject))
        {
            itemsInZone.Remove(other.gameObject);
        }
    }

    private void CheckForCraft() //Crafting process
    {
        if (itemsInZone.Count < 2) return;

        GameObject item1 = itemsInZone[0];
        GameObject item2 = itemsInZone[1];

        GameObject result = GetCraftResult(item1, item2);

        Destroy(item1);
        Destroy(item2);
        itemsInZone.Clear();

        Instantiate(result, outputPoint.position, Quaternion.identity);
    }

    private GameObject GetCraftResult(GameObject item1, GameObject item2)
    {
        foreach (var recipe in recipes)
        {
            if ((IsSamePrefab(item1, recipe.itemA) && IsSamePrefab(item2, recipe.itemB)) ||
                (IsSamePrefab(item1, recipe.itemB) && IsSamePrefab(item2, recipe.itemA)))
            {
                return recipe.result;
            }
        }

        return trashPrefab;
    }

    private bool IsSamePrefab(GameObject obj, GameObject prefab)
    {
        string objName = obj.name.Replace("(Clone)", "").Trim();
        string prefabName = prefab.name.Replace("(Clone)", "").Trim();
        return objName == prefabName;
    }
}
