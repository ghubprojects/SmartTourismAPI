using NetTopologySuite.IO.Esri;
using SmartTourismAPI.Application.Abstractions.Providers;
using SmartTourismAPI.Domain.Models;
using SmartTourismAPI.Domain.Models.Geometries;

namespace SmartTourismAPI.Infrastructure.Providers;

public class GisOsmProvider : IGisOsmProvider {
    private readonly GridFile _gridFile;
    private readonly int _pageCapacity = 20;

    public GisOsmProvider() {
        var _shpPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\shapefiles");
        var mbb = GetBoundingBox(Path.Combine(_shpPath, "hanoi.shp"));
        _gridFile = new GridFile(mbb.MinX, mbb.MaxX, mbb.MinY, mbb.MaxY, _pageCapacity);
        LoadPOIs(Path.Combine(_shpPath, "hanoi_pois.shp"));
        Console.WriteLine("GisOsmService: Dữ liệu POI đã được tải vào bộ nhớ.");
    }

    public GridFile GetGridFile() => _gridFile;

    private static NetTopologySuite.Geometries.Envelope GetBoundingBox(string filePath) {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Không tìm thấy file {filePath}");
        return Shapefile.ReadAllFeatures(filePath).First().BoundingBox;
    }

    private void LoadPOIs(string filePath) {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Không tìm thấy file {filePath}");

        foreach (var feature in Shapefile.ReadAllFeatures(filePath)) {
            var osmId = feature.Attributes["osm_id"].ToString() ?? throw new Exception("parse osm id failed!");
            var point = new Point(feature.Geometry.Coordinate.X, feature.Geometry.Coordinate.Y);
            _gridFile.Insert(osmId, point);
        }
    }
}