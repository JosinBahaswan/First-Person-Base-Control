using UnityEngine;

public class FireDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f; // Amount of damage per tick
    [SerializeField] private float damageTickRate = 0.5f; // How often the damage is applied in seconds
    
    private float nextDamageTime;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            // Use parent lookup so child colliders still route to the player's Health component
            Health playerHealth = other.GetComponentInParent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                nextDamageTime = Time.time + damageTickRate;
            }
        }
    }
}