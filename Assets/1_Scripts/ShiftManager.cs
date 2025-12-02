using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;


public class ShiftManager : MonoBehaviour
{
    [Header("References")]
    public Light mainLight;
    public CustomerOrderManager orderManager;
    [SerializeField] GameObject SurfaceLadder;
    public Button nextDay;
    private bool nextDayClicked = false;
    [SerializeField] private Image timeFillImage;
    [SerializeField] Animator stopWatchAnim;
    [SerializeField] private GameObject gameOverPanel;

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

        //nextDayButton.gameObject.SetActive(true);
        nextDay.onClick.AddListener(OnNextDayClicked);

        lifeCount = maxLife;
       
        gameOverPanel.SetActive(false);

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
                gameOverPanel.SetActive(true);

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
        SurfaceLadder.gameObject.SetActive(true);

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

        SurfaceLadder.gameObject.SetActive(false);

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
            bool shouldBeEnabled = i < lifeCount;

            // If the icon is currently enabled but needs to be disabled ? play pop animation
            if (lifeIcons[i].enabled && !shouldBeEnabled)
            {
                Animator anim = lifeIcons[i].GetComponent<Animator>();
                if (anim != null)
                    StartCoroutine(PopAndDisable(lifeIcons[i], anim));
            }
            else
            {
                // If icon is still part of remaining lives, keep it enabled
                lifeIcons[i].enabled = shouldBeEnabled;
            }
        }
    }

    private IEnumerator PopAndDisable(UnityEngine.UI.Image icon, Animator anim)
    {
        anim.SetTrigger("Pop");            // play pop animation
        yield return new WaitForSeconds(3f); // wait for animation duration
        icon.enabled = false;              // hide after popping
    }

    private IEnumerator UpdateUITimer()
    {
        if (timeFillImage == null)
            yield break;

        timeFillImage.fillAmount = 0f;
        timeFillImage.color = Color.green; // starting color

        float elapsed = 0f;

        while (elapsed < dayDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / dayDuration);
            timeFillImage.fillAmount = progress;

            // --- COLOR CHANGE LOGIC ---
            if (progress >= 0.5f)
            {
                // Normalize 0.5 ? 1.0 into 0 ? 1
                float t = (progress - 0.5f) / 0.5f;

                stopWatchAnim.ResetTrigger("Idle");
                stopWatchAnim.SetTrigger("Blink");
                

                // Lerp from green to red
                timeFillImage.color = Color.Lerp(Color.green, Color.red, t);
            }

            yield return null;
        }

        timeFillImage.fillAmount = 1f;
        timeFillImage.color = Color.red;
        stopWatchAnim.ResetTrigger("Blink");
        stopWatchAnim.SetTrigger("Idle");
    }

}
