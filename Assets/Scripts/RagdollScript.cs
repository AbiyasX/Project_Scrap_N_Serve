using UnityEngine;
using System.Collections;

public class RagdollController : MonoBehaviour
{
    [Header("Ragdoll Settings")]
    public Transform ragdollRoot;
    public bool startRagdoll = false;

    private Rigidbody[] ragdollRigidbodies;
    public Collider[] ragdollColliders;
    private Animator animator;
    private PlayerControls characterController;
    private MonoBehaviour[] playerScripts;

    private bool isRagdoll = false;
    private Coroutine recoverCoroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<PlayerControls>();
        ragdollRigidbodies = ragdollRoot.GetComponentsInChildren<Rigidbody>();
        playerScripts = GetComponents<MonoBehaviour>();

        SetRagdoll(startRagdoll);
    }

    public void ToggleRagdoll(bool active)
    {
        // Stop any ongoing recovery coroutine to avoid conflicts
        if (recoverCoroutine != null)
            StopCoroutine(recoverCoroutine);

        SetRagdoll(active);

        // If we just entered ragdoll, start auto recovery
        if (active)
            recoverCoroutine = StartCoroutine(RecoverAfterDelay(0.5f));
    }

    private IEnumerator RecoverAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetRagdoll(false);
        yield return new WaitForSeconds(1);
        animator.enabled = true;
    }

    private void SetRagdoll(bool active)
    {
        isRagdoll = active;

        animator.enabled = !active;
        characterController.enabled = !active;

        // Disable/enable player control scripts
        foreach (var script in playerScripts)
        {
            if (script == this || script == animator) continue;
            script.enabled = !active;
        }

        // Enable/disable ragdoll physics
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = !active;
        }

        foreach (var col in ragdollColliders)
        {
            col.enabled = active;
        }

        // Align back to ragdoll pose when recovering
        if (!active)
        {
            transform.position = ragdollRoot.position;
            transform.rotation = ragdollRoot.rotation;
        }
    }

    public void ApplyRagdollForce(Vector3 force)
    {
        if (!isRagdoll) return;
        foreach (var rb in ragdollRigidbodies)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

    public bool IsRagdollActive => isRagdoll;
}