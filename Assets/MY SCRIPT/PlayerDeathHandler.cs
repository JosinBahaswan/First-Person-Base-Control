using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    private Health health;
    private FirstPersonMovement movement;
    private bool isDead = false;

    void Awake()
    {
        health = GetComponent<Health>();
        movement = GetComponent<FirstPersonMovement>();
        if (health != null)
        {
            health.OnDeath += OnPlayerDeath;
        }
        else
        {
            Debug.LogError("PlayerDeathHandler: Tidak ditemukan komponen Health pada " + gameObject.name);
        }
    }

    void OnPlayerDeath()
    {
        if (isDead) return;
        isDead = true;
        if (movement != null)
        {
            movement.enabled = false;
        }
        Debug.Log("Player mati! (PlayerDeathHandler)");
        // Tambahkan aksi lain di sini, misal: animasi mati, respawn, dsb.
    }
}
