using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Light))]
public class FlashlightItem : Item
{
    [Header("Flashlight Settings")]
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
    public GameObject mobileToggleButton; // GameObject yang berisi tombol toggle senter untuk mobile
    
    // Private variables
    private float currentBatteryLife;
    private bool isFlashlightOn = false;
    private Light spotLight;
    private PlayerInteractionNoInventory flashlightPlayerInteraction;
    private bool isInitialized = false;
    private float lastToggleTime = 0f;
    private const float TOGGLE_COOLDOWN = 0.5f; // Cooldown antara toggle untuk mencegah double trigger
    
    // Override OnEnable instead of Awake since Item's Awake is private
    private void OnEnable()
    {
        // Setup spotlight
        spotLight = GetComponent<Light>();
        if (spotLight == null)
        {
            Debug.LogError("Flashlight: No Light component found! Please add a Spot Light to this GameObject.");
        }
        else
        {
            Debug.Log("Flashlight: Light component found and connected.");
            // Sync flashlight state with the light component state
            isFlashlightOn = spotLight.enabled;
            // Make sure spotlight is initially off (unless set otherwise in inspector for testing)
            if (!isFlashlightOn)
            {
                spotLight.enabled = false;
            }
            Debug.Log($"Flashlight: Initial state set to {(isFlashlightOn ? "ON" : "OFF")}.");
        }
        
        // Try to get player interaction system, but don't error if not available yet
        flashlightPlayerInteraction = FindObjectOfType<PlayerInteractionNoInventory>();
        if (flashlightPlayerInteraction == null)
        {
            Debug.LogWarning("Flashlight: PlayerInteractionNoInventory not found yet. Will try again in Update.");
        }
        
        // Initialize battery
        currentBatteryLife = batteryLifeInSeconds;
        Debug.Log($"Flashlight: Battery initialized with {currentBatteryLife} seconds of life.");
        
        isInitialized = true;
    }
    
    private void Start()
    {
        // Setup mobile button if available
        if (flashlightButton != null)
        {
            flashlightButton.onClick.AddListener(ToggleFlashlight);
            Debug.Log("Flashlight: Mobile button connected.");
        }
        
        // Make sure UI is properly set
        UpdateUI();
        
        // Toggle UI visibility based on settings
        if (flashlightUI != null)
        {
            flashlightUI.SetActive(isShowUI);
            Debug.Log($"Flashlight: UI visibility set to {isShowUI}");
        }
        else
        {
            Debug.LogWarning("Flashlight: flashlightUI reference is null. Battery UI will not be shown.");
        }
        
        // Hide mobile toggle button initially
        if (mobileToggleButton != null)
        {
            mobileToggleButton.SetActive(false);
        }
    }
    
    private void Update()
    {
        // Try to get player interaction if not found yet
        if (flashlightPlayerInteraction == null)
        {
            flashlightPlayerInteraction = PlayerInteractionNoInventory.Instance;
            if (flashlightPlayerInteraction == null)
            {
                return; // Skip rest of update until we have a valid instance
            }
        }
        
        // Only allow toggling flashlight when held by player
        bool isBeingHeld = (flashlightPlayerInteraction.holdItem == this);
        
        // Show/hide mobile toggle button based on if flashlight is being held
        if (mobileToggleButton != null)
        {
            mobileToggleButton.SetActive(isBeingHeld);
        }
        
        if (isBeingHeld)
        {
            // Handle keyboard input (PC)
            if (Input.GetKeyDown(toggleKey) && CanToggle())
            {
                Debug.Log($"Flashlight: {toggleKey} key pressed, toggling flashlight.");
                ToggleFlashlight();
            }
            
            // Handle battery drain and intensity changes
            if (isFlashlightOn && spotLight != null)
            {
                if (isUsingPowerDrain)
                {
                    // Drain battery
                    currentBatteryLife -= Time.deltaTime * batteryDrainRate;
                    currentBatteryLife = Mathf.Max(0f, currentBatteryLife);
                    
                    // Turn off flashlight if battery is depleted
                    if (currentBatteryLife <= 0f)
                    {
                        Debug.Log("Flashlight: Battery depleted, turning off.");
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
        else
        {
            // Turn off the flashlight when not held
            if (isFlashlightOn)
            {
                Debug.Log("Flashlight: No longer held by player, turning off.");
                isFlashlightOn = false;
                spotLight.enabled = false;
            }
        }
    }
    
    // Check if enough time has passed since last toggle to allow a new toggle
    private bool CanToggle()
    {
        if (Time.time - lastToggleTime >= TOGGLE_COOLDOWN)
        {
            lastToggleTime = Time.time;
            return true;
        }
        return false;
    }
    
    public void ToggleFlashlight()
    {
        // Prevent rapid toggling
        if (!CanToggle())
        {
            Debug.Log("Flashlight: Toggle ignored - too soon after previous toggle");
            return;
        }
        
        if (spotLight == null)
        {
            Debug.LogError("Flashlight: Cannot toggle, spotLight is null!");
            return;
        }
        
        // If attempting to turn on and battery is depleted, prevent it
        if (!isFlashlightOn && currentBatteryLife <= 0f && isUsingPowerDrain)
        {
            Debug.Log("Flashlight: Cannot turn on, battery is depleted.");
            return;
        }
        
        // Toggle state
        isFlashlightOn = !isFlashlightOn;
        spotLight.enabled = isFlashlightOn;
        Debug.Log($"Flashlight: Toggled to {(isFlashlightOn ? "ON" : "OFF")}");
        
        // Update intensity based on battery level when turning on
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
            Debug.Log($"Flashlight: Set to HIGH intensity ({highIntensity})");
        }
        else if (batteryPercentage > batteryLowThreshold)
        {
            spotLight.intensity = mediumIntensity;
            Debug.Log($"Flashlight: Set to MEDIUM intensity ({mediumIntensity})");
        }
        else
        {
            spotLight.intensity = lowIntensity;
            Debug.Log($"Flashlight: Set to LOW intensity ({lowIntensity})");
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
        Debug.Log($"Flashlight: Added {amount} seconds of battery life. Current battery: {currentBatteryLife}/{batteryLifeInSeconds}");
        UpdateUI();
        
        if (isFlashlightOn && isUsingPowerState)
        {
            UpdateFlashlightIntensity();
        }
    }
    
    // Override the OnInteract method to pick up the flashlight
    public override void OnInteract()
    {
        Debug.Log("Flashlight: Interacted with, picking up...");
        base.OnInteract();
        // The base.OnInteract() will call PlayerInteractionNoInventory.Instance.HoldItem(this)
    }
    
    // Tambahkan method publik untuk mengecek dan menyinkronkan status
    public void SyncFlashlightState()
    {
        if (spotLight != null)
        {
            // Sync state
            isFlashlightOn = spotLight.enabled;
            Debug.Log($"Flashlight: State synced to {(isFlashlightOn ? "ON" : "OFF")}");
            
            // If light is on, update intensity based on battery
            if (isFlashlightOn && isUsingPowerState)
            {
                UpdateFlashlightIntensity();
            }
            
            UpdateUI();
        }
    }
} 