using UnityEngine;

[System.Serializable]
public class itemList
{
    public ItemData item;
    public int Quantity;
}
[CreateAssetMenu(fileName = "AssemblyRecipeData", menuName = "Assembly Station/Product Recipe")]
public class AssemblyRecipeData : ScriptableObject
{
    [Header("Ingredients")]
    public itemList[] itemDatas;
    [Header("Product")]
    public ItemData product;
}
