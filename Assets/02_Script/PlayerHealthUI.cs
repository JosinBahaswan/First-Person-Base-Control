using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Game/UI/Player Health UI")]
public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Slider UI for health bar.")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider damageSlider; // Slider kedua untuk efek damage
    [Tooltip("Reference to the player's Health component.")]
    [SerializeField] private Health playerHealth;

    [Header("Blood Overlay")]
    [Tooltip("Image yang akan muncul ketika player terkena damage (gambar darah di layar).")]
    [SerializeField] private Image bloodOverlayImage;
    [Tooltip("HP threshold untuk mulai menampilkan blood overlay (dalam persen, 0-1).")]
    [SerializeField] private float bloodOverlayThreshold = 0.5f; // 50% HP
    [Tooltip("Opacity maksimum blood overlay ketika HP sangat rendah (0-1).")]
    [SerializeField] private float maxBloodAlpha = 0.7f;
    [Tooltip("Kecepatan fade in blood overlay ketika terkena damage.")]
    [SerializeField] private float bloodFadeInSpeed = 2f;
    [Tooltip("Kecepatan fade out blood overlay ketika HP pulih.")]
    [SerializeField] private float bloodFadeOutSpeed = 1f;

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float damageDelay = 0.5f;
    [SerializeField] private float damageAnimationDuration = 1f;

    private Image healthFillImage;
    private Image damageFillImage;
    private float targetHealth;
    private Coroutine damageCoroutine;
    private float currentBloodAlpha = 0f;
    private float previousHealthPercent = 1f;

    private void Awake()
    {
        if (playerHealth == null)
        {
            Debug.Log("PlayerHealthUI: Player Health reference not set!");
            enabled = false;
            return;
        }
        if (healthSlider == null || damageSlider == null)
        {
            Debug.Log("PlayerHealthUI: Slider references not set!");
            enabled = false;
            return;
        }

        // Dapatkan reference ke fill image dari kedua slider
        healthFillImage = healthSlider.fillRect.GetComponent<Image>();
        damageFillImage = damageSlider.fillRect.GetComponent<Image>();

        // Set warna awal
        healthFillImage.color = normalColor;
        damageFillImage.color = damageColor;

        // Inisialisasi blood overlay
        if (bloodOverlayImage != null)
        {
            Color bloodColor = bloodOverlayImage.color;
            bloodColor.a = 0f;
            bloodOverlayImage.color = bloodColor;
        }
    }

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthUI;
        UpdateHealthUI(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void Update()
    {
        // Update blood overlay secara smooth
        if (bloodOverlayImage != null)
        {
            UpdateBloodOverlay();
        }
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthUI;
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
    }

    private void UpdateHealthUI(float current, float max)
    {
        healthSlider.maxValue = max;
        damageSlider.maxValue = max;

        // Update health bar langsung
        healthSlider.value = current;

        // Simpan health percent untuk blood overlay
        previousHealthPercent = current / max;

        // Mulai animasi damage
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        damageCoroutine = StartCoroutine(AnimateDamage(current));
    }

    private void UpdateBloodOverlay()
    {
        float currentHealthPercent = playerHealth.CurrentHealth / playerHealth.MaxHealth;
        float targetAlpha = 0f;

        // Hitung target alpha berdasarkan HP
        if (currentHealthPercent < bloodOverlayThreshold)
        {
            // Semakin rendah HP, semakin tinggi alpha
            // Normalisasi dari 0 sampai bloodOverlayThreshold menjadi 0 sampai maxBloodAlpha
            float normalizedHealth = currentHealthPercent / bloodOverlayThreshold;
            targetAlpha = Mathf.Lerp(maxBloodAlpha, 0f, normalizedHealth);
        }

        // Smooth lerp ke target alpha
        float fadeSpeed = targetAlpha > currentBloodAlpha ? bloodFadeInSpeed : bloodFadeOutSpeed;
        currentBloodAlpha = Mathf.Lerp(currentBloodAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

        // Apply alpha ke blood overlay image
        Color bloodColor = bloodOverlayImage.color;
        bloodColor.a = currentBloodAlpha;
        bloodOverlayImage.color = bloodColor;
    }

    private IEnumerator AnimateDamage(float targetValue)
    {
        // Tunggu sebentar sebelum mulai animasi
        yield return new WaitForSeconds(damageDelay);

        float startValue = damageSlider.value;
        float elapsedTime = 0f;

        while (elapsedTime < damageAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / damageAnimationDuration;

            // Gunakan fungsi smoothstep untuk animasi yang lebih halus
            t = t * t * (3f - 2f * t);

            damageSlider.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }

        damageSlider.value = targetValue;
    }
}

// TIP: For mobile, set CanvasScaler to "Scale With Screen Size" for responsive UI.
