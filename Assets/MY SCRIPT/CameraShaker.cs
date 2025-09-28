using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }

    [Header("Walking Shake Settings")]
    public float walkShakeIntensity = 0.1f;
    public float walkShakeFrequency = 2f;

    [Header("Explosion Shake Settings")]
    public float explosionShakeIntensity = 0.5f;
    public float explosionShakeDuration = 0.5f;
    public float explosionShakeFrequency = 4f;

    private Vector3 originalPosition;
    private bool isShaking = false;

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
        StartCoroutine(ExplosionShake());
    }

    public void StartWalkingShake()
    {
        if (!isShaking)
        {
            StartCoroutine(WalkingShake());
        }
    }

    public void StopWalkingShake()
    {
        isShaking = false;
        transform.localPosition = originalPosition;
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

        transform.localPosition = originalPosition;
    }

    private IEnumerator WalkingShake()
    {
        isShaking = true;
        float sinTime = 0f;

        while (isShaking)
        {
            sinTime += Time.deltaTime * walkShakeFrequency;

            float x = originalPosition.x;
            float y = originalPosition.y + Mathf.Sin(sinTime) * walkShakeIntensity;

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
