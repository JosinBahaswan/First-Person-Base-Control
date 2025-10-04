using UnityEngine;

public class ExplosionTrigger : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public float damage = 50f;
    public GameObject explosionEffect;

    public void Explode()
    {
        // Trigger camera shake
        if (CameraShaker.Instance != null)
        {
            CameraShaker.Instance.ShakeFromExplosion();
        }

        // Create explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Find objects in explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            // Apply explosion force to rigidbodies
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Deal damage to player
            if (hit.CompareTag("Player"))
            {
                PlayerManager.Instance.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize explosion radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
