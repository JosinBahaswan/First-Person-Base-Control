using UnityEngine;

/// <summary>
/// Helper script untuk auto-unlock weapon setelah di-hold
/// Attach ke weapon pickup GameObject bersama WeaponPickupItem.cs
/// </summary>
public class WeaponAutoUnlock : MonoBehaviour
{
    [Header("Auto Unlock Settings")]
    [SerializeField] private float autoUnlockDelay = 1f; // Delay sebelum auto unlock (detik)
    [SerializeField] private bool showDebugMessages = true;

    private bool isInHoldParent = false;
    private WeaponPickupItem weaponPickup;

    void Start()
    {
        weaponPickup = GetComponent<WeaponPickupItem>();
    }

    void Update()
    {
        // Check jika weapon berada di holdItemParent
        if (!isInHoldParent && IsInHoldParent())
        {
            isInHoldParent = true;
            
            if (showDebugMessages)
                Debug.Log($"[WeaponAutoUnlock] {weaponPickup.name} di-hold. Auto unlock dalam {autoUnlockDelay} detik...");
            
            // Schedule auto unlock
            Invoke(nameof(AutoUnlock), autoUnlockDelay);
        }
    }

    bool IsInHoldParent()
    {
        // Check apakah parent mengandung nama "HoldItem" atau "Hold"
        if (transform.parent != null)
        {
            string parentName = transform.parent.name.ToLower();
            return parentName.Contains("hold") || parentName.Contains("item");
        }
        return false;
    }

    void AutoUnlock()
    {
        if (weaponPickup != null && IsInHoldParent())
        {
            if (showDebugMessages)
                Debug.Log($"[WeaponAutoUnlock] Auto unlocking {weaponPickup.name}...");
            
            weaponPickup.UnlockWeaponFromHold();
        }
    }

    void OnDisable()
    {
        // Cancel invoke jika object disabled
        CancelInvoke();
    }
}
