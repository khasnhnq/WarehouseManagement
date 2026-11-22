using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementAPI.DTOs;

public class LoginRequest
{
    [Required, MaxLength(120)] public string UsernameOrEmail { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public class CreateStaffRequest
{
    [Required, MaxLength(50)] public string Username { get; set; } = string.Empty;
    [Required, EmailAddress, MaxLength(120)] public string Email { get; set; } = string.Empty;
    [Required, MinLength(8)] public string Password { get; set; } = string.Empty;
}

public record AuthResponse(string Token, DateTime ExpiresAtUtc, UserResponse User);
public record UserResponse(int Id, string Username, string Email, string Role);
