using Project_tasks.DTO;

// This interface defines authentication-related services

namespace Project_tasks.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        string GenerateJwtToken(long userId, string email);
    }
}