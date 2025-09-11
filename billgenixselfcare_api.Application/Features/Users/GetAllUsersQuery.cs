using billgenixselfcare_api.Application.DTOs.User;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace billgenixselfcare_api.Application.Features.Users
{
    public class GetAllUsersQuery : IRequest<Result<List<UserDto>>>
    {
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<UserDto>>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userManager.Users.ToListAsync(cancellationToken);
                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        DOB = user.DOB,
                        Gender = user.Gender,
                        Address = user.Address,
                        IsActive = user.IsActive,
                        Email = user.Email ?? "",
                        CreatedAt = user.CreatedAt,
                        Roles = roles.ToList()
                    });
                }

                return Result<List<UserDto>>.SuccessResult(userDtos);
            }
            catch (Exception ex)
            {
                return Result<List<UserDto>>.FailureResult("Failed to retrieve users", new List<string> { ex.Message });
            }
        }
    }
}
