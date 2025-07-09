using SmartTourismAPI.Domain.Entities;

namespace SmartTourismAPI.Application.Abstractions.Repositories;

public interface IPlaceDetailRepository {
    Task<List<MPlaceDetail>> GetDataListAsync(List<string> osmIds, string? type);
    Task<MPlaceDetail?> GetDataAsync(string osmId);
    Task<bool> IsExistAsync(int detailId);
}