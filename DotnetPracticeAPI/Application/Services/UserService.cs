using AutoMapper;
using Application.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Application.Interfaces.Services;
using FluentValidation;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateUserDto> _validator;
        private readonly ICacheService _cache;

        public UserService(IUnitOfWork uow, IMapper mapper, IValidator<CreateUserDto> validator, ICacheService cache)
        {
            _uow = uow;
            _mapper = mapper;
            _validator = validator;
            _cache = cache;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(bool fromCache = true)
        {
            const string cacheKey = "users:all";

            if (fromCache)
            {
                var cachedUsers = await _cache.GetAsync<IEnumerable<UserDto>>(cacheKey);

                if (cachedUsers != null) return cachedUsers;
            }


            var users = await _uow.Users.GetAllAsync();
            var result = _mapper.Map<IEnumerable<UserDto>>(users);

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }

        public async Task<UserDto?> GetByIdAsync(Guid id, bool fromCache = true)
        {
            string cacheKey = $"user:{id}";

            if (fromCache)
            {
                var cachedUser = await _cache.GetAsync<UserDto>(cacheKey);
                if (cachedUser != null) return cachedUser;
            }

            var user = await _uow.Users.GetByIdAsync(id);
            var result = _mapper.Map<UserDto>(user);

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));
            return result;
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var validationResult = _validator.Validate(dto);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

            var user = _mapper.Map<User>(dto);
            await _uow.Users.AddAsync(user);
            await _uow.SaveChangesAsync();

            await _cache.RemoveAsync("users:all");

            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user is null) return;

            _uow.Users.Delete(user);
            await _uow.SaveChangesAsync();

            await _cache.RemoveAsync($"user:{id}");
            await _cache.RemoveAsync("users:all");

        }
    }
}
