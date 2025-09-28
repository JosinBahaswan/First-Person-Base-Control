using UnityEngine;

public class HealItem : MonoBehaviour
{
    public enum HealSize
    {
        Small,
        Large
    }

    public HealSize healSize = HealSize.Small;
    public float smallHealAmount = 25f;
    public float largeHealAmount = 75f;
    public GameObject pickupEffect; // Optional particle effect

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float healAmount = healSize == HealSize.Small ? smallHealAmount : largeHealAmount;
            PlayerManager.Instance.Heal(healAmount);

            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
