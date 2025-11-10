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
    [SerializeField] private Image timeFillImage;


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
    public int newQuota = 0;
    [SerializeField] float difficultyMultiplier = 0.25f;

    [Header("UI References")]
    [SerializeField] TMP_Text dayCountText;
    [SerializeField] TMP_Text orderQuotaText;

    PlayerControls playerControls;

    public int lifeCount;
    public int maxLife = 3;
    public Image[] lifeIcons;

    private Coroutine cycleRoutine;
    private Coroutine dayTimerRoutine;

    private void Awake()
    {
        if (autoSwitch)
        {
            if (cycleRoutine != null)
                StopCoroutine(cycleRoutine);

            cycleRoutine = StartCoroutine(DayNightCycle());
        }
    }

    private void Start()
    {
        playerControls = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>();

        nextDayButton.gameObject.SetActive(true);
        nextDay.onClick.AddListener(OnNextDayClicked);

        lifeCount = maxLife;
       
        UpdateUI();
    }

    private void Update()
    {
        playerControls.flashLight(isNight);
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

        if (orderManager.dayEarnings >= newQuota)
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

        if (dayTimerRoutine != null)
        {
            StopCoroutine(dayTimerRoutine);
            timeFillImage.fillAmount = 0f;
        }
    }

    public void StartDayShift()
    {

        StartCoroutine(ChangeLighting(dayLightColor, dayLightIntensity));

        if (!isNight) return;
        isNight = false;

        nextDayButton.gameObject.SetActive(false);

        dayCount++;
        orderManager.dayEarnings = 0;

        if (dayCount == 1)
        {
            newQuota = orderQuota;
        }

        newQuota = Mathf.RoundToInt(orderQuota * difficultyMultiplier);

        orderManager.completedOrders = 0;

        UpdateUI();

        Debug.Log("Day shift started — orders resumed!");
        if (orderManager != null)
            orderManager.ResumeOrders();

        if(dayTimerRoutine != null)
            StopCoroutine(dayTimerRoutine);
        dayTimerRoutine = StartCoroutine(UpdateUITimer());
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
        if (orderQuotaText != null)
            orderQuotaText.text = $"Quota: {orderQuota}";

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (i < lifeCount)
                lifeIcons[i].enabled = true;
            else
                lifeIcons[i].enabled = false;
        }
    }

    private IEnumerator UpdateUITimer()
    {
        if (timeFillImage == null)
            yield break;

        timeFillImage.fillAmount = 0f; 

        float elapsed = 0f;

        while (elapsed < dayDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / dayDuration);
            timeFillImage.fillAmount = progress;
            yield return null;
        }

        timeFillImage.fillAmount = 1f; 
    }
}
