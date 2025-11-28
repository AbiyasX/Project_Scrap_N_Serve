using UnityEngine;

public class LadderDetection : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public GameObject ladderNote;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ladderNote.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ladderNote.SetActive(false);
        }
    }
}
