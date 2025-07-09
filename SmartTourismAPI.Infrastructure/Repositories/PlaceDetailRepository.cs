using LinqKit;
using Microsoft.EntityFrameworkCore;
using SmartTourismAPI.Application.Abstractions.Repositories;
using SmartTourismAPI.Domain.Entities;
using SmartTourismAPI.Infrastructure.DataContext;

namespace SmartTourismAPI.Infrastructure.Repositories;

public class PlaceDetailRepository(AppDbContext context) : IPlaceDetailRepository {
    private readonly AppDbContext _context = context;

    private IQueryable<MPlaceDetail> GetQueryAsync() {
        return _context.MPlaceDetails
             .Include(d => d.Type)
             .Include(d => d.TPlacePhotos)
             .ThenInclude(p => p.File)
             .Include(d => d.TPlaceReviews.OrderByDescending(r => r.LastModifiedDate))
             .ThenInclude(r => r.User)
             .ThenInclude(u => u.Avatar)
             .AsSplitQuery();
    }

    public async Task<List<MPlaceDetail>> GetDataListAsync(List<string> osmIds, string? type) {
        var expr = PredicateBuilder.New<MPlaceDetail>(p => osmIds.Contains(p.OsmId));

        if (!string.IsNullOrEmpty(type)) {
            expr = expr.And(p => p.Type.TypeName == type);
        }

        var dictResult = await GetQueryAsync()
            .Where(expr)
            .OrderByDescending(x => x.Name)
            .GroupBy(p => p.OsmId)
            .ToDictionaryAsync(g => g.Key, g => g.First());

        return [.. dictResult.Values];
    }

    public async Task<MPlaceDetail?> GetDataAsync(string osmId) {
        return await GetQueryAsync().FirstOrDefaultAsync(p => p.OsmId == osmId);
    }

    public async Task<bool> IsExistAsync(int detailId) {
        return await _context.MPlaceDetails.AnyAsync(p => p.DetailId == detailId);
    }
}

