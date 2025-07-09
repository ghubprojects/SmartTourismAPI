using SmartTourismAPI.Application.DTOs;

namespace SmartTourismAPI.Application.Abstractions.Services;

public interface IPlaceService {
    Task<List<PlaceDetailDto>> GetPlacesAsync(double centerLon, double centerLat, double radiusMeters, string? type);
    Task<List<string>> GetTypesAsync();
}