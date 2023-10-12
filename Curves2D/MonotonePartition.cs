global using ReadOnlyListsOfPoints = System.Collections.Generic.IReadOnlyList<System.Collections.Generic.IReadOnlyList<System.Windows.Point>>;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Curves2D;

delegate bool IntComparer(int a, int b);

public record MonotonePartition(ReadOnlyListsOfPoints Points) : IEnumerable<IReadOnlyList<Point>>
{
    public static MonotonePartition GeneratePartition(List<Point> Points, Comparison<Point> Comparison)
    {
        var list = new List<List<Point>>();

        var currentList = new List<Point>();

        // TEST: May need to reverse this.
        var forward = Comparison;
        var reverse = new Comparison<Point>((a, b) => -Comparison(a, b));
        var currentComparer = forward;

        foreach (var point in Points)
        {
            if (currentList.Count <= 1 || currentComparer(point, currentList[^1]) < 0)
            {
                if (currentList.Count == 1)
                    currentComparer = Comparison(currentList[0], point) < 0 ? forward : reverse;

                currentList.Add(point);
                continue;
            }

            if (currentComparer == reverse) currentList.Reverse();

            list.Add(currentList);
            currentList.Clear();
        }

        return new(list);
    }

    public IEnumerator<IReadOnlyList<Point>> GetEnumerator() => Points.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Points).GetEnumerator();
}