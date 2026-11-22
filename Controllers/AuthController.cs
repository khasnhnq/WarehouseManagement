using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementAPI.Data;
using WarehouseManagementAPI.DTOs;
using WarehouseManagementAPI.Models;
using WarehouseManagementAPI.Services;

namespace WarehouseManagementAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    AppDbContext db,
    IPasswordHasher<AppUser> hasher,
    TokenService tokenService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var input = request.UsernameOrEmail.Trim();
        var user = await db.Users.FirstOrDefaultAsync(x => x.Username == input || x.Email == input);
        if (user is null || !user.IsActive)
            return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác." });

        var verification = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác." });

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
            user.PasswordHash = hasher.HashPassword(user, request.Password);

        var token = tokenService.CreateToken(user);
        await db.SaveChangesAsync();
        return Ok(new AuthResponse(token.Token, token.ExpiresAtUtc,
            new UserResponse(user.Id, user.Username, user.Email, user.Role)));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> Me()
    {
        var idText = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idText, out var id)) return Unauthorized();
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (user is null) return NotFound();
        return Ok(new UserResponse(user.Id, user.Username, user.Email, user.Role));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("staff")]
    public async Task<ActionResult<UserResponse>> CreateStaff(CreateStaffRequest request)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim();
        if (await db.Users.AnyAsync(x => x.Username == username || x.Email == email))
            return Conflict(new { message = "Username hoặc email đã tồn tại." });

        var user = new AppUser { Username = username, Email = email, Role = AppRoles.Staff };
        user.PasswordHash = hasher.HashPassword(user, request.Password);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Ok(new UserResponse(user.Id, user.Username, user.Email, user.Role));
    }
}
