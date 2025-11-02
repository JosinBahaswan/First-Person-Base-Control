using UnityEngine;

/// <summary>
/// Base class untuk semua senjata
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] protected string weaponName = "Weapon";
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float fireRate = 0.5f; // Delay antar tembakan (detik)
    [SerializeField] protected float range = 100f;
    [SerializeField] protected int maxAmmo = 30;
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected int reserveAmmo = 90;
    [SerializeField] protected float reloadTime = 2f;

    [Header("Effects")]
    [SerializeField] protected GameObject muzzleFlashEffect;
    [SerializeField] protected GameObject impactEffect;
    [SerializeField] protected Transform muzzlePoint;

    [Header("Audio")]
    [SerializeField] protected AudioClip shootSound;
    [SerializeField] protected AudioClip reloadSound;
    [SerializeField] protected AudioClip emptySound;

    [Header("Animation")]
    [SerializeField] protected Animator weaponAnimator;

    protected AudioSource audioSource;
    protected Camera mainCamera;
    protected float nextFireTime = 0f;
    protected bool isReloading = false;

    // Properties
    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => isReloading;
    public string WeaponName => weaponName;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        mainCamera = Camera.main;
        currentAmmo = maxAmmo;
    }

    /// <summary>
    /// Method untuk menembak
    /// </summary>
    public virtual void Fire()
    {
        if (isReloading)
            return;

        if (Time.time < nextFireTime)
            return;

        if (currentAmmo <= 0)
        {
            PlayEmptySound();
            return;
        }

        PerformShoot();
        currentAmmo--;
        nextFireTime = Time.time + fireRate;
    }

    /// <summary>
    /// Method untuk reload
    /// </summary>
    public virtual void Reload()
    {
        if (isReloading || currentAmmo == maxAmmo || reserveAmmo <= 0)
            return;

        StartCoroutine(ReloadCoroutine());
    }

    protected virtual System.Collections.IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        if (weaponAnimator != null)
            weaponAnimator.SetTrigger("Reload");

        PlayReloadSound();

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
    }

    /// <summary>
    /// Logic penembakan actual
    /// </summary>
    protected virtual void PerformShoot()
    {
        // Play effects
        if (muzzleFlashEffect != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashEffect, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(flash, 0.1f);
        }

        if (weaponAnimator != null)
            weaponAnimator.SetTrigger("Shoot");

        PlayShootSound();

        // Raycast untuk hit detection
        RaycastHit hit;
        Vector3 shootDirection = mainCamera.transform.forward;

        if (Physics.Raycast(mainCamera.transform.position, shootDirection, out hit, range))
        {
            OnHit(hit);
        }
    }

    /// <summary>
    /// Method yang dipanggil saat raycast mengenai sesuatu
    /// </summary>
    protected virtual void OnHit(RaycastHit hit)
    {
        // Spawn impact effect
        if (impactEffect != null)
        {
            GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 2f);
        }

        // Apply damage jika target memiliki Health component
        Health health = hit.collider.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        // Debug line untuk melihat raycast
        Debug.DrawLine(mainCamera.transform.position, hit.point, Color.red, 1f);
    }

    /// <summary>
    /// Method untuk menambah ammo
    /// </summary>
    public virtual void AddAmmo(int amount)
    {
        reserveAmmo += amount;
    }

    protected void PlayShootSound()
    {
        if (shootSound != null && audioSource != null)
            audioSource.PlayOneShot(shootSound);
    }

    protected void PlayReloadSound()
    {
        if (reloadSound != null && audioSource != null)
            audioSource.PlayOneShot(reloadSound);
    }

    protected void PlayEmptySound()
    {
        if (emptySound != null && audioSource != null)
            audioSource.PlayOneShot(emptySound);
    }
}
