using UnityEngine;
using UnityEngine.Events;

public class ItemPlaceholder : Interactable
{
    [Header("Placeholder Settings")]
    public string requiredItemName; // Nama item yang harus dipasang
    public Transform placeholderPoint; // Posisi tempat meletakkan item

    [Header("Debug")]
    public Item currentItem; // Item yang saat ini ditempatkan
    public bool isPlaced;

    private string defaultInteractMessage;
    private PlayerInteractionNoInventory playerInteract;

    public override void Start()
    {
        base.Start();
        playerInteract = PlayerInteractionNoInventory.Instance;
        defaultInteractMessage = interactMessage;
    }

    private void Update()
    {
        if (playerInteract.currentInteractable != this) return;

        if (playerInteract.holdItem != null && playerInteract.holdItem.objectName == requiredItemName)
        {
            interactMessage = $"Place '{requiredItemName}'";
        }
        else
        {
            interactMessage = defaultInteractMessage;
        }
    }

    public override void OnInteract()
    {
        base.OnInteract();

        if (playerInteract.holdItem != null && playerInteract.holdItem.objectName == requiredItemName)
        {
            PlaceItem(playerInteract.holdItem);
        }
        else
        {
            OnInteractFailed();
        }
    }

    private void PlaceItem(Item item)
    {
        currentItem = item;
        playerInteract.holdItem = null;
        currentItem.gameObject.layer = 1 << 0;
        Destroy(currentItem);
        Destroy(item.GetComponent<Rigidbody>());

        item.transform.SetParent(null);
        item.transform.position = placeholderPoint.position;
        item.transform.rotation = placeholderPoint.rotation;
        item.transform.SetParent(placeholderPoint, true);

        OnInteractSucceed();
        isPlaced = true;
        defaultInteractMessage = $"'{requiredItemName}' already placed";
    }
}
