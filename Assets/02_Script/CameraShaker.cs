using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }

    [Header("Global")] 
    public bool enableShake = true;

    [Header("Walking Shake Settings")] 
    public bool enableWalkShake = true;
    public float walkShakeIntensity = 0.1f;
    public float walkShakeFrequency = 2f;

    [Header("Running Shake Settings")] 
    public bool enableRunShake = true;
    public float runShakeIntensity = 0.18f;
    public float runShakeFrequency = 3.2f;

    [Header("Explosion Shake Settings")] 
    public float explosionShakeIntensity = 0.5f;
    public float explosionShakeDuration = 0.5f;
    public float explosionShakeFrequency = 4f;

    private Vector3 originalPosition;
    // Offsets for layered shake effects
    private Vector3 walkRunOffset = Vector3.zero;
    private Vector3 explosionOffset = Vector3.zero;
    // Track last applied total offset so we can apply shake additively without clobbering other systems (e.g., crouch)
    private Vector3 lastAppliedOffset = Vector3.zero;

    private enum ShakeMode { None, Walk, Run }
    private ShakeMode currentMode = ShakeMode.None;
    private Coroutine continuousShakeRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        originalPosition = transform.localPosition;
        walkRunOffset = Vector3.zero;
        explosionOffset = Vector3.zero;
        lastAppliedOffset = Vector3.zero;
    }

    public void ShakeFromExplosion()
    {
        if (!enableShake) return;
        StartCoroutine(ExplosionShake());
    }

    public void StartWalkingShake()
    {
        if (!enableShake || !enableWalkShake) return;
        SetMode(ShakeMode.Walk);
    }

    public void StopWalkingShake()
    {
        // Only stop if currently in Walk mode
        if (currentMode == ShakeMode.Walk)
        {
            SetMode(ShakeMode.None);
        }
    }

    public void StartRunningShake()
    {
        if (!enableShake || !enableRunShake) return;
        SetMode(ShakeMode.Run);
    }

    public void StopRunningShake()
    {
        if (currentMode == ShakeMode.Run)
        {
            SetMode(ShakeMode.None);
        }
    }

    public void StopAllShake()
    {
        SetMode(ShakeMode.None);
    }

    private IEnumerator ExplosionShake()
    {
        float elapsed = 0f;
        float timeAccumulator = 0f;

        while (elapsed < explosionShakeDuration)
        {
            // Optionally use frequency to update the random each frame
            timeAccumulator += Time.deltaTime * explosionShakeFrequency;

            // Random offset around zero, layered with walk/run
            float x = Random.Range(-1f, 1f) * explosionShakeIntensity;
            float y = Random.Range(-1f, 1f) * explosionShakeIntensity;
            explosionOffset = new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Clear explosion offset; continuous shake remains
        explosionOffset = Vector3.zero;
    }

    private void SetMode(ShakeMode mode)
    {
        if (currentMode == mode) return;
        currentMode = mode;

        if (continuousShakeRoutine != null)
        {
            StopCoroutine(continuousShakeRoutine);
            continuousShakeRoutine = null;
        }

        if (currentMode != ShakeMode.None)
        {
            continuousShakeRoutine = StartCoroutine(ContinuousShake());
        }
        else
        {
            // Clear continuous offset when stopping
            walkRunOffset = Vector3.zero;
        }
    }

    private IEnumerator ContinuousShake()
    {
        float sinTime = 0f;
        while (currentMode != ShakeMode.None)
        {
            float intensity;
            float frequency;

            if (currentMode == ShakeMode.Run)
            {
                intensity = runShakeIntensity;
                frequency = runShakeFrequency;
            }
            else
            {
                intensity = walkShakeIntensity;
                frequency = walkShakeFrequency;
            }

            sinTime += Time.deltaTime * frequency;
            float y = Mathf.Sin(sinTime) * intensity;
            walkRunOffset = new Vector3(0f, y, 0f);

            yield return null;
        }
        walkRunOffset = Vector3.zero;
    }

    private void LateUpdate()
    {
        // Combine offsets so effects can layer
        // IMPORTANT: Apply shake as a delta, not by overwriting the position, so crouch or other systems can freely adjust the base position.
        Vector3 newOffset = (enableShake ? (walkRunOffset + explosionOffset) : Vector3.zero);
        Vector3 delta = newOffset - lastAppliedOffset;
        if (delta.sqrMagnitude > 0f)
        {
            transform.localPosition += delta;
            lastAppliedOffset = newOffset;
        }
    }

    private void OnDisable()
    {
        // Remove any residual offset when disabled so camera returns to its true base position
        if (lastAppliedOffset.sqrMagnitude > 0f)
        {
            transform.localPosition -= lastAppliedOffset;
            lastAppliedOffset = Vector3.zero;
        }
        walkRunOffset = Vector3.zero;
        explosionOffset = Vector3.zero;
    }
}
