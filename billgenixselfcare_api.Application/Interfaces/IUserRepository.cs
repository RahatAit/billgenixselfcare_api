using billgenixselfcare_api.Application.DTOs;
using billgenixselfcare_api.Domain.Common;

namespace billgenixselfcare_api.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<PaginatedList<UserDto>> GetAllAsync(string search, int pageNumber, int pageSize, string roleId, bool? isActive);
    }
}
