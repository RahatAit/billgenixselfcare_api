using billgenixselfcare_api.Application.DTOs;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace billgenixselfcare_api.Application.Features.Users
{
    public class CreateUserCommand : IRequest<Result<UserDto>>
    {
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public Gender? Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; } = true;
        public string CreateBy { get; set; }
        public string RoleId { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateUserCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return Result<UserDto>.FailureResult($"Sorry! '{request.Email}' already exists.");
                }

                var user = new ApplicationUser
                {
                    Name = request.Name,
                    DOB = request.DOB,
                    Gender = request.Gender,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    Address = request.Address,
                    Image = request.Image,
                    UserName = request.UserName,
                    CreatedBy = request.CreateBy,
                    LockoutEnd = request.IsActive == false ? DateTime.UtcNow.AddYears(100) : null,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return Result<UserDto>.FailureResult("Creation failed", result.Errors.Select(e => e.Description).ToList());
                }

                // Add Role
                var role = await _roleManager.FindByIdAsync(request.RoleId);
                result = await _userManager.AddToRoleAsync(user, role.Name);

                var dto = new UserDto { Id = user.Id };
                return Result<UserDto>.SuccessResult(dto, "Created successfully");
            }
            catch (Exception ex)
            {
                return Result<UserDto>.FailureResult("Failed to create", new List<string> { ex.Message });
            }
        }
    }
}
