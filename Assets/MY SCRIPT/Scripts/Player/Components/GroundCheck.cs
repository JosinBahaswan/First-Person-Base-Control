using UnityEngine;

[ExecuteInEditMode]
public class GroundCheck : MonoBehaviour
{
    public float distanceThreshold = 0.15f;
    public bool isGrounded = true;
    public event System.Action Grounded;

    const float OriginOffset = 0.001f;
    Vector3 RaycastOrigin => transform.position + Vector3.up * OriginOffset;
    float RaycastDistance => distanceThreshold + OriginOffset;

    void LateUpdate()
    {
        // Cek apakah karakter menyentuh tanah
        bool isGroundedNow = Physics.Raycast(RaycastOrigin, Vector3.down, RaycastDistance);

        // Jika status grounded berubah
        if (isGroundedNow && !isGrounded)
        {
            Grounded?.Invoke();
        }

        isGrounded = isGroundedNow;
        // Debug.Log("Grounded: " + isGrounded); // Debug untuk memantau status grounded
    }

    void OnDrawGizmosSelected()
    {
        Debug.DrawLine(RaycastOrigin, RaycastOrigin + Vector3.down * RaycastDistance, isGrounded ? Color.white : Color.red);
    }
}