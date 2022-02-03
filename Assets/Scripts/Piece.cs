using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Piece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Canvas canvas = null!;
    [HideInInspector] public RectTransform rect = null!;
    [HideInInspector] public Image img = null!;

    [HideInInspector] public Vector2 drag;
    public bool isDragged;
    public CanvasRenderer renderer = null!;

    public void Initialize()
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        renderer.transform.SetAsLastSibling();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragged = false;
        renderer.transform.SetAsFirstSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDragged = true;
        drag += eventData.delta / canvas.scaleFactor;
    }
}