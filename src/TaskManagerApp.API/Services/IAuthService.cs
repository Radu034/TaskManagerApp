using TaskManagerApp.API.DTOs;
using TaskManagerApp.API.Models;

namespace TaskManagerApp.API.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(string email, string password);
    Task<AppUser?> RegisterAsync(RegisterDto dto);
    string HashPassword(string password);
    bool VerifyPassword(string hash, string password);
}
