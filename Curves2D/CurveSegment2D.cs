using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace Curves2D;

// Use Point here over Point2D - structs are better for performance here, and I don't
// need the reference in this situation. Will need Point2D for the Curve2D class for
// reference nodes which are intended to be clickable.
public class CurveSegment2D : IReadOnlyList<Point>, ICollection<Point>
{
    public Rect Boundary { get; private set; } = Rect.Empty;
    public Point this[int index] => _points[index];

    public int Count => ((IReadOnlyCollection<Point>)_points).Count;

    public bool IsReadOnly => false;

    private readonly List<Point> _pointsSortedByX = new();
    private readonly List<Point> _pointsSortedByY = new();
    private readonly List<Point> _points = new();

    private static int Compare(Point point, List<Point> points, Comparison<Point> comparison)
    {
        int i = points.BinarySearch(point, Comparer<Point>.Create(comparison));
        return i < 0 ? ~i : i;
    }

    private int FindX(Point point) => Compare(point, _pointsSortedByX, (p, q) => p.X.CompareTo(q.X));
    private int FindY(Point point) => Compare(point, _pointsSortedByY, (p, q) => p.Y.CompareTo(q.Y));

    private void UpdateBoundaryOnAddPoint(Point point)
    {
        if (Boundary.IsEmpty)
            Boundary = new Rect(point, new Size(0, 0));
        else if (point.X < Boundary.Left || point.Y < Boundary.Top)
            Boundary = new Rect(point, new Point(Boundary.Right, Boundary.Bottom));
        else if (point.X > Boundary.Right || point.Y > Boundary.Bottom)
            Boundary = new Rect(new Point(Boundary.Left, Boundary.Top), point);
    }

    public void Add(Point point)
    {
        UpdateBoundaryAndSortPoint(point);
        _points.Add(point);
    }

    public void Append(Point point) => Add(point);

    public void Prepend(Point point) => Insert(0, point);

    public void Insert(int index, Point point)
    {
        UpdateBoundaryAndSortPoint(point);
        _points.Insert(index, point);
    }

    private void UpdateBoundaryAndSortPoint(Point point)
    {
        _pointsSortedByX.Insert(FindX(point), point);
        _pointsSortedByY.Insert(FindY(point), point);
        UpdateBoundaryOnAddPoint(point);
    }

    public void Clear() => _points.Clear();

    public bool Contains(Point item) => ((ICollection<Point>)_points).Contains(item);

    public void CopyTo(Point[] array, int arrayIndex) => ((ICollection<Point>)_points).CopyTo(array, arrayIndex);

    public IEnumerator<Point> GetEnumerator() => ((IEnumerable<Point>)_points).GetEnumerator();

    public bool Remove(Point item)
    {
        bool removed = ((ICollection<Point>)_points).Remove(item);
        if (!removed) return removed;

        _pointsSortedByX.RemoveAt(FindX(item));
        _pointsSortedByY.RemoveAt(FindY(item));
        UpdateBoundaryOnRemovePoint();

        return removed;
    }

    private void UpdateBoundaryOnRemovePoint()
    {
        if (_points.Count == 0) { Boundary = Rect.Empty; return; }
        
        double left = _pointsSortedByX[0].X;
        double right = _pointsSortedByX[^1].X;
        double top = _pointsSortedByY[0].Y;
        double bottom = _pointsSortedByY[^1].Y;
        Boundary = new Rect(new Point(left, top), new Point(right, bottom));
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_points).GetEnumerator();

    public static CurveSegment2D operator +(CurveSegment2D segment, Point point)
    {
        segment.Add(point);
        return segment;
    }

    public Rect IntersectingBoundary(Rect rect)
        => Boundary.IntersectsWith(rect) ? Rect.Intersect(rect, Boundary) : Rect.Empty;

    public Rect IntersectingBoundary(CurveSegment2D curveSegment)
        => IntersectingBoundary(curveSegment.Boundary);
}