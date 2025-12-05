using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WorkstationIndicator : MonoBehaviour
{
    [Header("Indicator Settings")]
    [SerializeField] private GameObject indicatorUI;  // UI Canvas with Image
    [SerializeField] private Image outputIconImage;   // The Image component showing the icon
    [SerializeField] private float indicatorHeight = 3.5f;  // Height above workstation

    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 1f;  // Duration for one up/down cycle
    [SerializeField] private float moveAmount = 0.3f;  // How much to move up/down
    [SerializeField] private Ease moveEase = Ease.InOutSine;  // Animation curve

    private WorkstationProcessor workstationProcessor;
    private Camera mainCamera;
    private Vector3 basePosition;
    private Tween moveTween;

    private void Start()
    {
        workstationProcessor = GetComponent<WorkstationProcessor>();
        mainCamera = Camera.main;

        // Hide indicator at start
        if (indicatorUI != null)
            indicatorUI.SetActive(false);
    }

    private void Update()
    {
        UpdateIndicator();

        // Update indicator rotation to face camera
        if (indicatorUI != null && indicatorUI.activeSelf)
        {
            UpdateIndicatorRotation();
        }
    }

    private void UpdateIndicator()
    {
        // Check if workstation is busy
        if (workstationProcessor == null || workstationProcessor.isBusy)
        {
            HideIndicator();
            return;
        }

        // Check if workstation data exists
        if (workstationProcessor.workstationData == null)
        {
            HideIndicator();
            return;
        }

        // Get what the player is currently holding
        GameObject heldItem = PickUpSystem.CurrentHeldItem;

        if (heldItem == null)
        {
            HideIndicator();
            return;
        }

        // Get the name of the held item
        string heldItemName = heldItem.name.Replace("(Clone)", "").Trim();

        // Check if this item matches any input recipe
        foreach (var recipe in workstationProcessor.workstationData.recipes)
        {
            if (recipe.inputPrefab != null &&
                heldItemName.Equals(recipe.inputPrefab.materialName, System.StringComparison.OrdinalIgnoreCase))
            {
                // Found a match! Show the output icon from the recipe's output item
                if (recipe.outputPrefab != null && recipe.outputPrefab.materialIcon != null)
                {
                    ShowIndicator(recipe.outputPrefab.materialIcon);
                }
                return;
            }
        }

        // No match found
        HideIndicator();
    }

    private void ShowIndicator(Sprite outputIcon)
    {
        if (indicatorUI == null || outputIconImage == null) return;

        bool wasActive = indicatorUI.activeSelf;

        indicatorUI.SetActive(true);
        outputIconImage.sprite = outputIcon;

        // Start animation if not already running
        if (!wasActive)
        {
            StartFloatingAnimation();
        }
    }

    private void HideIndicator()
    {
        if (indicatorUI != null)
            indicatorUI.SetActive(false);

        // Kill the tween to stop animation
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }
    }

    private void StartFloatingAnimation()
    {
        if (indicatorUI == null) return;

        // Kill any existing tween
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }

        // Set base position
        basePosition = transform.position + Vector3.up * indicatorHeight;
        indicatorUI.transform.position = basePosition;

        // Create floating animation using DOTween
        // Move up, then down, then loop
        moveTween = indicatorUI.transform
            .DOMoveY(basePosition.y + moveAmount, moveDuration / 2f)
            .SetEase(moveEase)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(UpdateType.Normal);
    }

    private void UpdateIndicatorRotation()
    {
        if (mainCamera == null || indicatorUI == null) return;

        // Make it face the camera
        indicatorUI.transform.LookAt(mainCamera.transform);
        indicatorUI.transform.Rotate(0, 180, 0);
    }

    private void OnDestroy()
    {
        // Clean up tween when object is destroyed
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }
    }
}