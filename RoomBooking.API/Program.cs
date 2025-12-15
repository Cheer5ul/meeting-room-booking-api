using Microsoft.EntityFrameworkCore;
using RoomBooking.API.Middlewares;
using RoomBooking.Application.Services;
using RoomBooking.Application.Validations.Abstractions.Bookings;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Application.Validations.Validators.Bookings;
using RoomBooking.Application.Validations.Validators.Users;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.DataAccess.DbContext;
using RoomBooking.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

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
builder.Services.AddDbContext<RoomBookingDbContext>(
    options =>
    {
        options.UseNpgsql(configuration.GetConnectionString(nameof(RoomBookingDbContext)));
    });

//DI
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();


//Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserDeletionValidator,  UserDeletionValidator>();
builder.Services.AddScoped<IBookingCreationValidator, BookingCreationValidator>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddScoped<ILogger, Logger<GlobalExceptionHandler>>();
builder.Services.AddLogging();

// builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler();

// app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();

app.Run();
app.Logger.LogInformation("------------------RoomBooking API has been started-------------------");