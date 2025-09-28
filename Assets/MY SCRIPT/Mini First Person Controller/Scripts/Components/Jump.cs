using System.Collections;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public float jumpStrength = 7f;
    public event System.Action Jumped;
    public KeyCode key = KeyCode.Space;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    GroundCheck groundCheck;

    private FirstPersonMovement fpp;
    private CharacterController characterController;
    private bool isJumpPressed;
    private bool isJumpButtonPressed;
    private float verticalVelocity;
    private float jumpCooldown = 0.3f; // Cooldown untuk mencegah spam lompat
    private float lastJumpTime = 0f;
    private bool isJumping = false; // Status lompat

    void Awake()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private void Start()
    {
        fpp = FirstPersonMovement.instance;
        characterController = fpp.characterController;
        fpp.jumpButton.onClick.AddListener(OnJumpButtonClicked);

        Physics.gravity = new Vector3(0, -20f, 0); // Sesuaikan gravitasi
    }

    void LateUpdate()
    {
        // Cek input lompat (keyboard atau tombol di mobile)
        if (!fpp.useJoystick) isJumpPressed = Input.GetKeyDown(key);
        else isJumpPressed = isJumpButtonPressed;

        // Terapkan gravitasi
        if (groundCheck && groundCheck.isGrounded)
        {
            verticalVelocity = -2f; // Reset kecepatan vertikal saat di tanah
            isJumping = false; // Reset status lompat saat menyentuh tanah
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime; // Terapkan gravitasi
        }

        // Lompat jika karakter di tanah, tidak sedang melompat, dan cooldown selesai
        if (isJumpPressed && groundCheck.isGrounded && !isJumping && Time.time > lastJumpTime + jumpCooldown)
        {
            verticalVelocity = Mathf.Sqrt(jumpStrength * -2f * Physics.gravity.y); // Hitung kecepatan lompatan
            Jumped?.Invoke(); // Panggil event Jumped
            isJumping = true; // Set status lompat
            lastJumpTime = Time.time; // Set waktu terakhir lompat
        }

        // Terapkan kecepatan vertikal ke CharacterController
        Vector3 movement = new Vector3(0, verticalVelocity, 0);
        characterController.Move(movement * Time.deltaTime);
    }

    void OnJumpButtonClicked()
    {
        StartCoroutine(OnJumpButtonClickedDelay());
        IEnumerator OnJumpButtonClickedDelay()
        {
            isJumpButtonPressed = true;
            yield return new WaitForSeconds(0);
            isJumpButtonPressed = false;
        }
    }
}