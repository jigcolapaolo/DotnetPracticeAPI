using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Services;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence.UnitOfWork;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetPracticeAPI.Benchmark
{

    [MemoryDiagnoser]
    public class UserServiceBenchmark
    {
        private UserService _userService = default!;
        private CreateUserDto _dto = default!;
        private Mock<IValidator<CreateUserDto>> _validatorMock = default!;
        private Mock<ICacheService> _cacheMock = default!;

        [GlobalSetup]
        public void Setup()
        {
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Users.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _cacheMock = new Mock<ICacheService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateUserDto, User>();
                cfg.CreateMap<User, UserDto>();
            });

            _validatorMock = new Mock<IValidator<CreateUserDto>>();
            _validatorMock.Setup(v => v.Validate(It.IsAny<CreateUserDto>()))
                .Returns(new FluentValidation.Results.ValidationResult());

            _userService = new UserService(uowMock.Object, config.CreateMapper(), _validatorMock.Object, _cacheMock.Object);

            _dto = new CreateUserDto
            {
                UserName = "TestUser",
                Email = "test@example.com",
                PasswordHash = "TestPassword123-"
            };
        }

        [Benchmark]
        public async Task CreateUserBenchmark()
        {
            await _userService.CreateAsync(_dto);
        }
    }
}
