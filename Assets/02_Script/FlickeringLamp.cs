using UnityEngine;
using System.Collections;

public class FlickeringLamp : MonoBehaviour
{
    [Header("Lamp Components")]
    [SerializeField] private Light lampLight;
    [SerializeField] private MeshRenderer lampMeshRenderer;
    [SerializeField] private int emissiveMaterialIndex = 0;
    
    [Header("Flickering Settings")]
    [SerializeField] private bool enableFlickering = true;
    [SerializeField] private float minFlickerInterval = 0.1f;
    [SerializeField] private float maxFlickerInterval = 0.5f;
    [SerializeField] private float flickerDuration = 0.05f;
    [SerializeField] private float flickerIntensityMultiplier = 0.3f;
    
    [Header("Switch Settings")]
    [SerializeField] private bool enableSwitch = true;
    [SerializeField] private int switchCountToTurnOff = 3;
    [SerializeField] private float switchResetTime = 2f;
    [SerializeField] private KeyCode switchKey = KeyCode.E;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Light Settings")]
    [SerializeField] private float normalIntensity = 1f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color emissiveColor = Color.white;
    [SerializeField] private float emissiveIntensity = 1f;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip switchSound;
    [SerializeField] private AudioClip flickerSound;
    
    [Header("UI Feedback")]
    [SerializeField] private bool showSwitchPrompt = true;
    [SerializeField] private string switchPromptText = "Press E to switch";
    
    // Private variables
    private bool isLampOn = true;
    private bool isFlickering = false;
    private int currentSwitchCount = 0;
    private float lastSwitchTime = 0f;
    private Material emissiveMaterial;
    private Color originalEmissiveColor;
    private float originalIntensity;
    private bool playerInRange = false;
    private Transform playerTransform;
    
    private void Start()
    {
        InitializeLamp();
        
        if (enableFlickering && isLampOn)
        {
            StartCoroutine(FlickerRoutine());
        }
    }
    
    private void InitializeLamp()
    {
        // Initialize light component
        if (lampLight == null)
        {
            lampLight = GetComponent<Light>();
        }
        
        if (lampLight != null)
        {
            originalIntensity = lampLight.intensity;
            normalIntensity = originalIntensity;
        }
        
        // Initialize emissive material
        if (lampMeshRenderer != null && lampMeshRenderer.materials.Length > emissiveMaterialIndex)
        {
            emissiveMaterial = lampMeshRenderer.materials[emissiveMaterialIndex];
            originalEmissiveColor = emissiveMaterial.GetColor("_EmissionColor");
            
            // Set initial emissive color
            if (isLampOn)
            {
                SetEmissiveColor(emissiveColor * emissiveIntensity);
            }
        }
        
        // Initialize audio source
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Find player
        FindPlayer();
    }
    
    private void Update()
    {
        if (enableSwitch)
        {
            CheckPlayerDistance();
            HandleSwitchInput();
            CheckSwitchTimeout();
        }
    }
    
