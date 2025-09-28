using System.Collections;
using UnityEngine;

public class AIInteractDoor : MonoBehaviour
{
    private bool isInteractingWithDoor = false;

    private void Update()
    {
        CheckForDoor();
    }

    private void CheckForDoor()
    {
        if (isInteractingWithDoor) return;

        RaycastHit hit;
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        if (Physics.Raycast(rayOrigin, transform.TransformDirection(Vector3.forward), out hit, 1))
        {
            if (hit.transform.TryGetComponent(out InteractableDoor door))
            {
                StartCoroutine(InteractDoor(door));
            }
        }
    }

    private IEnumerator InteractDoor(InteractableDoor door)
    {
        if (door.isInteracted) yield break;

        isInteractingWithDoor = true;

        door.ToggleOpenClose();
        yield return new WaitForSeconds(0.5f);

        door.ToggleOpenClose();
        yield return new WaitForSeconds(0.5f);

        isInteractingWithDoor = false;
    }
}
