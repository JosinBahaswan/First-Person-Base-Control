using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AIHunter : MonoBehaviour
{
    [Header("Wander Settings")]
    public List<Transform> wanderPoints;
    public float wanderSpeed = 3.5f;
    public float idleWanderDelay = 2f;
    public bool randomizeWanderPoint = false;

    [Header("Detection Settings")]
    public string targetTag = "Target";
    public float defaultDetectionRadius = 10f;
    public float chaseDetectionRadius = 15f;
    public float defaultDetectionAngle = 45f;
    public float chaseDetectionAngle = 90f;
    public float chaseSpeed = 5f;
    public float targetLostDistance = 15f;
    public int rayDetectCount = 5;
    public float rayHeight = 1.0f;

    [Header("Attack Settings")]
    public float attackDistance = 1.5f;
    public float attackCooldown = 3f; // Cooldown setelah attack/jumpscare
    public UnityEvent onAttackJumpscareEvent;

    [Header("Distract Settings")]
    public float distractIdleDelay = 3f;

    [Header("Animation Settings")]
    public string moveParameter = "Move";
    public string wanderBlendAnim = "Wander";
    public string chaseBlendAnim = "Chase";

    private NavMeshAgent agent;
    private Animator animator;
    private Transform currentTarget;
    private bool isDistracted = false;
    private Vector3 distractLocation;

    private int currentWanderIndex = 0;
    private bool isIdle = false;

    private float currentDetectionRadius;
    private float currentDetectionAngle;
    private bool isChasing = false;
    private float chaseStartTime;
    private bool isAttack = false;
    private float lastAttackTime = -999f; // Track waktu terakhir attack
    private bool wasChasing = false; // New variable to track chase state changes

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        WanderToNextPoint();
        currentDetectionRadius = defaultDetectionRadius;
        currentDetectionAngle = defaultDetectionAngle;
    }

    private void Update()
    {
        if (isDistracted)
            return;

        if (currentTarget)
        {
            if (isChasing)
            {
                if (Vector3.Distance(transform.position, currentTarget.position) > currentDetectionRadius)
                {
                    LoseTarget();
                }
                else
                {
                    ChaseTarget();
                    UpdateDetectionRadiusDuringChase();
                }
            }
        }
        else
        {
            DetectTarget();

            if (!isIdle && !agent.pathPending && agent.remainingDistance < 0.5f)
            {
                StartCoroutine(HandleIdle());
            }
        }

        UpdateAnimator(); // Update animator every frame
    }

    private void UpdateAnimator()
    {
        // Check for state change from !isChasing to isChasing
        if (isChasing && !wasChasing)
        {
            animator.Play(chaseBlendAnim);
        }
        else if (!isChasing && wasChasing)
        {
            animator.Play(wanderBlendAnim);
        }

        // Update move parameter based on agent's velocity magnitude
        float moveSpeed = Mathf.Clamp01(agent.velocity.magnitude / agent.speed);

        // SAFETY: Ensure Move parameter never stuck at negative values (jumpscare state)
        // Negative Move values should ONLY be set by AIHunterSupport during jumpscare
        float currentMove = animator.GetFloat(moveParameter);
        if (currentMove < 0f && this.enabled) // If AI is enabled but Move is negative, force fix
        {
            animator.SetFloat(moveParameter, moveSpeed);
            Debug.LogWarning($"[AIHunter] {name} - Detected stuck animator (Move={currentMove}), force reset to {moveSpeed}");
        }
        else
        {
            animator.SetFloat(moveParameter, moveSpeed);
        }

        // Update the state tracking variable
        wasChasing = isChasing;
    }

    private void DetectTarget()
    {
        if (!isChasing)
        {
            RaycastDetection();
        }
    }

    private void RaycastDetection()
    {
        float angleStep = currentDetectionAngle / rayDetectCount;
        for (int i = 0; i < rayDetectCount; i++)
        {
            float angle = -currentDetectionAngle / 2 + (angleStep * i);
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * transform.forward;

            Ray ray = new Ray(transform.position + Vector3.up * rayHeight, rayDirection);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, currentDetectionRadius))
            {
                if (hit.collider.CompareTag(targetTag))
                {
                    currentTarget = hit.transform;
                    agent.speed = chaseSpeed;
                    currentDetectionRadius = defaultDetectionRadius + 1;
                    currentDetectionAngle = chaseDetectionAngle;
                    isChasing = true;
                    chaseStartTime = Time.time;

                    // FORCE switch animator to chase immediately
                    if (animator != null)
                    {
                        animator.Play(chaseBlendAnim);
                        Debug.Log($"[AIHunter] {name} - Player detected! Switching to CHASE animation");
                    }

                    return;
                }
            }
        }
    }

    private void ChaseTarget()
    {
        if (currentTarget == null) return;

        agent.SetDestination(currentTarget.position);

        if (Vector3.Distance(transform.position, currentTarget.position) <= attackDistance)
        {
            // Check attack cooldown
            if (!isAttack && Time.time - lastAttackTime >= attackCooldown)
            {
                onAttackJumpscareEvent.Invoke();
                isAttack = true;
                lastAttackTime = Time.time;
            }
        }
    }

    private void LoseTarget()
    {
        currentTarget = null;
        agent.speed = wanderSpeed;
        currentDetectionRadius = defaultDetectionRadius;
        currentDetectionAngle = defaultDetectionAngle;
        isChasing = false;
        WanderToNextPoint();
    }

    private void WanderToNextPoint()
    {
        if (wanderPoints.Count == 0) return;

        int nextIndex = randomizeWanderPoint
            ? Random.Range(0, wanderPoints.Count)
            : currentWanderIndex;

        agent.speed = wanderSpeed;
        agent.SetDestination(wanderPoints[nextIndex].position);

        if (!randomizeWanderPoint)
        {
            currentWanderIndex = (currentWanderIndex + 1) % wanderPoints.Count;
        }
    }

    private IEnumerator HandleIdle()
    {
        isIdle = true;
        yield return new WaitForSeconds(idleWanderDelay);
        isIdle = false;
        WanderToNextPoint();
    }

    public void Distract(Vector3 location)
    {
        if (currentTarget != null) return;
        isDistracted = true;
        distractLocation = location;
        agent.SetDestination(distractLocation);
        StartCoroutine(HandleDistract());
    }

    private IEnumerator HandleDistract()
    {
        while (agent.pathPending || agent.remainingDistance > 0.5f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(distractIdleDelay);
        isDistracted = false;
        WanderToNextPoint();
    }

    private void UpdateDetectionRadiusDuringChase()
    {
        if (isChasing && Time.time >= chaseStartTime + 3f)
        {
            currentDetectionRadius = chaseDetectionRadius;
        }
    }

    /// <summary>
    /// Public method untuk reset chase state dan kembali ke wander mode
    /// Dipanggil oleh AIHunterSupport setelah jumpscare
    /// </summary>
    public void ResetToWanderMode()
    {
        // Reset chase state
        currentTarget = null;
        isChasing = false;
        isAttack = false;

        // Reset detection settings
        currentDetectionRadius = defaultDetectionRadius;
        currentDetectionAngle = defaultDetectionAngle;

        // Reset speed
        agent.speed = wanderSpeed;

        // CRITICAL: Force reset animator ke wander state
        // Clear any stuck animation states (jumpscare, chase, etc)
        if (animator != null)
        {
            // Set Move parameter ke small positive value untuk ensure proper blend tree transition
            animator.SetFloat(moveParameter, 0.01f);
            animator.Update(0f);

            // Play wander blend tree explicitly
            animator.Play(wanderBlendAnim);

            Debug.Log($"[AIHunter] {name} - Animator FORCE RESET to wander blend tree");
        }

        // Go back to wander
        WanderToNextPoint();

        Debug.Log($"[AIHunter] {name} - Reset to wander mode");
    }

    private void OnDrawGizmosSelected()
    {
        if (!isChasing)
        {
            float angleStep = currentDetectionAngle / rayDetectCount;
            for (int i = 0; i < rayDetectCount; i++)
            {
                float angle = -currentDetectionAngle / 2 + (angleStep * i);
                Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * transform.forward;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position + Vector3.up * rayHeight, transform.position + Vector3.up * rayHeight + rayDirection * currentDetectionRadius);
            }
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, currentDetectionRadius);
        }
    }
}
