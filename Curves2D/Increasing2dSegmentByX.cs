using System.Collections.Generic;
using System.Windows;

namespace Curves2D;

public struct Increasing2dSegmentByX
{
    public Increasing2dSegmentByX(IReadOnlyList<Point> points, bool isForward = true)
        => (this.Points, this.IsForward) = (points, isForward);

    public bool IsForward { get; }
    public object Points { get; }
}