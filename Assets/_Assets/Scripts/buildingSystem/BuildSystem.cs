using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingSystem : MonoBehaviour {
    [SerializeField] private CinemachineFreeLook freeLook;
    [SerializeField] private ListOfBlocks listOfBlocks;
    [SerializeField] private GameObject inventory;
    [SerializeField] private Canvas canvas;
    [SerializeField] private LayerMask buildableLayer;

    private RectTransform inventoryRectTransform;
    private CinemachineInputProvider inputProvider;
    private bool blockInstantiated;
    private Transform instantiatedBlock;

    private void Awake() {
        // Subscribe to events
        InventorySlot.DragPerformed += OnDragPerformed;
        InventorySlot.DragEnded += OnDragEnded;
    }

    private void Start() {
        // Get references
        inventoryRectTransform = inventory.GetComponent<RectTransform>();
        inputProvider = freeLook.GetComponent<CinemachineInputProvider>();
    }


    public bool IsTouchInInventory(Vector2 touchPosition) {
        // Check if touch is within inventory bounds
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inventoryRectTransform, touchPosition, null, out Vector2 localPoint);
        return inventoryRectTransform.rect.Contains(localPoint);
    }

    public void OnDragPerformed(RectTransform rectTransform, Image image, PointerEventData pointerEventData) {
        // Handle dragging logic
        inputProvider.enabled = false;
        if (!IsTouchInInventory(Input.GetTouch(0).position) && !blockInstantiated) {
            // Instantiate the block if not in inventory and not already instantiated
            foreach (var material in listOfBlocks.materials) {
                if (material.image == image.sprite) {
                    instantiatedBlock = Instantiate(material.block);
                    SetDraggedObjectVisibility(rectTransform, false);
                    blockInstantiated = true;
                    break;
                }
            }
        }
        else {
            // Move the dragged object
            rectTransform.anchoredPosition += pointerEventData.delta / canvas.scaleFactor;
        }

        // Place instantiated block if exists
        if (instantiatedBlock != null) {
            PlaceInstantiatedBlock();
        }
    }

    private void PlaceInstantiatedBlock() {
        // Raycast to place instantiated block on buildable layer
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, buildableLayer)) {
            instantiatedBlock.transform.position = hit.point;
            Debug.Log($"Placed block at {hit.point}");
        }
    }

    public void OnDragEnded(RectTransform rectTransform) {
        // Reset visuals and state when drag ends
        SetDraggedObjectVisibility(rectTransform, true);
        instantiatedBlock = null;
        blockInstantiated = false;
        inputProvider.enabled = true;
    }

    private void SetDraggedObjectVisibility(RectTransform rectTransform, bool visible) {
        // Set alpha visibility of dragged object and its child
        rectTransform.GetComponent<CanvasRenderer>().SetAlpha(visible ? 1f : 0f);
        rectTransform.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(visible ? 1f : 0f);

    }
}
