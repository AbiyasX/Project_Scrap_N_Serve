using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorkstationTrigger : MonoBehaviour
{
    private WorkstationProcessor processor;

    private void Awake()
    {
        processor = GetComponent<WorkstationProcessor>();
        GetComponent<Collider>().isTrigger = true; // ensure trigger mode
    }

    private void OnTriggerEnter(Collider other)
    {
        if (processor != null)
        {
            processor.TryProcessItem(other.gameObject);
        }
    }
}
