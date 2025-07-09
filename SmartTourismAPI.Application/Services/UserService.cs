using Microsoft.AspNetCore.Http;
using SmartTourismAPI.Application.Abstractions.Repositories;
using SmartTourismAPI.Application.Abstractions.Services;
using SmartTourismAPI.Application.DTOs;

namespace SmartTourismAPI.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService {
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserDto> GetUserAsync(string username) {
        var user = await _userRepository.GetUserAsync(username)
            ?? throw new BadHttpRequestException("Người dùng không tồn tại.");

        return new UserDto(user);
    }
}
