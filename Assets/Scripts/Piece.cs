using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Piece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Canvas canvas = null!;
    [HideInInspector] public RectTransform rect = null!;
    [HideInInspector] public Image img = null!;

    [HideInInspector] public Vector2 dragOffset;
    public CanvasRenderer canvasRenderer = null!;

    [HideInInspector] public bool isDragged;
    [HideInInspector] public bool justPointerUp;
    [HideInInspector] public bool isBlocked;

    [Range(0.1f, 10f)] public float fallSpeed = 3f;
    [Range(0f, 10f)] public float horizontalSpread = 2f;

    private MyEngine ecs = null!;

    public void Initialize(MyEngine myEngine)
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        ecs = myEngine;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canvasRenderer.transform.SetAsLastSibling();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isBlocked) return;

        isDragged = false;
        justPointerUp = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isBlocked) return;

        isDragged = true;
        dragOffset += eventData.delta / canvas.scaleFactor / ecs.pixelPerMeter;
    }

    public void DestroyScriptInstance()
    {
        Destroy(this);
        Destroy(img);
    }
}