using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ShiftManager : MonoBehaviour
{
    [Header("References")]
    public Light mainLight;                         
    public CustomerOrderManager orderManager;
    [SerializeField] GameObject nextDayButton;
    public Button nextDay;
    private bool nextDayClicked = false;


    [Header("Day/Night Settings")]
    public float dayLightIntensity = 1.2f;
    public float nightLightIntensity = 0.1f;
    public Color dayLightColor = Color.white;
    public Color nightLightColor = new Color(0.2f, 0.3f, 0.6f);

    [Header("Transition Settings")]
    public float transitionDuration = 2f;           

    [Header("Shift Settings")]
    public bool isNight = false;
    public bool autoSwitch = false;
    public float dayDuration = 90f;                
    public float nightDuration = 90f;

    [Header("Day Tracker")]
    public int dayCount = 1;
    public int orderQuota = 30;
    [SerializeField] float difficultyMultiplier = 0.25f;

    [Header("UI References")]
    [SerializeField] TMP_Text dayCountText;
    [SerializeField] TMP_Text lifeCountText;
    [SerializeField] TMP_Text orderQuotaText;

    public int lifeCount = 3;
    private Coroutine cycleRoutine;

    private void Start()
    {
        nextDayButton.gameObject.SetActive(true);
        nextDay.onClick.AddListener(OnNextDayClicked);

        if (autoSwitch)
        {
            if (cycleRoutine != null)
                StopCoroutine(cycleRoutine);

            cycleRoutine = StartCoroutine(DayNightCycle());
        }
        UpdateUI();
    }

    private void OnNextDayClicked()
    {
        nextDayClicked = true;
    }

    private IEnumerator DayNightCycle()
    {
        while (true)
        {
            StartNightShift();
            yield return new WaitUntil(() => nextDayClicked);

            nextDayClicked = false;

            Debug.Log($"Day {dayCount} started!");
            StartDayShift(); 

            yield return new WaitForSeconds(dayDuration);

            CheckQuota();
        }
    }

    private void CheckQuota()
    {
        if (orderManager == null)
        {
            Debug.LogWarning("OrderManager reference missing — cannot check quota.");
            return;
        }

        orderManager.StopOrders();

        if (orderManager.dayEarnings >= orderQuota)
        {
            Debug.Log("All Orders Fullfilled. Shift Complete.");
            StartNightShift();
        }
        else
        {
            Debug.Log("Missed Orders. Deduct Life and Reputaion");
            lifeCount--;
            UpdateUI();
            if (lifeCount <= 0)
            {
                Debug.Log("No more chances left. Game Over!");
                if (cycleRoutine != null)
                    StopCoroutine(cycleRoutine);
                
                return;
            }
            else
            {
                StartNightShift();
            }
        }
    }

    public void StartNightShift()
    {
        Debug.Log("Night shift started — orders paused!");

        StartCoroutine(ChangeLighting(nightLightColor, nightLightIntensity));

        if (isNight) return;
        isNight = true;

        nextDayButton.gameObject.SetActive(true);
    }

    public void StartDayShift()
    {

        StartCoroutine(ChangeLighting(dayLightColor, dayLightIntensity));

        if (!isNight) return;
        isNight = false;

        nextDayButton.gameObject.SetActive(false);

        dayCount++;
        orderManager.dayEarnings = 0;

        orderQuota = Mathf.RoundToInt(orderQuota * difficultyMultiplier);

        orderManager.completedOrders = 0;

        UpdateUI();

        Debug.Log("Day shift started — orders resumed!");
        if (orderManager != null)
            orderManager.ResumeOrders();
    }

    private IEnumerator ChangeLighting(Color targetColor, float targetIntensity)
    {
        Debug.Log("Changing Lighting....");

        if (mainLight == null)
        {
            Debug.LogError("Main Light not assigned!");
            yield break;
        }

        float startIntensity = mainLight.intensity;
        Color startColor = mainLight.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / transitionDuration;
            mainLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            mainLight.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        Debug.Log("[Lighting] Transition complete!");
    }

    private void UpdateUI()
    {
        if (dayCountText != null)
            dayCountText.text = $"Day: {dayCount}";
        if (lifeCountText != null)
            lifeCountText.text = $"Lives: {lifeCount}";
        if (orderQuotaText != null)
            orderQuotaText.text = $"Quota: {orderQuota}";
    }

}
