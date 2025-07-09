using Microsoft.EntityFrameworkCore;
using SmartTourismAPI.Infrastructure.DataContext;
using SmartTourismAPI.Application.Abstractions.Repositories;

namespace SmartTourismAPI.Infrastructure.Repositories;

public class PlaceTypeRepository(AppDbContext context) : IPlaceTypeRepository {
    private readonly AppDbContext _context = context;

    public async Task<List<string>> GetTypesAsync() {
        return await _context.MPlaceTypes
            .GroupBy(pt => pt.TypeName)
            .Select(g => new {
                TypeName = g.Key,
                Count = g.SelectMany(pt => pt.MPlaceDetails).Count()
            })
            .OrderByDescending(x => x.Count)
            .Select(x => x.TypeName)
            .ToListAsync();
    }
}
