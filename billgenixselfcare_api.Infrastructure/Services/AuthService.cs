using billgenixselfcare_api.Application.DTOs.Auth;
using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace billgenixselfcare_api.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Result<AuthResponseDto>.FailureResult("Invalid email or password");
            }

            //if (!user.IsActive)
            //{
            //    return Result<AuthResponseDto>.FailureResult("Account is deactivated");
            //}

            var token = await GenerateTokenAsync(user);
            var userDto = await MapToUserDto(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiresMinutes"])),
                User = userDto
            };

            return Result<AuthResponseDto>.SuccessResult(response, "Login successful");
        }

        public async Task<Result> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return Result.FailureResult("Email already exists");
            }

            var user = new ApplicationUser
            {
                Name = registerDto.Name,
                Address = registerDto.Address,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return Result.FailureResult("Registration failed", result.Errors.Select(e => e.Description).ToList());
            }

            // Add default role
            await _userManager.AddToRoleAsync(user, "User");

            return Result.SuccessResult("Registration successful");
        }

        public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string token)
        {
            // Implementation for refresh token logic
            await Task.CompletedTask;
            return Result<AuthResponseDto>.FailureResult("Not implemented");
        }

        private async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var tokenClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? ""),
                new(ClaimTypes.Email, user.Email ?? ""),
                new("name", user.Name)
            };

            tokenClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            tokenClaims.AddRange(claims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: tokenClaims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiresMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserDto> MapToUserDto(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email ?? "",
                Roles = roles.ToList(),
                Claims = claims.Select(c => $"{c.Type}:{c.Value}").ToList()
            };
        }
    }
}
