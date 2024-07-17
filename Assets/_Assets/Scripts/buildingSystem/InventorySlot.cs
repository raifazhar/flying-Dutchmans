using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Vector2 startAnchoredPosition;
    private Transform startParent;
    private Image image;
    private GridLayoutGroup gridLayoutGroup;

    public static Action<RectTransform, Image, PointerEventData> DragPerformed;
    public static Action<RectTransform> DragEnded;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        gridLayoutGroup = GetComponentInParent<GridLayoutGroup>();
    }

    private void Start()
    {
        startAnchoredPosition = rectTransform.anchoredPosition;
        startParent = transform.parent;
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragPerformed?.Invoke(rectTransform, image, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragEnded?.Invoke(rectTransform);

        if (gridLayoutGroup != null)
        {
            // Disable layout group to move the item freely
            gridLayoutGroup.enabled = false;
        }

        // Reset anchored position and parent
        rectTransform.anchoredPosition = startAnchoredPosition;
        transform.SetParent(startParent);

        if (gridLayoutGroup != null)
        {
            // Re-enable layout group after resetting
            gridLayoutGroup.enabled = true;
        }
    }
}
