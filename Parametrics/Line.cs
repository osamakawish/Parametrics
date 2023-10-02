using System;
using System.Windows;

namespace Parametrics;

public static class SimpleGeometry
{
    public static bool HitTest(Point p1, Point p2, Point p, double delta)
    {
        var d = (p2 - p1).LengthSquared;
        var d1 = (p - p1).LengthSquared;
        var d2 = (p - p2).LengthSquared;
        return Math.Abs(d - d1 - d2) < delta;
    }

    public static double Orientation(Point p1, Point p2, Point p3)
        => (p2.X - p1.X) * (p3.Y - p2.Y) - (p3.X - p2.X) * (p2.Y - p1.Y);
    
    public static bool LineIntersects(Point p1, Point p2, Point q1, Point q2)
        => Orientation(p1, p2, q1) * Orientation(p1, p2, q2) < 0 
        && Orientation(q1, q2, p1) * Orientation(q1, q2, p2) < 0;

    public static bool LineIntersect(Point p1, Point p2, Point q1, Point q2, out Point p)
    {
        var a1 = p2.Y - p1.Y;
        var b1 = p1.X - p2.X;
        var c1 = a1 * p1.X + b1 * p1.Y;

        var a2 = q2.Y - q1.Y;
        var b2 = q1.X - q2.X;
        var c2 = a2 * q1.X + b2 * q1.Y;

        var det = a1 * b2 - a2 * b1;

        if (det == 0)
        {
            p = default;
            return false;  // lines are parallel or coincident
        }

        var x = (b2 * c1 - b1 * c2) / det;
        var y = (a1 * c2 - a2 * c1) / det;

        // Check if the intersection point is within both line segments
        if (Math.Min(p1.X, p2.X) <= x && x <= Math.Max(p1.X, p2.X) &&
            Math.Min(p1.Y, p2.Y) <= y && y <= Math.Max(p1.Y, p2.Y) &&
            Math.Min(q1.X, q2.X) <= x && x <= Math.Max(q1.X, q2.X) &&
            Math.Min(q1.Y, q2.Y) <= y && y <= Math.Max(q1.Y, q2.Y))
        {
            p = new(x, y);
            return true;
        }

        p = default;
        return false;  // no intersection

    }
}