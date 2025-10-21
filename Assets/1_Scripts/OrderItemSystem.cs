using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderMaterialSystem : MonoBehaviour, Iinteract
{

    [SerializeField] private Transform spawnItemTransform;

    [Header("UI Setup")]
    [SerializeField] private GameObject orderUI;

    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject materialButtonPrefab;
    [SerializeField] private Transform OrderQueueUI;
    [SerializeField] private GameObject ItemQueueUI;

    [SerializeField] private float timer;

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

    public void AddMaterialToMenu(ItemData material, int level, FacilitySlotScript lotRef)
    {
        addedMaterials.Add(material);

        GameObject button = Instantiate(materialButtonPrefab, buttonParent);
        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        var Image = button.transform.Find("MaterialImage").GetComponent<Image>();
        Image.sprite = material.materialIcon;
        text.text = material.materialName;

        FacilitySlotScript lot = lotRef;

        button.GetComponent<Button>().onClick.AddListener(() =>
        {
            if(OrderQueueUI.childCount >= 5)
            {
                return;
            }
            int currentlevel = lot != null ? lot.GetCurrentLevel() : level;
            switch (currentlevel)
            {
                case 1: timer = 10f; break;
                case 2: timer = 5f; break;
                case 3: timer = 2f; break;
                default: timer = 15f; break;
            }

            Debug.Log($"Ordered: {material.name}");
            GameObject spawnQueueItem = Instantiate(ItemQueueUI, OrderQueueUI);
            Destroy(spawnQueueItem, timer + 0.1f);
            Image[] images = spawnQueueItem.GetComponentsInChildren<Image>();
            foreach (var img in images)
            {
                img.sprite = material.materialIcon;
            }

            Transform fillBar = spawnQueueItem.transform.Find("Fill");

            if (fillBar != null)
            {
                Image fillImage = fillBar.GetComponent<Image>();

                StartCoroutine(FillTimer(fillImage, timer, material));
            }
        });
    }

    private IEnumerator FillTimer(Image fillImage, float cookTime, ItemData material)
    {
        float timer = 0f;
        while (timer < cookTime)
        {
            if (fillImage == null)
                yield break;

            timer += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(timer / cookTime);
            yield return null;
        }

        GameObject spawnedItem = Instantiate(material.materialPrefab, spawnItemTransform.position, spawnItemTransform.rotation);
        spawnedItem.name = material.name;
        Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
        Vector3 throwDirection = spawnItemTransform.forward;
        rb.AddForce(throwDirection * 10f, ForceMode.Impulse);

        fillImage.fillAmount = 1f;
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