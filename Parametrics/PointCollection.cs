using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Parametrics;

public class PointCollection : IEnumerable<Point>
{
    private IDoubleComparer<Point> Comparer { get; }
    private List<Point> Points { get; } = new();

    public PointCollection(IDoubleComparer<Point> comparer, IEnumerable<Point> points) { Comparer = comparer; AddRange(points); }
    public PointCollection(IDoubleComparer<Point> comparer, params Point[] points) : this(comparer, points.AsEnumerable()) { }
    public static PointCollection FromComparer<TComparer>(TComparer comparer, IEnumerable<Point> points) where TComparer : IDoubleComparer<Point>
        => new(comparer, points);
    public static PointCollection FromComparer<TComparer>(TComparer comparer, params Point[] points) where TComparer : IDoubleComparer<Point>
        => new(comparer, points);

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

    public void Clear() => Points.Clear();
    public IEnumerator<Point> GetEnumerator() => Points.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Points.GetEnumerator();

    public IReadOnlyList<Point> AsReadOnlyList => Points;
}
