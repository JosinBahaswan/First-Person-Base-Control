using System.Collections;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    public KeyCode key = KeyCode.LeftControl;
    public FirstPersonMovement movement;
    public float movementSpeed = 2;
    public Transform headToLower;
    [HideInInspector] public float? defaultHeadYLocalPosition;
    public float crouchYHeadPosition = 1;
    public float checkAbove = 1;
    public CapsuleCollider colliderToLower;
    [HideInInspector] public float? defaultColliderHeight;

    public bool IsCrouched { get; private set; }
    public event System.Action CrouchStart, CrouchEnd;

    FirstPersonMovement fpp;
    bool toggleState = false;
    bool isCrouchPressed;
    bool isCrouchButtonPressed;

    void Awake()
    {
        movement = GetComponentInParent<FirstPersonMovement>();
        headToLower = movement.GetComponentInChildren<Camera>().transform;
        colliderToLower = movement.GetComponentInChildren<CapsuleCollider>();
    }

    private void Start()
    {
        fpp = FirstPersonMovement.instance;
        fpp.crouchButton.onClick.AddListener(OnCrouchButtonClicked);
    }

    void OnCrouchButtonClicked()
    {
        StartCoroutine(OnCrouchButtonClickedDelay());
        IEnumerator OnCrouchButtonClickedDelay()
        {
            isCrouchButtonPressed = true;
            yield return new WaitForSeconds(0);
            isCrouchButtonPressed = false;
        }
    }

    void LateUpdate()
    {
        if (!fpp.useJoystick) isCrouchPressed = Input.GetKeyDown(key);
        else isCrouchPressed = isCrouchButtonPressed;

        if (isCrouchPressed)
        {
            toggleState = !toggleState;
            if (toggleState)
            {
                StartCrouch();
            }
            else
            {
                TryStand();
            }
        }
    }

    void StartCrouch()
    {
        if (headToLower)
        {
            if (!defaultHeadYLocalPosition.HasValue)
            {
                defaultHeadYLocalPosition = headToLower.localPosition.y;
            }
            headToLower.localPosition = new Vector3(headToLower.localPosition.x, crouchYHeadPosition, headToLower.localPosition.z);
        }

        if (colliderToLower)
        {
            if (!defaultColliderHeight.HasValue)
            {
                defaultColliderHeight = colliderToLower.height;
            }
            float loweringAmount = defaultHeadYLocalPosition.HasValue
                ? defaultHeadYLocalPosition.Value - crouchYHeadPosition
                : defaultColliderHeight.Value * .5f;

            colliderToLower.height = Mathf.Max(defaultColliderHeight.Value - loweringAmount, 0);
            colliderToLower.center = Vector3.up * colliderToLower.height * .5f;
        }

        if (!IsCrouched)
        {
            IsCrouched = true;
            SetSpeedOverrideActive(true);
            CrouchStart?.Invoke();
        }
    }

    public void TryStand()
    {
        if (Physics.Raycast(headToLower.position, transform.up, checkAbove))
        {
            Debug.Log("Cannot stand up, obstacle above!");
            return;
        }

        if (headToLower)
        {
            headToLower.localPosition = new Vector3(headToLower.localPosition.x, defaultHeadYLocalPosition.Value, headToLower.localPosition.z);
        }

        if (colliderToLower)
        {
            colliderToLower.height = defaultColliderHeight.Value;
            colliderToLower.center = Vector3.up * colliderToLower.height * .5f;
        }

        if (IsCrouched)
        {
            IsCrouched = false;
            SetSpeedOverrideActive(false);
            CrouchEnd?.Invoke();
        }
    }

    void SetSpeedOverrideActive(bool state)
    {
        if (!movement)
        {
            return;
        }

        if (state)
        {
            if (!movement.speedOverrides.Contains(SpeedOverride))
            {
                movement.speedOverrides.Add(SpeedOverride);
            }
        }
        else
        {
            if (movement.speedOverrides.Contains(SpeedOverride))
            {
                movement.speedOverrides.Remove(SpeedOverride);
            }
        }
    }

    float SpeedOverride() => movementSpeed;
}