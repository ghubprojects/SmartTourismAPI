namespace SmartTourismAPI.Domain.Models.Geometries;

public class Point : IEquatable<Point?> {
    public double X { get; }
    public double Y { get; }

    public Point(double x, double y) {
        X = x;
        Y = y;
    }

    public override bool Equals(object? obj) {
        return Equals(obj as Point);
    }

    public bool Equals(Point? other) {
        return other is not null &&
               X == other.X &&
               Y == other.Y;
    }

    public override int GetHashCode() {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Point? left, Point? right) {
        return EqualityComparer<Point>.Default.Equals(left, right);
    }

    public static bool operator !=(Point? left, Point? right) {
        return !(left == right);
    }
}
