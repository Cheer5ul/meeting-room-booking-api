using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using RoomBooking.API.FailureHandlers;
using RoomBooking.API.Middlewares.ExceptionHandlers;
using RoomBooking.Application.Services;
using RoomBooking.Application.Validations.Abstractions.Rooms;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Application.Validations.Abstractions.Validators;
using RoomBooking.Application.Validations.Converters;
using RoomBooking.Application.Validations.Validators.Bookings;
using RoomBooking.Application.Validations.Validators.Rooms;
using RoomBooking.Application.Validations.Validators.Users;
using RoomBooking.Application.Validations.Validators.Users.AddressInfo;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.DataAccess;
using RoomBooking.DataAccess.DbContext;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
builder.Services.Configure<RoomValidationSettings>(
    builder.Configuration.GetSection("RoomValidationSettings")); //using IOptions 

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
builder.Services.AddScoped<IUserEmailValidator, UserEmailValidator>();
builder.Services.AddScoped<IValidationToErrorConverter, ValidationToErrorConverter>();

//Fluent Validators
builder.Services.AddValidatorsFromAssemblyContaining<BookingValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<UserCreationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserUpdateValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddressInfoAddingDtoValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<RoomCreationValidator>();

builder.Services.AddScoped<IFailureHandler, FailureHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    rateLimiterOptions.OnRejected = async (context, token) =>
    {
        context.HttpContext.Request.Headers["Retry-After"] = "10";
        await context.HttpContext.Response.WriteAsync(
            "Too many requests. Please try again later", token);
    };
    
    //Fixed Window
    rateLimiterOptions.AddFixedWindowLimiter("fixed-by-ip", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
        options.QueueLimit = 0;
        options.AutoReplenishment = true;
    });
    
    //Sliding Window
    rateLimiterOptions.AddSlidingWindowLimiter("sliding-by-ip", options =>
    {
        options.Window = TimeSpan.FromSeconds(15);
        options.SegmentsPerWindow = 3;
        options.PermitLimit = 15;
        options.QueueLimit = 2;
    });

    //Token Bucket
    rateLimiterOptions.AddTokenBucketLimiter("token-by-ip", options =>
    {
        options.TokenLimit = 100; //Burst capacity
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(5);
        options.TokensPerPeriod = 10;
        options.QueueLimit = 3;
    });
    
    //Concurrency limiter
    rateLimiterOptions.AddConcurrencyLimiter("concurrency-by-ip", options =>
    {
        options.PermitLimit = 5;
        options.QueueLimit = 2;
    });
    
    //Fixed Window USER based (needs authorization)
    rateLimiterOptions.AddPolicy("fixed-by-user", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name?.ToString()
                          ?? "anonymous",
            factory: partition => new FixedWindowRateLimiterOptions()
            {
                Window = TimeSpan.FromSeconds(10),
                PermitLimit = 5,
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                AutoReplenishment = true 
            })
    );
    
    //Global fixed window ip based rate limiter
    rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            //IP-BASED key:
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() 
                          ?? "unknown",
            
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 50,
                Window = TimeSpan.FromMinutes(1)
            }));
});

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

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseRateLimiter(); //turning on the rate limiter

app.UseAuthorization();

app.MapControllers();/*.RequireRateLimiting("fixed");*/

logger.LogInformation("---------------------------RoomBooking API started---------------------------");
app.Run();
