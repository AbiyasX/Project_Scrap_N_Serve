using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "FacilitiesData", menuName = "Facilities/FacilitiesData")]
public class FacilitiesData : ScriptableObject
{
    [SerializeField] public string facilityName;
    [SerializeField] public float facilityCost;
    [SerializeField] public float facilityRepairCost;
    [SerializeField] public float facilityUpgrade;
    [Range(0f, 3f)]
    [SerializeField] public int facilityLevel;
    [SerializeField] public float facilityCookTime;
    [SerializeField] public GameObject facilityModel;
    [SerializeField] GameObject facilityProductionModel;
    [SerializeField] public bool isPurchased;
}
