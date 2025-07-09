using SmartTourismAPI.Domain.Entities;
using SmartTourismAPI.Infrastructure.DataContext;
using SmartTourismAPI.Application.Abstractions.Repositories;

namespace SmartTourismAPI.Infrastructure.Repositories;

public class ReviewRepository(AppDbContext context) : IReviewRepository {
    private readonly AppDbContext _context = context;

    public async Task AddReviewAsync(TPlaceReview review) {
        _context.TPlaceReviews.Add(review);
        await _context.SaveChangesAsync();
    }
}