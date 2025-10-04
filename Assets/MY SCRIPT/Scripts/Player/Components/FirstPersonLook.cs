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

    private Vector2 previousTouchPosition; // Menyimpan posisi sentuhan sebelumnya
    private bool isTouching = false; // Status sentuhan

    void Start()
    {
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

        // Rotasi kamera (atas/bawah) dan karakter (kiri/kanan)
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}