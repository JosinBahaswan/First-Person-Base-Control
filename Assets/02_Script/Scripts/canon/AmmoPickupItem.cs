using UnityEngine;

/// <summary>
/// Script untuk ammo pickup menggunakan sistem Item.cs yang sudah ada
/// Bisa dipickup dengan tombol E, atau Use/Drop/Throw seperti item biasa
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class AmmoPickupItem : Item
{
    [Header("Ammo Pickup Settings")]
    [SerializeField] private AmmoType ammoType = AmmoType.Universal; // Universal = semua weapon
    [SerializeField] private int ammoAmount = 30;
    [SerializeField] private bool destroyAfterPickup = true;
    [SerializeField] private bool canBeHeld = false; // Bisa di-hold seperti item atau langsung pickup?

    [Header("Visual Settings")]
    [SerializeField] private GameObject ammoModel;
    [SerializeField] private bool rotateWhenNotHeld = true;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private bool bobUpDown = true;
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private float bobHeight = 0.3f;

    private Vector3 startPosition;
    private bool isPickedUp = false;
    private WeaponManager weaponManager;

    public enum AmmoType
    {
        Universal,  // Untuk semua weapon
        Pistol,
        Rifle,
        Shotgun
    }

    public override void Start()
    {
        base.Start(); // Call parent Start()

        startPosition = transform.position;

        // Setup default interaction messages
        if (string.IsNullOrEmpty(objectName))
            objectName = ammoType == AmmoType.Universal ? "Ammo Box" : $"{ammoType} Ammo";

        if (string.IsNullOrEmpty(interactMessage))
            interactMessage = canBeHeld ? "Press E to pick up" : "Press E to collect";

        objectCategory = "Ammo";

        // Set static atau tidak based on canBeHeld
        isStatic = !canBeHeld;
    }

    private void Update()
    {
        // Visual effects saat belum dipickup
        if (!isPickedUp)
        {
            if (rotateWhenNotHeld && ammoModel != null)
            {
                ammoModel.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }

            if (bobUpDown)
            {
                float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }
    }

    public override void OnInteract()
    {
        // Cari WeaponManager
        if (weaponManager == null)
        {
            weaponManager = FindAnyObjectByType<WeaponManager>();
        }

        if (weaponManager == null)
        {
            Debug.LogWarning("WeaponManager tidak ditemukan di scene!");
            return;
        }

        // Jika tidak bisa di-hold, langsung tambah ammo
        if (!canBeHeld)
        {
            CollectAmmo();
        }
        else
        {
            // Gunakan sistem Item.cs default (hold item)
            base.OnInteract();
            isPickedUp = true;
        }
    }

    /// <summary>
    /// Method untuk mengumpulkan ammo dan menambahkannya ke weapon
    /// </summary>
    private void CollectAmmo()
    {
        // Check apakah player sedang hold weapon
        if (PlayerInteractionNoInventory.Instance != null && PlayerInteractionNoInventory.Instance.holdItem != null)
        {
            // Try get HoldableWeapon component dari held item
            HoldableWeapon heldWeapon = PlayerInteractionNoInventory.Instance.holdItem.GetComponent<HoldableWeapon>();
            if (heldWeapon != null)
            {
                // Check compatibility
                bool heldCompatible = false;

                switch (ammoType)
                {
                    case AmmoType.Universal:
                        heldCompatible = true;
                        break;
                    case AmmoType.Pistol:
                        heldCompatible = heldWeapon.WeaponName.Contains("Pistol");
                        break;
                    case AmmoType.Rifle:
                        heldCompatible = heldWeapon.WeaponName.Contains("Rifle");
                        break;
                    case AmmoType.Shotgun:
                        heldCompatible = heldWeapon.WeaponName.Contains("Shotgun");
                        break;
                }

                if (heldCompatible)
                {
                    heldWeapon.AddAmmo(ammoAmount);
                    Debug.Log($"✅ Added {ammoAmount} {ammoType} ammo to held {heldWeapon.WeaponName}. Reserve: {heldWeapon.GetReserveAmmo()}");

                    if (destroyAfterPickup)
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
                    Debug.LogWarning($"❌ {ammoType} ammo tidak compatible dengan {heldWeapon.WeaponName}!");
                    return;
                }
            }
        }

        // Fallback: Check WeaponManager untuk weapon yang aktif
        WeaponBase currentWeapon = weaponManager.GetCurrentWeapon();

        if (currentWeapon == null)
        {
            Debug.LogWarning("❌ Tidak ada weapon yang aktif atau di-hold!");
            Debug.LogWarning("Pickup weapon terlebih dahulu atau unlock weapon dari hold mode.");
            return;
        }

        // Check ammo type compatibility untuk WeaponManager weapon
        bool managerCompatible = false;

        switch (ammoType)
        {
            case AmmoType.Universal:
                managerCompatible = true;
                break;
            case AmmoType.Pistol:
                managerCompatible = currentWeapon.WeaponName.Contains("Pistol");
                break;
            case AmmoType.Rifle:
                managerCompatible = currentWeapon.WeaponName.Contains("Rifle");
                break;
            case AmmoType.Shotgun:
                managerCompatible = currentWeapon.WeaponName.Contains("Shotgun");
                break;
        }

        if (managerCompatible)
        {
            currentWeapon.AddAmmo(ammoAmount);
            Debug.Log($"Collected {ammoAmount} {ammoType} ammo for {currentWeapon.WeaponName}");

            if (destroyAfterPickup)
            {
                Destroy(gameObject);
            }
            else
            {
                isPickedUp = true;
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log($"Ammo tidak kompatibel dengan {currentWeapon.WeaponName}");
        }
    }

    /// <summary>
    /// Method untuk "use" ammo saat di-hold (optional)
    /// Bisa dipanggil dari sistem lain atau button "Use"
    /// </summary>
    public void UseAmmo()
    {
        CollectAmmo();
    }

    /// <summary>
    /// Setup ammo pickup via code
    /// </summary>
    public void SetupAmmoPickup(AmmoType type, int amount)
    {
        ammoType = type;
        ammoAmount = amount;
        objectName = type == AmmoType.Universal ? "Ammo Box" : $"{type} Ammo";
    }
}
