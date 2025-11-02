using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    Vector2 velocity;
    Vector2 frameVelocity;
    FirstPersonMovement fpp;

    // Recoil support
    public static FirstPersonLook Instance { get; private set; }
    private Vector2 recoilOffset = Vector2.zero; // Offset rotasi dari recoil
    
    [Header("Recoil Settings")]
    [SerializeField] private bool autoRecovery = true; // Auto recovery setelah berhenti menembak
    [SerializeField] private float recoilRecoverySpeed = 3f; // Kecepatan recovery (jika auto)
    [SerializeField] private float recoilDecaySpeed = 0.5f; // Natural decay (jika manual)
    
    private float timeSinceLastRecoil = 0f; // Waktu sejak recoil terakhir
    private float recoveryDelay = 0.3f; // Delay sebelum mulai recovery

    private Vector2 previousTouchPosition; // Menyimpan posisi sentuhan sebelumnya
    private bool isTouching = false; // Status sentuhan

    void Start()
    {
        Instance = this;
        fpp = FirstPersonMovement.instance;
        character = fpp.transform;
        if (!fpp.useJoystick) Cursor.lockState = CursorLockMode.Locked;

        previousTouchPosition = Vector2.zero; // Inisialisasi posisi sentuhan sebelumnya
    }

    void Update()
    {
        Vector2 mouseDelta;

        if (!fpp.useJoystick)
        {
            // Input mouse (PC)
            mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        }
        else
        {
            // Input joystick (Mobile)
            Vector2 currentTouchPosition = fpp.camTouchController.GetTouchPosition;

            // Jika sedang tidak menyentuh, abaikan input
            if (currentTouchPosition == Vector2.zero)
            {
                isTouching = false;
                return;
            }

            // Jika baru mulai menyentuh, set posisi sebelumnya ke posisi saat ini
            if (!isTouching)
            {
                previousTouchPosition = currentTouchPosition;
                isTouching = true;
            }

            // Hitung perbedaan (delta) antara posisi sentuhan saat ini dan sebelumnya
            mouseDelta = currentTouchPosition - previousTouchPosition;

            // Simpan posisi sentuhan saat ini untuk frame berikutnya
            previousTouchPosition = currentTouchPosition;

            // Jika tidak ada pergeseran (delta sangat kecil), abaikan input
            if (mouseDelta.magnitude < 0.01f)
            {
                mouseDelta = Vector2.zero;
            }
        }

        // Hitung kecepatan rotasi
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        // Update timer sejak recoil terakhir
        timeSinceLastRecoil += Time.deltaTime;
        
        // Recoil recovery system
        if (recoilOffset.magnitude > 0.01f)
        {
            if (autoRecovery)
            {
                // Auto recovery setelah delay (berhenti menembak)
                if (timeSinceLastRecoil > recoveryDelay)
                {
                    recoilOffset = Vector2.Lerp(recoilOffset, Vector2.zero, Time.deltaTime * recoilRecoverySpeed);
                }
                // Jika masih dalam delay, recoil tetap (tidak recovery)
            }
            else
            {
                // Manual control - hanya natural decay minimal
                recoilOffset = Vector2.Lerp(recoilOffset, Vector2.zero, Time.deltaTime * recoilDecaySpeed);
            }
        }
        else if (recoilOffset != Vector2.zero)
        {
            recoilOffset = Vector2.zero;
        }

        // Apply rotasi (termasuk recoil offset) - SELALU dipanggil
        ApplyRotation();
    }

    /// <summary>
    /// Apply recoil to camera (called from weapon)
    /// </summary>
    public void ApplyRecoil(float verticalRecoil, float horizontalRecoil)
    {
        // Apply recoil sebagai offset yang akan di-recover smooth
        // Vertical recoil = kick ke atas (positif)
        // Horizontal recoil = random kiri/kanan
        recoilOffset.y += verticalRecoil;
        recoilOffset.x += horizontalRecoil;
        
        // Reset timer - baru saja menembak!
        timeSinceLastRecoil = 0f;
        
        // IMMEDIATELY apply rotation untuk efek instant!
        ApplyRotation();
    }
    
    /// <summary>
    /// Apply rotation dengan recoil offset (dipanggil dari Update dan ApplyRecoil)
    /// </summary>
    private void ApplyRotation()
    {
        // Recoil offset MENGURANGI velocity.y (karena velocity.y negatif = look up)
        // Jadi recoil positif = kick ke atas = kurangi velocity.y yang negatif
        float finalVertical = -velocity.y - recoilOffset.y; // MINUS recoil untuk kick ke atas!
        float finalHorizontal = velocity.x + recoilOffset.x;
        
        transform.localRotation = Quaternion.AngleAxis(finalVertical, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(finalHorizontal, Vector3.up);
    }
    
    /// <summary>
    /// Set recovery delay (called from weapon untuk custom timing)
    /// </summary>
    public void SetRecoveryDelay(float delay)
    {
        recoveryDelay = delay;
    }
    
    /// <summary>
    /// Get current recoil info untuk debugging
    /// </summary>
    public Vector2 GetCurrentRecoil() => recoilOffset;
    public float GetTimeSinceLastRecoil() => timeSinceLastRecoil;
    public bool IsAutoRecovery() => autoRecovery;
}