using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Singleton instance
    public static PlayerManager Instance { get; private set; }

    [Header("Player Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;
    public float staminaDepletionRate = 20f;

    [Header("Player State")]
    public bool isRunning = false;
    public bool isDead = false;
    public bool hasFlashlight = true;
    public float flashlightBattery = 100f;
    public bool isFlashlightOn = false;

    [Header("Player Inventory")]
    public int batteries = 0;
    public int keys = 0;
    public int collectibles = 0;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePlayer();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePlayer()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        isDead = false;
    }

    private void Update()
    {
        HandleStaminaRegeneration();
    }

    private void HandleStaminaRegeneration()
    {
        if (!isRunning && currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + (staminaRegenRate * Time.deltaTime));
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    public void UseStamina(float amount)
    {
        currentStamina = Mathf.Max(0f, currentStamina - amount);
    }

    public bool HasEnoughStamina(float amount)
    {
        return currentStamina >= amount;
    }

    public void AddBattery()
    {
        batteries++;
    }

    public bool UseBattery()
    {
        if (batteries > 0)
        {
            batteries--;
            flashlightBattery = 100f;
            return true;
        }
        return false;
    }

    public void AddKey()
    {
        keys++;
    }

    public bool UseKey()
    {
        if (keys > 0)
        {
            keys--;
            return true;
        }
        return false;
    }

    public void AddCollectible()
    {
        collectibles++;
    }

    private void Die()
    {
        isDead = true;
        // Trigger death events or game over sequence
    }

    public void ResetPlayer()
    {
        InitializePlayer();
        batteries = 0;
        keys = 0;
        collectibles = 0;
        flashlightBattery = 100f;
        isFlashlightOn = false;
    }
}
