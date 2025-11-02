using UnityEngine;

/// <summary>
/// Script untuk trap tombak yang keluar dari dinding/lantai.
/// Menggunakan animasi atau movement untuk efek tombak yang keluar masuk.
/// </summary>
[AddComponentMenu("Game/Traps/Spike Trap")]
public class SpikeTrap : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Jarak tombak bergerak keluar")]
    [SerializeField] private float extendDistance = 2f;
    
    [Tooltip("Kecepatan tombak keluar")]
    [SerializeField] private float extendSpeed = 5f;
    
    [Tooltip("Durasi tombak tetap extended (detik)")]
    [SerializeField] private float extendedDuration = 2f;
    
    [Tooltip("Kecepatan tombak masuk kembali")]
    [SerializeField] private float retractSpeed = 3f;
    
    [Header("Activation")]
    [Tooltip("Aktif secara otomatis saat game start?")]
    [SerializeField] private bool autoActivate = true;
    
    [Tooltip("Delay sebelum aktivasi pertama (detik)")]
    [SerializeField] private float initialDelay = 0f;
    
    [Tooltip("Interval antara aktivasi (detik)")]
    [SerializeField] private float activationInterval = 5f;
    
    [Tooltip("Mode aktivasi")]
    [SerializeField] private ActivationMode mode = ActivationMode.Automatic;
    
    [Header("Direction")]
    [Tooltip("Arah tombak keluar (local space)")]
    [SerializeField] private Vector3 extendDirection = Vector3.up;
    
    [Header("Sound")]
    [SerializeField] private AudioClip extendSound;
    [SerializeField] private AudioClip retractSound;
    
    // Private variables
    private Vector3 startPosition;
    private Vector3 extendedPosition;
    private bool isExtended = false;
    private bool isMoving = false;
    private float timer = 0f;
    private AudioSource audioSource;
    
    private void Start()
    {
        startPosition = transform.localPosition;
        extendedPosition = startPosition + extendDirection.normalized * extendDistance;
        
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (extendSound != null || retractSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
        
        // Auto activate
        if (autoActivate && mode == ActivationMode.Automatic)
        {
            Invoke(nameof(StartTrapCycle), initialDelay);
        }
    }
    
    private void Update()
    {
        if (mode == ActivationMode.Automatic && !isMoving)
        {
            timer += Time.deltaTime;
        }
    }
    
    private void StartTrapCycle()
    {
        InvokeRepeating(nameof(ExtendSpike), 0f, activationInterval);
    }
    
    public void ExtendSpike()
    {
        if (!isMoving && !isExtended)
        {
            StopAllCoroutines();
            StartCoroutine(ExtendCoroutine());
        }
    }
    
    public void RetractSpike()
    {
        if (!isMoving && isExtended)
        {
            StopAllCoroutines();
            StartCoroutine(RetractCoroutine());
        }
    }
    
    private System.Collections.IEnumerator ExtendCoroutine()
    {
        isMoving = true;
        
        // Play extend sound
        if (audioSource != null && extendSound != null)
        {
            audioSource.PlayOneShot(extendSound);
        }
        
        // Move to extended position
        while (Vector3.Distance(transform.localPosition, extendedPosition) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                extendedPosition,
                extendSpeed * Time.deltaTime
            );
            yield return null;
        }
        
        transform.localPosition = extendedPosition;
        isExtended = true;
        isMoving = false;
        
        // Wait duration then retract
        if (mode == ActivationMode.Automatic)
        {
            yield return new WaitForSeconds(extendedDuration);
            StartCoroutine(RetractCoroutine());
        }
    }
    
    private System.Collections.IEnumerator RetractCoroutine()
    {
        isMoving = true;
        
        // Play retract sound
        if (audioSource != null && retractSound != null)
        {
            audioSource.PlayOneShot(retractSound);
        }
        
        // Move back to start position
        while (Vector3.Distance(transform.localPosition, startPosition) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                startPosition,
                retractSpeed * Time.deltaTime
            );
            yield return null;
        }
        
        transform.localPosition = startPosition;
        isExtended = false;
        isMoving = false;
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw extend direction
        Gizmos.color = Color.red;
        Vector3 start = Application.isPlaying ? startPosition : transform.localPosition;
        Vector3 end = start + extendDirection.normalized * extendDistance;
        
        Gizmos.DrawLine(transform.parent.TransformPoint(start), transform.parent.TransformPoint(end));
        Gizmos.DrawSphere(transform.parent.TransformPoint(end), 0.1f);
    }
}

public enum ActivationMode
{
    Automatic,  // Otomatis keluar masuk dengan interval
    Manual      // Harus dipanggil manual via script/event
}
