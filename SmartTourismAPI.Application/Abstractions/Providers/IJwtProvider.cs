using SmartTourismAPI.Domain.Entities;

namespace SmartTourismAPI.Application.Abstractions.Providers;
public interface IJwtProvider {
    string GenerateToken(MUser user);
}
