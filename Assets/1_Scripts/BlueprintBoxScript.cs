using Unity.VisualScripting;
using UnityEngine;

public class BlueprintBoxScript : MonoBehaviour, Iinteract
{

    [SerializeField] GameObject bluePrintPrefab;
    [SerializeField] GameObject highlight;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            highlight.SetActive(true);
        }     
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            highlight.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            Debug.Log("Get Item");
            
        }
    }

    public void Interact()
    {
        Debug.Log("Interact BluePrint");
        GameObject blueprint = Instantiate(bluePrintPrefab);
        PickUpSystem pickUpSystem = FindAnyObjectByType<PickUpSystem>();
        pickUpSystem.ForcePickUp(blueprint);
        
    }
}
