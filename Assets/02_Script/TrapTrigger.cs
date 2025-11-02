using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Script untuk trigger/aktivator trap seperti pressure plate, tripwire, dll.
/// Dapat mengaktifkan trap lain saat player atau object masuk ke trigger area.
/// </summary>
[AddComponentMenu("Game/Traps/Trap Trigger")]
public class TrapTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Layer yang bisa trigger (biasanya Player)")]
    [SerializeField] private LayerMask triggerLayers;

    [Tooltip("Trigger hanya sekali atau berulang?")]
    [SerializeField] private bool singleUse = false;

    [Tooltip("Delay sebelum trigger aktif (detik)")]
    [SerializeField] private float activationDelay = 0f;

    [Header("Target Traps")]
    [Tooltip("Trap yang akan diaktifkan saat triggered")]
    [SerializeField] private GameObject[] trapsToActivate;

    [Tooltip("Method yang akan dipanggil pada trap (biasanya 'ActivateTrap' atau 'ExtendSpike')")]
    [SerializeField] private string activationMethodName = "ActivateTrap";

    [Header("Visual Feedback")]
    [Tooltip("Material saat trigger belum aktif")]
    [SerializeField] private Material inactiveMaterial;

    [Tooltip("Material saat trigger sudah aktif")]
    [SerializeField] private Material activeMaterial;

    [Tooltip("Renderer object untuk visual feedback")]
    [SerializeField] private MeshRenderer visualRenderer;

    [Header("Audio")]
    [SerializeField] private AudioClip triggerSound;

    [Header("Animation")]
    [Tooltip("Animator untuk animasi trigger")]
    [SerializeField] private Animator triggerAnimator;

    [Tooltip("Trigger name untuk animasi")]
    [SerializeField] private string animationTriggerName = "Triggered";

    [Header("Events")]
    [Tooltip("Event yang dipanggil saat trigger aktif")]
    public UnityEvent OnTriggered;

    [Tooltip("Event yang dipanggil saat trigger reset")]
    public UnityEvent OnReset;

    [Header("Debug")]
    [SerializeField] private bool showDebugMessages = true;

    // Private variables
    private bool hasBeenTriggered = false;
    private AudioSource audioSource;

    private void Start()
    {
        // Setup audio
        if (triggerSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }

        // Pastikan collider adalah trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Set material awal
        if (visualRenderer != null && inactiveMaterial != null)
        {
            visualRenderer.material = inactiveMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check layer
        if ((triggerLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        // Check single use
        if (singleUse && hasBeenTriggered)
            return;

        if (showDebugMessages)
        {
            Debug.Log($"[TrapTrigger] {other.gameObject.name} mengaktifkan trigger {gameObject.name}!");
        }

        // Activate dengan delay
        if (activationDelay > 0)
        {
            Invoke(nameof(ActivateTraps), activationDelay);
        }
        else
        {
            ActivateTraps();
        }
    }

    private void ActivateTraps()
    {
        hasBeenTriggered = true;

        // Visual feedback
        if (visualRenderer != null && activeMaterial != null)
        {
            visualRenderer.material = activeMaterial;
        }

        // Animation
        if (triggerAnimator != null && !string.IsNullOrEmpty(animationTriggerName))
        {
            triggerAnimator.SetTrigger(animationTriggerName);
        }

        // Sound
        if (audioSource != null && triggerSound != null)
        {
            audioSource.PlayOneShot(triggerSound);
        }

        // Activate all connected traps
        foreach (GameObject trap in trapsToActivate)
        {
            if (trap != null)
            {
                // Send message to trap
                trap.SendMessage(activationMethodName, SendMessageOptions.DontRequireReceiver);

                if (showDebugMessages)
                {
                    Debug.Log($"[TrapTrigger] Mengaktifkan trap: {trap.name}");
                }
            }
        }

        // Invoke Unity Event
        OnTriggered?.Invoke();
    }

    /// <summary>
    /// Reset trigger sehingga bisa digunakan lagi
    /// </summary>
    public void ResetTrigger()
    {
        hasBeenTriggered = false;

        // Reset visual
        if (visualRenderer != null && inactiveMaterial != null)
        {
            visualRenderer.material = inactiveMaterial;
        }

        // Invoke reset event
        OnReset?.Invoke();

        if (showDebugMessages)
        {
            Debug.Log($"[TrapTrigger] Trigger {gameObject.name} direset");
        }
    }

    /// <summary>
    /// Aktifkan trigger secara manual (tanpa collision)
    /// </summary>
    public void ManualActivate()
    {
        if (!hasBeenTriggered || !singleUse)
        {
            ActivateTraps();
        }
    }
}
