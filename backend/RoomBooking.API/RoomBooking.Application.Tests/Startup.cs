using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoomBooking.Application.DTOs.User;
using RoomBooking.Application.Interfaces.Auth.Hashers;
using RoomBooking.Application.Interfaces.Auth.Providers;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Application.Validations.Abstractions.Validators;
using RoomBooking.Application.Validations.Converters;
using RoomBooking.Application.Validations.Validators.Users;
using RoomBooking.Application.Validations.Validators.Users.AddressInfo;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Models.User;
using RoomBooking.Infrastructure.Hashers;
using RoomBooking.Infrastructure.Providers;
using Serilog.Core;

namespace RoomBooking.Application.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IUserRepository, IUserRepository>();
        //ILogger ???
        services.AddTransient<IUserGettingValidator, UserGettingValidator>();
        services.AddTransient<IUserEmailValidator, UserEmailValidator>();
        services.AddTransient<IValidator<User>, UserCreationValidator>();
        services.AddTransient<IValidator<UserUpdateDto>, UserUpdateValidator>();
        services.AddTransient<IValidator<AddressInfo>, AddressInfoAddingDtoValidator>();
        services.AddTransient<IValidationToErrorConverter, ValidationToErrorConverter>();
        services.AddTransient<IUserPasswordHasher, UserPasswordHasher>();
        services.AddTransient<IJwtProvider, JwtProvider>();

    }
}