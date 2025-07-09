using SmartTourismAPI.Application.DTOs;

namespace SmartTourismAPI.Application.Abstractions.Services;

public interface IUserService {
    Task<UserDto> GetUserAsync(string username);
}
