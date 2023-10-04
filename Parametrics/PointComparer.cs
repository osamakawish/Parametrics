using System.Windows;

namespace Parametrics;

public class PointComparer : IDoubleComparer<Point>
{
    private double _epsilon = 0.0001 * 0.0001;

    public double Epsilon { get => _epsilon; set => _epsilon = value; }
    public double Compare(Point x, Point y) => (x - y).LengthSquared;
}

public class PointComparerX : IDoubleComparer<Point>
{
    private double _epsilon = 0.0001;

    public double Epsilon { get => _epsilon; set => _epsilon = value; }
    public double Compare(Point x, Point y) => x.X - y.X;
}

public class PointComparerY : IDoubleComparer<Point>
{
    private double _epsilon = 0.0001;

    public double Epsilon { get => _epsilon; set => _epsilon = value; }
    public double Compare(Point x, Point y) => x.Y - y.Y;
}