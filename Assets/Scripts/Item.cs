using UnityEngine;
using bob;

public class ItemPickup : MonoBehaviour, Interactable
{
    private bool isHeld = false;
    private Player player;

   

    public void PickUp()
    {
        if (isHeld) return;

        isHeld = true;
        GetComponent<Collider>().enabled = false;
        player.HoldItem(gameObject);
    }

    public void Drop()
    {
        if (!isHeld) return;

        isHeld = false;

        var col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true; // ✅ Re-enable collisions

        player.DropItem(gameObject);
    }


    public void Interact()
    {
        if (isHeld)
            Drop();
        else
            PickUp();
    }
}
