using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class KilledPiece : MonoBehaviour
{
    public bool falling;

    private const float speed = 16f;
    private const float gravity = 16f;
    private Vector2 moveDir;
    private RectTransform rect = null!;
    private Image img = null!;

    public void initialize(Sprite piece, Vector2 start)
    {
        falling = true;

        moveDir = Vector2.up;
        moveDir.x = (float) (new Random().NextDouble() * 2f - 1f);
        moveDir *= speed / 2;

        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        img.sprite = piece;
        rect.anchoredPosition = start;
    }

    void Update()
    {
        if (!falling) return;

        moveDir.y -= Time.deltaTime * gravity;
        moveDir.x = Mathf.Lerp(moveDir.x, 0, Time.deltaTime);
        rect.anchoredPosition += moveDir * (Time.deltaTime * speed);
        var p = rect.position;
        if (p.x < -64f || p.x > Screen.width + 64f || p.y < -64f || p.y > Screen.height + 64f) falling = false;
    }
}