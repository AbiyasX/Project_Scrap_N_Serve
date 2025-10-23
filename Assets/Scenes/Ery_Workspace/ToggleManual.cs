using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleManual : MonoBehaviour
{
    public GameObject Manual;
    public bool isOpen = false;
    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            // toggle canvas on/off
            isOpen = !isOpen;
            Manual.SetActive(isOpen);
            
        }
    }
}
