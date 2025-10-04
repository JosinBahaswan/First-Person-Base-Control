using UnityEngine;

public class FlashlightDebugHelper : MonoBehaviour
{
    [Header("References")]
    public FlashlightItem targetFlashlight;
    
    [Header("Battery Controls")]
    [SerializeField] private float batteryAmount = 60f;
    
    [Header("Debug Controls")]
    [SerializeField] private bool forceOn = false;
    [SerializeField] private bool forceOff = false;
    [SerializeField] private bool recharge = false;
    [SerializeField] private bool syncState = false;
    
    void Update()
    {
        if (targetFlashlight == null)
        {
            // Try to find a flashlight in the scene
            targetFlashlight = FindObjectOfType<FlashlightItem>();
            return;
        }
        
        // Handle debug controls
        if (forceOn)
        {
            forceOn = false;
            ForceFlashlightOn();
        }
        
        if (forceOff)
        {
            forceOff = false;
            ForceFlashlightOff();
        }
        
        if (recharge)
        {
            recharge = false;
            RechargeFlashlight();
        }
        
        if (syncState)
        {
            syncState = false;
            SyncFlashlightState();
        }
    }
    
    // Button methods for inspector buttons
    public void ForceFlashlightOn()
    {
        if (targetFlashlight != null)
        {
            Light light = targetFlashlight.GetComponent<Light>();
            if (light != null)
            {
                light.enabled = true;
                targetFlashlight.SyncFlashlightState();
                Debug.Log("Debug Helper: Forced flashlight ON");
            }
        }
    }
    
    public void ForceFlashlightOff()
    {
        if (targetFlashlight != null)
        {
            Light light = targetFlashlight.GetComponent<Light>();
            if (light != null)
            {
                light.enabled = false;
                targetFlashlight.SyncFlashlightState();
                Debug.Log("Debug Helper: Forced flashlight OFF");
            }
        }
    }
    
    public void RechargeFlashlight()
    {
        if (targetFlashlight != null)
        {
            targetFlashlight.AddBattery(batteryAmount);
            Debug.Log($"Debug Helper: Added {batteryAmount} seconds to flashlight battery");
        }
    }
    
    public void SyncFlashlightState()
    {
        if (targetFlashlight != null)
        {
            targetFlashlight.SyncFlashlightState();
            Debug.Log("Debug Helper: Synced flashlight state");
        }
    }
} 