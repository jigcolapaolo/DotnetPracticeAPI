using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Moq;

namespace DotnetPracticeAPI.Tests;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly IMapper _mapper;
    private readonly UserService _userService;
    private readonly Mock<IValidator<CreateUserDto>> _validatorMock;
    private readonly Mock<ICacheService> _cacheMock;

    public UserServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _validatorMock = new Mock<IValidator<CreateUserDto>>();
        _cacheMock = new Mock<ICacheService>();


        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserDto>();
            cfg.CreateMap<CreateUserDto, User>();
        });
        _mapper = config.CreateMapper();

        _validatorMock.Setup(v => v.Validate(It.IsAny<CreateUserDto>()))
                .Returns(new FluentValidation.Results.ValidationResult());

        _userService = new UserService(_uowMock.Object, _mapper, _validatorMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnUserDto()
    {
        var dto = new CreateUserDto
        {
            UserName = "testUser",
            Email = "test@example.com",
            PasswordHash = "PasswordHash123-"
        };

        _uowMock.Setup(u => u.Users.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _userService.CreateAsync(dto);

        Assert.Equal("testUser", result.UserName);
        Assert.Equal("test@example.com", result.Email);
    }
}
