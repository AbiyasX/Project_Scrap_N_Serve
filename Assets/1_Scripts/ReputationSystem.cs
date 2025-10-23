using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
public class ReputationSystem : MonoBehaviour
{
    [SerializeField] public int quota;
    [SerializeField] public int currentReputation;
    [SerializeField] public int maxReputation = 100;
    [SerializeField] public int playerCogs;
    [SerializeField] public int playerLife = 3;
    bool Dayshift = true;

    void Update()
    {
        // Dayshift Night Shift Condition;
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            Dayshift = false;
            ReputationStatus();
            if(playerLife == 0)
            {
                Debug.Log("GAME OVER! YOURE DEAD MEAT");
            }
        } 
    }
    public void ReputationStatus()
    {
        
        bool isQuota = playerCogs >= quota;
        if (isQuota)
            IncreasedReputation();
        else
            DecreasedReputation();

        if (currentReputation >= maxReputation)
        {
            currentReputation = maxReputation;
            Debug.Log("KEEP UP THE GOOD WORK! YOURE THE BEST :>");
            Dayshift = false;
        }

    }


    public void IncreasedReputation()
    {
        currentReputation += 10;
        Debug.Log("GOOD JOB! WE CAN SURVIVE THIS TOGETHER");
        Dayshift = true;
    }

    public void DecreasedReputation()
    {
        playerLife -= 1;
        currentReputation -= 10;
        Debug.Log("YOU DID NOT MEET QUOTA! YOU WILL REGRET THAT!");
        Dayshift = true;
    }

}
