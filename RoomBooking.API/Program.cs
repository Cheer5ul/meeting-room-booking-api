using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Services;
using RoomBooking.Application.Validations.Abstractions;
using RoomBooking.Application.Validations.Validators;
using RoomBooking.Core.Abstractions;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.DataAccess.DbContext;
using RoomBooking.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

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
builder.Services.AddScoped<IBookingCreationValidator, BookingCreationValidator>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();




var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();

app.Run();
