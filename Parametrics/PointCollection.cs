using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Parametrics;

public class PointCollection : IEnumerable<Point>
{
    private IComparer<Point> Comparer { get; }
    private List<Point> Points { get; } = new();

    public PointCollection(IComparer<Point> comparer, IEnumerable<Point> points) { Comparer = comparer; AddRange(points); }
    public PointCollection(IComparer<Point> comparer, params Point[] points) : this(comparer, points.AsEnumerable()) { }
    public int Count => Points.Count;
    public Point this[int index]
    {
        get => Points[index];
        set => Points[index] = value;
    }
    public void Add(Point point) => Points.Add(point);
    public void AddRange(IEnumerable<Point> points) => Points.AddRange(points);
    public void Clear() => Points.Clear();
    public IEnumerator<Point> GetEnumerator() => Points.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Points.GetEnumerator();
}
