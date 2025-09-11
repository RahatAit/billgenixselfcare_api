using billgenixselfcare_api.Application.DTOs;
using billgenixselfcare_api.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace billgenixselfcare_api.Application.Features.Roles
{
    public class GetRoleByIdQuery : IRequest<Result<RoleDto>>
    {
        public string Id { get; set; }
    }

    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetRoleByIdQueryHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _roleManager.FindByIdAsync(request.Id);
                if (data == null)
                {
                    return Result<RoleDto>.FailureResult("Not found");
                }

                var claims = await _roleManager.GetClaimsAsync(data);
                var dto = new RoleDto()
                {
                    Id = request.Id,
                    Name = data.Name,
                    Permissions = claims.Select(r => r.Value).ToList()
                };
                return Result<RoleDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return Result<RoleDto>.FailureResult("Failed to retrieve", new List<string> { ex.Message });
            }
        }
    }
}
