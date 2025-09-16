using billgenixselfcare_api.Application.DTOs;
using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Infrastructure.Data;

namespace billgenixselfcare_api.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<UserDto>> GetAllAsync(string search, int pageNumber, int pageSize, string roleId, bool? isActive)
        {
            var query = from user in _context.Users
                        join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                        join role in _context.Roles on userRoles.RoleId equals role.Id
                        select new UserDto
                        {
                            Id = user.Id,
                            Name = user.Name,
                            DOB = user.DOB,
                            Gender = user.Gender,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email,
                            Address = user.Address,
                            Image = user.Image,
                            UserName = user.UserName,
                            RoleId = role.Id,
                            RoleName = role.Name,
                            IsActive = user.LockoutEnd != null && user.LockoutEnd >= DateTime.UtcNow ? false : true,
                            CreatedAt = user.CreatedAt
                        };
            if (!string.IsNullOrWhiteSpace(search)) { query = query.Where(r => r.Name.Contains(search)); }
            if (!string.IsNullOrWhiteSpace(roleId)) { query = query.Where(r => r.RoleId == roleId); }
            if (isActive == true) { query = query.Where(r => r.LockoutEnd == null || r.LockoutEnd <= DateTime.UtcNow); }
            else if (isActive == false) { query = query.Where(r => r.LockoutEnd >= DateTime.UtcNow); }

            return await PaginatedList<UserDto>.CreateAsync(query, pageNumber, pageSize);
        }
    }
}
