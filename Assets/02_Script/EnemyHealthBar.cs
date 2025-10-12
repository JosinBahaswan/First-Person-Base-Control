using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Game/UI/Enemy Health Bar")]
public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("World-space Canvas containing the health bar.")]
    [SerializeField] private Canvas worldCanvas;
    [Tooltip("Slider UI for health bar.")]
    [SerializeField] private Slider healthSlider;
    [Tooltip("Reference to the enemy's Health component.")]
    [SerializeField] private Health enemyHealth;
    [Tooltip("Seconds to auto-hide after damage.")]
    [SerializeField] private float hideDelay = 2f;

    private Camera mainCamera;
    private float hideTimer = 0f;
    private bool isVisible = false;

    private void Awake()
    {
        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealthBar: Enemy Health reference not set!");
            enabled = false;
            return;
        }
        if (healthSlider == null)
        {
            Debug.LogError("EnemyHealthBar: Health Slider reference not set!");
            enabled = false;
            return;
        }
        if (worldCanvas == null)
        {
            Debug.LogError("EnemyHealthBar: World Canvas reference not set!");
            enabled = false;
            return;
        }
        mainCamera = Camera.main;
        worldCanvas.enabled = false;
    }

    private void OnEnable()
    {
        enemyHealth.OnHealthChanged += OnHealthChanged;
        enemyHealth.OnDeath += HideBar;
        // Initialize UI
        OnHealthChanged(enemyHealth.CurrentHealth, enemyHealth.MaxHealth);
    }

    private void OnDisable()
    {
        enemyHealth.OnHealthChanged -= OnHealthChanged;
        enemyHealth.OnDeath -= HideBar;
    }

    private void LateUpdate()
    {
        // Billboard: always face camera
        if (mainCamera && worldCanvas.enabled)
        {
            worldCanvas.transform.LookAt(worldCanvas.transform.position + mainCamera.transform.forward);
        }
        // Auto-hide after delay
        if (isVisible)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
            {
                HideBar();
            }
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;
        ShowBar();
    }

    private void ShowBar()
    {
        worldCanvas.enabled = true;
        isVisible = true;
        hideTimer = hideDelay;
    }

    private void HideBar()
    {
        worldCanvas.enabled = false;
        isVisible = false;
    }
}
