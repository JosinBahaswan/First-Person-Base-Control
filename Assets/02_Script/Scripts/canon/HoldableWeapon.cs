using UnityEngine;

/// <summary>
/// Script untuk weapon yang bisa ditembak saat di-hold di holdItemParent
/// Attach ke weapon pickup GameObject bersama WeaponPickupItem.cs
/// </summary>
public class HoldableWeapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] private string weaponName = "Weapon";
    [Tooltip("Damage per shot. Example: 10 damage = 10 shots to kill 100 HP enemy")]
    [SerializeField] private float damage = 10f;
    [Tooltip("Fire rate in seconds. Example: 0.1 = 10 rounds/second")]
    [SerializeField] private float fireRate = 0.1f;
    [Tooltip("Maximum shooting range in meters")]
    [SerializeField] private float range = 100f;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private int currentAmmo = 30;
    [SerializeField] private int reserveAmmo = 90;
    [SerializeField] private float reloadTime = 2f;

    [Header("Shooting Settings")]
    [SerializeField] private LayerMask hitLayers = ~0; // Layer yang bisa kena tembak
    [SerializeField] private bool useAutoFire = true; // True = otomatis, False = semi-auto
    [SerializeField] private bool useMobileControls = false; // TRUE untuk mobile button, FALSE untuk mouse click

    [Header("Effects (Optional)")]
    [SerializeField] private GameObject muzzleFlashEffect; // Optional
    [SerializeField] private GameObject impactEffect; // Optional
    [SerializeField] private Transform muzzlePoint; // Optional - untuk spawn muzzle flash
    [SerializeField] private ParticleSystem shellEjectionEffect; // Optional

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip shootSound; // Optional
    [SerializeField] private AudioClip reloadSound; // Optional
    [SerializeField] private AudioClip emptySound; // Optional

    [Header("Recoil")]
    [SerializeField] private float recoilAmount = 0.4f; // Weapon kickback (lebih besar = lebih terlihat)
    [SerializeField] private float recoilSpeed = 4f; // Recovery speed (lebih kecil = lebih lambat = lebih terlihat)
    [SerializeField] private float recoilRotation = 5f; // Weapon rotation kick (degrees)
    [SerializeField] private float cameraRecoilAmount = 2f; // Camera kick vertical (degrees) - Player kontrol!
    [SerializeField] private float cameraRecoilHorizontal = 1f; // Camera horizontal spread (degrees)
    [SerializeField] private float recoilRecoveryDelay = 0.3f; // Delay sebelum camera recovery (seconds)

    [Header("Hold Position (Adjust di Play Mode)")]
    [SerializeField] private Vector3 holdPositionOffset = new Vector3(0.2f, -0.15f, 0.4f);
    [SerializeField] private Vector3 holdRotationOffset = new Vector3(-85f, 0f, 0f);

    // Private variables
    private bool isReloading = false;
    private float nextFireTime = 0f;
    private Camera mainCamera;
    private AudioSource audioSource;
    private Vector3 originalPosition;
    private Vector3 originalRotation; // Original rotation untuk recovery
    private Vector3 currentRotationKick = Vector3.zero; // Current rotation recoil
    private bool isHeld = false;
    private bool isFiring = false; // Flag untuk mobile fire button
    private Collider weaponCollider;

    // Properties
    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => isReloading;
    public string WeaponName => weaponName;
    public bool UseMobileControls => useMobileControls;

    void Start()
    {
        // Audio setup (optional)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (shootSound != null || reloadSound != null || emptySound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        mainCamera = Camera.main;

        // Get collider reference
        weaponCollider = GetComponent<Collider>();

        // Auto-detect mobile controls dari FirstPersonMovement
        FirstPersonMovement fpm = FindAnyObjectByType<FirstPersonMovement>();
        if (fpm != null)
        {
            useMobileControls = fpm.useJoystick;
            Debug.Log($"[HoldableWeapon] Use Mobile Controls: {useMobileControls}");
        }

        // FORCE auto-fire untuk mobile controls
        if (useMobileControls && !useAutoFire)
        {
            useAutoFire = true;
            Debug.LogWarning($"[HoldableWeapon] FORCED useAutoFire to TRUE for mobile controls!");
        }

        // FORCE recoil values untuk memastikan efek terlihat (override Inspector values yang lama)
        if (recoilAmount < 0.25f)
        {
            recoilAmount = 0.4f;
            Debug.LogWarning($"[HoldableWeapon] FORCED recoilAmount to 0.4 (was too small)");
        }
        if (cameraRecoilAmount < 1f)
        {
            cameraRecoilAmount = 2f;
            Debug.LogWarning($"[HoldableWeapon] FORCED cameraRecoilAmount to 2 (was {cameraRecoilAmount})");
        }
        if (cameraRecoilHorizontal < 1f)
        {
            cameraRecoilHorizontal = 2f;
            Debug.LogWarning($"[HoldableWeapon] FORCED cameraRecoilHorizontal to 2");
        }

        // Set recovery delay ke FirstPersonLook
        if (FirstPersonLook.Instance != null)
        {
            FirstPersonLook.Instance.SetRecoveryDelay(recoilRecoveryDelay);
        }

        Debug.Log($"[HoldableWeapon] {weaponName} initialized. Auto-fire: {useAutoFire}, Recoil: {recoilAmount}, CamRecoil: {cameraRecoilAmount}, RecoveryDelay: {recoilRecoveryDelay}s");
    }

    void Update()
    {
        // Check apakah weapon sedang di-hold
        if (PlayerInteractionNoInventory.Instance != null)
        {
            Item heldItem = PlayerInteractionNoInventory.Instance.holdItem;
            Item thisItem = GetComponent<Item>();
            isHeld = (heldItem == thisItem);

            // Apply hold position saat di-hold
            if (isHeld && transform.parent == PlayerInteractionNoInventory.Instance.holdItemParent)
            {
                // Simpan original position & rotation untuk recoil (hanya sekali)
                if (originalPosition == Vector3.zero)
                {
                    originalPosition = holdPositionOffset;
                    originalRotation = holdRotationOffset;
                    transform.localPosition = holdPositionOffset; // Set initial position
                }

                // Apply rotation dengan kick recoil
                transform.localEulerAngles = holdRotationOffset + currentRotationKick;

                // Disable collider saat held (agar tidak kena raycast sendiri)
                if (weaponCollider != null && weaponCollider.enabled)
                {
                    weaponCollider.enabled = false;
                    Debug.Log("[HoldableWeapon] Collider disabled while held");
                }
            }
            else if (!isHeld)
            {
                // Reset original position & rotation saat weapon di-drop
                if (originalPosition != Vector3.zero)
                {
                    originalPosition = Vector3.zero;
                    originalRotation = Vector3.zero;
                    currentRotationKick = Vector3.zero;
                }

                // Enable collider kembali saat tidak di-hold
                if (weaponCollider != null && !weaponCollider.enabled)
                {
                    weaponCollider.enabled = true;
                    Debug.Log("[HoldableWeapon] Collider enabled (not held)");
                }
            }
        }

        // Hanya bisa tembak kalau di-hold
        if (!isHeld || isReloading)
            return;

        // MOBILE CONTROLS: Pakai button fire, BUKAN mouse click
        if (useMobileControls)
        {
            // Shooting dihandle oleh OnFireButtonDown/Up dari mobile UI
            if (isFiring)
            {
                if (useAutoFire)
                {
                    Fire(); // Auto fire saat button di-hold
                }
            }
        }
        // PC CONTROLS: Pakai mouse click
        else
        {
            // Handle shooting input (PC only)
            if (useAutoFire)
            {
                if (Input.GetMouseButton(0)) // Hold untuk auto fire
                {
                    Fire();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0)) // Click untuk semi-auto
                {
                    Fire();
                }
            }

            // Keyboard reload untuk PC
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartReload();
            }
        }

        // Reload input (keyboard atau button)
        // Tidak ada auto reload - harus manual press R atau reload button
    }

    /// <summary>
    /// Method untuk menembak
    /// </summary>
    public void Fire()
    {
        if (isReloading)
        {
            Debug.Log("[HoldableWeapon] Cannot fire - reloading");
            return;
        }

        if (Time.time < nextFireTime)
        {
            return; // Fire rate limit
        }

        if (currentAmmo <= 0)
        {
            Debug.LogWarning("[HoldableWeapon] Out of ammo!");
            PlayEmptySound();
            return;
        }

        PerformShoot();
        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        Debug.Log($"[HoldableWeapon] Fired! Ammo: {currentAmmo}/{maxAmmo}");
    }

    /// <summary>
    /// Perform shooting raycast & effects
    /// </summary>
    protected virtual void PerformShoot()
    {
        // Play shoot sound
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // Muzzle flash
        if (muzzleFlashEffect != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashEffect, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(flash, 0.1f);
        }

        // Shell ejection
        if (shellEjectionEffect != null)
        {
            shellEjectionEffect.Play();
        }

        // Raycast shooting
        Vector3 rayOrigin = mainCamera.transform.position;
        Vector3 rayDirection = mainCamera.transform.forward;

        // Jika ada muzzle point, gunakan sebagai origin
        if (muzzlePoint != null)
        {
            rayOrigin = muzzlePoint.position;
            rayDirection = (mainCamera.transform.position + mainCamera.transform.forward * range - muzzlePoint.position).normalized;
        }

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, range, hitLayers))
        {
            Debug.Log($"[HoldableWeapon] Hit: {hit.collider.name}");

            bool hitDamageableTarget = false;

            // Try UniversalEnemyHealth first (new system)
            UniversalEnemyHealth universalHealth = hit.collider.GetComponent<UniversalEnemyHealth>();
            if (universalHealth == null)
            {
                // Try parent if not on current object
                universalHealth = hit.collider.GetComponentInParent<UniversalEnemyHealth>();
            }

            if (universalHealth != null && !universalHealth.IsDead)
            {
                universalHealth.TakeDamage(damage);
                hitDamageableTarget = true;
                Debug.Log($"[HoldableWeapon] Applied damage to UniversalEnemyHealth on {hit.collider.name}");
            }
            else
            {
                // Fallback to old Health system
                Health health = hit.collider.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    hitDamageableTarget = true;
                    Debug.Log($"[HoldableWeapon] Applied damage to Health on {hit.collider.name}");
                }
            }

            OnHit(hit, hitDamageableTarget);

            // Impact effect
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }

        // Recoil effect
        ApplyRecoil();
    }

    /// <summary>
    /// Start reload process
    /// </summary>
    public void StartReload()
    {
        if (isReloading || currentAmmo == maxAmmo || reserveAmmo <= 0)
            return;

        isReloading = true;

        // Play reload sound
        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        Debug.Log($"[HoldableWeapon] Reloading...");
        Invoke(nameof(FinishReload), reloadTime);
    }

    /// <summary>
    /// Finish reload process
    /// </summary>
    private void FinishReload()
    {
        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        Debug.Log($"[HoldableWeapon] Reload complete! Ammo: {currentAmmo}/{maxAmmo} (Reserve: {reserveAmmo})");
    }

    /// <summary>
    /// Add ammo to reserve
    /// </summary>
    public void AddAmmo(int amount)
    {
        reserveAmmo += amount;
        Debug.Log($"[HoldableWeapon] Added {amount} ammo. Reserve: {reserveAmmo}");
    }

    /// <summary>
    /// Set initial ammo
    /// </summary>
    public void SetAmmo(int current, int reserve)
    {
        currentAmmo = Mathf.Min(current, maxAmmo);
        reserveAmmo = reserve;
    }

    /// <summary>
    /// Apply recoil effect (weapon kickback + camera shake)
    /// </summary>
    private void ApplyRecoil()
    {
        if (!isHeld)
            return;

        // Weapon position kickback
        if (recoilAmount > 0)
        {
            transform.localPosition -= Vector3.forward * recoilAmount;
        }

        // Weapon rotation kick (visual feedback)
        if (recoilRotation > 0)
        {
            currentRotationKick += new Vector3(recoilRotation, 0, 0); // Kick ke atas (X axis positive untuk weapon)
        }

        // Camera recoil melalui FirstPersonLook
        if (FirstPersonLook.Instance != null && cameraRecoilAmount > 0)
        {
            float verticalRecoil = cameraRecoilAmount; // Kick ke atas (degrees)
            float horizontalRecoil = Random.Range(-cameraRecoilHorizontal, cameraRecoilHorizontal); // Random horizontal (degrees)

            FirstPersonLook.Instance.ApplyRecoil(verticalRecoil, horizontalRecoil);
            Debug.Log($"[Recoil] Applied! Weapon pos: {transform.localPosition}, Weapon rot kick: {currentRotationKick}, Camera: V={verticalRecoil}°");
        }
        else if (FirstPersonLook.Instance == null)
        {
            Debug.LogError("[Recoil] FirstPersonLook.Instance is NULL!");
        }
    }

    // ==================== MOBILE CONTROLS ====================

    /// <summary>
    /// Called dari mobile fire button (PointerDown)
    /// </summary>
    public void OnFireButtonDown()
    {
        if (!isHeld || isReloading)
            return;

        isFiring = true;

        // Semi-auto: Fire sekali saat button pressed
        if (!useAutoFire)
        {
            Fire();
        }
    }

    /// <summary>
    /// Called dari mobile fire button (PointerUp)
    /// </summary>
    public void OnFireButtonUp()
    {
        isFiring = false;
    }

    /// <summary>
    /// Called dari mobile reload button
    /// </summary>
    public void OnReloadButtonPressed()
    {
        Debug.Log("[HoldableWeapon] Reload button pressed");
        StartReload();
    }

    /// <summary>
    /// Called when bullet hits something
    /// </summary>
    protected virtual void OnHit(RaycastHit hit, bool hitEnemy)
    {
        // Override this in child classes for special effects
    }

    /// <summary>
    /// Play empty sound
    /// </summary>
    private void PlayEmptySound()
    {
        if (emptySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(emptySound);
        }
    }

    void LateUpdate()
    {
        // Return weapon to original position & rotation (recoil recovery)
        if (isHeld && originalPosition != Vector3.zero)
        {
            // Position recovery
            Vector3 targetPos = holdPositionOffset;
            if (transform.localPosition != targetPos)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * recoilSpeed);
            }

            // Rotation kick recovery
            if (currentRotationKick.magnitude > 0.01f)
            {
                currentRotationKick = Vector3.Lerp(currentRotationKick, Vector3.zero, Time.deltaTime * recoilSpeed);
            }
            else
            {
                currentRotationKick = Vector3.zero;
            }
        }
    }

    void OnDisable()
    {
        // Cancel reload if disabled
        CancelInvoke();
        isReloading = false;
    }

    // Public getters for UI
    public int GetCurrentAmmo() => currentAmmo;
    public int GetReserveAmmo() => reserveAmmo;
    public int GetMaxAmmo() => maxAmmo;
}
