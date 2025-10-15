using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class OrderMaterialSystem : MonoBehaviour, Iinteract
{
    [Header("Facility Link")]
    [SerializeField] private FacilitiesData facilityData;
    [SerializeField] private Transform spawnItemTransform;
    [Header("UI Setup")]
    [SerializeField] private GameObject orderUI;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject materialButtonPrefab;

    private Renderer[] renderers;
    private HashSet<ItemData> addedMaterials = new HashSet<ItemData>();
    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void Interact()
    {      
        orderUI.SetActive(true);   
    }

    public void AddMaterialToMenu(ItemData material)
    {
      
        addedMaterials.Add(material);

        GameObject button = Instantiate(materialButtonPrefab, buttonParent);
        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        text.text = material.materialName;
        // Optional: add button functionality
        button.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log($"Ordered: {material.name}");
            GameObject spawnedItem = Instantiate(material.materialPrefab, spawnItemTransform.position, spawnItemTransform.rotation);
            Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
            Vector3 throwDirection = spawnItemTransform.forward;
            rb.AddForce(throwDirection * 10f, ForceMode.Impulse);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var mat in renderers)
                mat.material.EnableKeyword("_EMISSION");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            orderUI.SetActive(false);
            foreach (var mat in renderers)
                mat.material.DisableKeyword("_EMISSION");
        }
    }
}