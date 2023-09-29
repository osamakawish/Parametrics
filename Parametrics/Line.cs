using System;
using System.Windows;

namespace Parametrics;

public static class Line
{
    public static bool HitTest(Point p1, Point p2, Point p, double delta)
    {
        var d = (p2 - p1).LengthSquared;
        var d1 = (p - p1).LengthSquared;
        var d2 = (p - p2).LengthSquared;
        return Math.Abs(d - d1 - d2) < delta;
    }

    public static bool Intersect(Point p1, Point p2, Point q1, Point q1, out Point p)
    {
        // Find poi between two lines
        
    }
}