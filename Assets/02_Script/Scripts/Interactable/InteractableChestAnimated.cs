using UnityEngine;

public class InteractableChestAnimated : LockableInteractable
{
    [Header("Animator Settings")]
    [Tooltip("Animator pada prefab chest. Jika kosong, akan dicari otomatis di GameObject ini lalu ke anak-anaknya.")]
    public Animator animator;                // Animator on the chest prefab
    public bool useTrigger = true;           // Use Trigger parameters, otherwise use a Bool
    public string openTrigger = "Open";      // Trigger name to open
    public string closeTrigger = "Close";    // Trigger name to close
    public string isOpenBool = "";           // If not using trigger, set a bool parameter name

    [Header("Behavior")]
    [Tooltip("Jika true, chest hanya bisa dibuka sekali dan tidak dapat ditutup kembali.")]
    public bool openOneShot = true;          // If true, chest can only be opened once (no close)

    private void Awake()
    {
        AutoAssignAnimator();
    }

    public override void Start()
    {
        base.Start();
        AutoAssignAnimator();

        // Try to sync initial state from animator bool if provided
        if (!string.IsNullOrEmpty(isOpenBool) && animator != null)
        {
            isOpen = animator.GetBool(isOpenBool);
        }
    }

    private void OnValidate()
    {
        // Bantu auto-assign saat di editor ketika komponen ditambahkan
        if (animator == null)
        {
            AutoAssignAnimator(false);
        }
    }

    private void AutoAssignAnimator(bool logIfMissing = true)
    {
        if (animator != null) return;

        // Coba cari di GameObject yang sama terlebih dahulu
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            // Jika tidak ada, cari di anak-anak
            animator = GetComponentInChildren<Animator>(true);
        }

        if (logIfMissing && animator == null)
        {
            Debug.LogWarning($"[InteractableChestAnimated] Animator tidak ditemukan pada '{gameObject.name}'. Tambahkan Animator ke prefab atau assign manual ke field 'animator'.");
        }
    }

    // Called by base LockableInteractable after lock checks pass
    public override void ToggleOpenClose()
    {
        if (animator == null) return;

        if (isOpen)
        {
            if (openOneShot)
            {
                // Already opened and one-shot: ignore close request
                return;
            }

            PlayClose();
            OnClosed?.Invoke();
        }
        else
        {
            PlayOpen();
            OnOpened?.Invoke();
        }

        isOpen = !isOpen;
    }

    private void PlayOpen()
    {
        if (useTrigger)
        {
            if (!string.IsNullOrEmpty(closeTrigger)) animator.ResetTrigger(closeTrigger);
            if (!string.IsNullOrEmpty(openTrigger)) animator.SetTrigger(openTrigger);
        }
        else if (!string.IsNullOrEmpty(isOpenBool))
        {
            animator.SetBool(isOpenBool, true);
        }
    }

    private void PlayClose()
    {
        if (useTrigger)
        {
            if (!string.IsNullOrEmpty(openTrigger)) animator.ResetTrigger(openTrigger);
            if (!string.IsNullOrEmpty(closeTrigger)) animator.SetTrigger(closeTrigger);
        }
        else if (!string.IsNullOrEmpty(isOpenBool))
        {
            animator.SetBool(isOpenBool, false);
        }
    }
}
