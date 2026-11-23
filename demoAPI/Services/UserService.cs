//using demoAPI.DTOs;
//using demoAPI.Models;
//using demoAPI.Repositories;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;

//namespace demoAPI.Services
//{
//    public class UserService : IUserService
//    {
//        private readonly IUserRepository _repository;
//        private readonly IConfiguration _configuration;

//        public UserService(
//            IUserRepository repository,
//            IConfiguration configuration)
//        {
//            _repository = repository;
//            _configuration = configuration;
//        }

//        // REGISTER
//        public string Register(RegisterDto registerDto)
//        {
//            var existingUser =
//                _repository.GetByUsername(registerDto.Username);

//            if (existingUser != null)
//                return "Username Ä‘Ă£ tá»“n táº¡i.";

//            // Táº¡o PasswordHash + PasswordSalt
//            CreatePasswordHash(
//                registerDto.Password,
//                out byte[] passwordHash,
//                out byte[] passwordSalt
//            );

//            var user = new Users
//            {
//                Username = registerDto.Username,
//                Email = registerDto.Email,
//                FullName = registerDto.FullName,

//                PasswordHash = passwordHash,
//                PasswordSalt = passwordSalt,

//                Role = string.IsNullOrEmpty(registerDto.Role)
//                    ? "User"
//                    : registerDto.Role,

//                CreatedAt = DateTime.UtcNow
//            };

//            _repository.Create(user);

//            return "ÄÄƒng kĂ½ thĂ nh cĂ´ng.";
//        }

//        // LOGIN
//        public string Login(LoginDto loginDto)
//        {
//            var user =
//                _repository.GetByUsername(loginDto.Username);

//            if (user == null)
//                return "Username hoáº·c password khĂ´ng Ä‘Ăºng.";

//            // Kiá»ƒm tra PasswordHash + PasswordSalt
//            bool passwordValid = VerifyPasswordHash(
//                loginDto.Password,
//                user.PasswordHash,
//                user.PasswordSalt
//            );

//            if (!passwordValid)
//                return "Username hoáº·c password khĂ´ng Ä‘Ăºng.";

//            // Táº¡o JWT
//            var claims = new[]
//            {
//                new Claim(
//                    ClaimTypes.NameIdentifier,
//                    user.Id.ToString()
//                ),

//                new Claim(
//                    ClaimTypes.Name,
//                    user.Username
//                ),

//                new Claim(
//                    ClaimTypes.Role,
//                    user.Role
//                )
//            };

//            var key = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(
//                    _configuration["Jwt:Key"]!
//                )
//            );

//            var credentials = new SigningCredentials(
//                key,
//                SecurityAlgorithms.HmacSha256
//            );

//            var token = new JwtSecurityToken(
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(2),
//                signingCredentials: credentials
//            );

//            return new JwtSecurityTokenHandler()
//                .WriteToken(token);
//        }

//        // Táº O PASSWORD HASH + SALT
//        private void CreatePasswordHash(
//            string password,
//            out byte[] passwordHash,
//            out byte[] passwordSalt)
//        {
//            using var hmac = new HMACSHA512();

//            passwordSalt = hmac.Key;

//            passwordHash = hmac.ComputeHash(
//                Encoding.UTF8.GetBytes(password)
//            );
//        }

//        // KIá»‚M TRA PASSWORD
//        private bool VerifyPasswordHash(
//            string password,
//            byte[] passwordHash,
//            byte[] passwordSalt)
//        {
//            using var hmac =
//                new HMACSHA512(passwordSalt);

//            var computedHash = hmac.ComputeHash(
//                Encoding.UTF8.GetBytes(password)
//            );

