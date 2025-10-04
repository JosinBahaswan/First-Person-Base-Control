using UnityEngine;
using UnityEngine.UI;

public class FlashlightToggleButton : MonoBehaviour
{
    public Button toggleButton;
    private PlayerInteractionNoInventory playerInteraction;
    
    private void Start()
    {
        playerInteraction = PlayerInteractionNoInventory.Instance;
        
        if (toggleButton == null)
        {
            toggleButton = GetComponent<Button>();
        }
        
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleFlashlight);
        }
    }
    
    private void Update()
    {
        // Show/hide button based on whether player is holding a flashlight
        if (playerInteraction != null)
        {
            bool isHoldingFlashlight = (playerInteraction.holdItem != null && 
                                       playerInteraction.holdItem is FlashlightItem);
            
            // Only show button if player is holding a flashlight
            gameObject.SetActive(isHoldingFlashlight);
        }
    }
    
    public void ToggleFlashlight()
    {
        if (playerInteraction != null && 
            playerInteraction.holdItem != null && 
            playerInteraction.holdItem is FlashlightItem)
        {
            FlashlightItem flashlight = (FlashlightItem)playerInteraction.holdItem;
            flashlight.ToggleFlashlight();
        }
    }
} 