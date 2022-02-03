using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Match3 : MonoBehaviour
{
    private const string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
    private const int width = 9;
    private const int height = 14;

    private int[] fills = null!;
    private Node[,] board = null!;

    public ArrayLayout boardLayout = null!;

    [Header("UI Elements")] public Sprite[] pieces = null!;
    public RectTransform gameBoard = null!;
    public RectTransform killedBoard = null!;

    [Header("Prefabs")] public GameObject nodePiece = null!;
    public GameObject killedPiece = null!;

    private List<NodePiece> update = new List<NodePiece>();
    private List<NodePiece> dead = null!;
    private List<FlippedPieces> flipped = null!;
    private List<KilledPiece> killed = null!;

    private Random random = null!;

    void Start()
    {
        startGame();
    }
    
    void startGame()
    {
        board = new Node[width, height];

        fills = new int[width];
        random = new Random(getRandomSeed().GetHashCode());
        update = new List<NodePiece>();
        dead = new List<NodePiece>();
        flipped = new List<FlippedPieces>();
        killed = new List<KilledPiece>();

        initializeBoard();
        verifyBoard();
        instantiateBoard();
    }
    
    void Update()
    {
        var finishedUpdating = update.Where(piece => !piece.updatePiece()).ToList();
        foreach (var piece in finishedUpdating)
        {
            var flip = getFlipped(piece);
            NodePiece? flippedPiece = null;

            var x = piece.index.x;
            fills[x] = Mathf.Clamp(fills[x] - 1, 0, width);

            var connected = isConnected(piece.index, true);
            var wasFlipped = flip != null;

            if (wasFlipped)
            {
                flippedPiece = flip!.getOtherPiece(piece);
                addPoints(connected, isConnected(flippedPiece!.index, true));
            }

            if (connected.Count == 0)
            {
                if (wasFlipped)
                {
                    flipPieces(piece.index, flippedPiece!.index, false);
                }
            }
            else
            {
                foreach (var pnt in connected)
                {
                    killPiece(pnt);
                    var node = getNodeAtPoint(pnt);
                    if (node.piece != null)
                    {
                        node.piece.gameObject.SetActive(false);
                        dead.Add(node.piece);
                    }

                    node.setPiece(null);
                }

                applyGravityToBoard();
            }

            flipped.Remove(flip!);
            update.Remove(piece);
        }
    }

    void applyGravityToBoard()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = height - 1; y >= 0; y--)
            {
                var p = new Point(x, y);
                var node = getNodeAtPoint(p);
                var value = getValueAtPoint(p);
                if (value != 0) continue;
                for (var ny = y - 1; ny >= -1; ny--)
                {
                    var next = new Point(x, ny);
                    var nextValue = getValueAtPoint(next);
                    if (nextValue == 0) continue;
                    if (nextValue != -1)
                    {
                        var gotten = getNodeAtPoint(next);
                        var piece = gotten.piece;

                        node.setPiece(piece);
                        update.Add(piece!);

                        gotten.setPiece(null);
                    }
                    else
                    {
                        var newValue = fillPiece();
                        NodePiece piece;
                        var fallPoint = new Point(x, -1 - fills[x]);
                        if (dead.Count > 0)
                        {
                            piece = dead[0];
                            piece.gameObject.SetActive(true);

                            dead.RemoveAt(0);
                        }
                        else
                        {
                            var obj = Instantiate(nodePiece, gameBoard);
                            piece = obj.GetComponent<NodePiece>();
                        }

                        piece.initialize(newValue, p, pieces[newValue - 1]);
                        piece.rect.anchoredPosition = getPositionFromPoint(fallPoint);

                        var hole = getNodeAtPoint(p);
                        hole.setPiece(piece);
                        resetPiece(piece);
                        fills[x]++;
                    }

                    break;
                }
            }
        }
    }

    private FlippedPieces? getFlipped(NodePiece p)
    {
        return flipped.FirstOrDefault(t => t.getOtherPiece(p) != null);
    }
    void instantiateBoard()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var node = getNodeAtPoint(new Point(x, y));
                var value = board[x, y].value;
                if (value <= 0) continue;

                var p = Instantiate(nodePiece, gameBoard);
                var piece = p.GetComponent<NodePiece>();
                var rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + 64 * x, -32 - 64 * y);
                piece.initialize(value, new Point(x, y), pieces[value - 1]);
                node.setPiece(piece);
            }
        }
    }

    public void resetPiece(NodePiece piece)
    {
        piece.resetPosition();
        update.Add(piece);
    }

    public void flipPieces(Point one, Point two, bool main)
    {
        if (getValueAtPoint(one) < 0) return;

        var nodeOne = getNodeAtPoint(one);
        var pieceOne = nodeOne.piece;

        if (getValueAtPoint(two) > 0)
        {
            var nodeTwo = getNodeAtPoint(two);
            var pieceTwo = nodeTwo.piece;
            nodeOne.setPiece(pieceTwo);
            nodeTwo.setPiece(pieceOne);

            if (main) flipped.Add(new FlippedPieces(pieceOne, pieceTwo));

            update.Add(pieceOne!);
            update.Add(pieceTwo!);
        }
        else resetPiece(pieceOne!);
    }

    void initializeBoard()
    {
        board = new Node[width, height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                board[x, y] = new Node(boardLayout.rows[y].row[x] ? -1 : fillPiece(), new Point(x, y));
            }
        }
    }

    void verifyBoard()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var p = new Point(x, y);
                var value = getValueAtPoint(p);
                if (value <= 0) continue;

                var remove = new List<int>();
                while (isConnected(p, true).Count > 0)
                {
                    value = getValueAtPoint(p);
                    if (!remove.Contains(value)) remove.Add(value);

                    setValueAtPoint(p, newValue(remove));
                }
            }
        }
    }

    void killPiece(Point p)
    {
        var available = killed.Where(kill => !kill.falling).ToList();
        KilledPiece? set;
        if (available.Count > 0)
        {
            set = available[0];
        }
        else
        {
            var kill = Instantiate(killedPiece, killedBoard);
            var kPiece = kill.GetComponent<KilledPiece>();
            set = kPiece;
            killed.Add(kPiece);
        }

        var value = getValueAtPoint(p) - 1;
        if (set != null && value >= 0 && value < pieces.Length)
        {
            set.initialize(pieces[value], getPositionFromPoint(p));
        }
    }

    List<Point> isConnected(Point p, bool main)
    {
        var connected = new List<Point>();
        var value = getValueAtPoint(p);
        Point[] directions =
        {
            Point.up(),
            Point.right(),
            Point.down(),
            Point.left(),
        };

        foreach (Point dir in directions)
        {
            var line = new List<Point>();

            var same = 0;
            for (var i = 1; i < 3; i++)
            {
                var check = Point.add(p, Point.mult(dir, i));
                if (getValueAtPoint(check) != value) continue;

                line.Add(check);
                same++;
            }

            if (same > 1) addPoints(connected, line);
        }

        for (var i = 0; i < 2; i++)
        {
            var line = new List<Point>();
            var same = 0;

            var check = new[] {Point.add(p, directions[i]), Point.add(p, directions[i + 2])};
            foreach (var next in check)
            {
                if (getValueAtPoint(next) != value) continue;

                line.Add(next);
                same++;
            }

            if (same > 1) addPoints(connected, line);
        }

        for (var i = 0; i < 4; i++)
        {
            var square = new List<Point>();

            var same = 0;
            var next = i + 1;
            if (next >= 4) next -= 4;

            var check = new[]
            {
                Point.add(p, directions[i]), Point.add(p, directions[next]),
                Point.add(p, Point.add(directions[i], directions[next]))
            };
            foreach (var point in check)
            {
                if (getValueAtPoint(point) != value) continue;

                square.Add(point);
                same++;
            }

            if (same > 2) addPoints(connected, square);
        }

        if (!main) return connected;

        for (var i = 0; i < connected.Count; i++)
        {
            addPoints(connected, isConnected(connected[i], false));
        }

        return connected;
    }

    void addPoints(List<Point> points, List<Point> add)
    {
        // foreach (var p in add)
        // {
        //     var doAdd = true;
        //     for (var i = 0; i < points.Count; i++)
        //     {
        //         if (!add[i].Equals(p)) continue;
        //         doAdd = false;
        //         break;
        //     }
        //
        //     if (doAdd) points.Add(p);
        // }
        foreach (var p in add)
        {
            var doAdd = points.All(t => !t.Equals(p));

            if (doAdd) points.Add(p);
        }
    }

    int getValueAtPoint(Point p)
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1;
        return board[p.x, p.y].value;
    }

    void setValueAtPoint(Point p, int value)
    {
        board[p.x, p.y].value = value;
    }

    Node getNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }

    int newValue(List<int> remove)
    {
        var available = new List<int>();
        for (var i = 0; i < pieces.Length; i++) available.Add(i + 1);

        foreach (var i in remove) available.Remove(i);

        return available.Count <= 0 ? 0 : available[new Random().Next(available.Count)];
    }

    int fillPiece()
    {
        return random.Next(100) / (100 / pieces.Length) + 1;
    }

    string getRandomSeed()
    {
        return Enumerable.Range(0, 20).Aggregate("",
            (current, i) => current + acceptableChars[new Random().Next(acceptableChars.Length)]);
    }

    public Vector2 getPositionFromPoint(Point p)
    {
        return new Vector2(32 + 64 * p.x, -32 - 64 * p.y);
    }
}

[Serializable]
public class Node
{
    public int value; //0 = blanc, 1 = cube, 2 = sphere, 3 = cylinder, 4 = pyramid, 5 = diamond, -1 = hole
    public Point index;

    public NodePiece? piece { get; private set; }

    public Node(int v, Point i)
    {
        value = v;
        index = i;
    }

    public void setPiece(NodePiece? p)
    {
        piece = p;
        value = piece == null ? 0 : piece.value;

        if (piece == null) return;

        piece.setIndex(index);
    }
}

[Serializable]
public class FlippedPieces
{
    public NodePiece? one;
    public NodePiece? two;

    public FlippedPieces(NodePiece? o, NodePiece? t)
    {
        one = o;
        two = t;
    }

    public NodePiece? getOtherPiece(NodePiece p)
    {
        if (p == one) return two;
        if (p == two) return one;

        return null;
    }
}