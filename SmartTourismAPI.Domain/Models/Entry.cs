using SmartTourismAPI.Domain.Models.Geometries;

namespace SmartTourismAPI.Domain.Models;

public record Entry(int Oid, string OsmId, Point Point);