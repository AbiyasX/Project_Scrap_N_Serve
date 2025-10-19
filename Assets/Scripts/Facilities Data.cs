using UnityEngine;

[CreateAssetMenu(fileName = "FacilitiesData", menuName = "Facilities/FacilitiesData")]
public class FacilitiesData : ScriptableObject
{
    public Sprite facilityIcon;
    public string facilityName;
    public float facilityCost;
    public float facilityRepairCost;
    public float facilityUpgrade;
    public float facilityLevel;
    public GameObject facilityModel;
    public ItemData productionMaterial;
    public bool isPurchased;
}