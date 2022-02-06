using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Piece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Canvas canvas = null!;
    [HideInInspector] public RectTransform rect = null!;
    [HideInInspector] public Image img = null!;

    [HideInInspector] public Vector2 drag;
    public CanvasRenderer canvasRenderer = null!;

    public bool isDragged;
    public bool justPointerUp;
    public bool blocked;

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
        if (blocked) return;
        
        isDragged = false;
        justPointerUp = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (blocked) return;
        
        isDragged = true;
        drag += eventData.delta / canvas.scaleFactor / ecs.pixelPerMeter;
    }
    
    public void DestroyScriptInstance()
    {
        Destroy(this);
        Destroy(img);
    }
}