using Unity.VisualScripting;
using UnityEngine;

public interface IPickUp
{
    void Pickup();
}

public class PickUpSystem : MonoBehaviour
{
    [SerializeField] private float pickupRange = 1f;
    [SerializeField] private float sphereRadius = 1f;
    [SerializeField] private LayerMask itemLayer;

    [SerializeField] private Transform itemHolder;

    [Header("Throw Settings")]
    [SerializeField] private float minThrowForce;
    [SerializeField] private float maxThrowForce;
    [SerializeField] private float maxChargeTime;

    Animator grabAnim;
    private GameObject currentItem;
    private GameObject heldItem;
    private bool isHoldingItem = false;

    private float throwCharge = 0f;
    private bool isChargingThrow = false;

    private void Awake()
    {
        grabAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isHoldingItem)
            DetectItem();

        // Charge logic
        if (isChargingThrow)
        {
            throwCharge += Time.deltaTime;
            throwCharge = Mathf.Clamp(throwCharge, 0f, maxChargeTime);
        }

        if (grabAnim != null)
        {
            int grabLayerIndex = 1;
            float targetWeight = isHoldingItem ? 1f : 0f;
            float currentWeight = grabAnim.GetLayerWeight(grabLayerIndex);
            float newWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 10f);
            grabAnim.SetLayerWeight(grabLayerIndex, newWeight);
        }
    }

    public void DetectItem()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * pickupRange * 0.5f, sphereRadius, itemLayer);

        if (hits.Length > 0)
        {
            Collider closest = hits[0];
            float closestDist = Vector3.Distance(transform.position, closest.transform.position);

            foreach (var hit in hits)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closest = hit;
                    closestDist = dist;
                }
            }

            currentItem = closest.gameObject;
        }
        else
        {
            currentItem = null;
        }
    }

    public void ForcePickUp(GameObject item)
    {
        if (item == null) return;
        if (isHoldingItem) return;
        
        
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        MeshCollider meshCol = item.GetComponent<MeshCollider>();
        if (meshCol)
            meshCol.enabled = false;

        item.transform.SetParent(itemHolder);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        heldItem = item;
        isHoldingItem = true;
    }

    public void PickUpItem()
    {
        if (currentItem == null) return;
        if (isHoldingItem) return;
        AssemblySystem.Instance.RemoveItemManually(currentItem);
        Rigidbody rb = currentItem.GetComponent<Rigidbody>();

        TableScript[] allTables = FindObjectsByType<TableScript>(FindObjectsSortMode.None);
        foreach (TableScript table in allTables)
        {
            if (table != null && table.GetItemOnTable() == currentItem)
            {
                table.Pickup();
                break;
            }
        }

        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        MeshCollider meshCol = currentItem.GetComponent<MeshCollider>();
        if (meshCol)
            meshCol.enabled = false;
        currentItem.transform.SetParent(itemHolder);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;
   
        heldItem = currentItem;
        currentItem = null;
        isHoldingItem = true;
    }

    public void DropItem()
    {
        if (!isHoldingItem || heldItem == null) return;
        
        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(transform.forward * minThrowForce, ForceMode.Impulse);
        }
        heldItem.GetComponent<MeshCollider>().enabled = true;
        heldItem.transform.SetParent(null);
        heldItem = null;
        isHoldingItem = false;
    }

    public void ChargeAndThrow(bool isPressed)
    {
        if (!isHoldingItem || heldItem == null) return;
        
        if (isPressed)
        {
            // Start charging

            isChargingThrow = true;
            throwCharge = 0f;
        }
        else
        {
            // Release and throw
            if (!isChargingThrow) return;

            isChargingThrow = false;

            float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, throwCharge / maxChargeTime);

            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
                rb.useGravity = true;

                Vector3 throwDirection = (transform.forward + Vector3.up * 0.5f).normalized;
                rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }
            MeshCollider meshCol = heldItem.GetComponent<MeshCollider>();
            if (meshCol)
                meshCol.enabled = true;
            heldItem.transform.SetParent(null);

            heldItem = null;
            isHoldingItem = false;
            throwCharge = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * pickupRange, sphereRadius);
    }
}
