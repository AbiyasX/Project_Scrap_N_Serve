using UnityEngine;

public class OrderItemSystem : MonoBehaviour, Iinteract
{
    public Renderer[] rend;
    public void Interact() 
    {
        Debug.Log("Open OrderMenu");
    }

    private void Start()
    {
        rend = GetComponentsInChildren<Renderer>();

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (var mat in rend)
            {
                mat.material.EnableKeyword("_EMISSION");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (var mat in rend)
            {
                mat.material.DisableKeyword("_EMISSION");
            }
        }
    }
}
