using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Cinemachine;

public class LadderPortal : MonoBehaviour, Iinteract
{
    [Header("References")]
    public GameObject player;
    public Transform destinationPosition; // where player goes after interaction

    [Header("Cinemachine Cameras")]
    public CinemachineCamera thisLevelCam;
    public CinemachineCamera nextLevelCam;

    [Header("Transition UI")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    private bool isSwitching = false;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void Interact()
    {
        if (isSwitching) return;
        StartCoroutine(HandleTeleport());
    }

    private IEnumerator HandleTeleport()
    {
        isSwitching = true;

        // Fade Out
        yield return StartCoroutine(Fade(1f));

        // Move player to destination
        player.transform.position = destinationPosition.position;

        // Swap cameras
        thisLevelCam.Priority = 0;
        nextLevelCam.Priority = 10;

        // Small delay to avoid overlap trigger
        yield return new WaitForSeconds(0.2f);

        // Fade In
        yield return StartCoroutine(Fade(0f));

        isSwitching = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeImage == null)
            yield break;

        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeImage.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);
    }
}