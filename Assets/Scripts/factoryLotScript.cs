using UnityEngine;

public class factoryLotScript : MonoBehaviour, Iinteract
{
    [SerializeField] private GameObject textUI;
    private bool playerNearby = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            textUI.SetActive(true);
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            textUI.SetActive(false);
            GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
    public void Interact()
    {
        if (playerNearby)
        {
            Debug.Log("OpenShop");
        }
    }
}
