﻿global using ReadOnlyListsOfPoints = System.Collections.Generic.IReadOnlyList<Curves2D.Increasing2dSegment>;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Curves2D;

delegate bool IntComparer(int a, int b);

/// <summary>
/// A partitioning of the points into monotone segments.
/// </summary>
/// <param name="Points"></param>
public record MonotonePartition(ReadOnlyListsOfPoints Points) : IEnumerable<Increasing2dSegment>
{
    // Generate based on indices of points in original curve later. For now, this will do.
    public static MonotonePartition GeneratePartition(List<Point> Points, Comparison<Point> Comparison)
    {
        var list = new List<Increasing2dSegment>();

        var currentList = new List<Point>();

        // TEST: May need to reverse this.
        var forward = Comparison;
        var reverse = new Comparison<Point>((a, b) => -Comparison(a, b));
        var currentComparer = forward;

        foreach (var point in Points)
        {
            if (currentList.Count <= 1 || currentComparer(point, currentList[^1]) > 0)
            {
                if (currentList.Count == 1)
                    currentComparer = Comparison(currentList[0], point) > 0 ? forward : reverse;

                currentList.Add(point);
                continue;
            }

            bool isForward = currentComparer == forward;
            if (!isForward) currentList.Reverse();

            var segment = new Increasing2dSegment(points: currentList, isForward: isForward);

            list.Add(segment);
            currentList.Clear();
        }

        return new(list);
    }

    public IEnumerator<Increasing2dSegment> GetEnumerator() => Points.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Points).GetEnumerator();

    // Alternative partitioning:
    // - boolean: startsForward - this is the direction of the first segment.
    // - List<int> indices - this is the list of indices that partition the curve.
}