using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "Items Data/Item Database")]
public class ItemDataBase : ScriptableObject
{
    [Header("All ItemData assets (auto-filled)")]
    [SerializeField] private List<ItemData> allItems = new List<ItemData>();

    private static ItemDataBase instance;

    public static ItemDataBase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ItemDataBase>("ItemDatabase");
                if (instance == null)
                {
                    Debug.LogError("❌ ItemDatabase not found! Please place it under a 'Resources' folder.");
                }
            }
            return instance;
        }
    }

    public List<ItemData> GetAllItems() => allItems;

    public ItemData GetItemByName(string name)
    {
        return allItems.Find(i => i.materialName == name);
    }

#if UNITY_EDITOR
    [ContextMenu("🔄 Refresh Item List")]
    public void RefreshItems()
    {
        allItems.Clear();
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:ItemData");

        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            ItemData item = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (item != null)
                allItems.Add(item);
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"✅ Refreshed ItemDatabase — found {allItems.Count} items.");
    }
#endif
}
