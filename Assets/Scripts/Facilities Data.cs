using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "FacilitiesData", menuName = "Facilities/FacilitiesData")]
public class FacilitiesData : ScriptableObject
{
    [SerializeField] string facilityName;
    [SerializeField] float facilityCost;
    [SerializeField] float facilityRepairCost;
    [SerializeField] float facilityUpgrade;
    [Range(1f, 3f)]
    [SerializeField] int facilityLevel;
    [SerializeField] float facilityCookTime;
    [SerializeField] GameObject facilityModel;
    [SerializeField] GameObject facilityProductionModel;
}
