using UnityEngine;
using UnityEngine.UI;

public class FlashlightToggleUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button toggleButton;
    public GameObject toggleButtonObject;
    
    [Header("Optional")]
    public Sprite flashlightOnIcon;
    public Sprite flashlightOffIcon;
    
    [Header("Settings")]
    public float syncInterval = 0.5f; // Interval untuk sinkronisasi status flashlight
    
    private PlayerInteractionNoInventory flashlightTogglePlayerInteraction;
    private FlashlightItem currentFlashlight;
    private Image buttonImage;
    private bool didInitialSync = false;
    private float lastUpdateTime = 0f;
    
    private void Start()
    {
        if (toggleButton == null && toggleButtonObject != null)
        {
            toggleButton = toggleButtonObject.GetComponent<Button>();
        }
        
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleFlashlight);
            buttonImage = toggleButton.GetComponent<Image>();
            
            // Hide by default
            if (toggleButtonObject != null)
            {
                toggleButtonObject.SetActive(false);
            }
            else
            {
                toggleButton.gameObject.SetActive(false);
            }
        }
    }
    
    private void Update()
    {
        // Limit how often we run the update logic to reduce overhead
        if (Time.time - lastUpdateTime < 0.1f) return;
        lastUpdateTime = Time.time;
        
        // Try to get player interaction if not found yet
        if (flashlightTogglePlayerInteraction == null)
        {
            flashlightTogglePlayerInteraction = PlayerInteractionNoInventory.Instance;
            if (flashlightTogglePlayerInteraction == null) return;
        }
        
        // Check if player is holding a flashlight
        bool isHoldingFlashlight = false;
        
        if (flashlightTogglePlayerInteraction.holdItem != null && 
            flashlightTogglePlayerInteraction.holdItem is FlashlightItem)
        {
            FlashlightItem newFlashlight = (FlashlightItem)flashlightTogglePlayerInteraction.holdItem;
            
            // If this is a new flashlight instance or first detection
            if (currentFlashlight != newFlashlight)
            {
                currentFlashlight = newFlashlight;
                didInitialSync = true;
                
                // Sync flashlight state when first detected
                currentFlashlight.SyncFlashlightState();
            }
            
            isHoldingFlashlight = true;
            
            // Update icon based on flashlight state if icons are provided
            if (buttonImage != null && flashlightOnIcon != null && flashlightOffIcon != null)
            {
                // Use the light component to determine state
                Light flashlightLight = currentFlashlight.GetComponent<Light>();
                if (flashlightLight != null)
                {
                    buttonImage.sprite = flashlightLight.enabled ? flashlightOnIcon : flashlightOffIcon;
                }
            }
        }
        else
        {
            // Only reset currentFlashlight if it's no longer held
            if (currentFlashlight != null && 
                (flashlightTogglePlayerInteraction.holdItem == null || 
                 flashlightTogglePlayerInteraction.holdItem != currentFlashlight))
            {
                currentFlashlight = null;
                didInitialSync = false;
            }
        }
        
        // Show/hide toggle button based on whether a flashlight is being held
        if (toggleButtonObject != null)
        {
            toggleButtonObject.SetActive(isHoldingFlashlight);
        }
        else if (toggleButton != null)
        {
            toggleButton.gameObject.SetActive(isHoldingFlashlight);
        }
    }
    
    public void ToggleFlashlight()
    {
        if (currentFlashlight != null)
        {
            currentFlashlight.ToggleFlashlight();
            
            // No need to force sync immediately after toggle
            // The toggling already sets all states correctly
        }
    }
} 