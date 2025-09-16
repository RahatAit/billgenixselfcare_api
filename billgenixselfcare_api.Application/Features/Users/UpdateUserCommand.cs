using billgenixselfcare_api.Application.DTOs;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace billgenixselfcare_api.Application.Features.Users
{
    public class UpdateUserCommand : IRequest<Result<UserDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public Gender? Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
        public string RoleId { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
                user.PhoneNumber = request.PhoneNumber;
                user.Email = request.Email;
                user.Address = request.Address;
                user.Image = request.Image;
                user.UserName = request.UserName;
                user.LockoutEnd = request.IsActive == false ? DateTime.UtcNow.AddYears(100) : null;
                user.UpdatedBy = request.UpdatedBy;
                user.UpdatedAt = DateTime.UtcNow;
                user.PasswordHash = !string.IsNullOrEmpty(request.Password) ? _userManager.PasswordHasher.HashPassword(user, request.Password) : user.PasswordHash;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Result<UserDto>.FailureResult("Failed to update", result.Errors.Select(e => e.Description).ToList());
                }

                // Update role
                var role = await _roleManager.FindByIdAsync(request.RoleId);
                if (!await _userManager.IsInRoleAsync(user, role.Name))
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    result = await _userManager.RemoveFromRolesAsync(user, roles);
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                var dto = new UserDto { Id = user.Id };
                return Result<UserDto>.SuccessResult(dto, "Updated successfully");
            }
            catch (Exception ex)
            {
                return Result<UserDto>.FailureResult("Failed to update", new List<string> { ex.Message });
            }
        }
    }
}
