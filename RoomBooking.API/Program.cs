using FluentValidation;
using RoomBooking.API.FailureHandlers;
using RoomBooking.API.Middlewares.ExceptionHandlers;
using RoomBooking.Application.DTOs.AddressInfo;
using RoomBooking.Application.Services;
using RoomBooking.Application.Validations.Abstractions.Bookings;
using RoomBooking.Application.Validations.Abstractions.Rooms;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Application.Validations.Abstractions.Validators;
using RoomBooking.Application.Validations.Converters;
using RoomBooking.Application.Validations.Validators.Bookings;
using RoomBooking.Application.Validations.Validators.Rooms;
using RoomBooking.Application.Validations.Validators.Users;
using RoomBooking.Application.Validations.Validators.Users.AddressInfo;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models;
using RoomBooking.Core.Models.User;
using RoomBooking.DataAccess;
using RoomBooking.DataAccess.DbContext;
using RoomBooking.DataAccess.Migrations;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);

builder.Host.UseSerilog((context, services, configuration)
    => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services) 
        .Enrich.FromLogContext());

builder.Services.AddProblemDetails(configure =>
{
    configure.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Connecting Db
builder.Services.AddPersistence(builder.Configuration);

//Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IRoomService, RoomService>();

//Custom validators
builder.Services.AddScoped<IRoomGettingValidator, RoomGettingValidator>();
builder.Services.AddScoped<IUserGettingValidator,  UserGettingValidator>();
builder.Services.AddScoped<IValidationToErrorConverter, ValidationToErrorConverter>();

//Fluent Validators
builder.Services.AddValidatorsFromAssemblyContaining<BookingValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<UserCreationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserUpdateValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddressInfoAddingDtoValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<RoomCreationValidator>();

builder.Services.AddScoped<IFailureHandler, FailureHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

//Db initializing
using (var scope = app.Services.CreateScope())
{
    //Getting service provider, which is used for dependencies solving 
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context = serviceProvider.GetRequiredService<RoomBookingDbContext>();
        await DbInitializer.InitializeAsync(context);
    }
    catch (Exception exception)
    {
        logger.LogError(exception, exception.Message);
        throw;
    }
    //scope.Dispose(); should've used if not using 'using'
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (httpContext, elapsed, ex) =>
        ex != null ? LogEventLevel.Error :
        httpContext.Response.StatusCode > 499 ? LogEventLevel.Error :
        LogEventLevel.Information;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

logger.LogInformation("---------------------------RoomBooking API started---------------------------");
app.Run();
