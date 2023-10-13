using System;
using System.Windows;

namespace Curves2D;

public record LineSegment2D(Point Start, Point End)
{
    public double Orientation(Point point) => (End.Y - Start.Y) * (point.X - End.X) - (End.X - Start.X) * (point.Y - End.Y);

    public bool Intersects(LineSegment2D lineSegment)
    {
        var o1 = Orientation(lineSegment.Start);
        var o2 = Orientation(lineSegment.End);
        var o3 = lineSegment.Orientation(Start);
        var o4 = lineSegment.Orientation(End);
        
        return (o1 != o2 && o3 != o4)                       ||
                o1 == 0 && lineSegment.ContainsPoint(Start) ||
                o2 == 0 && lineSegment.ContainsPoint(End)   ||
                o3 == 0 && ContainsPoint(lineSegment.Start) ||
                o4 == 0 && ContainsPoint(lineSegment.End);
    }

    public bool ContainsPoint(Point point)
        => point.X <= Math.Max(Start.X, End.X) && point.X >= Math.Min(Start.X, End.X) &&
           point.Y <= Math.Max(Start.Y, End.Y) && point.Y >= Math.Min(Start.Y, End.Y);

    public static explicit operator Vector(LineSegment2D lineSegment) => lineSegment.End - lineSegment.Start;
}