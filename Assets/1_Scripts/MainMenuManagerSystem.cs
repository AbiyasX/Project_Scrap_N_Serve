using UnityEngine;
using UnityEngine.EventSystems; // Required for IPointerEnterHandler, etc.
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

// Implement the necessary interfaces to detect mouse/pointer events
public class MainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] GameObject fadeIn;
    [SerializeField] GameObject fadeOut;

    // Changed to RectTransform since you are using DOAnchorPosX, which is correct for UI
    public RectTransform playButton;
    public RectTransform settingsButton;
    public RectTransform quitButton;

    // Define the scale values for the hover and original state
    private readonly Vector3 normalScale = Vector3.one; // (1, 1, 1)
    private readonly Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f); // 110% scale
    private const float scaleDuration = 0.15f; // Duration for hover/un-hover

    private void Start()
    {
       StartCoroutine(MainMenuLoad());
    }

    IEnumerator MainMenuLoad()
    {
        fadeIn.SetActive(true);
        yield return new WaitForSeconds(1f);
       
        playButton.DOAnchorPosX(playButton.anchoredPosition.x + 450f, 1.2f)
            .SetEase(Ease.OutBack)
            .SetDelay(0f);

        settingsButton.DOAnchorPosX(settingsButton.anchoredPosition.x + 450f, 1.2f)
            .SetEase(Ease.OutBack)
            .SetDelay(0.15f);

        quitButton.DOAnchorPosX(quitButton.anchoredPosition.x + 450f, 1.2f)
            .SetEase(Ease.OutBack)
            .SetDelay(0.30f);

        yield return new WaitForSeconds(2f);
        fadeIn.SetActive(false);
    }

    // --- HOVER EFFECT (IPointerEnterHandler / IPointerExitHandler) ---

    // DOTween hover helper method
    private void ScaleButton(RectTransform button, Vector3 targetScale)
    {
        // Kills any running scale tween on the button to prevent conflicts
        button.DOKill(true);
        // Animate the local scale
        button.DOScale(targetScale, scaleDuration).SetEase(Ease.OutSine);
    }

    // Called when the mouse pointer enters the collider/RectTransform area
    public void OnPointerEnter(PointerEventData eventData)
    {
        // eventData.pointerCurrentRaycast.gameObject is the GameObject under the pointer
        // Get the RectTransform of the hovered object
        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out RectTransform hoveredRect))
        {
            ScaleButton(hoveredRect, hoverScale);
        }
    }

    // Called when the mouse pointer leaves the collider/RectTransform area
    public void OnPointerExit(PointerEventData eventData)
    {
        // eventData.pointerCurrentRaycast.gameObject is the GameObject that was under the pointer
        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out RectTransform exitedRect))
        {
            ScaleButton(exitedRect, normalScale);
        }
    }

    // --- PUNCH EFFECT (IPointerDownHandler) ---

    // Called when a mouse button is pressed down over the object
    public void OnPointerDown(PointerEventData eventData)
    {
        // eventData.pointerCurrentRaycast.gameObject is the GameObject that was clicked
        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out RectTransform clickedRect))
        {
            // Stop current scale tweens, then apply a 'punch' scale effect
            clickedRect.DOKill(true);

            // DOPunchScale causes the object to briefly punch out and spring back
            clickedRect.DOPunchScale(punch: new Vector3(0.1f, 0.1f, 0.1f), duration: 0.2f, vibrato: 5, elasticity: 1f)
                       .SetEase(Ease.OutQuad);
        }
    }

    // --- SCENE MANAGEMENT METHODS ---

    public void PlayGame()
    {
        // DOTween sequence example: shrink button before loading scene
        playButton.DOScale(Vector3.zero, 0.3f)
                  .SetEase(Ease.InBack)
                  .OnComplete(() =>
                  {
                      fadeOut.SetActive(true);
                      Debug.Log("Play button clicked! Loading Scene...");
                      SceneManager.LoadScene("Dan_Testing");
                  });
    }

    public void ExitGame()
    {
        // Animate all buttons to exit before quitting
        DOTween.Sequence()
               .Join(playButton.DOScale(Vector3.zero, 0.3f))
               .Join(settingsButton.DOScale(Vector3.zero, 0.3f).SetDelay(0.1f))
               .Join(quitButton.DOScale(Vector3.zero, 0.3f).SetDelay(0.2f))
               .OnComplete(() =>
               {
                   Debug.Log("Quit Game");
                   Application.Quit();
#if UNITY_EDITOR
                   UnityEditor.EditorApplication.isPlaying = false;
#endif
               });
    }
}