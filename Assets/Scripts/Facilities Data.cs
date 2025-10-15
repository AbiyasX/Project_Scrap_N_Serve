using UnityEngine;

[CreateAssetMenu(fileName = "FacilitiesData", menuName = "Facilities/FacilitiesData")]
public class FacilitiesData : ScriptableObject
{
    public Sprite facilityIcon;
    public string facilityName;
    public float facilityCost;
    public float facilityRepairCost;
    public float facilityUpgrade;

    [Range(0f, 3f)]
    public int facilityLevel;

    public float facilityCookTime;
    public GameObject facilityModel;
    public ItemData productionMaterial;
    public bool isPurchased;
}