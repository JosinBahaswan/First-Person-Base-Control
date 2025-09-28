using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractionNoInventory : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Button interactButton;
    public Button dropButton;
    public Button throwButton;
    public float interactDistance = 3f; // Maximum interaction distance
    public Camera playerCamera; // Player camera
    public Camera interactCamera;
    public RectTransform interactionUI;
    public Image interactionImg;
    public TMP_Text interactionText;
    public KeyCode interactKey = KeyCode.E; // Interaction key
    public KeyCode dropKey = KeyCode.G;
    public KeyCode throwKey = KeyCode.H;
    public Transform holdItemParent;
    public LayerMask holdedItemLayer;
    public bool isAllowUnlockedAll = false;

    [Header("Debug")]
    public Item holdItem;
    public string holdItemDefaultLayerName;
    public Interactable currentInteractable;

    public static PlayerInteractionNoInventory Instance;
    private FirstPersonMovement fpp;
    private bool isInteractPressed;
    private bool isDropPressed;
    private bool isThrowPressed;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        fpp = FirstPersonMovement.instance;

        // interactCamera = new GameObject("Interact Camera").AddComponent<Camera>();
        // interactCamera.clearFlags = CameraClearFlags.Depth;
        // interactCamera.fieldOfView = playerCamera.fieldOfView;
        // interactCamera.nearClipPlane = playerCamera.nearClipPlane;
        // interactCamera.farClipPlane = playerCamera.farClipPlane;
        // interactCamera.transform.SetParent(playerCamera.transform);
        // interactCamera.transform.localPosition = Vector3.zero;
        // interactCamera.transform.localEulerAngles = Vector3.zero;
        // interactCamera.enabled = false;
        interactCamera.cullingMask = holdedItemLayer.value;
        playerCamera.cullingMask = ~holdedItemLayer.value;

        interactButton.onClick.AddListener(OnInteractClick);
        dropButton.onClick.AddListener(OnDropClick);
        throwButton.onClick.AddListener(OnThrowClick);
    }

    void OnInteractClick()
    {
        isInteractPressed = true;
    }

    void OnDropClick()
    {
        StartCoroutine(OnDropClickDelay());
        IEnumerator OnDropClickDelay()
        {
            isDropPressed = true;
            yield return null;
            isDropPressed = false;
        }
    }

    void OnThrowClick()
    {
        StartCoroutine(OnThrowClickDelay());
        IEnumerator OnThrowClickDelay()
        {
            isThrowPressed = true;
            yield return null;
            isThrowPressed = false;
        }
    }

    private void Update()
    {
        // Check for interactable objects
        CheckForInteractableObject();

        if (!fpp.useJoystick)
        {
            // Detect interaction input
            if (Input.GetKeyDown(interactKey))
            {
                InteractWithObject();
            }
            if (Input.GetKeyDown(dropKey) && holdItem != null)
            {
                DropItem(holdItem);
            }
            if (Input.GetKeyDown(throwKey) && holdItem != null)
            {
                ThrowItem(holdItem);
            }
        }
        else
        {
            // Detect Mobile
            if (isInteractPressed)
            {
                InteractWithObject();
                isInteractPressed = false;
            }
            if (isDropPressed && holdItem != null)
            {
                DropItem(holdItem);
            }
            if (isThrowPressed && holdItem != null)
            {
                ThrowItem(holdItem);
            }
        }

        if (holdItem)
        {
            if (fpp.useJoystick)
            {
                dropButton.gameObject.SetActive(true);
                throwButton.gameObject.SetActive(true);
            }
        }
        else
        {
            if (fpp.useJoystick)
            {
                dropButton.gameObject.SetActive(false);
                throwButton.gameObject.SetActive(false);
            }
        }
    }

    void CheckForInteractableObject()
    {
        // Create a ray from the camera
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        // Raycast to detect objects in front of the camera
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // Check if the object hit has an Interactable component
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            currentInteractable = interactable;

            if (interactable != null)
            {
                if (fpp.useJoystick) interactButton.gameObject.SetActive(true);

                // Display interaction message
                interactionUI.gameObject.SetActive(true);
                interactionText.text = $"<b>{interactable.objectName}</b> - <i>{interactable.interactMessage}</i>";
                if (interactable.objectIcon != null)
                {
                    interactionImg.sprite = interactable.objectIcon;
                    interactionImg.gameObject.SetActive(true);
                }
                else
                {
                    interactionImg.gameObject.SetActive(false);
                }
            }
            else
            {
                // Hide message if no interactable object
                interactionUI.gameObject.SetActive(false);
                if (fpp.useJoystick) interactButton.gameObject.SetActive(false);
            }
        }
        else
        {
            // Hide message if ray does not hit anything
            interactionUI.gameObject.SetActive(false);
            if (fpp.useJoystick) interactButton.gameObject.SetActive(false);
        }
    }

    void InteractWithObject()
    {
        // Create a ray from the player camera
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        // Raycast to detect objects in front of the camera
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // Check if the object hit has an Interactable component
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                // Call the Interact() method on the interactable
                interactable.OnInteract();
            }
        }
    }

    public void HoldItem(Item item)
    {
        if (holdItem != null)
        {
            DropItem(holdItem);
        }

        // Pindahkan item ke tangan
        item.rb.isKinematic = true;
        item.coll.enabled = false;
        holdItem = item;
        holdItemDefaultLayerName = LayerMask.LayerToName(item.gameObject.layer);
        item.transform.SetParent(holdItemParent);
        item.transform.localPosition = item.holdPositionOffset;
        item.transform.localEulerAngles = item.holdRotationOffset;
        //interactCamera.enabled = true;
        item.gameObject.layer = Mathf.RoundToInt(Mathf.Log(holdedItemLayer.value, 2));
        foreach (Transform child in item.transform)
        {
            child.gameObject.layer = Mathf.RoundToInt(Mathf.Log(holdedItemLayer.value, 2));
        }
    }
    public void DropItem(Item item)
    {
        if (holdItem == item)
        {
            // Remove from hold item
            holdItem = null;

            // Enable physics
            item.rb.isKinematic = false;
            item.coll.enabled = true;

            // Set the layer to default layer (usually layer 0)
            item.gameObject.layer = LayerMask.NameToLayer(holdItemDefaultLayerName);
            foreach (Transform child in item.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer(holdItemDefaultLayerName);
            }

            // Detach from the player
            item.transform.SetParent(null);

            // Place item at the center of the camera view
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                // If there's a surface in front of the camera, place the item slightly in front of the hit point
                item.transform.position = hit.point - ray.direction * 0.1f; // Offset to prevent overlap
            }
            else
            {
                // Otherwise, place it a fixed distance in front of the camera
                item.transform.position = ray.origin + ray.direction * 1 / 2;
            }

            // Reset rotation
            item.transform.rotation = Quaternion.identity;
        }
    }

    public void ThrowItem(Item item, float throwForce = 10f)
    {
        if (holdItem == item)
        {
            // Remove from hold item
            holdItem = null;

            // Enable physics
            item.rb.isKinematic = false;
            item.coll.enabled = true;

            // Set the layer to default layer (usually layer 0)
            item.gameObject.layer = LayerMask.NameToLayer(holdItemDefaultLayerName);
            foreach (Transform child in item.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer(holdItemDefaultLayerName);
            }

            // Detach from the player
            item.transform.SetParent(null);

            // Place item at the center of the camera view
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                // If there's a surface in front of the camera, place the item slightly in front of the hit point
                item.transform.position = hit.point - ray.direction * 0.1f; // Offset to prevent overlap
            }
            else
            {
                // Otherwise, place it a fixed distance in front of the camera
                item.transform.position = ray.origin + ray.direction * 1;
            }

            // Reset rotation
            item.transform.rotation = Quaternion.identity;

            // Apply throw force to the item's Rigidbody
            item.rb.AddForce(ray.direction * throwForce, ForceMode.VelocityChange);
        }
    }

    public void ConsumeItem()
    {
        Destroy(holdItem.gameObject);
    }

    public bool IsHoldItemName(string itemName)
    {
        if (holdItem == null) return false;
        if (holdItem.objectName != itemName) return false;

        return true;
    }

    public bool IsHoldItemCategory(string itemCategory)
    {
        if (holdItem == null) return false;
        if (holdItem.objectCategory != itemCategory) return false;

        return true;
    }
}
