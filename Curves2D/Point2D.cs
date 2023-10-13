using System;
using System.Windows;

namespace Curves2D;

public delegate Point2D Function2D(double t);

public record Point2D(double X, double Y)
{
    public static Point2D Zero { get; } = new(0, 0);

    public static Comparison<Point2D> XComparison { get; } = (p, q) => p.X.CompareTo(q.X);
    public static Comparison<Point2D> YComparison { get; } = (p, q) => p.Y.CompareTo(q.Y);

    public static Point2D operator -(Point2D p) => new(-p.X, -p.Y);

    public static Point2D operator +(Point2D p, Point2D q) => new(p.X + q.X, p.Y + q.Y);

    public static Point2D operator -(Point2D p, Point2D q) => new(p.X - q.X, p.Y - q.Y);

    public static Point2D operator *(Point2D p, double s) => new(p.X * s, p.Y * s);

    public static Point2D operator *(double s, Point2D p) => p * s;

    public static Point2D operator *(Point2D p, Point2D q) => new(p.X * q.X, p.Y * q.Y);

    public static Point2D operator /(Point2D p, double s) => new(p.X / s, p.Y / s);

    public static Point2D operator /(double s, Point2D p) => new(s / p.X, s / p.Y);

    public static Point2D operator /(Point2D p, Point2D q) => new(p.X / q.X, p.Y / q.Y);

    public double Dot(Point2D other) => X * other.X + Y * other.Y;

    public double PerpDot(Point2D other) => X * other.Y - Y * other.X;

    public double LengthSquared => Dot(this);

    public double Length => Math.Sqrt(LengthSquared);

    public Point2D Normalized() => this / Length;

    public Point2D Perpendicular() => new(-Y, X);

    public Point2D Projected(Point2D other) => other * (Dot(other) / other.LengthSquared);

    public Point2D Rotated(double angle) => new(X * Math.Cos(angle) - Y * Math.Sin(angle), X * Math.Sin(angle) + Y * Math.Cos(angle));

    public Point2D Rotated(Point2D pivot, double angle) => (this - pivot).Rotated(angle) + pivot;

    public Point2D Reflected(Point2D pivot) => 2 * pivot - this;

    public Point2D Reflected(Point2D pivot, Point2D normal) => this + 2 * (pivot - this).Projected(normal);

    public Function2D LinearInterpolated(Point2D other) => t => this + (other - this) * t;

    public static explicit operator Size(Point2D p) => new(p.X, p.Y);

    public static implicit operator Vector(Point2D p) => new(p.X, p.Y);

    public static implicit operator Point(Point2D p) => new(p.X, p.Y);

    public static implicit operator Point2D(Point p) => new(p.X, p.Y);

    public static implicit operator (double X, double Y)(Point2D p) => (p.X, p.Y);

    public static implicit operator Point2D((double X, double Y) p) => new(p.X, p.Y);
}