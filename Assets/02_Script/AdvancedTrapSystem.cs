using UnityEngine;

/// <summary>
/// System trap yang lebih advanced dan fleksibel untuk berbagai macam jebakan.
/// Letakkan script ini pada objek kosong sebagai child dari objek trap (tombak, gergaji, dll).
/// Script ini support continuous damage, knockback, effects, dan berbagai tipe trap.
/// </summary>
[AddComponentMenu("Game/Advanced Trap System")]
public class AdvancedTrapSystem : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("Jumlah damage yang diberikan ke player")]
    [SerializeField] private float damageAmount = 20f;

    [Tooltip("Apakah damage diberikan sekali atau berulang saat player masih di area trap?")]
    [SerializeField] private bool isContinuousDamage = false;

    [Tooltip("Interval damage jika continuous (dalam detik)")]
    [SerializeField] private float damageInterval = 1f;

    [Tooltip("Layer yang bisa terkena damage (biasanya Player)")]
    [SerializeField] private LayerMask damageableLayers;

    [Header("Trap Type")]
    [Tooltip("Tipe jebakan untuk keperluan visual/sound")]
    public TrapCategory category = TrapCategory.Sharp;

    [Header("Visual & Audio Effects")]
    [Tooltip("Sound effect saat trap mengenai player (pertama kali)")]
    [SerializeField] private AudioClip hitSound;

    [Tooltip("Sound effect saat continuous damage (berulang)")]
    [SerializeField] private AudioClip continuousSound;

    [Tooltip("Particle effect saat trap mengenai player")]
    [SerializeField] private GameObject hitParticleEffect;

    [Tooltip("Posisi spawn particle (kosongkan untuk posisi player)")]
    [SerializeField] private Transform particleSpawnPoint;

    [Header("Knockback Settings")]
    [Tooltip("Apakah trap memberikan knockback?")]
    [SerializeField] private bool hasKnockback = false;

    [Tooltip("Kekuatan knockback")]
    [SerializeField] private float knockbackForce = 5f;

    [Tooltip("Arah knockback (dari trap ke player jika false, custom direction jika true)")]
    [SerializeField] private bool useCustomKnockbackDirection = false;

    [Tooltip("Custom knockback direction (jika useCustomKnockbackDirection = true)")]
    [SerializeField] private Vector3 customKnockbackDirection = Vector3.back;

    [Header("Advanced Options")]
    [Tooltip("Delay sebelum trap aktif (dalam detik)")]
    [SerializeField] private float activationDelay = 0f;

    [Tooltip("Apakah trap hanya bisa digunakan sekali?")]
    [SerializeField] private bool singleUse = false;

    [Tooltip("Destroy trap object setelah digunakan?")]
    [SerializeField] private bool destroyAfterUse = false;

    [Tooltip("Waktu sebelum destroy (detik)")]
    [SerializeField] private float destroyDelay = 2f;

    [Header("Animation & Visual Feedback")]
    [Tooltip("Animator component untuk trigger animasi")]
    [SerializeField] private Animator trapAnimator;

    [Tooltip("Nama trigger animation saat player terkena")]
    [SerializeField] private string animationTriggerName = "Activate";

    [Header("Debug")]
    [SerializeField] private bool showDebugMessages = true;
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private Color gizmoColor = Color.red;

    // Private variables
    private AudioSource audioSource;
    private float lastDamageTime;
    private bool playerInTrap = false;
    private bool trapUsed = false;
    private bool isActive = false;
    private Collider trapCollider;

    private void Start()
    {
        // Setup audio source
        if (hitSound != null || continuousSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
        }

        // Get collider component
        trapCollider = GetComponent<Collider>();
        if (trapCollider == null)
        {
            trapCollider = GetComponentInChildren<Collider>();
        }

        if (trapCollider == null && showDebugMessages)
        {
            Debug.LogWarning($"[AdvancedTrapSystem] Tidak ada Collider ditemukan pada {gameObject.name}! Tambahkan Collider dengan IsTrigger = true");
        }
        else
        {
            trapCollider.isTrigger = true; // Pastikan collider adalah trigger
        }

        // Aktivasi dengan delay jika perlu
        if (activationDelay > 0)
        {
            Invoke(nameof(ActivateTrap), activationDelay);
        }
        else
        {
            ActivateTrap();
        }
    }

    private void Update()
    {
        // Continuous damage logic
        if (isActive && isContinuousDamage && playerInTrap && !trapUsed)
        {
            if (Time.time >= lastDamageTime + damageInterval)
            {
                ApplyDamageToTarget();
                lastDamageTime = Time.time;

                // Play continuous sound
                if (audioSource != null && continuousSound != null)
                {
                    audioSource.PlayOneShot(continuousSound);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive || trapUsed) return;

        // Check if object is in damageable layers
        if ((damageableLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        if (showDebugMessages)
        {
            Debug.Log($"[AdvancedTrapSystem] {other.gameObject.name} terkena {category} trap pada {gameObject.name}!");
        }

        playerInTrap = true;

        // Trigger animation
        if (trapAnimator != null && !string.IsNullOrEmpty(animationTriggerName))
        {
            trapAnimator.SetTrigger(animationTriggerName);
        }

        // Apply damage immediately
        ApplyDamageToTarget(other.gameObject);
        lastDamageTime = Time.time;

        // Play hit sound
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Spawn particle effect
        if (hitParticleEffect != null)
        {
            Vector3 spawnPos = particleSpawnPoint != null ? particleSpawnPoint.position : other.transform.position;
            Instantiate(hitParticleEffect, spawnPos, Quaternion.identity);
        }

        // Apply knockback
        if (hasKnockback)
        {
            ApplyKnockback(other.gameObject);
        }

        // Single use logic
        if (singleUse)
        {
            trapUsed = true;

            if (destroyAfterUse)
            {
                Destroy(gameObject, destroyDelay);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isActive || trapUsed) return;

        if ((damageableLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            playerInTrap = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((damageableLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            if (showDebugMessages)
            {
                Debug.Log($"[AdvancedTrapSystem] {other.gameObject.name} keluar dari {category} trap");
            }

            playerInTrap = false;
        }
    }

    private void ApplyDamageToTarget(GameObject target = null)
    {
        GameObject targetObject = target;

        // Find target if not provided (untuk continuous damage)
        if (targetObject == null && playerInTrap)
        {
            // Cari player atau object yang masih di dalam trap
            Collider[] colliders = Physics.OverlapSphere(transform.position, 10f, damageableLayers);
            if (colliders.Length > 0)
            {
                targetObject = colliders[0].gameObject;
            }
        }

        if (targetObject != null)
        {
            // Try Health component first
            Health health = targetObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
                return;
            }

            // Try PlayerHealth (jika ada script dengan nama berbeda)
            var playerHealth = targetObject.GetComponent("PlayerHealth");
            if (playerHealth != null)
            {
                // Use reflection to call TakeDamage
                var method = playerHealth.GetType().GetMethod("TakeDamage");
                if (method != null)
                {
                    method.Invoke(playerHealth, new object[] { damageAmount });
                    return;
                }
            }

            // Fallback: Send message
            targetObject.SendMessage("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);

            if (showDebugMessages)
            {
                Debug.Log($"[AdvancedTrapSystem] Memberikan {damageAmount} damage ke {targetObject.name}");
            }
        }
    }

    private void ApplyKnockback(GameObject target)
    {
        Vector3 knockbackDir;

        if (useCustomKnockbackDirection)
        {
            // Gunakan custom direction (relative to trap)
            knockbackDir = transform.TransformDirection(customKnockbackDirection.normalized);
        }
        else
        {
            // Knockback dari trap ke target
            knockbackDir = (target.transform.position - transform.position).normalized;
        }

        knockbackDir.y = 0; // Hanya horizontal knockback (optional)

        // Try CharacterController
        CharacterController cc = target.GetComponent<CharacterController>();
        if (cc != null)
        {
            // Send message untuk custom knockback handler
            target.SendMessage("ApplyKnockback", knockbackDir * knockbackForce, SendMessageOptions.DontRequireReceiver);
        }

        // Try Rigidbody
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
        }
    }

    #region Public Methods

    /// <summary>
    /// Aktivasi trap secara manual
    /// </summary>
    public void ActivateTrap()
    {
        isActive = true;
        if (trapCollider != null)
        {
            trapCollider.enabled = true;
        }

        if (showDebugMessages)
        {
            Debug.Log($"[AdvancedTrapSystem] Trap {gameObject.name} diaktifkan!");
        }
    }

    /// <summary>
    /// Nonaktifkan trap
    /// </summary>
    public void DeactivateTrap()
    {
        isActive = false;
        if (trapCollider != null)
        {
            trapCollider.enabled = false;
        }

        if (showDebugMessages)
        {
            Debug.Log($"[AdvancedTrapSystem] Trap {gameObject.name} dinonaktifkan!");
        }
    }

    /// <summary>
    /// Set damage amount secara runtime
    /// </summary>
    public void SetDamageAmount(float newDamage)
    {
        damageAmount = newDamage;
    }

    /// <summary>
    /// Reset trap (bisa digunakan lagi)
    /// </summary>
    public void ResetTrap()
    {
        trapUsed = false;
        playerInTrap = false;
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        // Draw trap area
        Collider col = GetComponent<Collider>();
        if (col == null) col = GetComponentInChildren<Collider>();

        if (col != null)
        {
            Gizmos.color = gizmoColor;

            if (col is BoxCollider)
            {
                BoxCollider box = col as BoxCollider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphere = col as SphereCollider;
                Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius);
            }
        }

        // Draw knockback direction
        if (hasKnockback && useCustomKnockbackDirection)
        {
            Gizmos.color = Color.yellow;
            Vector3 direction = transform.TransformDirection(customKnockbackDirection.normalized);
            Gizmos.DrawRay(transform.position, direction * 2f);
            Gizmos.DrawSphere(transform.position + direction * 2f, 0.1f);
        }
    }

    #endregion
}

/// <summary>
/// Kategori tipe trap untuk visual/audio reference
/// </summary>
public enum TrapCategory
{
    Sharp,          // Tombak, pisau, duri
    Saw,            // Gergaji berputar
    Spike,          // Duri lancip
    Fire,           // Api
    Poison,         // Racun/gas beracun
    Electricity,    // Listrik
    Crushing,       // Penghancur/pemukul
    Projectile,     // Proyektil (panah, dll)
    Explosive,      // Ledakan
    Other           // Lainnya
}
