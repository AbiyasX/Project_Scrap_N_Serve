using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private RectTransform rect;
    private readonly Vector3 normalScale = Vector3.one;
    private readonly Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.DOKill(true);
        rect.DOScale(hoverScale, 0.15f).SetEase(Ease.OutSine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.DOKill(true);
        rect.DOScale(normalScale, 0.15f).SetEase(Ease.OutSine);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rect.DOKill(true);
        rect.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 5, 1f);
    }
}
