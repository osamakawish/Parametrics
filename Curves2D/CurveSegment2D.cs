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

    public void Add(Point item)
    {
        _pointsSortedByX.Insert(FindX(item), item);
        _pointsSortedByY.Insert(FindY(item), item);
        _points.Add(item);
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
        return removed;
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_points).GetEnumerator();
}
