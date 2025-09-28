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

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float damageDelay = 0.5f;
    [SerializeField] private float damageAnimationDuration = 1f;

    private Image healthFillImage;
    private Image damageFillImage;
    private float targetHealth;
    private Coroutine damageCoroutine;

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
    }

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthUI;
        UpdateHealthUI(playerHealth.CurrentHealth, playerHealth.MaxHealth);
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

        // Mulai animasi damage
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        damageCoroutine = StartCoroutine(AnimateDamage(current));
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
