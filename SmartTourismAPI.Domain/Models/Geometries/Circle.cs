namespace SmartTourismAPI.Domain.Models.Geometries;

public class Circle {
    public Point Center { get; }
    public double Radius { get; }

    public Circle(Point center, double radius) {
        Center = center;
        Radius = radius;
    }

    public bool Contains(Point point) {
        return Math.Pow(point.X - Center.X, 2) + Math.Pow(point.Y - Center.Y, 2) <= Math.Pow(Radius, 2);
    }

    public bool Contains3D(Point point) {
        const double EarthRadiusKm = 6371.0; // Bán kính Trái Đất (km)
        double radiusKm = Radius / 1000.0; // Chuyển bán kính sang km

        // Chuyển đổi độ sang radian
        double lat1 = ToRadians(Center.Y);
        double lat2 = ToRadians(point.Y);
        double dLat = lat2 - lat1;
        double dLon = ToRadians(point.X - Center.X);

        // Công thức Haversine
        double sinDLat = Math.Sin(dLat / 2);
        double sinDLon = Math.Sin(dLon / 2);
        double a = sinDLat * sinDLat + Math.Cos(lat1) * Math.Cos(lat2) * sinDLon * sinDLon;
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EarthRadiusKm * c;

        return distance <= radiusKm;
    }

    static double ToRadians(double degrees) {
        return degrees * (Math.PI / 180.0);
    }
}
