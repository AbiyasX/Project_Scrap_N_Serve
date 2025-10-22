using UnityEngine;
using System.Collections;
using TMPro;

public class ShiftManager : MonoBehaviour
{
    [Header("References")]
    public Light mainLight;                         
    public CustomerOrderManager orderManager;       

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
    public int orderQuota = 5;
    private int maxQuota = 10;
    //[SerializeField] float difficultyMultiplier = 0.2f;

    [Header("UI References")]
    [SerializeField] TMP_Text dayCountText;
    [SerializeField] TMP_Text lifeCountText;

    public int lifeCount = 3;
    private Coroutine cycleRoutine;

    private void Start()
    {
        if (autoSwitch)
        {
            if (cycleRoutine != null)
                StopCoroutine(cycleRoutine);

            cycleRoutine = StartCoroutine(DayNightCycle());
        }
        UpdateUI();
    }

    private IEnumerator DayNightCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(dayDuration);
            CheckQuota();

            yield return new WaitForSeconds(nightDuration);
            StartDayShift();
            //or can click a button to proceed to next Day
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

        if (orderManager.completedOrders >= orderQuota)
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

        if (isNight) return;
        isNight = true;

        StartCoroutine(ChangeLighting(nightLightColor, nightLightIntensity));
    }

    public void StartDayShift()
    {
        if (!isNight) return;
        isNight = false;

        dayCount++;
        orderQuota = Mathf.Min(orderQuota + 1, maxQuota);

        orderManager.completedOrders = 0;

        UpdateUI();

        Debug.Log("Day shift started — orders resumed!");
        if (orderManager != null)
            orderManager.ResumeOrders();

        StartCoroutine(ChangeLighting(dayLightColor, dayLightIntensity));
    }

    private IEnumerator ChangeLighting(Color targetColor, float targetIntensity)
    {
        if (mainLight == null) yield break;

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
    }

    private void UpdateUI()
    {
        if (dayCountText != null)
            dayCountText.text = $"Day: {dayCount}";
        if (lifeCountText != null)
            lifeCountText.text = $"Lives: {lifeCount}";
    }

}
