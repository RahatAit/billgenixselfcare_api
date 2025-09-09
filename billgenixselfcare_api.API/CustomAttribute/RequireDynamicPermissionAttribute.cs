using Microsoft.AspNetCore.Authorization;

namespace billgenixselfcare_api.API.CustomAttribute
{
    public class RequireDynamicPermissionAttribute : AuthorizeAttribute
    {
        public RequireDynamicPermissionAttribute()
        {
            Policy = "DynamicPermission";
        }
    }
}
