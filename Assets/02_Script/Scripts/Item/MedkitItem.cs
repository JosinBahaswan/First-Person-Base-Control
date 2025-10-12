using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Game/Items/Medkit Item")]
public class MedkitItem : Item
{
    [Header("Medkit Settings")]
    [Tooltip("Amount of health to restore")]
    [SerializeField] private float healAmount = 50f;
    [Tooltip("Time it takes to use the medkit")]
    [SerializeField] private float useDelay = 1.5f;
    [Tooltip("Key to use the medkit")]
    [SerializeField] private KeyCode useKey = KeyCode.F;
    
    [Header("UI References")]
    [SerializeField] private GameObject medkitUI;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Button useMedkitButton; // For mobile
    
    // Private variables
    private bool isUsing = false;
    private float useProgress = 0f;
    private Health playerHealth;
    private bool isInitialized = false;
    private float lastUseAttemptTime = 0f;
    private const float USE_COOLDOWN = 0.5f;
    
    private void OnEnable()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        if (isInitialized) return;
        
        // Get references
        if (PlayerInteractionNoInventory.Instance != null)
        {
            playerHealth = PlayerInteractionNoInventory.Instance.GetComponent<Health>();
        }
        
        // Setup mobile controls if they exist
        if (useMedkitButton != null)
        {
            useMedkitButton.onClick.AddListener(AttemptUseMedkit);
        }
        
        isInitialized = true;
    }
    
    private void Update()
    {
        if (!isInitialized) return;
        
        // Check for input
        if ((Input.GetKeyDown(useKey) || isUsing) && PlayerInteractionNoInventory.Instance.holdItem == this)
        {
            AttemptUseMedkit();
        }
        
        // Update healing progress
        if (isUsing)
        {
            useProgress += Time.deltaTime / useDelay;
            UpdateUI();
            
            if (useProgress >= 1f)
            {
                CompleteMedkitUse();
            }
        }
    }
    
    private void AttemptUseMedkit()
    {
        // Prevent spam usage
        if (Time.time - lastUseAttemptTime < USE_COOLDOWN) return;
        lastUseAttemptTime = Time.time;
        
        // Check if we can use medkit
        if (playerHealth == null || playerHealth.CurrentHealth >= playerHealth.MaxHealth || isUsing)
        {
            return;
        }
        
        // Start using medkit
        isUsing = true;
        useProgress = 0f;
        UpdateUI();
    }
    
    private void CompleteMedkitUse()
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
        }
        
        // Reset state
        isUsing = false;
        useProgress = 0f;
        UpdateUI();
        
        // Remove the medkit after use
        if (PlayerInteractionNoInventory.Instance.holdItem == this)
        {
            PlayerInteractionNoInventory.Instance.DropItem(this);
        }
        Destroy(gameObject);
    }
    
    private void UpdateUI()
    {
        if (medkitUI == null || progressSlider == null) return;
        
        progressSlider.value = useProgress;
        medkitUI.SetActive(isUsing);
    }
    
    private void OnDisable()
    {
        if (isUsing)
        {
            // Cancel healing if disabled
            isUsing = false;
            useProgress = 0f;
            UpdateUI();
        }
    }
    
    public override void OnInteract()
    {
        base.OnInteract();
        
        if (isUsing)
        {
            isUsing = false;
            useProgress = 0f;
            UpdateUI();
        }
    }
}