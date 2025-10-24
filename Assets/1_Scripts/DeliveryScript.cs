using UnityEngine;

public class DeliveryScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        CustomerOrderManager.Instance.CheckDelivery(collision.gameObject);
    }
}