    private void FindPlayer()
    {
        // Try to find player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            // Try to find main camera as fallback
            if (Camera.main != null)
            {
                playerTransform = Camera.main.transform;
            }
        }
    }
    
    private void CheckPlayerDistance()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }
        
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        playerInRange = distance <= interactionDistance;
    }
    
    private void HandleSwitchInput()
    {
        if (!playerInRange || !isLampOn) return;
        
        if (Input.GetKeyDown(switchKey))
        {
            SwitchLamp();
        }
    }
    
    private void CheckSwitchTimeout()
    {
        // Reset switch count after timeout
        if (currentSwitchCount > 0 && Time.time - lastSwitchTime > switchResetTime)
        {
            currentSwitchCount = 0;
        }
    }
    
    private void SwitchLamp()
    {
        currentSwitchCount++;
        lastSwitchTime = Time.time;
        
        // Play switch sound
        PlaySound(switchSound);
        
        // Quick flicker effect when switching
        StartCoroutine(QuickFlicker());
        
        // Check if reached switch count to turn off
        if (currentSwitchCount >= switchCountToTurnOff)
        {
            TurnOffLamp();
        }
    }
    
    private IEnumerator QuickFlicker()
    {
        if (!isLampOn) yield break;
        
        for (int i = 0; i < 3; i++)
        {
            SetLampIntensity(normalIntensity * 0.5f);
            yield return new WaitForSeconds(0.05f);
            SetLampIntensity(normalIntensity);
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    private IEnumerator FlickerRoutine()
    {
        while (isLampOn && enableFlickering)
        {
            // Wait for random interval
            float waitTime = Random.Range(minFlickerInterval, maxFlickerInterval);
            yield return new WaitForSeconds(waitTime);
            
            if (!isLampOn) break;
            
            // Perform flicker
            isFlickering = true;
            
            // Random flicker pattern
            int flickerCount = Random.Range(1, 4);
            for (int i = 0; i < flickerCount; i++)
            {
                // Dim the light
                float flickerIntensity = normalIntensity * flickerIntensityMultiplier;
                SetLampIntensity(flickerIntensity);
                SetEmissiveColor(emissiveColor * (emissiveIntensity * flickerIntensityMultiplier));
                
                // Play flicker sound occasionally
                if (flickerSound != null && Random.value > 0.7f)
                {
                    PlaySound(flickerSound);
                }
                
                yield return new WaitForSeconds(flickerDuration);
                
                // Restore light
                SetLampIntensity(normalIntensity);
                SetEmissiveColor(emissiveColor * emissiveIntensity);
                
                yield return new WaitForSeconds(flickerDuration * 0.5f);
            }
            
            isFlickering = false;
        }
    }
    
    private void TurnOffLamp()
    {
        isLampOn = false;
        StopAllCoroutines();
        
        // Turn off light
        if (lampLight != null)
        {
            lampLight.enabled = false;
        }
        
        // Turn off emissive
        SetEmissiveColor(Color.black);
        
        currentSwitchCount = 0;
    }
    
    public void TurnOnLamp()
    {
        isLampOn = true;
        currentSwitchCount = 0;
        
        // Turn on light
        if (lampLight != null)
        {
            lampLight.enabled = true;
            lampLight.intensity = normalIntensity;
        }
        
        // Turn on emissive
        SetEmissiveColor(emissiveColor * emissiveIntensity);
        
        // Restart flickering
        if (enableFlickering)
        {
            StartCoroutine(FlickerRoutine());
        }
    }
    
    private void SetLampIntensity(float intensity)
    {
        if (lampLight != null)
        {
            lampLight.intensity = intensity;
        }
    }
    
    private void SetEmissiveColor(Color color)
    {
        if (emissiveMaterial != null)
        {
            emissiveMaterial.SetColor("_EmissionColor", color);
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    // Public methods for external control
    public void SetFlickering(bool enabled)
    {
        enableFlickering = enabled;
        
        if (enabled && isLampOn && !isFlickering)
        {
            StartCoroutine(FlickerRoutine());
        }
    }
    
    public void SetSwitchEnabled(bool enabled)
    {
        enableSwitch = enabled;
    }
    
    public bool IsLampOn()
    {
        return isLampOn;
    }
    
    public int GetCurrentSwitchCount()
    {
        return currentSwitchCount;
    }
    
    // Gizmos for editor visualization
    private void OnDrawGizmosSelected()
    {
        if (enableSwitch)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }
    }
    
    private void OnGUI()
    {
        if (!showSwitchPrompt || !enableSwitch || !playerInRange || !isLampOn) return;
        
        // Simple UI prompt
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        
        if (screenPos.z > 0)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 16;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            
            string promptText = $"{switchPromptText} ({currentSwitchCount}/{switchCountToTurnOff})";
            Vector2 size = style.CalcSize(new GUIContent(promptText));
            
            // Draw background
            GUI.color = new Color(0, 0, 0, 0.5f);
            GUI.DrawTexture(new Rect(screenPos.x - size.x / 2 - 5, Screen.height - screenPos.y - 30, size.x + 10, 30), Texture2D.whiteTexture);
            
            // Draw text
            GUI.color = Color.white;
            GUI.Label(new Rect(screenPos.x - size.x / 2, Screen.height - screenPos.y - 25, size.x, 20), promptText, style);
        }
    }
}
