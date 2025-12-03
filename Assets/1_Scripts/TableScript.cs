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
        Rigidbody rb = onTable.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Bounds tableBounds = GetComponent<Renderer>().bounds;
        float tableTopY = tableBounds.max.y;

        Bounds itemBounds = item.GetComponentInChildren<Renderer>().bounds;
        float itemBottomY = itemBounds.min.y;

        float heightAdjustment = tableTopY - itemBottomY;

        item.transform.SetParent(null);
        item.transform.position += new Vector3(0f, heightAdjustment, 0f);

    }

    public void Pickup()
    {
        onTable.GetComponent<Rigidbody>().isKinematic = false;
        onTable = null;
    }

    public GameObject GetItemOnTable() => onTable;
}
