using Application.DTOs;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
        Task<ClaimsPrincipal> LoginHttpContextAsync(LoginDto dto);
    }
}
