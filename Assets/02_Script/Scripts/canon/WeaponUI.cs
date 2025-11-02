using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI untuk menampilkan informasi senjata (ammo, nama weapon, dll)
/// </summary>
public class WeaponUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponManager weaponManager; // Optional - untuk WeaponManager system

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reserveAmmoText;
    [SerializeField] private Image reloadIndicator;
    [SerializeField] private GameObject crosshair;

    [Header("Colors")]
    [SerializeField] private Color normalAmmoColor = Color.white;
    [SerializeField] private Color lowAmmoColor = Color.yellow;
    [SerializeField] private Color noAmmoColor = Color.red;

    private WeaponBase currentWeapon;
    private HoldableWeapon currentHoldableWeapon;

    void Start()
    {
        if (weaponManager == null)
            weaponManager = FindAnyObjectByType<WeaponManager>();

        if (reloadIndicator != null)
            reloadIndicator.gameObject.SetActive(false);
    }

    void Update()
    {
        UpdateWeaponUI();
    }

    private void UpdateWeaponUI()
    {
        // PRIORITY 1: Check HoldableWeapon (weapon yang di-hold)
        if (PlayerInteractionNoInventory.Instance != null && PlayerInteractionNoInventory.Instance.holdItem != null)
        {
            currentHoldableWeapon = PlayerInteractionNoInventory.Instance.holdItem.GetComponent<HoldableWeapon>();
            if (currentHoldableWeapon != null)
            {
                UpdateHoldableWeaponUI();
                return;
            }
        }

        // PRIORITY 2: Check WeaponManager (weapon system lama)
        if (weaponManager != null)
        {
            currentWeapon = weaponManager.GetCurrentWeapon();
            if (currentWeapon != null)
            {
                UpdateWeaponManagerUI();
                return;
            }
        }

        // No weapon active - hide UI
        HideWeaponUI();
    }

    /// <summary>
    /// Update UI untuk HoldableWeapon (weapon yang di-hold)
    /// </summary>
    private void UpdateHoldableWeaponUI()
    {
        // Update weapon name
        if (weaponNameText != null)
            weaponNameText.text = currentHoldableWeapon.WeaponName;

        // Update ammo text
        if (ammoText != null)
        {
            ammoText.text = currentHoldableWeapon.CurrentAmmo.ToString();

            // Change color based on ammo count
            float ammoPercentage = (float)currentHoldableWeapon.CurrentAmmo / currentHoldableWeapon.MaxAmmo;
            if (ammoPercentage <= 0)
                ammoText.color = noAmmoColor;
            else if (ammoPercentage <= 0.3f)
                ammoText.color = lowAmmoColor;
            else
                ammoText.color = normalAmmoColor;
        }

        // Update reserve ammo
        if (reserveAmmoText != null)
            reserveAmmoText.text = currentHoldableWeapon.ReserveAmmo.ToString();

        // Update reload indicator
        if (reloadIndicator != null)
            reloadIndicator.gameObject.SetActive(currentHoldableWeapon.IsReloading);

        // Update crosshair visibility
        if (crosshair != null)
            crosshair.SetActive(!currentHoldableWeapon.IsReloading);
    }

    /// <summary>
    /// Update UI untuk WeaponManager weapon (system lama)
    /// </summary>
    private void UpdateWeaponManagerUI()
    {

        // Update weapon name
        if (weaponNameText != null)
            weaponNameText.text = currentWeapon.WeaponName;

        // Update ammo text
        if (ammoText != null)
        {
            ammoText.text = currentWeapon.CurrentAmmo.ToString();

            // Change color based on ammo count
            float ammoPercentage = (float)currentWeapon.CurrentAmmo / currentWeapon.MaxAmmo;
            if (ammoPercentage <= 0)
                ammoText.color = noAmmoColor;
            else if (ammoPercentage <= 0.3f)
                ammoText.color = lowAmmoColor;
            else
                ammoText.color = normalAmmoColor;
        }

        // Update reserve ammo
        if (reserveAmmoText != null)
            reserveAmmoText.text = currentWeapon.ReserveAmmo.ToString();

        // Update reload indicator
        if (reloadIndicator != null)
            reloadIndicator.gameObject.SetActive(currentWeapon.IsReloading);

        // Update crosshair visibility
        if (crosshair != null)
            crosshair.SetActive(!currentWeapon.IsReloading);
    }

    /// <summary>
    /// Hide weapon UI saat tidak ada weapon
    /// </summary>
    private void HideWeaponUI()
    {
        if (weaponNameText != null)
            weaponNameText.text = "";

        if (ammoText != null)
            ammoText.text = "-";

        if (reserveAmmoText != null)
            reserveAmmoText.text = "-";

        if (reloadIndicator != null)
            reloadIndicator.gameObject.SetActive(false);
    }

    /// <summary>
    /// Optional: Method untuk menampilkan hit marker
    /// </summary>
    public void ShowHitMarker()
    {
        // Implementasi hit marker bisa ditambahkan di sini
        // Misalnya dengan animasi atau perubahan warna crosshair
    }
}
