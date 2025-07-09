using SmartTourismAPI.Application.Abstractions.Providers;
using SmartTourismAPI.Application.Abstractions.Repositories;
using SmartTourismAPI.Application.Abstractions.Services;
using SmartTourismAPI.Application.DTOs;
using SmartTourismAPI.Domain.Models.Geometries;

namespace SmartTourismAPI.Application.Services;

public class PlaceService(
    IGisOsmProvider gisOsmProvider,
    IPlaceDetailRepository poiDetailRepository,
    IPlaceTypeRepository placeTypeRepository) : IPlaceService {
    private readonly IGisOsmProvider _gisOsmProvider = gisOsmProvider;
    private readonly IPlaceDetailRepository _poiDetailRepository = poiDetailRepository;
    private readonly IPlaceTypeRepository _placeTypeRepository = placeTypeRepository;

    public async Task<List<PlaceDetailDto>> GetPlacesAsync(double centerLon, double centerLat, double radiusMeters, string? type) {
        var entries = _gisOsmProvider
            .GetGridFile()
            .RadiusQuery(new Circle(new Point(centerLon, centerLat), radiusMeters));

        if (entries.Count == 0) {
            return [];
        }

        var osmIds = entries.Select(x => x.OsmId).ToList();
        var poiDetails = await _poiDetailRepository.GetDataListAsync(osmIds, type);
        var poiDetailsDict = poiDetails
            .GroupBy(p => p.OsmId)
            .ToDictionary(g => g.Key, g => g.First());

        var result = new List<PlaceDetailDto>();
        foreach (var e in entries) {
            if (poiDetailsDict.TryGetValue(e.OsmId, out var poiDetail) && e.OsmId != "11634634900") {
                result.Add(new PlaceDetailDto(e, poiDetail));
            }
        }

        return result;
    }

    public async Task<List<string>> GetTypesAsync() {
        return await _placeTypeRepository.GetTypesAsync();
    }
}
