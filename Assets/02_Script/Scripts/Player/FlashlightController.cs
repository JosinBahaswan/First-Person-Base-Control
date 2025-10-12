using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light spotLight;
    public KeyCode toggleKey = KeyCode.F;
    public float batteryLifeInSeconds = 300f; // 5 minutes by default
    
    [Header("Power System")]
    public bool isUsingPowerDrain = true;
    public bool isUsingPowerState = true;
    public float batteryDrainRate = 0.05f; // Battery percentage drained per second
    
    [Header("Power State Settings")]
    [Range(0f, 1f)]
    public float batteryMediumThreshold = 0.5f;
    [Range(0f, 1f)]
    public float batteryLowThreshold = 0.25f;
    
    [Header("Light Intensity Settings")]
    public float highIntensity = 1.0f;
    public float mediumIntensity = 0.6f;
    public float lowIntensity = 0.3f;
    
    [Header("UI Settings")]
    public bool isShowUI = true;
    public Image batteryFillImage;
    public TextMeshProUGUI batteryPercentText;
    public GameObject flashlightUI;
    
    // Mobile controls
    public Button flashlightButton;
    
    // Private variables
    private float currentBatteryLife;
    private bool isFlashlightOn = false;
    
    private void Start()
    {
        // Initialize with full battery
        currentBatteryLife = batteryLifeInSeconds;
        
        // Setup mobile button if available
        if (flashlightButton != null)
        {
            flashlightButton.onClick.AddListener(ToggleFlashlight);
        }
        
        // Make sure UI is properly set
        UpdateUI();
        
        // Initialize flashlight (start off)
        if (spotLight != null)
        {
            spotLight.enabled = false;
        }
        else
        {
            Debug.LogError("Spot Light reference is missing! Please assign it in the inspector.");
        }
        
        // Toggle UI visibility based on settings
        if (flashlightUI != null)
        {
            flashlightUI.SetActive(isShowUI);
        }
    }
    
    private void Update()
    {
        // Handle keyboard input (PC)
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight();
        }
        
        // Handle battery drain and intensity changes
        if (isFlashlightOn && spotLight != null)
        {
            if (isUsingPowerDrain)
            {
                // Drain battery
                currentBatteryLife -= Time.deltaTime;
                currentBatteryLife = Mathf.Max(0f, currentBatteryLife);
                
                // Turn off flashlight if battery is depleted
                if (currentBatteryLife <= 0f)
                {
                    isFlashlightOn = false;
                    spotLight.enabled = false;
                }
                
                // Update flashlight intensity based on battery level
                if (isUsingPowerState)
                {
                    UpdateFlashlightIntensity();
                }
            }
            
            // Update UI
            UpdateUI();
        }
    }
    
    public void ToggleFlashlight()
    {
        if (spotLight == null) return;
        
        // Don't allow turning on if battery is depleted
        if (!isFlashlightOn && currentBatteryLife <= 0f && isUsingPowerDrain)
        {
            return;
        }
        
        isFlashlightOn = !isFlashlightOn;
        spotLight.enabled = isFlashlightOn;
        
        if (isFlashlightOn && isUsingPowerState)
        {
            UpdateFlashlightIntensity();
        }
        
        UpdateUI();
    }
    
    private void UpdateFlashlightIntensity()
    {
        float batteryPercentage = currentBatteryLife / batteryLifeInSeconds;
        
        if (batteryPercentage > batteryMediumThreshold)
        {
            spotLight.intensity = highIntensity;
        }
        else if (batteryPercentage > batteryLowThreshold)
        {
            spotLight.intensity = mediumIntensity;
        }
        else
        {
            spotLight.intensity = lowIntensity;
        }
    }
    
    private void UpdateUI()
    {
        if (!isShowUI || flashlightUI == null) return;
        
        float batteryPercentage = currentBatteryLife / batteryLifeInSeconds;
        
        if (batteryFillImage != null)
        {
            batteryFillImage.fillAmount = batteryPercentage;
            
            // Change color based on battery level
            if (batteryPercentage > batteryMediumThreshold)
            {
                batteryFillImage.color = Color.green;
            }
            else if (batteryPercentage > batteryLowThreshold)
            {
                batteryFillImage.color = Color.yellow;
            }
            else
            {
                batteryFillImage.color = Color.red;
            }
        }
        
        if (batteryPercentText != null)
        {
            batteryPercentText.text = Mathf.RoundToInt(batteryPercentage * 100) + "%";
        }
    }
    
    // Public method to add battery life (for battery pickup items)
    public void AddBattery(float amount)
    {
        currentBatteryLife += amount;
        currentBatteryLife = Mathf.Min(currentBatteryLife, batteryLifeInSeconds);
        UpdateUI();
        
        if (isFlashlightOn && isUsingPowerState)
        {
            UpdateFlashlightIntensity();
        }
    }
} 