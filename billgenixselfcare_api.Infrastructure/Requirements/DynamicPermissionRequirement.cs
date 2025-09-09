using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace billgenixselfcare_api.Infrastructure.Requirements
{
    // Dynamic Permission Requirement (uses route data)
    public class DynamicPermissionRequirement : IAuthorizationRequirement
    {
    }

    // Dynamic Permission Handler
    public class DynamicPermissionHandler : AuthorizationHandler<DynamicPermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicPermissionRequirement requirement)
        {
            if (context.Resource is HttpContext httpContext)
            {
                var routeData = httpContext.GetRouteData();
                var controllerName = routeData?.Values["controller"]?.ToString();
                var actionName = routeData?.Values["action"]?.ToString();

                if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName))
                {
                    var requiredPermission = $"{controllerName}.{actionName}";

                    if (context.User.HasClaim("Permission", requiredPermission))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
