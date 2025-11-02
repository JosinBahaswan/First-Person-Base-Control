using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Support script untuk AIHunter - menangani jumpscare animator dan disable setelah button release 5x
/// Attach script ini ke enemy GameObject yang sama dengan AIHunter
/// </summary>
public class AIHunterSupport : MonoBehaviour
{
    [Header("Jumpscare Settings")]
    [SerializeField] private Animator zombieAnimator;
    [Tooltip("Durasi animasi jumpscare (detik) - waktu tunggu sebelum play idle animation")]
    [SerializeField] private float jumpscareAnimationDuration = 2f;
    [SerializeField] private GameObject jumpscareCamera; // Kamera khusus untuk jumpscare scene
    [SerializeField] private int releasesToDisable = 5;
    [SerializeField] private float jumpscareDamage = 20f; // Damage yang diberikan ke player saat jumpscare
    [SerializeField] private bool showDebugLogs = true;
    [Tooltip("Jika animation clip di-loop, set true untuk force stop animation setelah escape")]
    [SerializeField] private bool forceStopLoopedAnimation = true;

    [Header("AI Re-Enable Settings")]
    [Tooltip("Toggle untuk re-enable AI setelah jumpscare")]
    [SerializeField] private bool reEnableAIAfterJumpscare = true;
    [Tooltip("Jeda waktu (detik) sebelum AIHunter script diaktifkan kembali setelah escape")]
    [SerializeField] private float aiReEnableDelay = 3f;

    [Header("Multiple Jumpscare Settings")]
    [Tooltip("Allow zombie untuk jumpscare berkali-kali")]
    [SerializeField] private bool allowMultipleJumpscares = false;
    [Tooltip("Cooldown (detik) sebelum bisa jumpscare lagi")]
    [SerializeField] private float jumpscareCooldown = 10f;

    [Header("UI Button (Optional - untuk Mobile)")]
    [SerializeField] private Button releaseButton; // Drag UI Button disini
    [SerializeField] private GameObject buttonParent; // Parent GameObject button untuk show/hide

    [Header("Release Detection")]
    [SerializeField] private KeyCode releaseKey = KeyCode.Space;
    [SerializeField] private string releaseButtonName = "Jump"; // Untuk Input.GetButtonDown
    [SerializeField] private bool useMobileButton = false; // Jika true, pakai UI button

    [Header("Jumpscare State")]
    [SerializeField] private bool isJumpscareActive = false;
    [SerializeField] private int currentReleaseCount = 0;

    [Header("Events")]
    public UnityEvent onJumpscareStart;
    public UnityEvent onJumpscareEnd;
    public UnityEvent onReleasePressed; // Setiap kali button ditekan
    public UnityEvent onAllReleasesComplete; // Setelah 5x release

    private AIHunter aiHunter;
    private bool jumpscareTriggered = false;
    private float lastJumpscareTime = -999f; // Track waktu jumpscare terakhir
    private bool isEscaping = false; // Flag untuk mencegah input setelah escape complete
    private Coroutine endJumpscareCoroutine = null; // Track coroutine yang sedang berjalan
    private float lastInputTime = 0f; // Debounce untuk input
    private const float INPUT_DEBOUNCE = 0.2f; // 200ms debounce between inputs
    private bool isProcessingInput = false; // Prevent re-entrant calls

