namespace SmartTourismAPI.Application.Abstractions.Repositories;

public interface IPlaceTypeRepository {
    Task<List<string>> GetTypesAsync();
}