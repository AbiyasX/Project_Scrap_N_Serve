using UnityEngine;

public class TableScript : MonoBehaviour, IPickUp
{
    [SerializeField] GameObject tableHolder;
    [SerializeField] GameObject onTable;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Blueprint"))
        {
            if(onTable != null)
                return;
            onTable = collision.gameObject;
            onTable.transform.position = tableHolder.transform.position;
            onTable.transform.rotation = tableHolder.transform.rotation;
            onTable.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

   public void Pickup()
    {
        onTable.GetComponent<Rigidbody>().isKinematic = false;
        onTable = null;
    }

    public GameObject GetItemOnTable() => onTable;
}
