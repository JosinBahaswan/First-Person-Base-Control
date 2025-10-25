using DG.Tweening;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public string triggerTag = "Player";
    public Transform targetTween;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            StartTween(0, 3);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            StartTween(1, 10);
        }
    }

    private void StartTween(float yTarget, float duration)
    {
        targetTween.DOScaleY(yTarget, duration);
    }


}
