using UnityEngine;
using bob;

public class Player : MonoBehaviour
{
    private Camera _camera;

    public float pickupRange = 3f;
    public Transform holdPoint;
    public float dropSpeed = 2f;

    private GameObject heldItem;
    private Rigidbody heldRigidbody;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        // Drop if currently holding an item
        if (heldItem != null && Input.GetButtonDown("Fire1"))
        {
            DropItem(heldItem);
            return;
        }

        // Otherwise, try to pick up a nearby interactable
        GameObject nearestGameObject = GetNearestGameObject();
        if (nearestGameObject == null) return;

        if (Input.GetButtonDown("Fire1"))
        {
            var interactable = nearestGameObject.GetComponent<Interactable>();
            interactable?.Interact();
        }
    }

    public GameObject GetNearestGameObject()
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit, pickupRange))
        {
            return hit.transform.gameObject;
        }
        return null;
    }

    public void HoldItem(GameObject item)
    {
        heldItem = item;

        var rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        item.transform.SetParent(holdPoint);
        item.transform.localPosition = new Vector3(0, 0, 1);
        item.transform.localRotation = Quaternion.identity;
    }

    public void DropItem(GameObject item)
    {
        if (heldItem == null) return;

        item.transform.SetParent(null);

        var rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = _camera.transform.forward * dropSpeed;
        }

        heldItem = null;
    }
}
