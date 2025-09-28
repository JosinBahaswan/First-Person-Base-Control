using UnityEngine;
using UnityEngine.UI;

public class FlashlightBatteryRecharger : MonoBehaviour
{
    [Header("Recharger Settings")]
    public KeyCode rechargeKey = KeyCode.R;
    public float rechargeAmount = 60f; // Jumlah detik yang ditambahkan saat recharge
    public Button rechargeButton; // Tombol untuk recharge di mobile
    
    private PlayerInteractionNoInventory playerInteraction;
    
    private void Start()
    {
        playerInteraction = PlayerInteractionNoInventory.Instance;
        
        if (rechargeButton != null)
        {
            rechargeButton.onClick.AddListener(RechargeFlashlight);
        }
    }
    
    private void Update()
    {
        // Hanya tampilkan tombol recharge jika player memegang flashlight
        if (rechargeButton != null)
        {
            bool isHoldingFlashlight = (playerInteraction != null && 
                                        playerInteraction.holdItem != null && 
                                        playerInteraction.holdItem is FlashlightItem);
            
            rechargeButton.gameObject.SetActive(isHoldingFlashlight);
        }
        
        // Recharge dengan tombol keyboard
        if (Input.GetKeyDown(rechargeKey))
        {
            RechargeFlashlight();
        }
    }
    
    public void RechargeFlashlight()
    {
        if (playerInteraction != null && 
            playerInteraction.holdItem != null && 
            playerInteraction.holdItem is FlashlightItem)
        {
            FlashlightItem flashlight = (FlashlightItem)playerInteraction.holdItem;
            flashlight.AddBattery(rechargeAmount);
            Debug.Log($"FlashlightRecharger: Added {rechargeAmount} seconds of battery life.");
        }
        else
        {
            Debug.Log("FlashlightRecharger: No flashlight held to recharge.");
        }
    }
} 