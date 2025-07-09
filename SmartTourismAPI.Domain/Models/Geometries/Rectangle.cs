namespace SmartTourismAPI.Domain.Models.Geometries;

public class Rectangle {
    public Point UpperLeft { get; }
    public double Width { get; }
    public double Height { get; }

    public double Left => UpperLeft.X;
    public double Right => UpperLeft.X + Width;
    public double Top => UpperLeft.Y;
    public double Bottom => UpperLeft.Y + Height;

    public Rectangle(Point upperLeft, double width, double height) {
        UpperLeft = upperLeft;
        Width = width;
        Height = height;
    }

    public bool Contains(Point p) {
        return Left <= p.X && p.X < Right &&
            Top <= p.Y && p.Y < Bottom;
    }
}
