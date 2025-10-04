using UnityEngine;

public class FlashlightBatteryItem : Item
{
    [Header("Battery Settings")]
    public float batteryLifeAmount = 60f; // Jumlah detik yang ditambahkan ke senter
    public bool destroyOnUse = true; // Apakah baterai akan hilang setelah digunakan
    
    [Header("Visual Settings")]
    public float rotationSpeed = 50f; // Kecepatan rotasi visual untuk baterai
    public bool rotateItem = true; // Apakah baterai berputar
    public GameObject visualObject; // Objek visual baterai
    
    private PlayerInteractionNoInventory batteryPlayerInteraction;
    
    private void Start()
    {
        // Pastikan visualObject diatur jika tidak diatur di inspector
        if (visualObject == null)
        {
            visualObject = gameObject;
        }
    }
    
    private void Update()
    {
        // Efek rotasi untuk baterai
        if (rotateItem && visualObject != null)
        {
            visualObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
    
    public override void OnInteract()
    {
        // Check if player has a flashlight
        if (batteryPlayerInteraction == null)
        {
            batteryPlayerInteraction = PlayerInteractionNoInventory.Instance;
            if (batteryPlayerInteraction == null) return;
        }
        
        if (batteryPlayerInteraction.holdItem != null && batteryPlayerInteraction.holdItem is FlashlightItem)
        {
            // Player memegang senter, langsung isi dayanya
            FlashlightItem flashlight = (FlashlightItem)batteryPlayerInteraction.holdItem;
            flashlight.AddBattery(batteryLifeAmount);
            Debug.Log($"Battery Item: Added {batteryLifeAmount} seconds to flashlight");
            
            // Hapus baterai setelah digunakan
            if (destroyOnUse)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Player tidak memegang senter, ambil baterainya seperti item biasa
            base.OnInteract();
        }
    }
} 