using Microsoft.AspNetCore.Http;
using SmartTourismAPI.Application.Abstractions.Providers;
using SmartTourismAPI.Application.Abstractions.Repositories;
using SmartTourismAPI.Application.Abstractions.Services;
using SmartTourismAPI.Application.DTOs;
using SmartTourismAPI.Domain.Entities;

namespace SmartTourismAPI.Application.Services;

public class AuthService(IJwtProvider jwtProvider, IUserRepository userRepository) : IAuthService {
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<LoginDto> Login(string username, string password) {
        var user = await _userRepository.GetUserAsync(username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) {
            throw new BadHttpRequestException("Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        var token = _jwtProvider.GenerateToken(user);
        return new LoginDto(user, token);
    }

    public async Task Register(string username, string password, string email) {
        var user = await _userRepository.GetUserAsync(username);
        if (user is not null)
            throw new BadHttpRequestException("Người dùng đã tồn tại.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var newUser = new MUser {
            Username = username,
            PasswordHash = passwordHash,
            Email = email,
            AvatarId = 1,
        };
        await _userRepository.AddUserAsync(newUser);
    }
}
