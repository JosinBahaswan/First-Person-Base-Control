using UnityEngine;
using DG.Tweening;

public class InteractableDoor : LockableInteractable
{
    [Header("Door")]
    public Transform doorPivot;
    public bool openAngleX = false;
    public bool openAngleY = true;
    public bool openAngleZ = false;
    public float openAngle = -90f;

    public bool openDirectionX = false;
    public bool openDirectionY = false;
    public bool openDirectionZ = true;
    public float openDirection = 1.5f; // Offset distance along the active axis

    public float openCloseDuration = 0.3f;
    public bool isOneWay = false;
    public bool useDirectionTween = false; // Use position-based opening/closing

    private Tweener currentTween;
    private Vector3 initialRotation; // Store the initial rotation

    public override void Start()
    {
        base.Start();

        initialRotation = doorPivot.localEulerAngles;
    }

    public override void ToggleOpenClose()
    {
        if (currentTween != null && currentTween.IsPlaying())
        {
            currentTween.Kill(); // Kill the current tween if it's still running
        }

        if (isOpen)
        {
            if (useDirectionTween)
            {
                CloseDoorPosition();
            }
            else
            {
                CloseDoorRotation();
            }
            OnClosed?.Invoke();
        }
        else
        {
            if (useDirectionTween)
            {
                OpenDoorPosition();
            }
            else
            {
                OpenDoorRotation();
            }
            OnOpened?.Invoke();

            if (isOneWay)
            {
                DestroyAfterTween();
            }
        }

        isOpen = !isOpen;
    }

    private void OpenDoorRotation()
    {
        Vector3 targetEulerAngles = initialRotation; // Start from the initial rotation

        if (openAngleX)
        {
            targetEulerAngles.x += openAngle;
        }
        else if (openAngleY)
        {
            targetEulerAngles.y += openAngle;
        }
        else if (openAngleZ)
        {
            targetEulerAngles.z += openAngle;
        }

        currentTween = doorPivot.DOLocalRotate(targetEulerAngles, openCloseDuration)
                                .SetEase(Ease.OutQuad);
    }

    private void CloseDoorRotation()
    {
        currentTween = doorPivot.DOLocalRotate(initialRotation, openCloseDuration)
                                .SetEase(Ease.OutQuad); // Return to the initial rotation
    }

    private void OpenDoorPosition()
    {
        Vector3 targetPosition = doorPivot.localPosition;

        if (openDirectionX)
        {
            targetPosition.x += openDirection;
        }
        else if (openDirectionY)
        {
            targetPosition.y += openDirection;
        }
        else if (openDirectionZ)
        {
            targetPosition.z += openDirection;
        }

        currentTween = doorPivot.DOLocalMove(targetPosition, openCloseDuration)
                                .SetEase(Ease.OutQuad);
    }

    private void CloseDoorPosition()
    {
        Vector3 targetPosition = doorPivot.localPosition;

        if (openDirectionX)
        {
            targetPosition.x -= openDirection;
        }
        else if (openDirectionY)
        {
            targetPosition.y -= openDirection;
        }
        else if (openDirectionZ)
        {
            targetPosition.z -= openDirection;
        }

        currentTween = doorPivot.DOLocalMove(targetPosition, openCloseDuration)
                                .SetEase(Ease.OutQuad);
    }

    private void DestroyAfterTween()
    {
        if (currentTween != null)
        {
            currentTween.OnComplete(() => Destroy(this));
        }
        else
        {
            Destroy(this);
        }
    }

    public void ForceSetIsLocked()
    {
        isLocked = true;
        isRequiredItem = true;
        requiredItemName = "AWEDAADAFASFADGFASDFAEDFAEFDASDF";
    }
}
