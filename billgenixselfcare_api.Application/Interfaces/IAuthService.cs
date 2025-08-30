using billgenixselfcare_api.Application.DTOs.Auth;
using billgenixselfcare_api.Domain.Common;

namespace billgenixselfcare_api.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<Result> RegisterAsync(RegisterDto registerDto);
        Task<Result<AuthResponseDto>> RefreshTokenAsync(string token);
    }
}
