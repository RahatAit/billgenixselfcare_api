using billgenixselfcare_api.Application.DTOs;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace billgenixselfcare_api.Application.Features.Users
{
    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public string Id { get; set; }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.Id);
                if (user == null)
                {
                    return Result<UserDto>.FailureResult("Not found");
                }
                var userRole = await _userManager.GetRolesAsync(user);
                var role = await _roleManager.FindByNameAsync(userRole.FirstOrDefault());

                var dto = new UserDto()
                {
                    Id = request.Id,
                    Name = user.Name,
                    DOB = user.DOB,
                    Gender = user.Gender,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Address = user.Address,
                    Image = user.Image,
                    UserName = user.UserName,
                    RoleId = role?.Id,
                    IsActive = user.LockoutEnd >= DateTime.UtcNow ? false : true,
                    CreatedAt = user.CreatedAt
                };
                return Result<UserDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return Result<UserDto>.FailureResult("Failed to retrieve", new List<string> { ex.Message });
            }
        }
    }
}
