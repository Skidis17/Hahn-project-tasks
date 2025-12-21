using System.ComponentModel.DataAnnotations;

// This file contains DTOs related to Authentication

namespace Project_tasks.DTO
{
    // LoginRequest.cs
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;
    }

    // LoginResponse.cs
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}