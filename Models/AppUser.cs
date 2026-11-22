namespace WarehouseManagementAPI.Models;

public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Staff";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Staff = "Staff";
    public const string AdminOrStaff = Admin + "," + Staff;
}
