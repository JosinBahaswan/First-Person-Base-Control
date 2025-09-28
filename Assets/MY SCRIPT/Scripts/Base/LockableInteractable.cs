using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class LockableInteractable : Interactable
{
    [Header("Lockable Interactable")]
    public bool isLocked = false;
    public bool isRequiredPlaceholder = false; // Cek apakah menggunakan placeholder
    public bool isChildBlocked = false;
    public bool isRequiredItem = false;
    public bool isInteractActivation = false;
    public string requiredItemName;
    public List<ItemPlaceholder> requiredPlaceholders; // Daftar placeholder yang diperlukan
    public List<Transform> childBlockeds;
    public Transform childBlockParent;
    public List<Interactable> interactActivations;

    [Header("Events")]
    public UnityEvent OnLocked;
    public UnityEvent OnUnlocked;
    public UnityEvent OnOpened;
    public UnityEvent OnClosed;

    protected bool isOpen = false;

    public override void Start()
    {
        base.Start();

        if (playerInteraction.isAllowUnlockedAll) isLocked = false;
    }

    public override void OnInteract()
    {
        base.OnInteract();

        if (isLocked)
        {
            if (isRequiredPlaceholder && CheckRequiredPlaceholder())
            {
                Unlock();
                return; // Stop further processing once unlocked
            }

            if (isInteractActivation)
            {
                //Unlock();
                if (CheckInteractActivations())
                {
                    ToggleOpenClose();
                    OnInteractSucceed();
                }
                else
                {
                    OnInteractFailed();
                }
                return;
            }

            if (isRequiredItem && playerInteraction.IsHoldItemName(requiredItemName))
            {
                playerInteraction.ConsumeItem();
                Unlock();
                return; // Stop further processing once unlocked
            }

            if (isChildBlocked && CheckChildBlocker())
            {
                Unlock();
                return; // Stop further processing once unlocked
            }

            Locked(); // If still locked, call Locked
            return;
        }

        ToggleOpenClose();
        OnInteractSucceed();
    }

    private bool CheckRequiredPlaceholder()
    {
        foreach (var placeholder in requiredPlaceholders)
        {
            if (!placeholder.isPlaced)
            {
                return false;
            }
        }

        foreach (var placeholder in requiredPlaceholders)
        {
            Destroy(placeholder);
        }

        return true;
    }

    private bool CheckChildBlocker()
    {
        foreach (Transform child in childBlockeds)
        {
            if (child.parent == childBlockParent)
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckInteractActivations()
    {
        foreach (var interactable in interactActivations)
        {
            if (!interactable.isInteracted)
            {
                return false;
            }
        }
        return true;
    }

    protected void Unlock()
    {
        isLocked = false;
        OnUnlocked?.Invoke();
        OnInteractSucceed();
    }

    protected void Locked()
    {
        OnLocked?.Invoke();
        OnInteractFailed();
    }

    public abstract void ToggleOpenClose(); // Method abstrak untuk membuka/menutup objek
}
