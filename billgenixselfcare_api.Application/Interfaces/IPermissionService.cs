using billgenixselfcare_api.Domain.Entities;

namespace billgenixselfcare_api.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<List<Permission>> GetAllPermissionsAsync();
        Task SyncPermissionsFromControllersAsync();
    }
}
