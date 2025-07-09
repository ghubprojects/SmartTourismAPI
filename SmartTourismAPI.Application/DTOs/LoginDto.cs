using SmartTourismAPI.Domain.Entities;

namespace SmartTourismAPI.Application.DTOs;

public class LoginDto {
    public UserDto User { get; set; }
    public string Token { get; set; }
    public LoginDto(MUser user, string token) {
        User = new UserDto(user);
        Token = token;
    }
}
