using RoomBooking.API.Middlewares;
using RoomBooking.Application.Services;
using RoomBooking.Application.Validations.Abstractions.Bookings;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Application.Validations.Validators.Bookings;
using RoomBooking.Application.Validations.Validators.Users;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.DataAccess;
using RoomBooking.DataAccess.DbContext;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
var env =  builder.Environment;

if (env.IsDevelopment())
{
    builder.Host.UseSerilog((hostContext, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .MinimumLevel.Debug()
            .WriteTo.File("logs/serilog-file.txt")
            .WriteTo.Console();
    });
}
else
{
    builder.Host.UseSerilog((hostContext, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .WriteTo.File("logs/serilog-file.txt")
            .WriteTo.Console();
    });
}

// builder.Services.AddLogging(logging =>
// {
//     // logging.ClearProviders();
//     
//     logging.AddJsonConsole(options =>
//     {
//         // options.IncludeScopes = true;
//
//         options.JsonWriterOptions = new()
//         {
//             Indented = true
//         };
//         
//         options.TimestampFormat = "HH:mm:ss";
//     });
// });


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
builder.Services.AddScoped<IUserDeletionValidator,  UserDeletionValidator>();
builder.Services.AddScoped<IBookingCreationValidator, BookingCreationValidator>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();

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

app.UseExceptionHandler();

// GlobalExceptionHandler is not middleware anymore, but it's an ExceptionHandler
// app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();

app.Logger.LogInformation("------------------RoomBooking API has been started-------------------");
app.Run();
