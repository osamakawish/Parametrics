using System;

namespace Parametrics;

public interface IDoubleComparer<T>
{
    public double Epsilon { get; internal set; }
    public double Compare(T x, T y);

    public bool Equals(T x, T y)
    {
        return Math.Abs(Compare(x, y)) <= Epsilon;
    }
}
