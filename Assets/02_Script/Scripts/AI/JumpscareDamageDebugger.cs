using UnityEngine;

/// <summary>
/// Debug helper untuk test jumpscare damage
/// Attach ke enemy yang sama dengan AIHunterSupport
/// Press F7 untuk manual test damage
/// </summary>
public class JumpscareDamageDebugger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AIHunterSupport hunterSupport;

    [Header("Debug Settings")]
    [SerializeField] private KeyCode testDamageKey = KeyCode.F7;
    [SerializeField] private float testDamageAmount = 10f;

    private void Awake()
    {
        // Auto-find AIHunterSupport jika tidak di-assign
        if (hunterSupport == null)
        {
            hunterSupport = GetComponent<AIHunterSupport>();
        }
    }

    private void Update()
    {
        // Test damage langsung ke player
        if (Input.GetKeyDown(testDamageKey))
        {
            TestDirectDamageToPlayer();
        }
    }

    /// <summary>
    /// Test damage langsung ke player (bypass jumpscare)
    /// Support PlayerManager DAN Health component
    /// </summary>
    private void TestDirectDamageToPlayer()
    {
        Debug.Log("=================================================");
        Debug.Log("🧪 [JumpscareDamageDebugger] MANUAL DAMAGE TEST");
        Debug.Log("=================================================");

        // === METHOD 1: Try PlayerManager Singleton FIRST ===
        PlayerManager playerManager = PlayerManager.Instance;
        if (playerManager != null)
        {
            Debug.Log("✅ Found PlayerManager Singleton!");
            Debug.Log($"   Current Health: {playerManager.currentHealth}");
            Debug.Log($"   Max Health: {playerManager.maxHealth}");
            Debug.Log($"   Is Dead: {playerManager.isDead}");

            float healthBefore = playerManager.currentHealth;
            Debug.Log($"\n💥 Applying {testDamageAmount} damage via PlayerManager...");
            playerManager.TakeDamage(testDamageAmount);
            float healthAfter = playerManager.currentHealth;

            Debug.Log($"✅ DAMAGE APPLIED!");
            Debug.Log($"   Health BEFORE: {healthBefore}");
            Debug.Log($"   Health AFTER: {healthAfter}");
            Debug.Log($"   Actual damage taken: {healthBefore - healthAfter}");

            if (healthBefore == healthAfter)
            {
                Debug.LogError("⚠️ WARNING: Health did NOT change! Check PlayerManager.TakeDamage() method");
            }

            Debug.Log("=================================================\n");
            return; // Exit jika berhasil dengan PlayerManager
        }
        else
        {
            Debug.LogWarning("⚠️ PlayerManager.Instance is NULL, trying Health component...");
        }

        // === METHOD 2: Try Health Component (Fallback) ===
        // Cari player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("❌ Player NOT FOUND with tag 'Player'");
            Debug.LogError("Available GameObjects in scene:");
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.ToLower().Contains("player"))
                {
                    Debug.LogError($"  - {obj.name} (Tag: {obj.tag})");
                }
            }
            return;
        }

        Debug.Log($"✅ Found player: {player.name}");
        Debug.Log($"   Tag: {player.tag}");
        Debug.Log($"   Layer: {LayerMask.LayerToName(player.layer)}");

        // Cari Health component
        Health health = player.GetComponent<Health>();
        if (health == null)
        {
            Debug.LogWarning("Health not on root, checking children...");
            health = player.GetComponentInChildren<Health>();
        }
        if (health == null)
        {
            Debug.LogWarning("Health not in children, checking parent...");
            health = player.GetComponentInParent<Health>();
        }

        if (health == null)
        {
            Debug.LogError("❌ NO Health system found!");
            Debug.LogError("Neither PlayerManager.Instance nor Health component exists!");
            Debug.LogError("\nComponents on player:");
            foreach (Component comp in player.GetComponents<Component>())
            {
                Debug.LogError($"  - {comp.GetType().Name}");
            }
            Debug.LogError("\nComponents in children:");
            foreach (Component comp in player.GetComponentsInChildren<Component>())
            {
                Debug.LogError($"  - {comp.GetType().Name} (on {comp.gameObject.name})");
            }
            Debug.LogError("\n⚠️ SOLUTION: Make sure PlayerManager GameObject exists in scene!");
            return;
        }

        Debug.Log($"✅ Found Health component!");
        Debug.Log($"   Current Health: {health.CurrentHealth}");
        Debug.Log($"   Max Health: {health.MaxHealth}");

        // Apply damage
        float healthBefore2 = health.CurrentHealth;
        Debug.Log($"\n💥 Applying {testDamageAmount} damage...");
        health.TakeDamage(testDamageAmount);
        float healthAfter2 = health.CurrentHealth;

        Debug.Log($"✅ DAMAGE APPLIED!");
        Debug.Log($"   Health BEFORE: {healthBefore2}");
        Debug.Log($"   Health AFTER: {healthAfter2}");
        Debug.Log($"   Actual damage taken: {healthBefore2 - healthAfter2}");

        if (healthBefore2 == healthAfter2)
        {
            Debug.LogError("⚠️ WARNING: Health did NOT change! Check Health.TakeDamage() method");
        }

        Debug.Log("=================================================\n");
    }

    /// <summary>
    /// Print diagnostic info
    /// </summary>
    [ContextMenu("Debug Jumpscare Settings")]
    private void DebugJumpscareSettings()
    {
        if (hunterSupport == null)
        {
            Debug.LogError("❌ AIHunterSupport not found!");
            return;
        }

        Debug.Log("=== JUMPSCARE DAMAGE SETTINGS ===");
        
        // Use reflection to read private fields (for debugging only)
        var type = hunterSupport.GetType();
        var enableDamageField = type.GetField("enableJumpscareDamage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var damageModeField = type.GetField("damageMode", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var damageField = type.GetField("jumpscareDamage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var intervalField = type.GetField("damageInterval", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (enableDamageField != null)
            Debug.Log($"Enable Jumpscare Damage: {enableDamageField.GetValue(hunterSupport)}");
        if (damageModeField != null)
            Debug.Log($"Damage Mode: {damageModeField.GetValue(hunterSupport)}");
        if (damageField != null)
            Debug.Log($"Jumpscare Damage: {damageField.GetValue(hunterSupport)}");
        if (intervalField != null)
            Debug.Log($"Damage Interval: {intervalField.GetValue(hunterSupport)}");

        Debug.Log("================================\n");
    }
}
