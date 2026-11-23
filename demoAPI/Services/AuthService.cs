
using demoAPI.DTOs;
using demoAPI.Models;
using demoAPI.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace demoAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IAuthRepository authRepository,
            IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _authRepository.GetByUsernameAsync(dto.Username);

            if (user == null)
            {
                return null;
            }

            // Kiểm tra mật khẩu bằng PasswordSalt và PasswordHash đã lưu
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(dto.Password)
            );

            // Kiểm tra độ dài trước khi so sánh để tránh lỗi ngoài phạm vi
            if (computedHash.Length != user.PasswordHash.Length)
            {
                return null;
            }

            // So sánh hash mật khẩu
            if (!CryptographicOperations.FixedTimeEquals(
                    computedHash,
                    user.PasswordHash))
            {
                return null;
            }

            // Đăng nhập thành công, tạo JWT
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(Users user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var secretKeyText = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutesText = jwtSettings["ExpiryMinutes"];

            if (string.IsNullOrWhiteSpace(secretKeyText) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience) ||
                !double.TryParse(expiryMinutesText, out var expiryMinutes) ||
                expiryMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "Cấu hình JwtSettings trong appsettings.json không hợp lệ."
                );
            }

            var secretKey = Encoding.UTF8.GetBytes(secretKeyText);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(secretKey);

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
