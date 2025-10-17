using UnityEngine;
using System;

[AddComponentMenu("Game/Health")]
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Maximum health value.")]
    [SerializeField] private float maxHealth = 100f;
    [Tooltip("Current health value.")]
    [SerializeField] private float currentHealth = 100f;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    // Event: Invoked when health changes (current, max)
    public event Action<float, float> OnHealthChanged;
    // Event: Invoked when health reaches zero
    public event Action OnDeath;

    private bool isDead = false;
    private float damageImmuneUntil = 0f; // timestamp until which damage is ignored

    private void Awake()
    {
        // Debug.Log($"=== HEALTH COMPONENT INITIALIZED ===");
        // Debug.LogError($"GameObject: {gameObject.name}");
        // Debug.LogError($"Initial Health: {currentHealth}/{maxHealth}");
        // Debug.LogError($"Has CharacterController?: {GetComponent<CharacterController>() != null}");
        // Debug.LogError($"Layer: {gameObject.layer}");
        // Clamp current health at start
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Apply damage to this object.
    /// </summary>
    public virtual void TakeDamage(float amount)
    {
        Debug.LogError($"=== Health Debug ===");
        Debug.LogError($"GameObject: {gameObject.name}");
        Debug.LogError($"isDead: {isDead}");
        Debug.LogError($"amount: {amount}");
        Debug.LogError($"currentHealth sebelum: {currentHealth}");
        Debug.LogError($"maxHealth: {maxHealth}");

        if (isDead) return;
        // Ignore damage if we are in a short immunity window (e.g., right after healing)
        if (Time.time < damageImmuneUntil) return;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        Debug.LogError($"currentHealth setelah: {currentHealth}");
        Debug.LogError($"OnHealthChanged null?: {OnHealthChanged == null}");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Heal this object.
    /// </summary>
    public void Heal(float amount, float postHealGraceSeconds = 0f)
    {
        if (isDead) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (postHealGraceSeconds > 0f)
        {
            damageImmuneUntil = Mathf.Max(damageImmuneUntil, Time.time + postHealGraceSeconds);
        }
    }

    /// <summary>
    /// Called when health reaches zero. Override for custom death logic.
    /// </summary>
    public virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        OnDeath?.Invoke();
        // Optionally: Destroy(gameObject);
    }

    /// <summary>
    /// Reset health to max (for respawn, etc).
    /// </summary>
    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
