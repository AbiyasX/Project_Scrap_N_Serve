using UnityEngine;
[System.Serializable]
public class RecipeList
{
    public ItemData inputPrefab;
    public ItemData outputPrefab;
    public float craftingTime = 2f;
}
[CreateAssetMenu(fileName = "WorkstationsData", menuName = "Workstations/WorkstationsData")]
public class WorkstationsData : ScriptableObject
{
    public Sprite workstationIcon;
    public string workstationName;
    public float workstationCost;
    public GameObject workstationModel;
    public bool isPurchased;
    [Header("Recipe Material")]
    public RecipeList[] recipes;
}