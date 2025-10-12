using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Interactable")]
    public string objectName = "Door Single";
    public string objectCategory = "Door";
    public string interactMessage = "Open Door";
    public string failedMessage = "Door Locked";
    public Sprite objectIcon;

    [Header("Interactable Event")]
    public UnityEvent onInteractSucceed;
    public UnityEvent onInteractFailed;

    [Header("Debug")]
    public bool isInteracted = false;

    protected PlayerInteractionNoInventory playerInteraction;
    private string succeedMessage = "";

    public virtual (string, string) GetData()
    {
        return (objectName, interactMessage);
    }

    public virtual void Start()
    {
        playerInteraction = PlayerInteractionNoInventory.Instance;
    }

    public virtual void OnInteract()
    {

    }

    public virtual void OnInteractFailed()
    {
        //Debug.Log(objectName + " Interact Failed");
        if (succeedMessage == "") succeedMessage = interactMessage;
        interactMessage = failedMessage;
        onInteractFailed.Invoke();
    }

    public virtual void OnInteractSucceed()
    {
        //Debug.Log(objectName + " Interact Succeed");
        if (succeedMessage == "") succeedMessage = interactMessage;
        interactMessage = succeedMessage;
        onInteractSucceed.Invoke();
        isInteracted = !isInteracted;
    }

    public virtual void DestroySelf()
    {
        Destroy(gameObject);
    }
}
