using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlashlightUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image batteryIcon;
    public Image batteryFillImage;
    public TextMeshProUGUI batteryPercentText;
    public Button mobileButton;
    
    [Header("References")]
    public FlashlightController flashlightController;
    
    private void Start()
    {
        if (flashlightController == null)
        {
            Debug.LogError("Flashlight Controller reference is missing! Please assign it in the inspector.");
            return;
        }
        
        // Connect UI elements to controller
        if (batteryFillImage != null)
        {
            flashlightController.batteryFillImage = batteryFillImage;
        }
        
        if (batteryPercentText != null)
        {
            flashlightController.batteryPercentText = batteryPercentText;
        }
        
        if (mobileButton != null)
        {
            flashlightController.flashlightButton = mobileButton;
        }
        
        // Set the flashlight UI reference
        flashlightController.flashlightUI = this.gameObject;
    }
} 