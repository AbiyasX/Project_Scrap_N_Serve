using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject fadeIn;
    [SerializeField] GameObject fadeOut;

    public RectTransform playButton;
    public RectTransform settingsButton;
    public RectTransform quitButton;

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

    public void PlayGame()
    {
        playButton.DOScale(Vector3.zero, 0.3f)
                  .SetEase(Ease.InBack)
                  .OnComplete(() =>
                  {
                      fadeOut.SetActive(true);
                      SceneManager.LoadScene("ScrapNServe");
                  });
    }

    public void ExitGame()
    {
        DOTween.Sequence()
               .Join(playButton.DOScale(Vector3.zero, 0.3f))
               .Join(settingsButton.DOScale(Vector3.zero, 0.3f).SetDelay(0.1f))
               .Join(quitButton.DOScale(Vector3.zero, 0.3f).SetDelay(0.2f))
               .OnComplete(() =>
               {
                   Application.Quit();
#if UNITY_EDITOR
                   UnityEditor.EditorApplication.isPlaying = false;
#endif
               });
    }
}