    private void Awake()
    {
        // Auto-find animator jika tidak di-assign
        if (zombieAnimator == null)
        {
            zombieAnimator = GetComponent<Animator>();
        }

        // Get reference ke AIHunter
        aiHunter = GetComponent<AIHunter>();

        // Hide button di awal
        if (buttonParent != null)
        {
            buttonParent.SetActive(false);
        }
        else if (releaseButton != null)
        {
            releaseButton.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        // Subscribe ke event attack jumpscare dari AIHunter
        if (aiHunter != null)
        {
            // Tambahkan method TriggerJumpscare ke onAttackJumpscareEvent
            aiHunter.onAttackJumpscareEvent.AddListener(TriggerJumpscare);
        }

        // Setup UI button click listener
        if (releaseButton != null)
        {
            releaseButton.onClick.AddListener(OnReleaseButtonPressed);
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - UI Button connected");
        }
    }

    private void Update()
    {
        // HANYA cek input dari keyboard jika TIDAK menggunakan mobile button
        // Jika pakai mobile button, input HANYA dari onClick event
        if (isJumpscareActive && !useMobileButton)
        {
            // Cek input dari keyboard
            bool releasePressed = Input.GetKeyDown(releaseKey);

            // DISABLE fallback ke Input Manager button untuk avoid double input
            // Jika ingin pakai Input Manager, set useMobileButton = false dan pakai keyboard saja

            if (releasePressed)
            {
                OnReleaseButtonPressed();
            }
        }
    }

    /// <summary>
    /// Dipanggil oleh AIHunter saat attack event terjadi
    /// </summary>
    public void TriggerJumpscare()
    {
        // Check jika tidak allow multiple dan sudah pernah trigger
        if (!allowMultipleJumpscares && jumpscareTriggered)
        {
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - Jumpscare already triggered (one-time only), skipping");
            return;
        }

        // Check cooldown jika allow multiple
        if (allowMultipleJumpscares && Time.time - lastJumpscareTime < jumpscareCooldown)
        {
            float remainingCooldown = jumpscareCooldown - (Time.time - lastJumpscareTime);
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - Jumpscare on cooldown ({remainingCooldown:F1}s remaining)");
            return;
        }

        // Check jika jumpscare sedang aktif
        if (isJumpscareActive)
        {
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - Jumpscare already active, skipping");
            return;
        }

        jumpscareTriggered = true;
        isJumpscareActive = true;
        isEscaping = false; // Reset escape flag untuk jumpscare baru
        currentReleaseCount = 0;
        lastJumpscareTime = Time.time;
        lastInputTime = 0f; // Reset input debounce untuk jumpscare baru

        // Apply damage ke player
        ApplyJumpscareDamageToPlayer();

        // 1. Set jumpscare camera active
        if (jumpscareCamera != null)
        {
            jumpscareCamera.SetActive(true);
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - Jumpscare camera activated");
        }

        // 2. Disable AIHunter script
        if (aiHunter != null)
        {
            aiHunter.enabled = false;
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - AIHunter disabled");
        }

        // 3. Stop NavMeshAgent dan set Animator ke jumpscare
        if (zombieAnimator != null)
        {
            // HARDCODED: Set Move parameter to -1 untuk trigger jumpscare atau freeze
            zombieAnimator.SetFloat("Move", -1f);

            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - Animator Move set to -1 (jumpscare/freeze)");
        }

        // Stop NavMeshAgent movement
        if (aiHunter != null)
        {
            NavMeshAgent agent = aiHunter.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
            }
        }

        // 4. Show UI button if using mobile
        if (useMobileButton)
        {
            if (buttonParent != null)
            {
                buttonParent.SetActive(true);
            }
            else if (releaseButton != null)
            {
                releaseButton.gameObject.SetActive(true);
            }
        }

        // Invoke event
        onJumpscareStart?.Invoke();

        if (showDebugLogs)
            Debug.Log($"[AIHunterSupport] {name} - Jumpscare started! Press {releaseKey} {releasesToDisable}x to escape");
    }

