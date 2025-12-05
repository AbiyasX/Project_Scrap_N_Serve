using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class TrashScript : MonoBehaviour
{
    [Header("Layer Settings")]
    [SerializeField] private LayerMask itemLayer;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeStrength = 0.5f;
    [SerializeField] private int shakeVibrato = 10;
    [SerializeField] private bool shakeTrash = true;  // Shake the trash itself
    [SerializeField] private bool shakeCamera = false;  // Shake the camera

    [Header("Audio Settings (Optional)")]
    [SerializeField] private AudioClip trashSound;

    [Header("Visual Effects (Optional)")]
    [SerializeField] private ParticleSystem trashEffect;

    private Camera mainCamera;
    private Vector3 originalTrashPosition;

    private void Start()
    {
        mainCamera = Camera.main;
        originalTrashPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if colliding object is on item layer
        if (((1 << collision.gameObject.layer) & itemLayer) != 0)
        {
            // Play sound if assigned
            if (trashSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(trashSound);
            }

            // Play particle effect if assigned
            if (trashEffect != null)
            {
                Instantiate(trashEffect, collision.contacts[0].point, Quaternion.identity);
            }

            // Shake effect
            DoShake();

            // Destroy the item
            Destroy(collision.gameObject);
            Debug.Log($"[Trash] Destroyed: {collision.gameObject.name}");
        }
    }

    private void DoShake()
    {
        // Shake the trash bin itself
        if (shakeTrash)
        {
            transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => transform.position = originalTrashPosition);
        }

        // Shake the camera
        if (shakeCamera && mainCamera != null)
        {
            mainCamera.transform.DOShakePosition(shakeDuration, shakeStrength * 0.5f, shakeVibrato)
                .SetEase(Ease.OutQuad);
        }
    }
}
