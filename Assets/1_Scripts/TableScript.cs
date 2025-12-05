using UnityEngine;
using Sirenix.OdinInspector;
public class TableScript : MonoBehaviour, IPickUp, Iinteract
{
    [SerializeField] GameObject tableHolder;
    [SerializeField] GameObject onTable;
    [ShowInInspector, ReadOnly]
    
    public void Interact()
    {
        if (onTable != null) return;
        PickUpSystem pickUp = FindAnyObjectByType<PickUpSystem>();
        GameObject held = pickUp.item();
        onPlace(held);
        pickUp.DropItem();
    }

    private void onPlace(GameObject item)
    {
        if (item == null) return;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        onTable = item;

        Collider tableCollider = GetComponent<Collider>();
        if (tableCollider == null) return;

        Collider itemCollider = item.GetComponentInChildren<Collider>();
        if (itemCollider == null) return;

        Bounds tableBounds = tableCollider.bounds;
        Bounds itemBounds = itemCollider.bounds;

        Vector3 newPosition = new Vector3(tableBounds.center.x, tableBounds.max.y + itemBounds.extents.y, tableBounds.center.z);

        item.transform.SetParent(null);
        item.transform.position = newPosition;
        item.transform.rotation = tableHolder.transform.rotation;
    }

    public void Pickup()
    {
        onTable.GetComponent<Rigidbody>().isKinematic = false;
        onTable = null;
    }

    public GameObject GetItemOnTable() => onTable;
}
