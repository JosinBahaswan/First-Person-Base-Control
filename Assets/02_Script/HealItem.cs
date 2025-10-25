using UnityEngine;
using UnityEngine.UI;

public class HealItem : Item
{
    public enum HealSize
    {
        Small,
        Large
    }

    public HealSize healSize = HealSize.Small;
    public float smallHealAmount = 25f;
    public float largeHealAmount = 75f;
    public GameObject pickupEffect; // Optional particle effect

    [Header("UI")]
    [Tooltip("Assign the 'Use' button here (copied from Interact).")]
    [SerializeField] private Button useButton;

    // Internal flag to avoid adding multiple listeners
    private bool useListenerBound = false;

    public override void Start()
    {
        base.Start();
        // Default state: Use button hidden until item is held
        if (useButton != null && useButton.gameObject.activeSelf)
        {
            useButton.gameObject.SetActive(false);
        }
        UnwireUseButton();
    }

    bool IsHeldByPlayer()
    {
        var pii = PlayerInteractionNoInventory.Instance;
        if (pii == null || pii.holdItem == null) return false;
        var held = pii.holdItem;
        var thisItemOnSelf = GetComponent<Item>();
        var thisItemInParent = GetComponentInParent<Item>();
        var thisItemInChildren = GetComponentInChildren<Item>();

        if (held == thisItemOnSelf || held == thisItemInParent || held == thisItemInChildren)
            return true;

        if (held.transform == transform || transform.IsChildOf(held.transform) || held.transform.IsChildOf(transform))
            return true;

        return false;
    }

    void UnwireUseButton()
    {
        if (useButton == null) return;
        // Only remove our listener, do not hide the global button here
        useButton.onClick.RemoveListener(UseHealItem);
        useListenerBound = false;
    }

    void SetInteractButtonVisible(bool visible)
    {
        // Biarkan PlayerInteractionNoInventory yang mengatur visibility tombol interact
    }

    void UseHealItem()
    {
        // Resolve player's Health via PlayerInteractionNoInventory to avoid tag/hierarchy mismatches
        Health playerHealth = null;
        var piiRef = PlayerInteractionNoInventory.Instance;
        if (piiRef != null)
        {
            playerHealth = piiRef.GetComponent<Health>();
            if (playerHealth == null) playerHealth = piiRef.GetComponentInChildren<Health>(true);
            if (playerHealth == null) playerHealth = piiRef.GetComponentInParent<Health>();
        }

        if (playerHealth != null)
        {
            // Apply healing
            float healAmount = healSize == HealSize.Small ? smallHealAmount : largeHealAmount;
            playerHealth.Heal(healAmount);

            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            // Destroy the gameObject
            PlayerInteractionNoInventory.Instance.ConsumeItem();
        }

        // Hide and clean up UI after using
        HideUseButton();
    }

    private void Update()
    {
        // Jika item sedang dipegang dan player menekan tombol use (misalnya E)
        bool isHoldingThis = IsHeldByPlayer();
        if (isHoldingThis)
        {
            // Show the Use button and wire it
            ShowUseButton();
            if (Input.GetKeyDown(KeyCode.E))
            {
                UseHealItem();
            }
        }
        else
        {
            // Not holding anymore => remove our listener only; do not hide global button
            UnwireUseButton();
            // If no item is held or held item is not a HealItem, hide the Use button to avoid covering Interact
            var pii2 = PlayerInteractionNoInventory.Instance;
            bool shouldHideUse = pii2 == null || pii2.holdItem == null || pii2.holdItem.GetComponent<HealItem>() == null;
            if (shouldHideUse)
            {
                HideUseButton();
            }
            // Re-show Interact button when not holding this item
            SetInteractButtonVisible(true);
        }
    }

    public override void OnInteract()
    {
        // Interact hanya untuk mengambil item, tidak untuk menggunakan/heal
        // Healing hanya terjadi ketika tombol Use (E) ditekan saat item sedang dipegang
        base.OnInteract();
        // Saat diambil, tampilkan tombol use (akan di-maintain oleh Update())
        ShowUseButton();
    }

    private void ShowUseButton()
    {
        if (useButton == null) return;
        // Ensure parent hierarchy is active
        Transform p = useButton.transform.parent;
        while (p != null)
        {
            if (!p.gameObject.activeSelf) p.gameObject.SetActive(true);
            p = p.parent;
        }
        // Ensure CanvasGroup allows interaction
        var cg = useButton.GetComponentInParent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
        if (!useButton.gameObject.activeSelf)
            useButton.gameObject.SetActive(true);
        if (!useListenerBound)
        {
            useButton.onClick.AddListener(UseHealItem);
            useListenerBound = true;
        }
        // Hide Interact button while Use is available to avoid overlap
        SetInteractButtonVisible(false);
    }

    private void HideUseButton()
    {
        if (useButton == null) return;
        // Selalu remove listener dan sembunyikan tombol saat kita benar-benar selesai (mis. setelah use)
        useButton.onClick.RemoveListener(UseHealItem);
        useListenerBound = false;
        if (useButton.gameObject.activeSelf)
            useButton.gameObject.SetActive(false);
        // Re-show Interact button after using
        SetInteractButtonVisible(true);
    }

    private void OnDisable()
    {
        // Pastikan listener di-reset saat object di-disable, tapi jangan menyembunyikan tombol global
        UnwireUseButton();
    }

    private void OnDestroy()
    {
        // Pastikan listener di-reset saat object dihancurkan, tapi jangan menyembunyikan tombol global
        UnwireUseButton();
    }
}
