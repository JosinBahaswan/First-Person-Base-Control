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
        animator.SetFloat(moveParameter, moveSpeed);

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
            if (!isAttack)
            {
                onAttackJumpscareEvent.Invoke();
                isAttack = true;
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
