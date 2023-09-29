using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace Parametrics;

public delegate double Function(double t);

public record ParametricFunc(Function X, Function Y)
{
    public required double MinT { get; set; }
    public required double MaxT { get; set; }
    public required ushort Steps { get; set; }
    public bool IsClosed { get; set; }

    internal double[] ValuesT => Enumerable.Range(0, Steps + 1)
        .Select(i => MinT + i * (MaxT - MinT) / Steps)
        .ToArray();

    internal double[] ValuesX => ValuesT.Select(t => X(t)).ToArray();
    internal double[] ValuesY => ValuesT.Select(t => Y(t)).ToArray();

    internal Point[] Points => ValuesT.Select(t => new Point(X(t), Y(t))).ToArray();

    internal (Point, Point)[] Segments => Points.Zip(Points.Skip(1), (p1, p2) => (p1, p2)).ToArray();

    //public bool HitTest(Point p, double delta) => Segments.Any(s => s.HitTest(p, delta)) || Points.Any(p1 => p1.HitTest(p, delta));
}