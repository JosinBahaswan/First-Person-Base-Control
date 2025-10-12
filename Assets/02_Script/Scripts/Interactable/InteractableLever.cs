using UnityEngine;
using DG.Tweening;

public class InteractableLever : Interactable
{
    [Header("Lever Settings")]
    public Transform leverPivot;
    public bool rotateOnX = false;
    public bool rotateOnY = false;
    public bool rotateOnZ = true;
    public float rotationAngle = 45f; // Sudut rotasi saat diaktifkan
    public float initRotationAngle = -45f;
    public float animationDuration = 0.3f;

    private Tweener currentTween;
    private bool isLeverActive = false;
    private Vector3 targetRotation;

    public override void Start()
    {
        base.Start();

        targetRotation = leverPivot.localEulerAngles;

        if (rotateOnX) targetRotation.x = initRotationAngle;
        if (rotateOnY) targetRotation.y = initRotationAngle;
        if (rotateOnZ) targetRotation.z = initRotationAngle;
        
        leverPivot.DOLocalRotate(targetRotation, 0);
    }

    public override void OnInteract()
    {
        base.OnInteract();
        ToggleLever();
        OnInteractSucceed(); // Selalu dianggap berhasil
    }

    private void ToggleLever()
    {
        if (currentTween != null && currentTween.IsPlaying())
        {
            currentTween.Kill(); // Hentikan animasi sebelumnya jika masih berjalan
        }

        if (isLeverActive)
        {
            // Kembali ke posisi semula
            if (rotateOnX) targetRotation.x = initRotationAngle;
            if (rotateOnY) targetRotation.y = initRotationAngle;
            if (rotateOnZ) targetRotation.z = initRotationAngle;
        }
        else
        {
            // Rotasi ke posisi aktif
            if (rotateOnX) targetRotation.x = rotationAngle;
            if (rotateOnY) targetRotation.y = rotationAngle;
            if (rotateOnZ) targetRotation.z = rotationAngle;
        }

        SetRotation();

        currentTween = leverPivot.DOLocalRotate(targetRotation, animationDuration)
                                .SetEase(Ease.InOutQuad);

        isLeverActive = !isLeverActive;
    }

    void SetRotation()
    {
        if (isLeverActive)
        {
            // Kembali ke posisi semula
            if (rotateOnX) targetRotation.x = initRotationAngle;
            if (rotateOnY) targetRotation.y = initRotationAngle;
            if (rotateOnZ) targetRotation.z = initRotationAngle;
        }
        else
        {
            // Rotasi ke posisi aktif
            if (rotateOnX) targetRotation.x = rotationAngle;
            if (rotateOnY) targetRotation.y = rotationAngle;
            if (rotateOnZ) targetRotation.z = rotationAngle;
        }
    }
}
