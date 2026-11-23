//using demoAPI.Data;
//using demoAPI.DTOs;
//using demoAPI.Models;
//using demoAPI.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace demoAPI.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class UserController : ControllerBase
//    {
//        private readonly IUserService _userService;
//        private readonly AppDbContext _db;

//        public UserController(
//            IUserService userService,
//            AppDbContext db)
//        {
//            _userService = userService;
//            _db = db;
//        }

//        // ------------------------------------------------------------------------
//        // CREATE / REGISTER (POST)
//        // POST: api/User/register
//        // ------------------------------------------------------------------------
//        [HttpPost("register")]
//        public ActionResult<UsersDto> Register([FromBody] RegisterDto request)
//        {
//            // Kiá»ƒm tra Username hoáº·c Email Ä‘Ă£ tá»“n táº¡i chÆ°a
//            bool isExist = _db.Users.Any(u =>
//                u.Username == request.Username ||
//                u.Email == request.Email);

//            if (isExist)
//            {
//                return BadRequest(new
//                {
//                    message = "Username hoáº·c Email Ä‘Ă£ Ä‘Æ°á»£c sá»­ dá»¥ng."
//                });
//            }

//            // Táº¡o Password Hash + Salt
//            CreatePasswordHash(
//                request.Password,
//                out byte[] passwordHash,
//                out byte[] passwordSalt
//            );

//            // Táº¡o User má»›i
//            var newUser = new Users
//            {
//                Username = request.Username,
//                Email = request.Email,
//                PasswordHash = passwordHash,
//                PasswordSalt = passwordSalt,
//                FullName = request.FullName,
//                Role = string.IsNullOrEmpty(request.Role)
//                    ? "User"
//                    : request.Role,
//                CreatedAt = DateTime.UtcNow
//            };

//            // LÆ°u vĂ o database
//            _db.Users.Add(newUser);
//            _db.SaveChanges();

//            // Dá»¯ liá»‡u tráº£ vá»
//            var response = new UsersDto
//            {
//                Id = newUser.Id,
//                Username = newUser.Username,
//                Email = newUser.Email,
//                FullName = newUser.FullName,
//                Role = newUser.Role,
//                CreatedAt = newUser.CreatedAt
//            };

//            return CreatedAtAction(
//                nameof(GetById),
//                new { id = response.Id },
//                response
//            );
//        }

//        // ------------------------------------------------------------------------
//        // READ ALL (GET)
//        // GET: api/User
//        // ------------------------------------------------------------------------
//        [HttpGet]
//        public ActionResult<List<UsersDto>> GetAll()
//        {
//            var users = _db.Users
//                .Select(u => new UsersDto
//                {
//                    Id = u.Id,
//                    Username = u.Username,
//                    Email = u.Email,
//                    FullName = u.FullName,
//                    Role = u.Role,
//                    CreatedAt = u.CreatedAt
//                })
//                .ToList();

//            return Ok(users);
//        }

//        // ------------------------------------------------------------------------
//        // GET USER BY ID
//        // GET: api/User/{id}
//        // ------------------------------------------------------------------------
//        [HttpGet("{id:int}")]
//        public ActionResult<UsersDto> GetById(int id)
//        {
//            var user = _db.Users.FirstOrDefault(u => u.Id == id);

//            if (user == null)
//            {
//                return NotFound(new
//                {
//                    message = $"KhĂ´ng tĂ¬m tháº¥y User cĂ³ Id = {id}"
//                });
//            }

//            var response = new UsersDto
//            {
//                Id = user.Id,
//                Username = user.Username,
//                Email = user.Email,
//                FullName = user.FullName,
//                Role = user.Role,
//                CreatedAt = user.CreatedAt
//            };

//            return Ok(response);
//        }

//        // ------------------------------------------------------------------------
//        // UPDATE (PUT)
//        // PUT: api/User/{id}
//        // ------------------------------------------------------------------------
//        [HttpPut("{id:int}")]
//        public IActionResult Update(int id, [FromBody] RegisterDto request)
//        {
//            var user = _db.Users.FirstOrDefault(u => u.Id == id);

//            if (user == null)
//            {
//                return NotFound(new
//                {
//                    message = $"KhĂ´ng tĂ¬m tháº¥y ngÆ°á»i dĂ¹ng cĂ³ Id = {id}"
//                });
//            }

//            if (!string.IsNullOrEmpty(request.Email))
//                user.Email = request.Email;

