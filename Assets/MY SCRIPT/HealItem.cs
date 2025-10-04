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

    void UseHealItem()
    {
        // Get player's Health component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Health playerHealth = player.GetComponent<Health>();

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
        bool isHoldingThis = PlayerInteractionNoInventory.Instance != null && PlayerInteractionNoInventory.Instance.holdItem == this;
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
            // Not holding anymore => ensure UI is hidden and listener removed
            HideUseButton();
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
        if (!useButton.gameObject.activeSelf)
            useButton.gameObject.SetActive(true);
        if (!useListenerBound)
        {
            useButton.onClick.AddListener(UseHealItem);
            useListenerBound = true;
        }
    }

    private void HideUseButton()
    {
        if (useButton == null) return;
        if (useListenerBound)
        {
            useButton.onClick.RemoveListener(UseHealItem);
            useListenerBound = false;
        }
        if (useButton.gameObject.activeSelf)
            useButton.gameObject.SetActive(false);
    }
}
