using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// Controls a URP Vignette effect based on the player's stamina.
/// Attach this to any GameObject (recommended: the Camera). Assign a global Volume or it will try to find one.
/// </summary>
public class LowStaminaVignette : MonoBehaviour
{
    [Header("Feature Toggle")]
    [Tooltip("Master switch to enable/disable the low-stamina vignette effect.")]
    public bool enableFeature = true;

    [Header("References")]
    [Tooltip("Global Volume that contains a Vignette override. If empty, the script will try to find one in the scene.")]
    public Volume targetVolume;

    [Header("Behavior")]    
    [Tooltip("Below this stamina value, the vignette will start to appear.")]
    public float staminaThreshold = 30f;

    [Tooltip("Maximum vignette intensity when stamina is 0.")]
    [Range(0f, 1f)] public float maxIntensity = 0.45f;

    [Tooltip("Minimum vignette intensity when above threshold.")]
    [Range(0f, 1f)] public float minIntensity = 0f;

    [Tooltip("How fast the vignette intensity interpolates to the target.")]
    public float smoothSpeed = 6f;

    [Header("Visuals")]
    public Color vignetteColor = Color.black;
    
    [Header("Debug")]
    public bool debugLog = false;
    [Tooltip("Force show vignette for debug regardless of stamina.")]
    public bool forceShowForDebug = false;
    [Range(0f,1f)] public float forceIntensity = 0.8f;

    // URP vignette
    private UnityEngine.Rendering.Universal.Vignette _urpVignette;
    // HDRP vignette
    private UnityEngine.Rendering.HighDefinition.Vignette _hdrpVignette;
    private bool _isURP;
    private float _currentIntensity;

    void Awake()
    {
        if (debugLog)
        {
            var srp = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            Debug.Log($"[LowStaminaVignette] Current SRP: {(srp != null ? srp.GetType().Name : "Built-in Pipeline")} ");
        }
        DetectSRP();
        EnsureVolumeAndVignette();
        if (_isURP) EnsureCameraPostProcessing();
        else EnsureHDRPCameraSettings();
    }

    void Update()
    {
        if (forceShowForDebug)
        {
            ForceTestIntensity(forceIntensity);
        }
    }

    void EnsureHDRPCameraSettings()
    {
        if (Camera.main == null) return;
        var cam = Camera.main;
        var hdData = cam.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData>();
        if (hdData == null)
        {
            hdData = cam.gameObject.AddComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData>();
        }
        if (hdData != null)
        {
            // Make sure camera reads all volume layers and uses itself as anchor
            hdData.volumeLayerMask = ~0; // Everything
            hdData.volumeAnchorOverride = cam.transform;
            if (debugLog)
            {
                Debug.Log("[LowStaminaVignette] Configured HDRP camera volume mask to Everything and anchor to camera.");
            }
        }
    }