    /// <summary>
    /// Method yang dipanggil saat button release ditekan (keyboard, gamepad, atau UI button)
    /// </summary>
    public void OnReleaseButtonPressed()
    {
        // PREVENT RE-ENTRANT CALLS
        if (isProcessingInput)
        {
            if (showDebugLogs)
                Debug.LogWarning($"[AIHunterSupport] {name} - ❌ Input REJECTED: Already processing input (re-entrant call blocked)");
            return;
        }

        isProcessingInput = true;

        // SUPER DETAILED DEBUG
        if (showDebugLogs)
        {
            Debug.Log($"[AIHunterSupport] {name} - ⚡ OnReleaseButtonPressed CALLED at frame {Time.frameCount}, time {Time.time:F3}");
            Debug.Log($"   Current State: isActive={isJumpscareActive}, isEscaping={isEscaping}, count={currentReleaseCount}/{releasesToDisable}");
        }

        // DEBOUNCE: Cegah spam input terlalu cepat
        if (Time.time - lastInputTime < INPUT_DEBOUNCE)
        {
            if (showDebugLogs)
                Debug.LogWarning($"[AIHunterSupport] {name} - ❌ Input REJECTED: Too fast (debounce) - last input was {Time.time - lastInputTime:F3}s ago");
            isProcessingInput = false;
            return;
        }

        // Jangan terima input jika sedang escaping atau jumpscare tidak aktif
        if (!isJumpscareActive)
        {
            if (showDebugLogs)
                Debug.LogWarning($"[AIHunterSupport] {name} - ❌ Input REJECTED: Jumpscare not active");
            isProcessingInput = false;
            return;
        }

        if (isEscaping)
        {
            if (showDebugLogs)
                Debug.LogWarning($"[AIHunterSupport] {name} - ❌ Input REJECTED: Already escaping, waiting for animation to finish");
            isProcessingInput = false;
            return;
        }

        // PENTING: Cek jika sudah mencapai max, jangan tambah lagi
        if (currentReleaseCount >= releasesToDisable)
        {
            if (showDebugLogs)
                Debug.LogWarning($"[AIHunterSupport] {name} - ❌ Input REJECTED: Already at max releases ({currentReleaseCount}/{releasesToDisable})");
            isProcessingInput = false;
            return;
        }

        // UPDATE last input time SEBELUM increment
        lastInputTime = Time.time;

        // INCREMENT counter
        currentReleaseCount++;

        if (showDebugLogs)
            Debug.Log($"[AIHunterSupport] {name} - ✅✅✅ Release ACCEPTED! ({currentReleaseCount}/{releasesToDisable}) at time {Time.time:F3}");

        // Check apakah sudah mencapai jumlah yang dibutuhkan SEBELUM invoke event
        if (currentReleaseCount >= releasesToDisable)
        {
            isEscaping = true; // Set flag agar tidak bisa input lagi
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - 🎯🎯🎯 ALL {releasesToDisable} RELEASES COMPLETE! Starting escape sequence...");

            isProcessingInput = false; // Reset flag sebelum EndJumpscare

            EndJumpscare();
        }
        else
        {
            // Invoke event hanya jika belum complete (untuk update UI, etc)
            // NOTE: Jangan hook OnReleaseButtonPressed ke onReleasePressed event!
            onReleasePressed?.Invoke();

            isProcessingInput = false;
        }
    }

    /// <summary>
    /// Akhiri jumpscare setelah release count tercapai
    /// </summary>
    private void EndJumpscare()
    {
        // Safety check: jika coroutine sudah berjalan, jangan jalankan lagi
        if (endJumpscareCoroutine != null)
        {
            if (showDebugLogs)
                Debug.LogWarning($"[AIHunterSupport] {name} - EndJumpscare already running, skipping duplicate call");
            return;
        }

        if (showDebugLogs)
            Debug.Log($"[AIHunterSupport] {name} - Jumpscare escape! All releases complete ({currentReleaseCount}/{releasesToDisable}). Waiting for animation to finish...");

        // Start coroutine untuk handle animation dan re-enable AI
        endJumpscareCoroutine = StartCoroutine(HandleJumpscareEnd());
    }

