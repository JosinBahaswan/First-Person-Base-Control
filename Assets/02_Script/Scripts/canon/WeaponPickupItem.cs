using UnityEngine;

/// <summary>
/// Script untuk weapon yang bisa dipickup menggunakan sistem Item.cs yang sudah ada
/// Inherit dari Item.cs, jadi otomatis support Use/Drop/Throw
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class WeaponPickupItem : Item
{
    [Header("Weapon Pickup Settings")]
    [SerializeField] private WeaponType weaponType = WeaponType.Pistol;
    [SerializeField] private int ammoAmount = 30; // Ammo yang didapat saat pickup
    [SerializeField] private bool autoUnlockWeapon = false; // FALSE = weapon di-hold di tangan (tidak hilang), TRUE = langsung unlock ke WeaponManager
    [SerializeField] private bool destroyPickupAfterUnlock = true; // Destroy PICKUP object setelah unlock

    [Header("Visual Settings")]
    [SerializeField] private GameObject weaponModel; // Model senjata untuk display
    [SerializeField] private bool rotateWhenNotHeld = true;
    [SerializeField] private float rotationSpeed = 50f;

    private bool isPickedUp = false;
    private WeaponManager weaponManager;

    public enum WeaponType
    {
        Pistol,
        Rifle,
        Shotgun
    }

    public override void Start()
    {
        base.Start(); // Call parent Start()

        // Setup default interaction messages
        if (string.IsNullOrEmpty(objectName))
            objectName = weaponType.ToString();

        if (string.IsNullOrEmpty(interactMessage))
            interactMessage = "Press E to pick up";

        objectCategory = "Weapon";

        // Set as non-static agar bisa diambil
        isStatic = false;
    }

    private void Update()
    {
        // Rotasi visual saat belum dipickup
        if (rotateWhenNotHeld && !isPickedUp && weaponModel != null)
        {
            weaponModel.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    public override void OnInteract()
    {
        // MODE 1: Auto Unlock Weapon (langsung aktif di tangan)
        if (autoUnlockWeapon)
        {
            // Cari WeaponManager di scene
            if (weaponManager == null)
            {
                weaponManager = FindAnyObjectByType<WeaponManager>();
            }

            if (weaponManager != null)
            {
                // Check apakah player sudah punya weapon ini
                bool alreadyHasWeapon = weaponManager.HasWeapon(weaponType.ToString());

                if (alreadyHasWeapon)
                {
                    // Tambah ammo ke weapon yang sudah ada
                    WeaponBase weapon = weaponManager.GetWeaponByType(weaponType.ToString());
                    if (weapon != null)
                    {
                        weapon.AddAmmo(ammoAmount);
                        Debug.Log($"Added {ammoAmount} ammo to {weaponType}");
                    }
                }
                else
                {
                    // Berikan weapon baru
                    weaponManager.UnlockWeapon(weaponType.ToString(), ammoAmount);
                    Debug.Log($"✅ Unlocked {weaponType} with {ammoAmount} ammo");
                }

                // Destroy atau disable PICKUP object setelah unlock
                // Weapon asli ada di WeaponHolder, pickup ini hanya trigger
                if (destroyPickupAfterUnlock)
                {
                    Destroy(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
                return;
            }
            else
            {
                Debug.LogWarning("WeaponManager tidak ditemukan! Falling back to hold item mode.");
            }
        }

        // MODE 2: Hold Item Mode (pindah ke holdItemParent)
        // Gunakan sistem Item.cs default
        base.OnInteract();
        isPickedUp = true;
    }

    /// <summary>
    /// Method untuk unlock weapon saat sudah di-hold
    /// Dipanggil manual dari button "Use" atau system lain
    /// </summary>
    public void UnlockWeaponFromHold()
    {
        if (weaponManager == null)
        {
            weaponManager = FindAnyObjectByType<WeaponManager>();
        }

        if (weaponManager != null)
        {
            bool alreadyHasWeapon = weaponManager.HasWeapon(weaponType.ToString());

            if (alreadyHasWeapon)
            {
                WeaponBase weapon = weaponManager.GetWeaponByType(weaponType.ToString());
                if (weapon != null)
                {
                    weapon.AddAmmo(ammoAmount);
                    Debug.Log($"[Hold Mode] Added {ammoAmount} ammo to {weaponType}");
                }
            }
            else
            {
                weaponManager.UnlockWeapon(weaponType.ToString(), ammoAmount);
                Debug.Log($"[Hold Mode] Unlocked {weaponType} with {ammoAmount} ammo");
            }

            // Destroy item setelah unlock
            if (PlayerInteractionNoInventory.Instance.holdItem == this)
            {
                PlayerInteractionNoInventory.Instance.ConsumeItem();
            }
        }
        else
        {
            Debug.LogWarning("WeaponManager tidak ditemukan!");
        }
    }    /// <summary>
         /// Method untuk setup weapon pickup via code
         /// </summary>
    public void SetupWeaponPickup(WeaponType type, int ammo)
    {
        weaponType = type;
        ammoAmount = ammo;
        objectName = type.ToString();
    }
}
