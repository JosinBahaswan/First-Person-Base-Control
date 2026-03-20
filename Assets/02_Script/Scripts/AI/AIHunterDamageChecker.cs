using UnityEngine;

/// <summary>
/// Simple checker script untuk memastikan damage setting benar
/// Attach ke enemy yang sama dengan AIHunterSupport
/// </summary>
[RequireComponent(typeof(AIHunterSupport))]
public class AIHunterDamageChecker : MonoBehaviour
{
    private AIHunterSupport hunterSupport;

    private void Awake()
    {
        hunterSupport = GetComponent<AIHunterSupport>();
    }

    private void Start()
    {
        CheckDamageSettings();
    }

    [ContextMenu("Check Damage Settings")]
    private void CheckDamageSettings()
    {
        if (hunterSupport == null)
        {
            Debug.LogError($"[DamageChecker] {name} - AIHunterSupport NOT FOUND!");
            return;
        }

        Debug.Log("========================================");
        Debug.Log($"[DamageChecker] Checking {name}");
        Debug.Log("========================================");

        // Use reflection to read private fields
        var type = hunterSupport.GetType();

        var enableDamageField = type.GetField("enableJumpscareDamage",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var damageModeField = type.GetField("damageMode",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var damageField = type.GetField("jumpscareDamage",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var intervalField = type.GetField("damageInterval",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var showDebugField = type.GetField("showDebugLogs",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        bool enableDamage = enableDamageField != null ? (bool)enableDamageField.GetValue(hunterSupport) : false;
        object damageMode = damageModeField?.GetValue(hunterSupport);
        float damage = damageField != null ? (float)damageField.GetValue(hunterSupport) : 0f;
        float interval = intervalField != null ? (float)intervalField.GetValue(hunterSupport) : 0f;
        bool showDebug = showDebugField != null ? (bool)showDebugField.GetValue(hunterSupport) : false;

        Debug.Log($"✓ Enable Jumpscare Damage: {enableDamage}");
        Debug.Log($"✓ Damage Mode: {damageMode}");
        Debug.Log($"✓ Jumpscare Damage: {damage}");
        Debug.Log($"✓ Damage Interval: {interval}s");
        Debug.Log($"✓ Show Debug Logs: {showDebug}");

        Debug.Log("\n--- VALIDATION ---");

        if (!enableDamage)
        {
            Debug.LogError("❌ PROBLEM: Enable Jumpscare Damage is FALSE!");
            Debug.LogError("   FIX: Centang 'Enable Jumpscare Damage' di Inspector!");
        }
        else
        {
            Debug.Log("✅ Enable Jumpscare Damage is enabled");
        }

        if (damage <= 0)
        {
            Debug.LogError("❌ PROBLEM: Jumpscare Damage is 0 or negative!");
            Debug.LogError("   FIX: Set 'Jumpscare Damage' to value > 0 (misal: 20)");
        }
        else
        {
            Debug.Log($"✅ Jumpscare Damage is set to {damage}");
        }

        if (!showDebug)
        {
            Debug.LogWarning("⚠️ WARNING: Show Debug Logs is disabled");
            Debug.LogWarning("   Enable it untuk melihat detail damage logs di console");
        }
        else
        {
            Debug.Log("✅ Show Debug Logs is enabled");
        }

        Debug.Log("\n--- HEALTH SYSTEM CHECK ---");

        // Check PlayerManager
        PlayerManager pm = PlayerManager.Instance;
        if (pm != null)
        {
            Debug.Log($"✅ PlayerManager found!");
            Debug.Log($"   Current Health: {pm.currentHealth}");
            Debug.Log($"   Max Health: {pm.maxHealth}");
            Debug.Log($"   Is Dead: {pm.isDead}");
        }
        else
        {
            Debug.LogWarning("⚠️ PlayerManager.Instance is NULL");
        }

        // Check Health component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health health = player.GetComponentInChildren<Health>();
            if (health != null)
            {
                Debug.Log($"✅ Health component found!");
                Debug.Log($"   Current Health: {health.CurrentHealth}");
                Debug.Log($"   Max Health: {health.MaxHealth}");
            }
            else
            {
                Debug.LogWarning("⚠️ Health component not found on player");
            }
        }
        else
        {
            Debug.LogError("❌ Player GameObject not found with tag 'Player'!");
        }

        Debug.Log("========================================\n");

        // Summary
        if (enableDamage && damage > 0)
        {
            Debug.Log("🎯 RESULT: Damage settings look CORRECT!");
            Debug.Log("   If damage still not working, check console logs during jumpscare.");
        }
        else
        {
            Debug.LogError("❌ RESULT: Damage settings are INCORRECT!");
            Debug.LogError("   Fix the problems above, then test again.");
        }
    }
}
