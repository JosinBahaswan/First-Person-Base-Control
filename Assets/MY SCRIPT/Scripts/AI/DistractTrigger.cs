using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractTrigger : MonoBehaviour
{
    public string triggerTag = "Player";

    AIHunter hunter;

    private void Start()
    {
        hunter = SglGameManager.instance.hunter;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(triggerTag)) return;

        hunter.Distract(transform.position);
    }
}
