using System.Collections.Generic;
using System.Windows;

namespace Curves2D;

/// <summary>
/// The monotone increasing segment of a curve. Will convert to a method.
/// </summary>
public struct Increasing2dSegment
{
    public Increasing2dSegment(IReadOnlyList<Point> points, bool isForward = true)
        => (Points, IsForward) = (points, isForward);

    public bool IsForward { get; }
    public object Points { get; }
}

public record MonotonePartition_(CurveSegment2D Segment, IReadOnlyList<int> Partition, bool StartsIncreasing);