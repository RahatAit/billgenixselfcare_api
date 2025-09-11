using billgenixselfcare_api.Application.DTOs;
using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;

namespace billgenixselfcare_api.Application.Features.Roles
{
    public class UpdateRoleCommand : IRequest<Result<RoleDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Permissions { get; set; }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<RoleDto>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRepository<Permission> _repository;

        public UpdateRoleCommandHandler(RoleManager<IdentityRole> roleManager, IRepository<Permission> repository)
        {
            _roleManager = roleManager;
            _repository = repository;
        }

        public async Task<Result<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _roleManager.FindByIdAsync(request.Id);
                if (data == null)
                {
                    return Result<RoleDto>.FailureResult("Not found");
                }

                // Update properties
                data.Name = request.Name;

                var result = await _roleManager.UpdateAsync(data);
                if (!result.Succeeded)
                {
                    return Result<RoleDto>.FailureResult("Failed to update", result.Errors.Select(e => e.Description).ToList());
                }

                // Get current claims
                var currentClaims = await _roleManager.GetClaimsAsync(data);

                // Remove all existing claims
                foreach (var item in currentClaims)
                {
                    result = await _roleManager.RemoveClaimAsync(data, item);
                }

                // Add claims
                if (request.Permissions.Any())
                {
                    var claims = await _repository.GetAllAsync();
                    foreach (var item in request.Permissions)
                    {
                        var claim = claims.Where(r => r.Name == item).FirstOrDefault();
                        await _roleManager.AddClaimAsync(data, new Claim(claim.ControllerName, claim.Name));
                    }
                }

                var dto = new RoleDto
                {
                    Id = request.Id,
                    Name = request.Name,
                    Permissions = request.Permissions
                };
                return Result<RoleDto>.SuccessResult(dto, "Updated successfully");
            }
            catch (Exception ex)
            {
                return Result<RoleDto>.FailureResult("Failed to update", new List<string> { ex.Message });
            }
        }
    }
}
