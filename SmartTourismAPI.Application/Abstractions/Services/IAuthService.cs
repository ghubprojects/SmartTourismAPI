using SmartTourismAPI.Application.DTOs;

namespace SmartTourismAPI.Application.Abstractions.Services;

public interface IAuthService {
    Task<LoginDto> Login(string username, string password);
    Task Register(string username, string password, string email);
}
