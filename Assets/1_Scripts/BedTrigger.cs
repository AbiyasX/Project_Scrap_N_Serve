using UnityEngine;

public class BedTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public GameObject nexDayButton;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nexDayButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nexDayButton.SetActive(false);
        }
    }
}
