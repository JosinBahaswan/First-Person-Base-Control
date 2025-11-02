using UnityEngine;

/// <summary>
/// Script khusus untuk trap gergaji berputar.
/// Tambahkan script ini untuk menambahkan efek rotasi pada gergaji.
/// </summary>
[AddComponentMenu("Game/Traps/Rotating Saw Trap")]
public class RotatingSawTrap : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Kecepatan rotasi gergaji")]
    [SerializeField] private float rotationSpeed = 360f;
    
    [Tooltip("Axis rotasi (X, Y, atau Z)")]
    [SerializeField] private RotationAxis rotationAxis = RotationAxis.Z;
    
    [Header("Sound")]
    [Tooltip("Sound loop saat gergaji berputar")]
    [SerializeField] private AudioClip sawSound;
    
    [Tooltip("Volume sound")]
    [SerializeField] private float volume = 0.5f;
    
    private AudioSource audioSource;
    
    private void Start()
    {
        // Setup audio source untuk sound loop
        if (sawSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = sawSound;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.Play();
        }
    }
    
    private void Update()
    {
        // Rotate berdasarkan axis yang dipilih
        Vector3 rotation = Vector3.zero;
        
        switch (rotationAxis)
        {
            case RotationAxis.X:
                rotation = Vector3.right;
                break;
            case RotationAxis.Y:
                rotation = Vector3.up;
                break;
            case RotationAxis.Z:
                rotation = Vector3.forward;
                break;
        }
        
        transform.Rotate(rotation * rotationSpeed * Time.deltaTime);
    }
}

public enum RotationAxis
{
    X,
    Y,
    Z
}
