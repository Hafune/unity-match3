using System;
using UnityEngine;

public class MovePieces : MonoBehaviour
{
    public static MovePieces instance = null!;
    private Match3 game = null!;

    private NodePiece? moving;
    private Point newIndex = null!;
    private Vector2 mouseStart;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        game = GetComponent<Match3>();
    }

    void Update()
    {
        if (moving == null) return;

        var dir = (Vector2) Input.mousePosition - mouseStart;
        var nDir = dir.normalized;
        var aDir = new Vector2(Math.Abs(dir.x), Math.Abs(dir.y));

        newIndex = Point.clone(moving.index);
        var add = Point.zero();

        if (dir.magnitude > 32)
        {
            add = aDir.x > aDir.y ? new Point(nDir.x > 0 ? 1 : -1, 0) : new Point(0, nDir.y > 0 ? -1 : 1);
        }

        newIndex.add(add);

        var position = game.getPositionFromPoint(moving.index);

        if (!newIndex.Equals(moving.index)) position += Point.mult(new Point(add.x, -add.y), 16).toVector();

        moving.movePositionTo(position);
    }

    public void movePiece(NodePiece piece)
    {
        if (moving != null) return;
        moving = piece;
        mouseStart = Input.mousePosition;
    }

    public void dropPiece()
    {
        if (moving == null) return;

        if (!newIndex.Equals(moving.index)) game.flipPieces(moving.index, newIndex, true);
        else game.resetPiece(moving);

        moving = null;
    }
}