    /// <summary>
    /// Coroutine untuk handle end jumpscare dengan timing yang benar
    /// </summary>
    private IEnumerator HandleJumpscareEnd()
    {
        // PENTING: AIHunter harus tetap DISABLED sampai proses selesai
        if (aiHunter != null && aiHunter.enabled)
        {
            aiHunter.enabled = false;
            if (showDebugLogs)
                Debug.LogWarning($"[AIHunterSupport] {name} - Force disable AIHunter during jumpscare end");
        }

        if (showDebugLogs)
            Debug.Log($"[AIHunterSupport] {name} - [STEP 1] IMMEDIATELY stopping jumpscare camera and animation!");

        // 1. LANGSUNG Deactivate jumpscare camera (tidak tunggu animation)
        if (jumpscareCamera != null)
        {
            jumpscareCamera.SetActive(false);
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - ✅ Jumpscare camera deactivated IMMEDIATELY");
        }

        // 2. FORCE STOP animation jika looped dan RESET animator state
        if (zombieAnimator != null)
        {
            if (forceStopLoopedAnimation)
            {
                // Untuk animation yang di-loop, HARUS set speed ke 0 untuk stop
                zombieAnimator.speed = 0f;
                if (showDebugLogs)
                    Debug.Log($"[AIHunterSupport] {name} - ✅ Animator speed set to 0 (FORCE STOP LOOP)");
            }

            // CRITICAL: Reset SEMUA animator parameters untuk clear jumpscare state
            // Set Move ke nilai POSITIF KECIL (0.01f) untuk trigger blend tree reset
            // Jangan set 0 atau -1 karena bisa stuck di jumpscare state
            zombieAnimator.SetFloat("Move", 0.01f);

            // Force update animator multiple times untuk ensure state change
            zombieAnimator.Update(0f);
            zombieAnimator.Update(Time.deltaTime);

            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - ✅ Animator Move set to 0.01 (force idle/wander state)");
        }

        // Wait 2 frames untuk animator sync dan blend tree transition
        yield return null;
        yield return new WaitForEndOfFrame();

        // 3. Resume animator speed dan set proper idle value (jika di-pause)
        if (zombieAnimator != null)
        {
            if (forceStopLoopedAnimation)
            {
                zombieAnimator.speed = 1f;
                if (showDebugLogs)
                    Debug.Log($"[AIHunterSupport] {name} - Animator speed restored to 1");
            }

            // Set Move ke 0 untuk true idle (AFTER animator reset)
            zombieAnimator.SetFloat("Move", 0f);
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - Animator Move set to 0 (true idle state)");
        }

        // 4. Hide UI button
        if (useMobileButton)
        {
            if (buttonParent != null)
            {
                buttonParent.SetActive(false);
            }
            else if (releaseButton != null)
            {
                releaseButton.gameObject.SetActive(false);
            }
        }

        // 5. Reset counters dan flags SETELAH animasi selesai
        currentReleaseCount = 0;
        isJumpscareActive = false;
        isEscaping = false;

        if (showDebugLogs)
            Debug.Log($"[AIHunterSupport] {name} - [STEP 2] Flags reset. isJumpscareActive: false, isEscaping: false");

        // 6. Reset jumpscare flag jika allow multiple
        if (allowMultipleJumpscares)
        {
            jumpscareTriggered = false;
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - Multiple jumpscares allowed, flag reset");
        }

        // 7. Delay untuk zombie idle animation
        if (reEnableAIAfterJumpscare && aiHunter != null)
        {
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - [STEP 3] Zombie in IDLE state for {aiReEnableDelay}s... AIHunter: STILL DISABLED");

            yield return new WaitForSeconds(aiReEnableDelay);

            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - [STEP 4] Idle delay finished. Re-enabling AIHunter now...");

            // Final check: Pastikan animator COMPLETELY RESET sebelum re-enable AI
            if (zombieAnimator != null)
            {
                // Reset ke wander/idle value (small positive untuk ensure proper blend tree state)
                zombieAnimator.SetFloat("Move", 0.01f);
                zombieAnimator.Update(0f);

                // Kemudian set ke true idle
                zombieAnimator.SetFloat("Move", 0f);
                zombieAnimator.Update(0f);

                if (showDebugLogs)
                    Debug.Log($"[AIHunterSupport] {name} - Final animator DOUBLE RESET: Move = 0.01 -> 0 (ensure clean state)");
            }

            // Resume NavMeshAgent
            NavMeshAgent agent = aiHunter.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = false;
                agent.ResetPath(); // Clear any old path
                if (showDebugLogs)
                    Debug.Log($"[AIHunterSupport] {name} - NavMeshAgent resumed and path reset");
            }

            // Wait 1 frame untuk NavMeshAgent sync
            yield return null;

            // Re-enable AIHunter
            aiHunter.enabled = true;

            // Wait 1 frame sebelum reset to wander
            yield return null;

            // Reset chase state dan kembali ke wander mode
            aiHunter.ResetToWanderMode();

            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - [STEP 5] ✅✅✅ AIHunter RE-ENABLED and reset to WANDER mode. Zombie will patrol now.");
        }
        else
        {
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - AIHunter will NOT be re-enabled (reEnableAIAfterJumpscare = false)");
        }

