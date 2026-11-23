//using demoAPI.Data;
//using demoAPI.Models;

//namespace demoAPI.Repositories
//{
//    public class UserRepository : IUserRepository
//    {
//        private readonly AppDbContext _context;

//        public UserRepository(AppDbContext context)
//        {
//            _context = context;
//        }

//        public Users? GetByUsername(string username)
//        {
//            return _context.Users
//                .FirstOrDefault(u => u.Username == username);
//        }

//        public Users? GetById(int id)
//        {
//            return _context.Users
//                .FirstOrDefault(u => u.Id == id);
//        }

//        public Users Create(Users user)
//        {
//            _context.Users.Add(user);
//            _context.SaveChanges();

//            return user;
//        }
//    }
//}




using demoAPI.Data;
using demoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace demoAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        // GET ALL
        public async Task<demoAPI.DTOs.Common.PagedResult<Users>> GetAllAsync(
    demoAPI.DTOs.Common.UserQueryParameters queryParams)
{
    var query = _db.Users.AsNoTracking().AsQueryable();

    // SEARCH
    if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
    {
        var term = queryParams.SearchTerm.Trim().ToLower();

        query = query.Where(u =>
            u.Username.ToLower().Contains(term) ||
            u.Email.ToLower().Contains(term));
    }

    // FILTER
    if (!string.IsNullOrWhiteSpace(queryParams.Role))
    {
        query = query.Where(u => u.Role == queryParams.Role);
    }

    // SORTING
    query = queryParams.SortBy?.ToLower() switch
    {
        "email" => queryParams.IsDescending
            ? query.OrderByDescending(u => u.Email)
            : query.OrderBy(u => u.Email),

        "createdat" => queryParams.IsDescending
            ? query.OrderByDescending(u => u.CreatedAt)
            : query.OrderBy(u => u.CreatedAt),

        _ => queryParams.IsDescending
            ? query.OrderByDescending(u => u.Username)
            : query.OrderBy(u => u.Username)
    };

    // COUNT
    var totalCount = await query.CountAsync();

    // PAGINATION
    var pageNumber = Math.Max(1, queryParams.PageNumber);
    var pageSize = Math.Clamp(queryParams.PageSize, 1, 100000);

    var items = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new demoAPI.DTOs.Common.PagedResult<Users>(
        items, totalCount, pageNumber, pageSize);
}

        // GET BY ID
        public async Task<Users?> GetByIdAsync(int id)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // GET BY USERNAME
        public async Task<Users?> GetByUsernameAsync(
            string username)
        {
            return await _db.Users
                .FirstOrDefaultAsync(
                    u => u.Username == username);
        }

        // GET BY USERNAME OR EMAIL
        public async Task<Users?> GetByUsernameOrEmailAsync(
            string username,
            string email)
        {
            return await _db.Users
                .FirstOrDefaultAsync(
                    u => u.Username == username ||
                         u.Email == email);
        }

        // ADD USER
        public async Task AddAsync(Users user)
        {
            await _db.Users.AddAsync(user);
        }

        // UPDATE USER
        public void Update(Users user)
        {
            _db.Users.Update(user);
        }

        // DELETE USER
        public void Delete(Users user)
        {
            _db.Users.Remove(user);
        }

        // SAVE CHANGES
        public async Task<bool> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }
    }
}


