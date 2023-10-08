using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Parametrics;

[Obsolete]
public class PointCollection : IEnumerable<Point>
{
    private IDoubleComparer<Point> Comparer { get; } // May want to implement multiple comparers
    private List<Point> Points { get; } = new();

    public PointCollection(IDoubleComparer<Point> comparer, IEnumerable<Point> points) { Comparer = comparer; AddRange(points); }
    public PointCollection(IDoubleComparer<Point> comparer, params Point[] points) : this(comparer, points.AsEnumerable()) { }
    public static PointCollection FromComparer<TComparer>(IEnumerable<Point> points) where TComparer : IDoubleComparer<Point>, new()
        => new(new TComparer(), points);
    public static PointCollection FromComparer<TComparer>(params Point[] points) where TComparer : IDoubleComparer<Point>, new()
        => new(new TComparer(), points);

    public int Count => Points.Count;
    public Point this[int index] => Points[index];
    public void Add(Point point) => Points.Insert(Find(point), point);

    public void AddRange(IEnumerable<Point> points) => points.ForEach(Add);

    public void RemoveAt(int index) => Points.RemoveAt(index);

    public void Remove(Point point) => Points.RemoveAt(Find(point));

    /// <summary>
    /// Finds the point closest to the given point.
    /// </summary>
    /// <param name="point">The given point.</param>
    /// <returns>The index of the closest point.</returns>
    public int Find(Point point) => Points.BinarySearch(point, Comparer);
    public Point FindPoint(Point point) => Points[Find(point)];
    public bool Contains(Point point) => Comparer.Equals(point, FindPoint(point));

    public bool IsEmpty => Count == 0;

    public void Clear() => Points.Clear();
    public IEnumerator<Point> GetEnumerator() => Points.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Points.GetEnumerator();

    public IReadOnlyList<Point> AsReadOnlyList => Points;

    public Rect IntersectionRange(PointCollection points)
    {
        var thisRect = new Rect(
            new Point(Points.Min(p => p.X), Points.Min(p => p.Y)),
            new Vector(Points.Max(p => p.X), Points.Max(p => p.Y)));

        var otherPoints = points.AsReadOnlyList;
        var otherRect = new Rect(
            new Point(otherPoints.Min(p => p.X), otherPoints.Min(p => p.Y)),
            new Vector(otherPoints.Max(p => p.X), otherPoints.Max(p => p.Y)));

        thisRect.Intersect(otherRect);
        return thisRect;
    }

    public List<Point> GetPointsInRect(Rect rect)
    {
        var points = new List<Point>();
        foreach (var point in Points)
        {
           
            if (rect.Contains(point))
                points.Add(point);
        }
        return points;
    }

    /// <summary>
    /// Organizes the points in two lists by order, given a comparer.
    /// </summary>
    /// <remarks><b>Remarks.</b> Used for finding points of intersection.</remarks>
    /// <param name="p"></param>
    /// <param name="q"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<(Point point, bool isInP)> SortPoints(List<Point> p, List<Point> q, IDoubleComparer<Point> comparer)
    {
        var points = new List<(Point point, bool isInP)>();

        int i = 0, j = 0;

        while (i < p.Count && j < q.Count)
        {
            var pPoint = p[i];
            var qPoint = q[j];
            var comparison = comparer.Compare(pPoint, qPoint);

            switch (comparison)
            {
                case 0:
                    points.Add((pPoint, true));
                    points.Add((qPoint, false));
                    i++;
                    j++;
                    break;
                case < 0:
                    points.Add((pPoint, true));
                    i++;
                    break;
                default:
                    points.Add((qPoint, false));
                    j++;
                    break;
            }
        }

        if (i < p.Count) for (; i < p.Count; i++) points.Add((p[i], true));
        else if (j < q.Count) for (; j < q.Count; j++) points.Add((q[j], false));

        return points;
    }
}
