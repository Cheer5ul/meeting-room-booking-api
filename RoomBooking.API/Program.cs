using RoomBooking.API.FailureHandlers;
using RoomBooking.API.Middlewares;
using RoomBooking.API.Middlewares.ExceptionHandlers;
using RoomBooking.Application.Services;
using RoomBooking.Application.Validations.Abstractions.Bookings;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Application.Validations.Validators.Bookings;
using RoomBooking.Application.Validations.Validators.Users;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.DataAccess;
using RoomBooking.DataAccess.DbContext;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(new JsonFormatter(),
        "serilog-file-.txt",
        rollingInterval: RollingInterval.Day)
    .MinimumLevel.Debug()
    .CreateLogger();

builder.Host.UseSerilog();

// builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

// builder.Services.AddProblemDetails(configure =>
// {
//     configure.CustomizeProblemDetails = context =>
//     {
//         context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
//     };
// });

// builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
// builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Connecting Db
builder.Services.AddPersistence(builder.Configuration);

//Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserDeletionValidator,  UserDeletionValidator>();
builder.Services.AddScoped<IBookingCreationValidator, BookingCreationValidator>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IRoomService, RoomService>();

try
{
    builder.Services.AddScoped<IFailureHandler, FailureHandler>();
}
catch (Exception ex)
{
    Console.WriteLine($"----------------Registration Error: {ex.Message}---------------");
}


builder.Services.AddSingleton(typeof(IServiceLogger<>), typeof(ServiceLogger<>));

// builder.Services.AddProblemDetails();

// old version
//builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

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
app.UseHttpsRedirection();

app.UseAuthorization();

// app.UseExceptionHandler();

// GlobalExceptionHandler is not middleware anymore, but it's an ExceptionHandler
// app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();

app.Logger.LogInformation("------------------RoomBooking API has been started-------------------");
app.Run();
