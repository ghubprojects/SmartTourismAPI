using SmartTourismAPI.Domain.Entities;

namespace SmartTourismAPI.Application.Abstractions.Repositories;

public interface IReviewRepository {
    Task AddReviewAsync(TPlaceReview review);
}