//            return computedHash.SequenceEqual(passwordHash);
//        }
//    }
//}

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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _configuration;

        public UserService(
            IUserRepository userRepo,
            IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
        }

        // GET ALL
        public async Task<demoAPI.DTOs.Common.PagedResult<UsersDto>> GetAllAsync(
    demoAPI.DTOs.Common.UserQueryParameters queryParams)
{
    var pagedUsers = await _userRepo.GetAllAsync(queryParams);

    var mappedItems = pagedUsers.Items
        .Select(MapToResponseDto)
        .ToList();

    return new demoAPI.DTOs.Common.PagedResult<UsersDto>(
        mappedItems,
        pagedUsers.TotalCount,
        pagedUsers.PageNumber,
        pagedUsers.PageSize);
}

        // GET BY ID
        public async Task<UsersDto?> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);

            return user == null
                ? null
                : MapToResponseDto(user);
        }

        // REGISTER
        public async Task<(bool IsSuccess, string Message, UsersDto? Data)>
            RegisterAsync(RegisterDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return (false,
                    "Username, Email vĂ  Password khĂ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng.",
                    null);
            }

            var existingUser =
                await _userRepo.GetByUsernameOrEmailAsync(
                    request.Username, request.Email);

            if (existingUser != null)
            {
                return (false,
                    "Username hoáº·c Email Ä‘Ă£ Ä‘Æ°á»£c sá»­ dá»¥ng.",
                    null);
            }

            CreatePasswordHash(
                request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            var newUser = new Users
            {
                Username = request.Username,
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = string.IsNullOrWhiteSpace(request.Role)
                    ? "User"
                    : request.Role.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(newUser);
            await _userRepo.SaveChangesAsync();

            return (true,
                "ÄÄƒng kĂ½ thĂ nh cĂ´ng.",
                MapToResponseDto(newUser));
        }

        // UPDATE
        public async Task<(bool IsSuccess, string Message)>
            UpdateAsync(int id, RegisterDto request)
        {
            var user = await _userRepo.GetByIdAsync(id);

            if (user == null)
            {
                return (false,
                    $"KhĂ´ng tĂ¬m tháº¥y ngÆ°á»i dĂ¹ng cĂ³ Id = {id}");
            }

            if (!string.IsNullOrWhiteSpace(request.Username) &&
                request.Username != user.Username)
            {
                var existing =
                    await _userRepo.GetByUsernameAsync(
                        request.Username);

                if (existing != null && existing.Id != id)
                    return (false, "Username Ä‘Ă£ tá»“n táº¡i.");

                user.Username = request.Username;
            }

            if (!string.IsNullOrWhiteSpace(request.Email) &&
                request.Email != user.Email)
            {
                var existing =
                    await _userRepo.GetByUsernameOrEmailAsync(
                        user.Username, request.Email);

                if (existing != null && existing.Id != id)
                    return (false, "Email Ä‘Ă£ tá»“n táº¡i.");

                user.Email = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;

            // KhĂ´ng cho phĂ©p thay Ä‘á»•i Role qua API nĂ y

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                CreatePasswordHash(
                    request.Password,
                    out byte[] passwordHash,
                    out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();

            return (true,
                $"Cáº­p nháº­t thĂ nh cĂ´ng ngÆ°á»i dĂ¹ng Id = {id}");
        }

        // DELETE
        public async Task<(bool IsSuccess, string Message)>
            DeleteAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);

            if (user == null)
            {
                return (false,
                    $"KhĂ´ng tĂ¬m tháº¥y ngÆ°á»i dĂ¹ng cĂ³ Id = {id}");
            }

            _userRepo.Delete(user);
            await _userRepo.SaveChangesAsync();

            return (true,
                $"ÄĂ£ xĂ³a ngÆ°á»i dĂ¹ng Id = {id}");
        }

        // LOGIN + JWT
        public async Task<string?> LoginAsync(LoginDto request)
        {
            var user = await _userRepo
                .GetByUsernameAsync(request.Username);

            if (user == null)
                return null;

            bool valid = VerifyPasswordHash(
                request.Password,
                user.PasswordHash,
                user.PasswordSalt);

            if (!valid)
                return null;

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        // MAP ENTITY -> DTO
        private static UsersDto MapToResponseDto(Users user)
        {
            return new UsersDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        // HASH PASSWORD
        private static void CreatePasswordHash(
            string password,
            out byte[] passwordHash,
            out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();

            passwordSalt = hmac.Key;

            passwordHash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(password));
        }

        // VERIFY PASSWORD
        private static bool VerifyPasswordHash(
            string password,
            byte[] passwordHash,
            byte[] passwordSalt)
        {
            if (passwordHash == null ||
                passwordSalt == null ||
                passwordHash.Length != 64 ||
                passwordSalt.Length != 128)
                return false;

            using var hmac = new HMACSHA512(passwordSalt);

            var computedHash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(password));

            return CryptographicOperations.FixedTimeEquals(
                computedHash, passwordHash);
        }
    }
}

