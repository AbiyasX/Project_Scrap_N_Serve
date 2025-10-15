using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public bool canHitPlayer = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RagdollController ragdoll = other.GetComponentInParent<RagdollController>();
            ragdoll.ToggleRagdoll(canHitPlayer);
            ragdoll.ApplyRagdollForce(other.gameObject.transform.forward * 5);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canHitPlayer = false;
        }
    }
}
