using UnityEngine;

[CreateAssetMenu(fileName = "WorkstationsData", menuName = "Workstations/WorkstationsData")]
public class WorkstationsData : ScriptableObject
{
    public Sprite workstationIcon;
    public string workstationName;
    public float workstationCost;
   
    public float workstationCraftTime;
    public GameObject workstationModel;
    public ItemData[] inputMaterial;
    public ItemData[] outputMaterial;
    public bool isPurchased;
}