//            if (!string.IsNullOrEmpty(request.FullName))
//                user.FullName = request.FullName;

//            if (!string.IsNullOrEmpty(request.Role))
//                user.Role = request.Role;

//            if (!string.IsNullOrEmpty(request.Password))
//            {
//                CreatePasswordHash(
//                    request.Password,
//                    out byte[] passwordHash,
//                    out byte[] passwordSalt
//                );

//                user.PasswordHash = passwordHash;
//                user.PasswordSalt = passwordSalt;
//            }

//            _db.SaveChanges();

//            return Ok(new
//            {
//                message = $"Cáº­p nháº­t thĂ nh cĂ´ng ngÆ°á»i dĂ¹ng Id = {id}"
//            });
//        }


//        // ------------------------------------------------------------------------
//        // DELETE
//        // DELETE: api/User/{id}
//        // ------------------------------------------------------------------------
//        [HttpDelete("{id:int}")]
//        public IActionResult Delete(int id)
//        {
//            var user = _db.Users.FirstOrDefault(u => u.Id == id);

//            if (user == null)
//            {
//                return NotFound(new
//                {
//                    message = $"KhĂ´ng tĂ¬m tháº¥y ngÆ°á»i dĂ¹ng cĂ³ Id = {id}"
//                });
//            }

//            _db.Users.Remove(user);
//            _db.SaveChanges();

//            return Ok(new
//            {
//                message = $"ÄĂ£ xĂ³a ngÆ°á»i dĂ¹ng Id = {id}"
//            });
//        }

//        // ------------------------------------------------------------------------
//        // LOGIN
//        // POST: api/User/login
//        // ------------------------------------------------------------------------
//        [HttpPost("login")]
//        public IActionResult Login([FromBody] LoginDto request)
//        {
//            var result = _userService.Login(request);

//            if (result == "Username hoáº·c password khĂ´ng Ä‘Ăºng.")
//            {
//                return BadRequest(new
//                {
//                    message = result
//                });
//            }

//            return Ok(new
//            {
//                message = "ÄÄƒng nháº­p thĂ nh cĂ´ng.",
//                token = result
//            });
//        }

//        // ------------------------------------------------------------------------
//        // CREATE PASSWORD HASH
//        // ------------------------------------------------------------------------
//        private void CreatePasswordHash(
//            string password,
//            out byte[] passwordHash,
//            out byte[] passwordSalt)
//        {
//            using var hmac =
//                new System.Security.Cryptography.HMACSHA512();

//            passwordSalt = hmac.Key;

//            passwordHash = hmac.ComputeHash(
//                System.Text.Encoding.UTF8.GetBytes(password)
//            );
//        }
//    }
//}


using demoAPI.DTOs;
using demoAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace demoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        // Inject IUserService thay vĂ¬ AppDbContext
        public UserController(
      IUserService userService,
      IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        // GET: api/User
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<demoAPI.DTOs.Common.PagedResult<UsersDto>>> GetAll(
    [FromQuery] demoAPI.DTOs.Common.UserQueryParameters queryParams)
{
    var result = await _userService.GetAllAsync(queryParams);
    return Ok(result);
}

        // GET: api/User/1
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsersDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = $"KhĂ´ng tĂ¬m tháº¥y ngÆ°á»i dĂ¹ng cĂ³ Id = {id}"
                });
            }

            return Ok(user);
        }

        // POST: api/User/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UsersDto>> Register(
            [FromBody] RegisterDto request)
        {
            var result = await _userService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    message = result.Message
                });
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.Id },
                result.Data
            );
        }

        // PUT: api/User/1
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] RegisterDto request)
        {
            var result = await _userService.UpdateAsync(id, request);

            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    message = result.Message
                });
            }

            return Ok(new
            {
                message = result.Message
            });
        }

        // DELETE: api/User/1
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(new
                {
                    message = result.Message
                });
            }

            return Ok(new
            {
                message = result.Message
            });
        }

        // POST: api/User/login

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var token = await _authService.LoginAsync(request);

            if (token == null)
            {
                return Unauthorized(new
                {
                    message = "TĂªn Ä‘Äƒng nháº­p hoáº·c máº­t kháº©u khĂ´ng chĂ­nh xĂ¡c."
                });
            }

            return Ok(new
            {
                message = "ÄÄƒng nháº­p thĂ nh cĂ´ng.",
                token = token
            });
        }
    }
    }

