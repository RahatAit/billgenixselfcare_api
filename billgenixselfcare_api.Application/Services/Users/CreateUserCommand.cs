using billgenixselfcare_api.Application.DTOs.User;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace billgenixselfcare_api.Application.Services.Users
{
    public class CreateUserCommand : IRequest<Result<UserDto>>
    {
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public Gender? Gender { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserId { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return Result<UserDto>.FailureResult("Email already exists");
                }

                var user = new ApplicationUser
                {
                    Name = request.Name,
                    DOB = request.DOB,
                    Gender = request.Gender,
                    Address = request.Address,
                    CreatedBy = request.UserId,
                    Email = request.Email ?? "",
                    UserName = request.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return Result<UserDto>.FailureResult("User creation failed",
                        result.Errors.Select(e => e.Description).ToList());
                }

                // Add roles
                if (request.Roles.Any())
                {
                    await _userManager.AddToRolesAsync(user, request.Roles);
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    DOB = user.DOB,
                    Gender = user.Gender,
                    Address = user.Address,
                    Email = user.Email,
                    Roles = request.Roles
                };

                return Result<UserDto>.SuccessResult(userDto, "User created successfully");
            }
            catch (Exception ex)
            {
                return Result<UserDto>.FailureResult("Failed to create user", new List<string> { ex.Message });
            }
        }
    }
}