        // Clear coroutine reference
        endJumpscareCoroutine = null;

        // Invoke events
        onJumpscareEnd?.Invoke();
        onAllReleasesComplete?.Invoke();

        if (showDebugLogs)
            Debug.Log($"[AIHunterSupport] {name} - ========== JUMPSCARE SEQUENCE COMPLETE ==========");
    }

    /// <summary>
    /// Apply damage ke player saat jumpscare
    /// </summary>
    private void ApplyJumpscareDamageToPlayer()
    {
        if (jumpscareDamage <= 0f)
        {
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - Jumpscare damage is 0, skipping damage");
            return;
        }

        // Cari player dengan tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            if (showDebugLogs)
                Debug.LogWarning($"[AIHunterSupport] {name} - Player not found with tag 'Player'");
            return;
        }

        // Coba get Health component dari player
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth == null)
        {
            // Coba cari di children
            playerHealth = player.GetComponentInChildren<Health>();
        }
        if (playerHealth == null)
        {
            // Coba cari di parent
            playerHealth = player.GetComponentInParent<Health>();
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(jumpscareDamage);
            if (showDebugLogs)
                Debug.Log($"[AIHunterSupport] {name} - Applied {jumpscareDamage} damage to player. Player health: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}");
        }
        else
        {
            if (showDebugLogs)
                Debug.LogWarning($"[AIHunterSupport] {name} - Health component not found on player!");
        }
    }

    /// <summary>
    /// Optional: Disable enemy setelah jumpscare
    /// </summary>
    public void DisableEnemy()
    {
        if (showDebugLogs)
            Debug.Log($"[AIHunterSupport] {name} - Disabling enemy");

        // Disable AI
        if (aiHunter != null)
        {
            aiHunter.enabled = false;
        }

        // Disable colliders
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Hide renderers
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }

    /// <summary>
    /// Reset jumpscare state (untuk testing atau respawn)
    /// </summary>
    public void ResetJumpscareState()
    {
        jumpscareTriggered = false;
        isJumpscareActive = false;
        currentReleaseCount = 0;

        if (showDebugLogs)
            Debug.Log($"[AIHunterSupport] {name} - Jumpscare state reset");
    }

    /// <summary>
    /// Force end jumpscare (emergency)
    /// </summary>
    public void ForceEndJumpscare()
    {
        if (isJumpscareActive)
        {
            EndJumpscare();
        }
    }

    // Public getters
    public bool IsJumpscareActive => isJumpscareActive;
    public int CurrentReleaseCount => currentReleaseCount;
    public int ReleasesRemaining => Mathf.Max(0, releasesToDisable - currentReleaseCount);
    public bool HasTriggered => jumpscareTriggered;

    // Debug visualization
    private void OnGUI()
    {
        if (isJumpscareActive && showDebugLogs)
        {
            // Show on-screen prompt
            GUI.color = Color.red;
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 30;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;

            string text = useMobileButton
                ? $"TAP THE BUTTON! ({currentReleaseCount}/{releasesToDisable})"
                : $"PRESS {releaseKey}! ({currentReleaseCount}/{releasesToDisable})";

            GUI.Label(new Rect(0, Screen.height / 2 - 50, Screen.width, 100), text, style);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe dari event saat destroy
        if (aiHunter != null)
        {
            aiHunter.onAttackJumpscareEvent.RemoveListener(TriggerJumpscare);
        }

        // Remove button listener
        if (releaseButton != null)
        {
            releaseButton.onClick.RemoveListener(OnReleaseButtonPressed);
        }
    }
}
