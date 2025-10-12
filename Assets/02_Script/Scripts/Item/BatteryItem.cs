using UnityEngine;

public class BatteryItem : MonoBehaviour
{
    [Header("Battery Settings")]
    public float batteryLifeAmount = 60f; // Amount of battery life to add in seconds
    
    [Header("Visual Settings")]
    public float rotationSpeed = 50f;
    public bool rotateItem = true;
    public GameObject visualObject;
    
    private void Update()
    {
        if (rotateItem && visualObject != null)
        {
            visualObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        FlashlightController flashlight = other.GetComponentInChildren<FlashlightController>();
        
        if (flashlight != null)
        {
            // Add battery life
            flashlight.AddBattery(batteryLifeAmount);
            
            // Play pickup sound or effect here if needed
            
            // Destroy the battery item
            Destroy(gameObject);
        }
    }
} 