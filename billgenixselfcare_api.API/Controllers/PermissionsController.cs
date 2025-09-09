using billgenixselfcare_api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace billgenixselfcare_api.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        //[RequireDynamicPermission]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            return Ok(permissions);
        }

        [HttpPost("sync")]
        //[RequireDynamicPermission]
        public async Task<IActionResult> SyncPermissions()
        {
            await _permissionService.SyncPermissionsFromControllersAsync();
            return Ok("Permissions synchronized successfully");
        }
    }
}
