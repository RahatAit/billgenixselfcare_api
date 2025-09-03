using billgenixselfcare_api.Application.DTOs.User;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace billgenixselfcare_api.Application.Services.Users
{
    public class UpdateUserCommand : IRequest<Result<UserDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public Gender? Gender { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.Id);
                if (user == null)
                {
                    return Result<UserDto>.FailureResult("Not found");
                }

                // Update user properties
                user.Name = request.Name;
                user.DOB = request.DOB;
                user.Gender = request.Gender;
                user.Address = request.Address;
                user.Email = request.Email;
                user.UserName = request.Email;
                user.IsActive = request.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return Result<UserDto>.FailureResult("Failed to update",
                        updateResult.Errors.Select(e => e.Description).ToList());
                }

                // Update roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToRemove = currentRoles.Except(request.Roles);
                var rolesToAdd = request.Roles.Except(currentRoles);

                if (rolesToRemove.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                }

                if (rolesToAdd.Any())
                {
                    await _userManager.AddToRolesAsync(user, rolesToAdd);
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    DOB = user.DOB,
                    Gender = user.Gender,
                    Address = user.Address,
                    Email = user.Email ?? "",
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = request.Roles
                };

                return Result<UserDto>.SuccessResult(userDto, "Updated successfully");
            }
            catch (Exception ex)
            {
                return Result<UserDto>.FailureResult("Failed to update", new List<string> { ex.Message });
            }
        }
    }
}
