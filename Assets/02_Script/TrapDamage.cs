using UnityEngine;

[AddComponentMenu("Game/Trap Damage")]
public class TrapDamage : MonoBehaviour
{
    [Header("Trap Settings")]
    [Tooltip("Damage dealt by this trap.")]
    [SerializeField] private float damage = 25f;
    [Tooltip("Which layers can be damaged by this trap.")]
    [SerializeField] private LayerMask damageLayers;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object is in the allowed layers
        if ((damageLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        // Try to get Health component
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
}
