using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomChestItem : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private GameObject[] availableItems;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool spawnOnStart = false;
    [SerializeField] private bool destroyAfterSpawn = false;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnForce = 5f;
    [SerializeField] private Vector3 spawnOffset = Vector3.up;
    [SerializeField] private bool addRandomRotation = false;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioSource audioSource;

    private bool hasSpawned = false;

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnRandomItem();
        }
    }

    /// <summary>
    /// Spawn item acak dari list availableItems
    /// </summary>
    public void SpawnRandomItem()
    {
        // Cek apakah sudah pernah spawn dan destroyAfterSpawn aktif
        if (hasSpawned && destroyAfterSpawn)
        {
            Debug.LogWarning("Chest sudah dibuka dan tidak bisa dibuka lagi!");
            return;
        }

        // Cek apakah ada item yang tersedia
        if (availableItems == null || availableItems.Length == 0)
        {
            Debug.LogError("Tidak ada item yang tersedia untuk di-spawn!");
            return;
        }

        // Pilih item secara acak
        int randomIndex = Random.Range(0, availableItems.Length);
        GameObject selectedItem = availableItems[randomIndex];

        if (selectedItem == null)
        {
            Debug.LogError("Item yang dipilih adalah null!");
            return;
        }

        // Tentukan posisi spawn
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position + spawnOffset;

        // Spawn item
        GameObject spawnedItem = Instantiate(selectedItem, spawnPosition, Quaternion.identity);

        // Tambahkan rotasi acak jika diinginkan
        if (addRandomRotation)
        {
            spawnedItem.transform.rotation = Random.rotation;
        }

        // Tambahkan force jika item memiliki Rigidbody
        Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
        if (rb != null && spawnForce > 0)
        {
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.5f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;

            rb.AddForce(randomDirection * spawnForce, ForceMode.Impulse);
        }

        // Play sound jika ada
        if (openSound != null)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(openSound);
            }
            else
            {
                AudioSource.PlayClipAtPoint(openSound, transform.position);
            }
        }

        hasSpawned = true;

        Debug.Log($"Item spawned: {selectedItem.name}");

        // Hancurkan chest jika diinginkan
        if (destroyAfterSpawn)
        {
            Destroy(gameObject, openSound != null ? openSound.length : 0f);
        }
    }

    /// <summary>
    /// Method untuk membuka chest dari luar (bisa dipanggil dari script lain)
    /// </summary>
    public void OpenChest()
    {
        SpawnRandomItem();
    }

    /// <summary>
    /// Reset chest agar bisa dibuka lagi
    /// </summary>
    public void ResetChest()
    {
        hasSpawned = false;
    }

    // Visualisasi spawn point di editor
    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.3f);
            Gizmos.DrawLine(transform.position, spawnPoint.position);
        }
        else
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + spawnOffset, 0.3f);
        }
    }
}
