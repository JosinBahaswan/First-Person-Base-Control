using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manager untuk mengontrol senjata player
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Setup")]
    [SerializeField] private WeaponBase[] weapons; // Array semua senjata
    [SerializeField] private int currentWeaponIndex = 0;
    [SerializeField] private Transform weaponHolder; // Parent object untuk weapon models

    [Header("Input Settings")]
    [SerializeField] private bool useNewInputSystem = true; // Toggle antara Input System baru/lama
    [SerializeField] private bool useMobileControls = false; // Toggle untuk mobile controls (Android/iOS)

    private WeaponBase currentWeapon;
    private bool isFiring = false;

    // Input Actions (untuk New Input System)
    private InputAction fireAction;
    private InputAction reloadAction;
    private InputAction switchWeaponAction;

    void Start()
    {
        InitializeWeapons();
        SetupInputSystem();

        if (weapons.Length > 0)
            SwitchWeapon(0);
    }

    void Update()
    {
        HandleInput();
    }

    private void InitializeWeapons()
    {
        // Dapatkan semua weapon dari children
        if (weapons == null || weapons.Length == 0)
        {
            weapons = GetComponentsInChildren<WeaponBase>(true);
        }

        // Disable semua weapon kecuali yang pertama
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null) // Null check
            {
                weapons[i].gameObject.SetActive(i == currentWeaponIndex);
            }
        }
    }

    private void SetupInputSystem()
    {
        if (useNewInputSystem)
        {
            // Setup untuk New Input System
            var playerInput = GetComponentInParent<PlayerInput>();
            if (playerInput != null)
            {
                fireAction = playerInput.actions["Fire"];
                reloadAction = playerInput.actions["Reload"];
                switchWeaponAction = playerInput.actions["SwitchWeapon"];
            }
        }
    }

    private void HandleInput()
    {
        if (currentWeapon == null)
            return;

        // Skip input handling jika menggunakan mobile controls
        // Mobile controls akan handle input melalui MobileWeaponControls.cs
        if (useMobileControls)
            return;

        if (useNewInputSystem)
        {
            HandleNewInputSystem();
        }
        else
        {
            HandleOldInputSystem();
        }
    }

    private void HandleNewInputSystem()
    {
        // Fire
        if (fireAction != null)
        {
            if (fireAction.IsPressed())
            {
                currentWeapon.Fire();
            }
            else if (fireAction.WasReleasedThisFrame())
            {
                // Release fire untuk weapon yang support
                // (Removed specific Pistol check - use WeaponBase methods instead)
            }
        }

        // Reload
        if (reloadAction != null && reloadAction.WasPressedThisFrame())
        {
            currentWeapon.Reload();
        }

        // Switch weapon dengan scroll wheel
        if (switchWeaponAction != null)
        {
            float scrollValue = switchWeaponAction.ReadValue<float>();
            if (scrollValue > 0)
                SwitchToNextWeapon();
            else if (scrollValue < 0)
                SwitchToPreviousWeapon();
        }

        // Switch weapon dengan angka 1-9
        for (int i = 0; i < weapons.Length && i < 9; i++)
        {
            if (Keyboard.current != null && Keyboard.current[(Key)(Key.Digit1 + i)].wasPressedThisFrame)
            {
                SwitchWeapon(i);
            }
        }
    }

    private void HandleOldInputSystem()
    {
        // Fire (Mouse Button 0 atau Left Mouse)
        if (Input.GetButton("Fire1"))
        {
            currentWeapon.Fire();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            // Release fire untuk weapon yang support
            // (Removed specific Pistol check - use WeaponBase methods instead)
        }

        // Reload (R key)
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeapon.Reload();
        }

        // Switch weapon dengan scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
            SwitchToNextWeapon();
        else if (scroll < 0f)
            SwitchToPreviousWeapon();

        // Switch weapon dengan angka 1-9
        for (int i = 0; i < weapons.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SwitchWeapon(i);
            }
        }
    }

    public void SwitchWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
            return;

        if (index == currentWeaponIndex)
            return;

        // Disable current weapon
        if (currentWeapon != null)
            currentWeapon.gameObject.SetActive(false);

        // Switch to new weapon
        currentWeaponIndex = index;
        currentWeapon = weapons[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);

        Debug.Log("Switched to: " + currentWeapon.WeaponName);
    }

    public void SwitchToNextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % weapons.Length;
        SwitchWeapon(nextIndex);
    }

    public void SwitchToPreviousWeapon()
    {
        int prevIndex = currentWeaponIndex - 1;
        if (prevIndex < 0)
            prevIndex = weapons.Length - 1;
        SwitchWeapon(prevIndex);
    }

    // Getter untuk UI atau sistem lain
    public WeaponBase GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }

    public int GetWeaponCount()
    {
        return weapons.Length;
    }

    /// <summary>
    /// Toggle mobile controls mode
    /// </summary>
    public void SetMobileControlsMode(bool enable)
    {
        useMobileControls = enable;
    }

    /// <summary>
    /// Check if using mobile controls
    /// </summary>
    public bool IsUsingMobileControls()
    {
        return useMobileControls;
    }

    /// <summary>
    /// Check if player has specific weapon type
    /// </summary>
    public bool HasWeapon(string weaponName)
    {
        foreach (WeaponBase weapon in weapons)
        {
            if (weapon != null && weapon.WeaponName.Contains(weaponName))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Get weapon by type/name
    /// </summary>
    public WeaponBase GetWeaponByType(string weaponName)
    {
        foreach (WeaponBase weapon in weapons)
        {
            if (weapon != null && weapon.WeaponName.Contains(weaponName))
            {
                return weapon;
            }
        }
        return null;
    }

    /// <summary>
    /// Unlock/Enable weapon dan berikan ammo
    /// Digunakan saat pickup weapon baru
    /// </summary>
    public void UnlockWeapon(string weaponName, int initialAmmo = 0)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null && weapons[i].WeaponName.Contains(weaponName))
            {
                // Enable weapon GameObject jika disabled
                weapons[i].gameObject.SetActive(true);

                // Tambah ammo jika ada
                if (initialAmmo > 0)
                {
                    weapons[i].AddAmmo(initialAmmo);
                }

                // Auto-switch ke weapon yang baru diambil
                SwitchWeapon(i);

                Debug.Log($"Unlocked {weaponName} with {initialAmmo} ammo");
                return;
            }
        }
    }
}
