using billgenixselfcare_api.Application.DTOs;
using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace billgenixselfcare_api.Application.Features.Roles
{
    public class CreateRoleCommand : IRequest<Result<RoleDto>>
    {
        public string Name { get; set; }
        public List<string> Permissions { get; set; }
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRepository<Permission> _repository;

        public CreateRoleCommandHandler(RoleManager<IdentityRole> roleManager, IRepository<Permission> repository)
        {
            _roleManager = roleManager;
            _repository = repository;
        }

        public async Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var isExist = await _roleManager.FindByNameAsync(request.Name);
                if (isExist != null)
                {
                    return Result<RoleDto>.FailureResult($"Sorry! '{request.Name}' already exists.");
                }

                var model = new IdentityRole { Name = request.Name };

                var result = await _roleManager.CreateAsync(model);
                if (!result.Succeeded)
                {
                    return Result<RoleDto>.FailureResult("Creation failed", result.Errors.Select(e => e.Description).ToList());
                }

                // Add claims
                if (request.Permissions.Any())
                {
                    var claims = await _repository.GetAllAsync();
                    foreach (var item in request.Permissions)
                    {
                        var claim = claims.Where(r => r.Name == item).FirstOrDefault();
                        await _roleManager.AddClaimAsync(model, new Claim(claim.ControllerName, claim.Name));
                    }
                }

                var dto = new RoleDto
                {
                    Name = request.Name,
                    Permissions = request.Permissions
                };
                return Result<RoleDto>.SuccessResult(dto, "Created successfully");
            }
            catch (Exception ex)
            {
                return Result<RoleDto>.FailureResult("Failed to create", new List<string> { ex.Message });
            }
        }
    }
}
