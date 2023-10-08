using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace Parametrics;

public delegate double Function(double t);

public record ParametricSegment(Function X, Function Y)
{
    private double _minT;
    private double _maxT;
    private ushort _steps;
    public required double MinT     { get => _minT;  set => _minT = value; }
    public required double MaxT     { get => _maxT;  set => _maxT = value; }
    public required ushort Steps    { get => _steps; set => _steps = value; }
    public          bool   IsClosed { get;           set; }
    internal PointCollection SortedPointsX { get; } = PointCollection.FromComparer<PointComparerX>();
    internal PointCollection SortedPointsY { get; } = PointCollection.FromComparer<PointComparerY>();

    internal double[] ValuesT => Enumerable.Range(0, Steps + 1)
        .Select(i => MinT + i * (MaxT - MinT) / Steps)
        .ToArray();

    internal double[] ValuesX => ValuesT.Select(t => X(t)).ToArray();
    internal double[] ValuesY => ValuesT.Select(t => Y(t)).ToArray();

    internal Point[] Points => ValuesT.Select(t => new Point(X(t), Y(t))).ToArray();

    internal (Point p1, Point p2)[] Segments => Points.Zip(Points.Skip(1), (p1, p2) => (p1, p2)).ToArray();

    public bool HitTest(Point p, double delta)
        => Segments.Any(s => SimpleGeometry.HitTest(s.p1, s.p2, p, delta))
                             || Points.Any(p1 => Math.Abs(p1.X - p.X) < delta && Math.Abs(p1.Y - p.Y) < delta);


    public List<Point> Intersect(ParametricSegment other, double delta)
    {
        var result = new List<Point>();

        var segments = Segments;
        var otherSegments = other.Segments;

        int i, j = 0;

        for (i = 0; i < segments.Length; i++) {
            var segment = segments[i];
            var otherSegment = otherSegments[j];

            if (SimpleGeometry.LineIntersects(segment.p1, segment.p2, otherSegment.p1, otherSegment.p2)) {
                // result.Add(p);

                // 
            }

            // 
        }

        return result;
    }
}