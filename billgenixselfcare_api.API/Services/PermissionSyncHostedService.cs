using billgenixselfcare_api.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace billgenixselfcare_api.API.Services
{
    public class PermissionSyncHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public PermissionSyncHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
            await permissionService.SyncPermissionsFromControllersAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
