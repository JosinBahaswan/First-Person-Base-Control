using UnityEngine;

public class LandMineTrapSimple : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 50f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 700f;

    [Header("Effects")]
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip explosionSound;

    private bool hasExploded = false;

    /// <summary>
    /// Dipanggil dari script luar jika pemain memicu jebakan
    /// </summary>
    public void TriggerFromPlayer(GameObject player)
    {
        if (hasExploded) return;
        Debug.Log($"=== LANDMINE TRIGGERED BY PLAYER {player.name} ===");

        hasExploded = true;

        Explode();
        DealDamage(player);

        Destroy(gameObject);
    }

    /// <summary>
    /// Otomatis jalan kalau ada collider masuk area trigger
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;
        Debug.Log($"=== LANDMINE COLLISION with {other.gameObject.name} ===");

        hasExploded = true;

        Explode();
        DealDamage(other.gameObject);

        Destroy(gameObject);
    }

    /// <summary>
    /// Efek ledakan & gaya fisik
    /// </summary>
    private void Explode()
    {
        // Camera shake
        if (CameraShaker.Instance != null)
        {
            CameraShaker.Instance.ShakeFromExplosion();
        }

        // Efek visual
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Efek suara
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // Gaya ledakan
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

    /// <summary>
    /// Memberikan damage ke objek dengan komponen Health
    /// </summary>
    private void DealDamage(GameObject target)
    {
        Health health = target.GetComponent<Health>();
        if (health != null)
        {
            Debug.Log($"Health found! Current: {health.CurrentHealth}");
            health.TakeDamage(damageAmount);
            Debug.Log($"Damage given! Health after: {health.CurrentHealth}");
        }
        else
        {
            Debug.LogWarning("No Health component found on target!");
        }
    }
}
