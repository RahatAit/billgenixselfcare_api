using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace billgenixselfcare_api.API.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IRepository<Permission> _permissionRepository;

        public PermissionService(IRepository<Permission> repository)
        {
            _permissionRepository = repository;
        }

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return (await _permissionRepository.GetAllAsync()).ToList();
        }

        public async Task SyncPermissionsFromControllersAsync()
        {
            var existingPermissions = await _permissionRepository.GetAllAsync();
            var controllerPermissions = GetPermissionsFromControllers();

            // Add new permissions
            foreach (var permission in controllerPermissions)
            {
                if (!existingPermissions.Any(p => p.Name == permission.Name))
                {
                    await _permissionRepository.AddAsync(permission);
                }
            }

            // Optionally mark missing permissions as inactive
            foreach (var existing in existingPermissions)
            {
                if (!controllerPermissions.Any(p => p.Name == existing.Name))
                {
                    existing.IsActive = false;
                    await _permissionRepository.UpdateAsync(existing);
                }
            }
        }

        private List<Permission> GetPermissionsFromControllers()
        {
            var permissions = new List<Permission>();
            var controllers = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
                .ToList();

            foreach (var controller in controllers)
            {
                var controllerName = controller.Name.Replace("Controller", "");

                var actions = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.IsPublic && !m.IsSpecialName && m.DeclaringType == controller)
                    .Where(m => m.GetCustomAttributes<HttpGetAttribute>().Any() ||
                               m.GetCustomAttributes<HttpPostAttribute>().Any() ||
                               m.GetCustomAttributes<HttpPutAttribute>().Any() ||
                               m.GetCustomAttributes<HttpDeleteAttribute>().Any())
                    .ToList();

                foreach (var action in actions)
                {
                    var actionName = action.Name;
                    var permissionName = $"{controllerName}.{actionName}";
                    var displayName = GenerateDisplayName(controllerName, actionName);

                    permissions.Add(new Permission
                    {
                        Name = permissionName,
                        DisplayName = displayName,
                        ControllerName = controllerName,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            return permissions;
        }

        private string GenerateDisplayName(string controller, string action)
        {
            // Convert camelCase to readable format
            var controllerReadable = string.Concat(controller.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).Trim();
            var actionReadable = string.Concat(action.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).Trim();

            return $"{actionReadable} {controllerReadable}";
        }
    }
}
