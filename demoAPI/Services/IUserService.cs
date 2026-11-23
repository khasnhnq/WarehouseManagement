//using demoAPI.DTOs;

//namespace demoAPI.Services
//{
//    public interface IUserService
//    {
//        string Register(RegisterDto registerDto);

//        string Login(LoginDto loginDto);
//    }
//}


using demoAPI.DTOs;

namespace demoAPI.Services
{
    public interface IUserService
    {
        Task<demoAPI.DTOs.Common.PagedResult<UsersDto>> GetAllAsync(demoAPI.DTOs.Common.UserQueryParameters queryParams);

        Task<UsersDto?> GetByIdAsync(int id);

        Task<(bool IsSuccess, string Message, UsersDto? Data)>
            RegisterAsync(RegisterDto request);

        Task<(bool IsSuccess, string Message)>
            UpdateAsync(int id, RegisterDto request);

        Task<(bool IsSuccess, string Message)>
            DeleteAsync(int id);

        Task<string?> LoginAsync(LoginDto request);
    }
}

