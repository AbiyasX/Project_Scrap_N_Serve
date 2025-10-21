using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class WorkstationSlotScript : MonoBehaviour, Iinteract
{
    [SerializeField] private GameObject buyTextUI;

    private bool playerNearby = false;
    [SerializeField] private Renderer[] rend;
    private bool purchased = false;
    [SerializeField] private WorkstationShopManager workstationShopManager;
    [SerializeField] GameObject EmptySlotOBJ;
    [SerializeField] GameObject TimerUI;

    private Coroutine timerRoutine;
    private void Awake()
    {
        rend = GetComponents<Renderer>();
        if (buyTextUI != null)
            buyTextUI.SetActive(false);
    }

    private void Update()
    {
        if (purchased)
        {
            buyTextUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            foreach (var obj in rend)
            {
                obj.material.EnableKeyword("_EMISSION");
                obj.material.SetColor("_EmissionColor", Color.white * 0.05f);
            }

            if (!purchased)
            {
                buyTextUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            buyTextUI.SetActive(false);
            workstationShopManager.CloseShop();
            foreach (var obj in rend)
            {
                obj.material.DisableKeyword("_EMISSION");
            }
        }
    }

    public void Interact()
    {
        if (playerNearby)
        {
            if (!purchased && workstationShopManager != null)
            {
                workstationShopManager.WorkstationOpenShop(this);
                Debug.Log("Opened Workstation Shop for this space");
            }
            else if (purchased)
            {
                Debug.Log("Item already purchased");
            }
        }
    }

    public void timerStart(Sprite itemIcon, float duration, bool setActive)
    {
        TimerUI.SetActive(setActive);
        if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        timerRoutine = StartCoroutine(TimerCountdown(itemIcon, duration));
    }

    private IEnumerator TimerCountdown(Sprite icon, float duration)
    {
        Image[] iconTimer = TimerUI.GetComponentsInChildren<Image>();
        foreach (var iconTimerItem in iconTimer)
            iconTimerItem.sprite = icon;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            iconTimer[1].fillAmount = 1f - t;
            yield return null;
        }

        iconTimer[1].fillAmount = 0f;
        TimerUI.SetActive(false); 
    }

    public void isPurchaseLot(bool isActive, WorkstationsData Workstation = null)
    {
        purchased = isActive;

        if (purchased && Workstation != null)
        {
            EmptySlotOBJ.SetActive(!purchased);
        }
    }
}