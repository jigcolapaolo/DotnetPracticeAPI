using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Moq;
using NBomber.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetPracticeAPI.StressTests.Scenarios
{
    public class UserServiceStressTest
    {
        public static void Run(int userCount, int durationSec)
        {
            var (uowMock, mapper, validatorMock) = ConfigureMocks();

            var scenario = Scenario.Create("create_users_stres", async ctx =>
            {
                var service = new UserService(uowMock.Object, mapper, validatorMock.Object);

                var response = await service.CreateAsync(new CreateUserDto
                {
                    UserName = $"user_{ctx.InvocationNumber}",
                    Email = $"user_{ctx.InvocationNumber}@test.com",
                    PasswordHash = "Password123-"
                });

                return Response.Ok();
            })
            .WithLoadSimulations(
                Simulation.RampingConstant(copies: 50, during: TimeSpan.FromSeconds(30)), // Rampa inicial
                Simulation.KeepConstant(copies: 100, during: TimeSpan.FromSeconds(30)) // Carga sostenida
            );

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithReportFolder("stress_reports")
                .Run();
        }

        private static (Mock<IUnitOfWork>, IMapper, Mock<IValidator<CreateUserDto>>) ConfigureMocks()
        {
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Users.AddAsync(It.IsAny<User>()))
                   .Returns(async () =>
                   {
                       await Task.Delay(new Random().Next(1, 10));
                       return Task.CompletedTask;
                   });
            uowMock.Setup(u => u.SaveChangesAsync())
                   .ReturnsAsync(1);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateUserDto, User>();
                cfg.CreateMap<User, UserDto>();
            });

            Mock<IValidator<CreateUserDto>> validatorMock = new Mock<IValidator<CreateUserDto>>();
            validatorMock.Setup(v => v.Validate(It.IsAny<CreateUserDto>()))
                .Returns(new FluentValidation.Results.ValidationResult());


            return (uowMock, config.CreateMapper(), validatorMock);
            
        }
    }
}
