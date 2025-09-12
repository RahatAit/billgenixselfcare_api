using billgenixselfcare_api.Application.DTOs;
using billgenixselfcare_api.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace billgenixselfcare_api.Application.Features.Roles
{
    public class GetAllRolesQuery : IRequest<Result<PaginatedList<RoleDto>>>
    {
        public string Search { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<PaginatedList<RoleDto>>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetAllRolesQueryHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result<PaginatedList<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _roleManager.Roles.AsQueryable();
                if (!string.IsNullOrWhiteSpace(request.Search)) { query = query.Where(r => r.Name.Contains(request.Search)); }

                var paginatedList = await PaginatedList<IdentityRole>.CreateAsync(query, request.PageNumber ?? 1, request.PageSize ?? 10);
                var dto = new List<RoleDto>();

                foreach (var item in paginatedList.Items)
                {
                    var claims = await _roleManager.GetClaimsAsync(item);
                    dto.Add(new RoleDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Permissions = claims.Select(r => r.Value).ToList()
                    });
                }
                var data = new PaginatedList<RoleDto>(dto, paginatedList.TotalCount, paginatedList.PageIndex, paginatedList.PageSize);
                return Result<PaginatedList<RoleDto>>.SuccessResult(data);
            }
            catch (Exception ex)
            {
                return Result<PaginatedList<RoleDto>>.FailureResult("Failed to retrieve", new List<string> { ex.Message });
            }
        }
    }
}
