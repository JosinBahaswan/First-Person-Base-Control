using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Custom scripts
using System;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDepletionRate = 10f;
    public float staminaRegenerationRate = 5f; // Fast regeneration when idle
    public float walkingRegenerationRate = 2f; // Slower regeneration when walking
    public float minStaminaToRun = 20f;
    public float idleTimeBeforeRegen = 5f; // Time to wait before fast regeneration

    [Header("Mobile")]
    public bool useJoystick;
    public SimpleTouchController moveJoystickController;
    public SimpleTouchController camTouchController;
    public Button jumpButton;
    public Button crouchButton;
    public Button runButton;

    [Header("UI")]
    public Image runButtonImage;
    public Text runButtonText;
    public Sprite runSprite;
    public Sprite walkSprite;

    [HideInInspector] public CharacterController characterController;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();
    public static FirstPersonMovement instance;

    private Vector3 velocity;
    private float idleTimer = 0f;
    private bool isMoving = false;
    private bool isWalking = false;

    void Awake()
    {
        instance = this;
        characterController = GetComponent<CharacterController>();
        if (useJoystick) moveJoystickController.transform.parent.gameObject.SetActive(true);
        else moveJoystickController.transform.parent.gameObject.SetActive(false);

        currentStamina = maxStamina;

        if (runButton != null)
        {
            runButton.onClick.AddListener(ToggleRun);
        }
    }

    void ToggleRun()
    {
        if (canRun && currentStamina > minStaminaToRun)
        {
            IsRunning = !IsRunning;
            UpdateRunButtonSprite();
        }
    }

    void UpdateRunButtonSprite()
    {
        if (runButtonImage != null)
        {
            runButtonImage.sprite = IsRunning ? runSprite : walkSprite;
        }

        if (runButtonText != null)
        {
            int staminaPercent = Mathf.RoundToInt((currentStamina / maxStamina) * 100);
            runButtonText.text = staminaPercent.ToString() + "%";
        }
    }

    void UpdateStamina()
    {
        // Check if player is moving (either via keyboard or joystick)
        bool wasMoving = isMoving;
        isMoving = false;
        isWalking = false;

        if (!useJoystick)
        {
            isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        }
        else
        {
            isMoving = moveJoystickController.GetTouchPosition.magnitude > 0.1f;
        }

        // If player is moving but not running, count as walking
        if (isMoving && !IsRunning)
        {
            isWalking = true;
        }

        // If player is moving and running, deplete stamina
        if (isMoving && IsRunning)
        {
            idleTimer = 0f;
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            if (currentStamina < 0) currentStamina = 0;

            if (currentStamina <= 0)
            {
                IsRunning = false;
            }
        }
        // If player is walking (moving but not running), regenerate stamina slowly
        else if (isWalking)
        {
            idleTimer = 0f;
            currentStamina += walkingRegenerationRate * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
        }
        // If player is completely idle, wait then regenerate fast
        else
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTimeBeforeRegen)
            {
                currentStamina += staminaRegenerationRate * Time.deltaTime;
                if (currentStamina > maxStamina) currentStamina = maxStamina;
            }
        }

        // Update run button state if movement state changed
        if (wasMoving != isMoving)
        {
            UpdateRunButtonSprite();
        }
    }

    void Update()
    {
        UpdateStamina();
        UpdateRunButtonSprite();

        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector2 targetVelocity;
        if (!useJoystick)
        {
            bool wasRunning = IsRunning;
            IsRunning = canRun && Input.GetKey(runningKey) && currentStamina > minStaminaToRun;

            if (IsRunning != wasRunning)
            {
                UpdateRunButtonSprite();
            }

            targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);
        }
        else
        {
            targetVelocity = new Vector2(moveJoystickController.GetTouchPosition.x * targetMovingSpeed, moveJoystickController.GetTouchPosition.y * targetMovingSpeed);
        }

        Vector3 movement = transform.rotation * new Vector3(targetVelocity.x, 0, targetVelocity.y);

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }

        movement.y = velocity.y;
        characterController.Move(movement * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Cek kedua tipe landmine
        var mine = hit.collider.GetComponent<LandMineTrapSimple>();
        if (mine != null)
        {
            Debug.Log("Player menyentuh LandMine (Simple): " + mine.name);
            return;
        }

        var mineOld = hit.collider.GetComponent<LandMineTrapSimple>();
        if (mineOld != null)
        {
            Debug.Log("Player menyentuh LandMine (Old): " + mineOld.name);
            mineOld.TriggerFromPlayer(this.gameObject);
        }
    }
}