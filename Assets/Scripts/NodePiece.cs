using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodePiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int value;
    public Point index = null!;

    [HideInInspector] public Vector2 position;
    [HideInInspector] public RectTransform rect = null!;

    private bool updating;
    private Image img = null!;

    public void initialize(int v, Point p, Sprite piece)
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        value = v;
        setIndex(p);
        img.sprite = piece;
    }

    public void setIndex(Point p)
    {
        index = p;
        resetPosition();
        updateName();
    }

    public void resetPosition()
    {
        position = new Vector2(32 + 64 * index.x, -32 - 64 * index.y);
    }

    public void movePositionTo(Vector2 move)
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, move, Time.deltaTime * 16f);
    }

    public void movePosition(Vector2 move)
    {
        rect.anchoredPosition += Vector2.Lerp(rect.anchoredPosition, move, Time.deltaTime * 16f);
    }

    public bool updatePiece()
    {
        if (Vector3.Distance(rect.anchoredPosition, position) > 1)
        {
            movePositionTo(position);
            updating = true;
            return true;
        }
        else
        {
            rect.anchoredPosition = position;
            updating = false;
            return false;
        }
    }

    void updateName()
    {
        transform.name = $"Node [{index.x}, {index.y}]";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (updating) return;
        MovePieces.instance.movePiece(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MovePieces.instance.dropPiece();
    }
}