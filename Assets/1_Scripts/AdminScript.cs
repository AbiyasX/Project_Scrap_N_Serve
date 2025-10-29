using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AdminScript : MonoBehaviour
{
    [Header("Player Currency")]
    [SerializeField] private float addMoney;

    [Header("Facility Modifier")]
    [Range(0, 100)]
    [SerializeField] private float damageFacility;

    [Range(0, 100)]
    [SerializeField] private float repairFacility;

    [HorizontalGroup("Admin")]
    [Button(ButtonSizes.Large)]
    private void ApplyChanges()
    {
        var currency = GetComponent<CurrencyManager>();
        if (currency != null)
            currency.AddMoney(addMoney);

        var allLots = FindObjectsByType<FacilitySlotScript>(FindObjectsSortMode.None);

        foreach (var lot in allLots)
        {
            lot.minusHealth(damageFacility);
            lot.addHealth(repairFacility);
        }

        addMoney = 0;
        damageFacility = 0;
        repairFacility = 0;
    }

    [FoldoutGroup("Item Datas", expanded: false)]
    [FolderPath(AbsolutePath = false)]
    public string processedMaterialPath = "Assets/ScriptableObjects/Processed Material";

    [FoldoutGroup("Item Datas")]
    [FolderPath(AbsolutePath = false)]
    public string rawMaterialPath = "Assets/ScriptableObjects/raw materials";

    [FoldoutGroup("Item Datas")]
    [FolderPath(AbsolutePath = false)]
    public string tier1Path = "Assets/ScriptableObjects/Tier 1 Items";

    [FoldoutGroup("Item Datas")]  
    [FolderPath(AbsolutePath = false)]
    public string tier2Path = "Assets/ScriptableObjects/Tier 2 Items";

    [FoldoutGroup("Item Datas")]
    [FolderPath(AbsolutePath = false)]
    public string tier3Path = "Assets/ScriptableObjects/Tier 3 Items";

    [FoldoutGroup("Item Datas")]
    [ListDrawerSettings(ShowFoldout = true)]
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
    public ItemData[] processedMaterials;

    [FoldoutGroup("Item Datas")]
    [ListDrawerSettings(ShowFoldout = true)]
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
    public ItemData[] rawMaterials;

    [FoldoutGroup("Item Datas")]
    [ListDrawerSettings(ShowFoldout = true)]
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
    public ItemData[] tier1Items;

    [FoldoutGroup("Item Datas")]
    [ListDrawerSettings(ShowFoldout = true)]
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
    public ItemData[] tier2Items;

    [FoldoutGroup("Item Datas")]
    [ListDrawerSettings(ShowFoldout = true)]
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
    public ItemData[] tier3Items;

    [FoldoutGroup("Item Datas")]
    [Button(ButtonSizes.Large)]
    private void AutoAssignAll()
    {
#if UNITY_EDITOR
        processedMaterials = LoadFromFolder(processedMaterialPath);
        rawMaterials = LoadFromFolder(rawMaterialPath);
        tier1Items = LoadFromFolder(tier1Path);
        tier2Items = LoadFromFolder(tier2Path);
        tier3Items = LoadFromFolder(tier3Path);

        Debug.Log("✅ Auto-assigned all blueprints from folders!");
#endif
    }

#if UNITY_EDITOR

    private ItemData[] LoadFromFolder(string path)
    {
        return AssetDatabase.FindAssets("t:ItemData", new[] { path })
            .Select(guid => AssetDatabase.LoadAssetAtPath<ItemData>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToArray();
    }

#endif

    public enum ItemCategory { ProcessedMaterials, RawMaterials, Tier1Items, Tier2Items, Tier3Items }
    [FoldoutGroup("Item Command")]
    public ItemCategory category;
    [FoldoutGroup("Item Command", expanded: true)]
    [FoldoutGroup("Item Command")]
    [ValueDropdown(nameof(GetItemList))]
    public ItemData itemName;

    [FoldoutGroup("Item Command")]
    public Transform itemspawnPos;
     PickUpSystem pickup;

    private IEnumerable<ItemData> GetItemList()
    {
        switch (category)
        {
            case ItemCategory.ProcessedMaterials:
                return processedMaterials ?? Enumerable.Empty<ItemData>();
            case ItemCategory.RawMaterials:
                return rawMaterials ?? Enumerable.Empty<ItemData>();
            case ItemCategory.Tier1Items:
                return tier1Items ?? Enumerable.Empty<ItemData>();
            case ItemCategory.Tier2Items:
                return tier2Items ?? Enumerable.Empty<ItemData>();
            case ItemCategory.Tier3Items:
                return tier3Items ?? Enumerable.Empty<ItemData>();
            default:
                return Enumerable.Empty<ItemData>();
        }
    }
    private void Start()
    {
        pickup = GameObject.FindGameObjectWithTag("Player").GetComponent<PickUpSystem>();
    }

    [FoldoutGroup("Item Command")]
    [Button(ButtonSizes.Large)]
    private void spawnItem()
    {
        GameObject item = Instantiate(itemName.materialPrefab, itemspawnPos);
        item.name = itemName.materialName;
        item.transform.SetParent(null);
        pickup.ForcePickUp(item);
    }
    [FoldoutGroup("Item Command")]
    [Button(ButtonSizes.Large)]
    private void ClearAllItems()
    {       
        int itemLayer = LayerMask.NameToLayer("Item");
        
        var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int count = 0;

        foreach (var obj in allObjects)
        {
            if (obj.layer == itemLayer)
            {
                Destroy(obj);
                count++;
            }
        }    
    }
}