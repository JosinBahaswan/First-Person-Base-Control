using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : Interactable
{
    [Header("Interact Type")]
    public bool isOnce;

    [Header("Requirement")]
    public bool isNeedItem;
    public int needItemTotal;
    public string needItemCategory;
    public UnityEvent onRequirementAdded;

    [Header("Debug")]
    public bool isOnceInteracted;
    public int currentNeededItem;

    public override void OnInteract()
    {
        base.OnInteract();

        if (isNeedItem)
        {
            if (playerInteraction.IsHoldItemCategory(needItemCategory))
            {
                currentNeededItem++;
                playerInteraction.ConsumeItem();
                OnRequirementAdded();
            }
            else
            {
                OnInteractFailed();
            }
        }
        else
        {
            OnInteractSucceed();
        }
    }

    public override void OnInteractSucceed()
    {
        if (isOnce && isOnceInteracted) return;

        base.OnInteractSucceed();
        isOnceInteracted = true;
    }

    public void OnRequirementAdded()
    {
        Debug.Log(objectName + " Requirement Added");
        onRequirementAdded.Invoke();
        if (currentNeededItem >= needItemTotal)
        {
            isNeedItem = false;
        }
    }
}