    void EnsureVolumeAndVignette()
    {
        if (targetVolume == null)
        {
            // Prefer a Volume on this object or the main camera, else find any global Volume
            targetVolume = GetComponent<Volume>();
            if (targetVolume == null && Camera.main != null)
                targetVolume = Camera.main.GetComponent<Volume>();
            if (targetVolume == null)
                targetVolume = FindAnyObjectByType<Volume>();
        }

        if (targetVolume != null)
        {
            // Ensure volume is global, fully weighted, and high priority so it wins blending
            targetVolume.isGlobal = true;
            targetVolume.weight = 1f;
            targetVolume.priority = 100f;

            var profile = targetVolume.profile;
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                targetVolume.profile = profile;
            }

            if (_isURP)
            {
                if (!profile.TryGet(out _urpVignette))
                {
                    _urpVignette = profile.Add<UnityEngine.Rendering.Universal.Vignette>(true);
                }
            }
            else
            {
                if (!profile.TryGet(out _hdrpVignette))
                {
                    _hdrpVignette = profile.Add<UnityEngine.Rendering.HighDefinition.Vignette>(true);
                }
            }

            // Initialize some defaults
            if (_isURP && _urpVignette != null)
            {
                _urpVignette.color.overrideState = true;
                _urpVignette.color.value = vignetteColor;
                _urpVignette.intensity.overrideState = true;
                _urpVignette.smoothness.overrideState = true;
                _urpVignette.smoothness.value = 0.6f;
                _urpVignette.active = false;
            }
            else if (!_isURP && _hdrpVignette != null)
            {
                _hdrpVignette.color.overrideState = true;
                _hdrpVignette.color.value = vignetteColor;
                _hdrpVignette.intensity.overrideState = true;
                _hdrpVignette.smoothness.overrideState = true;
                _hdrpVignette.smoothness.value = 0.6f;
                _hdrpVignette.active = false;
            }
            _currentIntensity = 0f;

            if (debugLog)
            {
                Debug.Log($"[LowStaminaVignette] Volume: {targetVolume.gameObject.name}. URP Vig: {_urpVignette != null}, HDRP Vig: {_hdrpVignette != null}");
            }
        }
        else if (debugLog)
        {
            Debug.LogWarning("[LowStaminaVignette] No Volume found in scene or on Camera. The effect cannot render.");
        }
    }

    void EnsureCameraPostProcessing()
    {
        if (Camera.main == null) return;
        var cam = Camera.main;
        var urpData = cam.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        if (urpData == null)
        {
            urpData = cam.gameObject.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        }
        if (urpData != null)
        {
            urpData.renderPostProcessing = true;
            urpData.volumeLayerMask = ~0; // Everything
            urpData.volumeTrigger = cam.transform;
            if (debugLog)
            {
                Debug.Log("[LowStaminaVignette] Enabled camera post processing and set volume mask to Everything.");
            }
        }
    }

    /// <summary>
    /// Public API to turn the feature on/off at runtime.
    /// </summary>
    public void SetEnabled(bool value)
    {
        enableFeature = value;
        if (!enableFeature)
        {
            ApplyIntensity(0f, true);
        }
    }

    public void Enable() => SetEnabled(true);
    public void Disable() => SetEnabled(false);

    /// <summary>
    /// Call this each frame (or when stamina changes) to update the vignette.
    /// </summary>
    public void UpdateByStamina(float currentStamina, float maxStamina)
    {
        bool hasVignette = _isURP ? (_urpVignette != null) : (_hdrpVignette != null);
        if (!enableFeature || !hasVignette || maxStamina <= 0f)
        {
            ApplyIntensity(0f, true);
            return;
        }

        // Only start effect below threshold
        float target = 0f;
        if (currentStamina <= staminaThreshold)
        {
            float t = Mathf.InverseLerp(staminaThreshold, 0f, Mathf.Clamp(currentStamina, 0f, staminaThreshold));
            target = Mathf.Lerp(minIntensity, maxIntensity, t);
        }

        // Smoothly interpolate
        float lerpT = Mathf.Clamp01(Time.deltaTime * smoothSpeed);
        _currentIntensity = Mathf.Lerp(_currentIntensity, target, lerpT);
        ApplyIntensity(_currentIntensity, false);
    }

    private void ApplyIntensity(float value, bool force)
    {
        if (_isURP)
        {
            if (_urpVignette == null) return;
            _urpVignette.color.value = vignetteColor;
            _urpVignette.intensity.value = value;
            _urpVignette.active = value > 0.001f;
        }
        else
        {
            if (_hdrpVignette == null) return;
            _hdrpVignette.color.value = vignetteColor;
            _hdrpVignette.intensity.value = value;
            _hdrpVignette.active = value > 0.001f;
        }
        if (force)
        {
            _currentIntensity = value;
        }
    }

    // Helper: force-show the vignette at a given intensity to validate pipeline/volume.
    public void ForceTestIntensity(float intensity = 0.8f)
    {
        if (!enableFeature) enableFeature = true;
        EnsureVolumeAndVignette();
        ApplyIntensity(Mathf.Clamp01(intensity), true);
        if (debugLog)
        {
            Debug.Log($"[LowStaminaVignette] ForceTestIntensity applied: {intensity}");
        }
    }

    [ContextMenu("Force Test Vignette 80%")]
    void ContextMenu_ForceTest()
    {
        ForceTestIntensity(0.8f);
    }

    void DetectSRP()
    {
        var srp = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
        _isURP = srp is UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
    }
}
