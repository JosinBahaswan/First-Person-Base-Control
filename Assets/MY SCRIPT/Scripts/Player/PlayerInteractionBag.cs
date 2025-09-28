// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using System.Collections.Generic;
// using System.Collections;

// public class PlayerInteractionBag : MonoBehaviour
// {
//     public static PlayerInteractionBag Instance { get; private set; }

//     [Header("Interaction Settings")]
//     public float interactDistance = 3f;
//     public Camera playerCamera;
//     public Transform holdItemParent;
//     public LayerMask holdedItemLayer;
//     public KeyCode interactKey = KeyCode.E;
//     public KeyCode dropKey = KeyCode.G;
//     public KeyCode throwKey = KeyCode.H;

//     [Header("UI References")]
//     public RectTransform interactionUI;
//     public Image interactionImg;
//     public TMP_Text interactionText;
//     public Button interactButton;
//     public Button dropButton;
//     public Button throwButton;

//     [Header("Inventory System")]
//     public bool hasBackpack;
//     public int inventoryCapacity = 5;
//     public List<Item> inventory = new List<Item>();

//     [Header("Debug")]
//     public Item holdItem;
//     private string holdItemDefaultLayerName;
//     private Interactable currentInteractable;
//     private FirstPersonMovement fpp;
//     private bool isInteractPressed;
//     private bool isDropPressed;
//     private bool isThrowPressed;

//     #region Initialization
//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//         }
//         else
//         {
//             Destroy(gameObject);
//             return;
//         }
//     }

//     private void Start()
//     {
//         fpp = GetComponent<FirstPersonMovement>();
//         InitializeCamera();
//         SetupButtonListeners();
//     }

//     private void InitializeCamera()
//     {
//         if (playerCamera != null)
//         {
//             playerCamera.cullingMask = ~holdedItemLayer.value;
//         }
//     }

//     private void SetupButtonListeners()
//     {
//         if (interactButton != null)
//         {
//             interactButton.onClick.AddListener(() => isInteractPressed = true);
//         }

//         if (dropButton != null)
//         {
//             dropButton.onClick.AddListener(OnDropClick);
//         }

//         if (throwButton != null)
//         {
//             throwButton.onClick.AddListener(OnThrowClick);
//         }
//     }
//     #endregion

//     #region Main Game Loop
//     private void Update()
//     {
//         CheckForInteractableObject();
//         HandleInput();
//         UpdateMobileUI();
//     }

//     private void HandleInput()
//     {
//         if (fpp.useJoystick)
//         {
//             HandleMobileInput();
//         }
//         else
//         {
//             HandleKeyboardInput();
//         }
//     }

//     private void HandleMobileInput()
//     {
//         if (isInteractPressed)
//         {
//             InteractWithObject();
//             isInteractPressed = false;
//         }

//         if (isDropPressed && holdItem != null)
//         {
//             DropItem(holdItem);
//         }

//         if (isThrowPressed && holdItem != null)
//         {
//             ThrowItem(holdItem);
//         }
//     }

//     private void HandleKeyboardInput()
//     {
//         if (Input.GetKeyDown(interactKey))
//         {
//             InteractWithObject();
//         }

//         if (Input.GetKeyDown(dropKey) && holdItem != null)
//         {
//             DropItem(holdItem);
//         }

//         if (Input.GetKeyDown(throwKey) && holdItem != null)
//         {
//             ThrowItem(holdItem);
//         }
//     }
//     #endregion

//     #region Interaction System
//     private void CheckForInteractableObject()
//     {
//         Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
//         if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance))
//         {
//             SetInteractionUI(null);
//             return;
//         }

//         currentInteractable = hit.collider.GetComponent<Interactable>();
//         SetInteractionUI(currentInteractable);
//     }

//     private void SetInteractionUI(Interactable interactable)
//     {
//         bool showUI = interactable != null;
//         interactionUI.gameObject.SetActive(showUI);

//         if (!showUI)
//         {
//             if (fpp.useJoystick)
//             {
//                 interactButton.gameObject.SetActive(false);
//             }
//             return;
//         }

//         interactionText.text = $"<b>{interactable.objectName}</b> - <i>{interactable.interactMessage}</i>";
//         interactionImg.gameObject.SetActive(interactable.objectIcon != null);
//         if (interactable.objectIcon != null)
//         {
//             interactionImg.sprite = interactable.objectIcon;
//         }

//         if (fpp.useJoystick)
//         {
//             interactButton.gameObject.SetActive(true);
//         }
//     }

//     private void InteractWithObject()
//     {
//         if (currentInteractable == null) return;
//         currentInteractable.OnInteract();
//     }
//     #endregion

