//using demoAPI.Models;

//namespace demoAPI.Repositories
//{
//    public interface IUserRepository
//    {
//        Users? GetByUsername(string username);

//        Users? GetById(int id);

//        Users Create(Users user);
//    }
//}


using demoAPI.Models;

namespace demoAPI.Repositories
{
    public interface IUserRepository
    {
        // Láº¥y danh sĂ¡ch táº¥t cáº£ User
        Task<demoAPI.DTOs.Common.PagedResult<Users>> GetAllAsync(demoAPI.DTOs.Common.UserQueryParameters queryParams);

        // TĂ¬m User theo ID
        Task<Users?> GetByIdAsync(int id);

        // TĂ¬m User theo Username
        Task<Users?> GetByUsernameAsync(string username);

        // TĂ¬m User theo Username hoáº·c Email
        Task<Users?> GetByUsernameOrEmailAsync(
            string username,
            string email
        );

        // ThĂªm User
        Task AddAsync(Users user);

        // Cáº­p nháº­t User
        void Update(Users user);

        // XĂ³a User
        void Delete(Users user);

        // LÆ°u thay Ä‘á»•i vĂ o database
        Task<bool> SaveChangesAsync();
    }
}


