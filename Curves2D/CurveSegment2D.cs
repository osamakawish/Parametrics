using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private static int FindFromSortedPoints(Point point, List<Point> points, Comparison<Point> comparison)
    {
        int i = points.BinarySearch(point, Comparer<Point>.Create(comparison));
        return i < 0 ? ~i : i;
    }

    private int FindX(Point point) => FindFromSortedPoints(point, _pointsSortedByX, (p, q) => p.X.CompareTo(q.X));
    private int FindY(Point point) => FindFromSortedPoints(point, _pointsSortedByY, (p, q) => p.Y.CompareTo(q.Y));

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

    public bool Contains(Point point) => ((ICollection<Point>)_points).Contains(point);

    public void CopyTo(Point[] array, int arrayIndex) => ((ICollection<Point>)_points).CopyTo(array, arrayIndex);

    public IEnumerator<Point> GetEnumerator() => ((IEnumerable<Point>)_points).GetEnumerator();

    public bool Remove(Point point)
    {
        bool isValidRemoval = ((ICollection<Point>)_points).Remove(point);

        if (!isValidRemoval) return false;

        RemoveFromSortedLists(point);
        UpdateBoundaryOnRemovePoint();

        return true;
    }

    private void RemoveFromSortedLists(Point point)
    {
        _pointsSortedByX.RemoveAt(FindX(point));
        _pointsSortedByY.RemoveAt(FindY(point));
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

    public List<Point> PointsInBounds(Rect bounds)
    {
        List<Point> points = new();
        if (!Boundary.IntersectsWith(bounds)) return points;

        foreach (Point point in _points)
            if (bounds.Contains(point))
                points.Add(point);

        return points;
    }

    public List<Point> PointsInBounds(CurveSegment2D curveSegment)
        => PointsInBounds(curveSegment.Boundary);

    public List<Point> PointsInBoundsSortedByX(Rect bounds)
    {
        List<Point> points = new();
        if (!Boundary.IntersectsWith(bounds)) return points;

        foreach (Point point in _pointsSortedByX)
            if (bounds.Contains(point))
                points.Add(point);

        return points;
    }

    public List<Point> PointsInBoundsSortedByX(CurveSegment2D curveSegment)
        => PointsInBoundsSortedByX(curveSegment.Boundary);

    public List<Point> PointsInBoundsSortedByY(Rect bounds)
    {
        List<Point> points = new();
        if (!Boundary.IntersectsWith(bounds)) return points;

        foreach (Point point in _pointsSortedByY)
            if (bounds.Contains(point))
                points.Add(point);

        return points;
    }

    public List<Point> PointsInBoundsSortedByY(CurveSegment2D curveSegment)
        => PointsInBoundsSortedByY(curveSegment.Boundary);

    // Note: Use max distance between points in the curve segment as tolerance, not a default of 1e-6.
    // Then set original points to a list of map of points to potential points of intersection.
    // TODO: Missed points of intersection when the curve segments when sub-segments intersect but points aren't
    // within the tolerance of each other.
    public List<Point> GetPointsofIntersection(CurveSegment2D segment, double testTolerance, double finalTolerance = 1e-6)
    {
        List<PotentialPointOfIntersection> points = new();
        
        var pointsInThisSegment = PointsInBoundsSortedByX(segment);
        var pointsInOtherSegment = segment.PointsInBoundsSortedByX(this);

        int i = 0, j = 0;
        var pointInThisSegment = pointsInThisSegment[i];
        
        while (i < pointsInThisSegment.Count && j < pointsInOtherSegment.Count)
        {
            var pointInOtherSegment = pointsInOtherSegment[j];
            var currentPoiMap = new PotentialPointOfIntersection(pointInThisSegment, new HashSet<Point>());

            if (pointInThisSegment.IsEqualTo(pointInOtherSegment, testTolerance))
            {
                currentPoiMap.PotentialPoints.Add(pointInOtherSegment);
            }
            else
            {
                points.Add(currentPoiMap);
                pointInThisSegment = pointsInThisSegment[++i];
            }
            j++;
        }

        return points.Where(p => p.ContainsWithinTolerance(finalTolerance)).Select(p => p.Point).ToList();
    }

    // Alternative partitioning:
    // - boolean: startsForward - this is the direction of the first segment.
    // - List<int> indices - this is the list of indices that partition the curve.
}

public static class CurveExt
{
    public static bool IsEqualTo(this double x, double y, double tolerance = 1e-6)
        => Math.Abs(x - y) < tolerance;

    public static bool IsEqualTo(this Point p, Point q, double tolerance = 1e-6)
        => p.X.IsEqualTo(q.X, tolerance) && p.Y.IsEqualTo(q.Y, tolerance);
}

internal record PotentialPointOfIntersection(Point Point, HashSet<Point> PotentialPoints)
{
    internal bool ContainsWithinTolerance(double tolerance) => PotentialPoints.Any(x => x.IsEqualTo(Point, tolerance));
}