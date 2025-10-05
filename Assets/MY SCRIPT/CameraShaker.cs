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

        while (elapsed < explosionShakeDuration)
        {
            float x = originalPosition.x + Random.Range(-1f, 1f) * explosionShakeIntensity;
            float y = originalPosition.y + Random.Range(-1f, 1f) * explosionShakeIntensity;

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset; if continuous mode is active, it will take over next frame
        transform.localPosition = originalPosition;
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
            transform.localPosition = originalPosition;
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
            float x = originalPosition.x;
            float y = originalPosition.y + Mathf.Sin(sinTime) * intensity;
            transform.localPosition = new Vector3(x, y, originalPosition.z);

            yield return null;
        }
        transform.localPosition = originalPosition;
    }
}
