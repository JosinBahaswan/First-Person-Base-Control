using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public string triggerTag = "Player";
    public UnityEvent onTriggerTagEnter;
    public UnityEvent onTriggerTagExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            onTriggerTagEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            onTriggerTagExit.Invoke();
        }
    }
}
