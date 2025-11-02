using UnityEngine;
using System.Collections;
using UnityEngine.AI;

/// <summary>
/// Universal Enemy Health System - Dapat digunakan pada enemy manapun
/// Enemy akan mati saat terkena damage dan bisa respawn otomatis
/// </summary>
public class UniversalEnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool showHealthInInspector = true; // Toggle untuk melihat health di inspector

    [Header("Enemy Settings")]
    [SerializeField] private bool canRespawn = true;
    [SerializeField] private float respawnTime = 5f;
    [SerializeField] private bool destroyOnDeath = false;

    [Header("Death Effects")]
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float deathEffectDuration = 2f;

    [Header("Respawn Settings")]
    [SerializeField] private bool respawnAtOriginalPosition = true;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject respawnEffectPrefab;
    [SerializeField] private AudioClip respawnSound;

    [Header("Optional Components")]
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private string deathAnimationTrigger = "Die";
    [SerializeField] private Collider[] collidersToDisable;
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isDead = false;
    private AudioSource audioSource;
    private NavMeshAgent navAgent;
    private AIHunter aiHunter;

    private void Awake()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Simpan posisi awal untuk respawn
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (deathSound != null || respawnSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Auto-find animator jika tidak di-assign
        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponent<Animator>();
        }

        // Auto-find colliders jika tidak di-assign
        if (collidersToDisable == null || collidersToDisable.Length == 0)
        {
            collidersToDisable = GetComponentsInChildren<Collider>();
        }

        // Cache NavMeshAgent dan AIHunter (jika ada) untuk auto-disable
        navAgent = GetComponent<NavMeshAgent>();
        aiHunter = GetComponent<AIHunter>();
    }

    /// <summary>
    /// Method untuk menerima damage - dipanggil oleh weapon atau trap
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // Kurangi health
        currentHealth -= damage;

        Debug.Log($"[UniversalEnemyHealth] {name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        // Trigger hit reaction (optional - bisa dipanggil event disini)
        OnDamageTaken(damage);

        // Check jika health habis
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Event yang dipanggil saat menerima damage (untuk visual feedback, dll)
    /// </summary>
    private void OnDamageTaken(float damage)
    {
        // Optional: tambahkan visual feedback disini
        // Contoh: flash red, spawn blood effect, play hurt sound, dll

        // Trigger hurt animation jika ada
        if (enemyAnimator != null)
        {
            // Optional: jika ada hurt animation parameter
            // enemyAnimator.SetTrigger("Hurt");
        }
    }

    /// <summary>
    /// Method untuk langsung membunuh enemy
    /// </summary>
    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Play death animation
        if (enemyAnimator != null && !string.IsNullOrEmpty(deathAnimationTrigger))
        {
            enemyAnimator.SetTrigger(deathAnimationTrigger);
        }

        // Play death sound
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Spawn death effect
        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, deathEffectDuration);
        }

        // Disable colliders
        foreach (Collider col in collidersToDisable)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        // Disable scripts
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        // Also try to disable common AI components if they weren't assigned
        if (navAgent != null)
        {
            navAgent.enabled = false;
            Debug.Log($"[UniversalEnemyHealth] {name} disabled NavMeshAgent");
        }

        if (aiHunter != null)
        {
            aiHunter.enabled = false;
            Debug.Log($"[UniversalEnemyHealth] {name} disabled AIHunter");
        }

        // Handle respawn atau destroy
        if (canRespawn)
        {
            StartCoroutine(RespawnRoutine());
        }
        else if (destroyOnDeath)
        {
            Destroy(gameObject, deathEffectDuration);
        }
    }

    private IEnumerator RespawnRoutine()
    {
        // Hide enemy
        SetEnemyVisible(false);

        // Wait for respawn time
        yield return new WaitForSeconds(respawnTime);

        // Respawn
        Respawn();
    }

    private void Respawn()
    {
        isDead = false;

        // Reset health to max
        currentHealth = maxHealth;
        Debug.Log($"[UniversalEnemyHealth] {name} respawned with {currentHealth} health");

        // Reset position
        if (respawnAtOriginalPosition)
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }
        else if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }

        // Show enemy
        SetEnemyVisible(true);

        // Enable colliders
        foreach (Collider col in collidersToDisable)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }

        // Enable scripts
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }

        // Re-enable cached components
        if (navAgent != null)
        {
            navAgent.enabled = true;
            Debug.Log($"[UniversalEnemyHealth] {name} re-enabled NavMeshAgent");
        }

        if (aiHunter != null)
        {
            aiHunter.enabled = true;
            Debug.Log($"[UniversalEnemyHealth] {name} re-enabled AIHunter");
        }

        // Reset animator
        if (enemyAnimator != null)
        {
            enemyAnimator.Rebind();
            enemyAnimator.Update(0f);
        }

        // Spawn respawn effect
        if (respawnEffectPrefab != null)
        {
            GameObject effect = Instantiate(respawnEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Play respawn sound
        if (respawnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(respawnSound);
        }
    }

    private void SetEnemyVisible(bool visible)
    {
        // Toggle renderers
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = visible;
        }

        // Toggle canvas (jika ada)
        Canvas[] canvases = GetComponentsInChildren<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = visible;
        }
    }

    /// <summary>
    /// Alternative method untuk damage dengan amount tertentu (compatibility)
    /// </summary>
    public void Damage(float amount)
    {
        TakeDamage(amount);
    }

    /// <summary>
    /// Method untuk force respawn
    /// </summary>
    public void ForceRespawn()
    {
        if (isDead)
        {
            StopAllCoroutines();
            Respawn();
        }
    }

    /// <summary>
    /// Method untuk force kill tanpa respawn
    /// </summary>
    public void KillPermanently()
    {
        canRespawn = false;
        destroyOnDeath = true;
        Die();
    }

    // Public getters
    public bool IsDead => isDead;
    public bool CanRespawn => canRespawn;
    public float RespawnTime => respawnTime;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float HealthPercentage => maxHealth > 0 ? (currentHealth / maxHealth) * 100f : 0f;

    // Debug
    private void OnDrawGizmosSelected()
    {
        if (respawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(respawnPoint.position, 0.5f);
            Gizmos.DrawLine(transform.position, respawnPoint.position);
        }
    }
}
