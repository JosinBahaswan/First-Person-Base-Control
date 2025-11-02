using UnityEngine;

/// <summary>
/// Script untuk membuat trap pendulum (ayunan) seperti gergaji ayunan atau bandul.
/// Letakkan script ini pada pivot point (titik ayunan).
/// </summary>
[AddComponentMenu("Game/Traps/Pendulum Trap")]
public class PendulumTrap : MonoBehaviour
{
    [Header("Pendulum Settings")]
    [Tooltip("Sudut maksimal ayunan (derajat)")]
    [SerializeField] private float maxSwingAngle = 45f;
    
    [Tooltip("Kecepatan ayunan")]
    [SerializeField] private float swingSpeed = 1f;
    
    [Tooltip("Axis rotasi (biasanya Z untuk ayunan 2D)")]
    [SerializeField] private SwingAxis swingAxis = SwingAxis.Z;
    
    [Tooltip("Mulai dari posisi tengah atau random?")]
    [SerializeField] private bool randomStartPosition = false;
    
    [Header("Advanced Physics")]
    [Tooltip("Gunakan physics realistic (lebih smooth tapi lebih berat)")]
    [SerializeField] private bool useRealisticPhysics = false;
    
    [Tooltip("Panjang pendulum (hanya untuk realistic physics)")]
    [SerializeField] private float pendulumLength = 3f;
    
    [Tooltip("Damping/friction (0 = no damping, 1 = heavy damping)")]
    [SerializeField] private float damping = 0f;
    
    [Header("Sound")]
    [Tooltip("Sound loop saat pendulum berayun")]
    [SerializeField] private AudioClip swingSound;
    
    [Tooltip("Volume sound")]
    [SerializeField] private float volume = 0.3f;
    
    // Private variables
    private float currentAngle;
    private float angularVelocity;
    private AudioSource audioSource;
    private float timeOffset;
    
    private void Start()
    {
        // Random start position
        if (randomStartPosition)
        {
            timeOffset = Random.Range(0f, Mathf.PI * 2f);
        }
        
        // Setup realistic physics
        if (useRealisticPhysics)
        {
            // Set initial angle
            currentAngle = maxSwingAngle * Mathf.Deg2Rad;
            angularVelocity = 0f;
        }
        
        // Setup audio
        if (swingSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.clip = swingSound;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.spatialBlend = 1f;
            audioSource.Play();
        }
    }
    
    private void Update()
    {
        if (useRealisticPhysics)
        {
            UpdateRealisticPendulum();
        }
        else
        {
            UpdateSimplePendulum();
        }
    }
    
    private void UpdateSimplePendulum()
    {
        // Simple sine wave swing
        float angle = maxSwingAngle * Mathf.Sin((Time.time + timeOffset) * swingSpeed);
        
        // Apply rotation based on axis
        Vector3 rotation = Vector3.zero;
        switch (swingAxis)
        {
            case SwingAxis.X:
                rotation = new Vector3(angle, 0, 0);
                break;
            case SwingAxis.Y:
                rotation = new Vector3(0, angle, 0);
                break;
            case SwingAxis.Z:
                rotation = new Vector3(0, 0, angle);
                break;
        }
        
        transform.localRotation = Quaternion.Euler(rotation);
    }
    
    private void UpdateRealisticPendulum()
    {
        // Realistic pendulum physics using differential equations
        // d²θ/dt² = -(g/L) * sin(θ) - damping * dθ/dt
        
        float gravity = 9.81f;
        float angularAcceleration = -(gravity / pendulumLength) * Mathf.Sin(currentAngle) - damping * angularVelocity;
        
        // Update velocity and angle
        angularVelocity += angularAcceleration * Time.deltaTime;
        currentAngle += angularVelocity * Time.deltaTime;
        
        // Convert to degrees
        float angleDegrees = currentAngle * Mathf.Rad2Deg;
        
        // Apply rotation
        Vector3 rotation = Vector3.zero;
        switch (swingAxis)
        {
            case SwingAxis.X:
                rotation = new Vector3(angleDegrees, 0, 0);
                break;
            case SwingAxis.Y:
                rotation = new Vector3(0, angleDegrees, 0);
                break;
            case SwingAxis.Z:
                rotation = new Vector3(0, 0, angleDegrees);
                break;
        }
        
        transform.localRotation = Quaternion.Euler(rotation);
    }
    
    /// <summary>
    /// Set kecepatan ayunan baru
    /// </summary>
    public void SetSwingSpeed(float newSpeed)
    {
        swingSpeed = newSpeed;
    }
    
    /// <summary>
    /// Set sudut maksimal baru
    /// </summary>
    public void SetMaxAngle(float newAngle)
    {
        maxSwingAngle = newAngle;
    }
    
    /// <summary>
    /// Stop pendulum
    /// </summary>
    public void Stop()
    {
        enabled = false;
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
    
    /// <summary>
    /// Resume pendulum
    /// </summary>
    public void Resume()
    {
        enabled = true;
        if (audioSource != null && swingSound != null)
        {
            audioSource.Play();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw swing arc
        Gizmos.color = Color.yellow;
        
        // Draw center point
        Gizmos.DrawSphere(transform.position, 0.1f);
        
        // Draw max swing positions
        float angleRad = maxSwingAngle * Mathf.Deg2Rad;
        Vector3 direction1 = Vector3.zero;
        Vector3 direction2 = Vector3.zero;
        
        switch (swingAxis)
        {
            case SwingAxis.X:
                direction1 = Quaternion.Euler(maxSwingAngle, 0, 0) * Vector3.down;
                direction2 = Quaternion.Euler(-maxSwingAngle, 0, 0) * Vector3.down;
                break;
            case SwingAxis.Y:
                direction1 = Quaternion.Euler(0, maxSwingAngle, 0) * Vector3.down;
                direction2 = Quaternion.Euler(0, -maxSwingAngle, 0) * Vector3.down;
                break;
            case SwingAxis.Z:
                direction1 = Quaternion.Euler(0, 0, maxSwingAngle) * Vector3.down;
                direction2 = Quaternion.Euler(0, 0, -maxSwingAngle) * Vector3.down;
                break;
        }
        
        float length = useRealisticPhysics ? pendulumLength : 3f;
        Gizmos.DrawLine(transform.position, transform.position + direction1 * length);
        Gizmos.DrawLine(transform.position, transform.position + direction2 * length);
        
        // Draw arc
        Gizmos.color = Color.cyan;
        Vector3 previousPoint = transform.position + direction1 * length;
        for (int i = 1; i <= 20; i++)
        {
            float t = i / 20f;
            float angle = Mathf.Lerp(-maxSwingAngle, maxSwingAngle, t);
            Vector3 dir = Vector3.zero;
            
            switch (swingAxis)
            {
                case SwingAxis.X:
                    dir = Quaternion.Euler(angle, 0, 0) * Vector3.down;
                    break;
                case SwingAxis.Y:
                    dir = Quaternion.Euler(0, angle, 0) * Vector3.down;
                    break;
                case SwingAxis.Z:
                    dir = Quaternion.Euler(0, 0, angle) * Vector3.down;
                    break;
            }
            
            Vector3 point = transform.position + dir * length;
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }
}

public enum SwingAxis
{
    X,
    Y,
    Z
}
