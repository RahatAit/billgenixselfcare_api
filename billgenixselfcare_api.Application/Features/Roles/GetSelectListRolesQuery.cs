using billgenixselfcare_api.Application.DTOs;
using billgenixselfcare_api.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace billgenixselfcare_api.Application.Features.Roles
{
    public class GetSelectListRolesQuery : IRequest<Result<List<RoleDto>>>
    {
    }

    public class GetSelectListRolesQueryHandler : IRequestHandler<GetSelectListRolesQuery, Result<List<RoleDto>>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetSelectListRolesQueryHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result<List<RoleDto>>> Handle(GetSelectListRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _roleManager.Roles.ToListAsync();
                var dto = new List<RoleDto>();

                foreach (var item in data)
                {
                    dto.Add(new RoleDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                    });
                }
                return Result<List<RoleDto>>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return Result<List<RoleDto>>.FailureResult("Failed to retrieve", new List<string> { ex.Message });
            }
        }
    }
}
