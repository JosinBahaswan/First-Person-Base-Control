using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Mobile controls untuk HoldableWeapon system
/// Attach ke Canvas atau UI Manager
/// </summary>
public class MobileHoldableWeaponControls : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button fireButton;
    [SerializeField] private Button reloadButton;

    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = Color.yellow;
    [SerializeField] private Color disabledColor = Color.gray;

    private HoldableWeapon currentWeapon;
    private Image fireButtonImage;
    private Image reloadButtonImage;

    void Start()
    {
        SetupButtons();
        Debug.Log("[MobileHoldableWeaponControls] Initialized");
    }

    void Update()
    {
        // Update current weapon reference
        if (PlayerInteractionNoInventory.Instance != null && PlayerInteractionNoInventory.Instance.holdItem != null)
        {
            HoldableWeapon newWeapon = PlayerInteractionNoInventory.Instance.holdItem.GetComponent<HoldableWeapon>();
            if (newWeapon != currentWeapon)
            {
                currentWeapon = newWeapon;
                if (currentWeapon != null)
                {
                    Debug.Log($"[MobileHoldableWeaponControls] Weapon detected: {currentWeapon.WeaponName}");
                }
            }
        }
        else
        {
            if (currentWeapon != null)
            {
                Debug.Log("[MobileHoldableWeaponControls] No weapon held");
            }
            currentWeapon = null;
        }

        // Update button states
        UpdateButtonVisuals();
    }

    private void SetupButtons()
    {
        // Setup Fire Button dengan EventTrigger untuk hold functionality
        if (fireButton != null)
        {
            fireButtonImage = fireButton.GetComponent<Image>();
            
            // PENTING: Remove Button component onClick listener untuk avoid conflict
            fireButton.onClick.RemoveAllListeners();

            // Add EventTrigger component
            EventTrigger trigger = fireButton.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = fireButton.gameObject.AddComponent<EventTrigger>();

            // Clear existing triggers
            trigger.triggers.Clear();

            // PointerDown event
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { OnFireButtonDown(); });
            trigger.triggers.Add(pointerDown);

            // PointerUp event
            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { OnFireButtonUp(); });
            trigger.triggers.Add(pointerUp);

            // PointerExit event (untuk kalau finger slide keluar dari button)
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnFireButtonUp(); });
            trigger.triggers.Add(pointerExit);

            Debug.Log("[MobileControls] Fire button EventTrigger setup complete");
        }
        else
        {
            Debug.LogError("[MobileControls] Fire button is NULL! Assign in Inspector.");
        }

        // Setup Reload Button
        if (reloadButton != null)
        {
            reloadButtonImage = reloadButton.GetComponent<Image>();
            reloadButton.onClick.RemoveAllListeners(); // Clear existing
            reloadButton.onClick.AddListener(OnReloadButtonPressed);

            Debug.Log("[MobileControls] Reload button setup complete");
        }
        else
        {
            Debug.LogError("[MobileControls] Reload button is NULL! Assign in Inspector.");
        }
    }

    private void OnFireButtonDown()
    {
        if (currentWeapon != null)
        {
            currentWeapon.OnFireButtonDown();

            // Visual feedback
            if (fireButtonImage != null)
                fireButtonImage.color = pressedColor;
        }
    }

    private void OnFireButtonUp()
    {
        if (currentWeapon != null)
        {
            currentWeapon.OnFireButtonUp();
        }

        // Visual feedback
        if (fireButtonImage != null)
            fireButtonImage.color = normalColor;
    }

    private void OnReloadButtonPressed()
    {
        Debug.Log($"[MobileControls] Reload button pressed. HasWeapon: {currentWeapon != null}");

        if (currentWeapon != null)
        {
            currentWeapon.OnReloadButtonPressed();
        }
        else
        {
            Debug.LogWarning("[MobileControls] No weapon to reload!");
        }
    }

    private void UpdateButtonVisuals()
    {
        bool hasWeapon = (currentWeapon != null);

        // Fire button - SELALU interactable jika ada weapon (biar bisa hold dan dengar empty sound)
        if (fireButton != null)
        {
            fireButton.interactable = hasWeapon && !currentWeapon.IsReloading;

            if (fireButtonImage != null)
            {
                if (!hasWeapon || currentWeapon.IsReloading)
                    fireButtonImage.color = disabledColor;
                else if (currentWeapon.CurrentAmmo <= 0)
                    fireButtonImage.color = Color.red;
                else
                    fireButtonImage.color = normalColor;
            }
        }

        // Reload button
        if (reloadButton != null)
        {
            reloadButton.interactable = hasWeapon && !currentWeapon.IsReloading && currentWeapon.CurrentAmmo < currentWeapon.MaxAmmo && currentWeapon.ReserveAmmo > 0;

            if (reloadButtonImage != null)
            {
                if (!hasWeapon || currentWeapon.IsReloading || currentWeapon.ReserveAmmo <= 0)
                    reloadButtonImage.color = disabledColor;
                else
                    reloadButtonImage.color = normalColor;
            }
        }
    }
}