//     #region Item Management
//     public bool TryAddToInventory(Item item)
//     {
//         if (!hasBackpack && item.itemType != Item.ItemType.Backpack)
//         {
//             Debug.Log("You need a backpack to collect items!");
//             return false;
//         }

//         if (inventory.Count >= inventoryCapacity)
//         {
//             Debug.Log("Inventory is full!");
//             return false;
//         }

//         inventory.Add(item);
//         item.gameObject.SetActive(false);

//         if (item.itemType == Item.ItemType.Backpack)
//         {
//             hasBackpack = true;
//             BackpackItem backpack = item.GetComponent<BackpackItem>();
//             if (backpack != null)
//             {
//                 inventoryCapacity += backpack.additionalSlots;
//                 Debug.Log($"Backpack equipped! New capacity: {inventoryCapacity}");
//             }
//         }

//         return true;
//     }

//     public void HoldItem(Item item)
//     {
//         if (!hasBackpack && item.itemType != Item.ItemType.Backpack)
//         {
//             Debug.Log("You need a backpack to hold items!");
//             return;
//         }

//         if (holdItem != null)
//         {
//             DropItem(holdItem);
//         }

//         item.rb.isKinematic = true;
//         item.coll.enabled = false;
//         holdItem = item;
//         holdItemDefaultLayerName = LayerMask.LayerToName(item.gameObject.layer);

//         item.transform.SetParent(holdItemParent);
//         item.transform.localPosition = item.holdPositionOffset;
//         item.transform.localEulerAngles = item.holdRotationOffset;

//         SetLayerRecursively(item.transform, Mathf.RoundToInt(Mathf.Log(holdedItemLayer.value, 2)));
//     }

//     public bool HasItemInInventory(string category, int amount = 1)
//     {
//         int count = 0;
//         foreach (Item item in inventory)
//         {
//             if (item.objectCategory == category)
//             {
//                 count++;
//                 if (count >= amount)
//                 {
//                     return true;
//                 }
//             }
//         }
//         return false;
//     }

//     public void RemoveItemsFromInventory(string category, int amount)
//     {
//         for (int i = inventory.Count - 1; i >= 0 && amount > 0; i--)
//         {
//             if (inventory[i].objectCategory == category)
//             {
//                 Destroy(inventory[i].gameObject);
//                 inventory.RemoveAt(i);
//                 amount--;
//             }
//         }
//     }

//     public void DropItem(Item item)
//     {
//         if (holdItem != item) return;

//         holdItem = null;
//         item.rb.isKinematic = false;
//         item.coll.enabled = true;
//         SetLayerRecursively(item.transform, LayerMask.NameToLayer(holdItemDefaultLayerName));

//         item.transform.SetParent(null);
//         PlaceItemInWorld(item);
//     }

//     public void ThrowItem(Item item, float throwForce = 10f)
//     {
//         DropItem(item);
//         item.rb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.VelocityChange);
//     }

//     public void ConsumeItem()
//     {
//         if (holdItem == null) return;
//         Destroy(holdItem.gameObject);
//         holdItem = null;
//     }

//     private void PlaceItemInWorld(Item item)
//     {
//         Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
//         if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
//         {
//             item.transform.position = hit.point - ray.direction * 0.1f;
//         }
//         else
//         {
//             item.transform.position = ray.GetPoint(interactDistance * 0.5f);
//         }
//         item.transform.rotation = Quaternion.identity;
//     }
//     #endregion

//     #region Utility Methods
//     private void SetLayerRecursively(Transform parent, int layer)
//     {
//         parent.gameObject.layer = layer;
//         foreach (Transform child in parent)
//         {
//             SetLayerRecursively(child, layer);
//         }
//     }

//     private void UpdateMobileUI()
//     {
//         if (!fpp.useJoystick) return;
//         bool showButtons = holdItem != null;
//         dropButton.gameObject.SetActive(showButtons);
//         throwButton.gameObject.SetActive(showButtons);
//     }

//     private void OnDropClick()
//     {
//         StartCoroutine(SetTemporaryFlag(ref isDropPressed));
//     }

//     private void OnThrowClick()
//     {
//         StartCoroutine(SetTemporaryFlag(ref isThrowPressed));
//     }

//     private IEnumerator SetTemporaryFlag(ref bool flag)
//     {
//         flag = true;
//         yield return null;
//         flag = false;
//     }
//     #endregion

//     #region Helper Properties
//     public bool IsHoldingItem => holdItem != null;
//     public bool IsHoldingItemNamed(string name) => IsHoldingItem && holdItem.objectName == name;
//     public bool IsHoldingItemOfType(string category) => IsHoldingItem && holdItem.objectCategory == category;
//     #endregion
// }