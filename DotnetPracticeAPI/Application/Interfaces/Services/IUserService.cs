using Application.DTOs;

namespace Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync(bool fromCache = true);
        Task<UserDto?> GetByIdAsync(Guid id, bool fromCache = true);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task DeleteAsync(Guid id);
    }
}
