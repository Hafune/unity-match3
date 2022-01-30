using UnityEngine;

[System.Serializable]
public class Point
{
    public int x;
    public int y;

    public Point(int nx, int ny)
    {
        x = nx;
        y = ny;
    }

    public void add(Point p)
    {
        x += p.x;
        y += p.y;
    }

    public Vector2 toVector()
    {
        return new Vector2(x, y);
    }

    public bool Equals(Point p)
    {
        return p.x == x && p.y == y;
    }

    public static Point fromVector(Vector2 v)
    {
        return new Point((int) v.x, (int) v.y);
    }

    public static Point fromVector(Vector3 v)
    {
        return new Point((int) v.x, (int) v.y);
    }

    public static Point mult(Point p, int multiply)
    {
        return new Point(p.x * multiply, p.y * multiply);
    }

    public static Point add(Point p0, Point p1)
    {
        return new Point(p0.x + p1.x, p0.y + p1.y);
    }

    public static Point clone(Point p)
    {
        return new Point(p.x, p.y);
    }

    public static Point zero()
    {
        return new Point(0, 0);
    }

    public static Point one()
    {
        return new Point(1, 1);
    }

    public static Point up()
    {
        return new Point(0, 1);
    }

    public static Point down()
    {
        return new Point(0, -1);
    }

    public static Point left()
    {
        return new Point(-1, 0);
    }

    public static Point right()
    {
        return new Point(1, 0);
    }
}