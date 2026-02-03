using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RoomBooking.Infrastructure.Providers;

namespace RoomBooking.API.Extensions;

public static class ApiExtensions
{
    public static void AddApiExtensions(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var jwtOptions = configuration
            .GetSection(nameof(JwtOptions))
            .Get<JwtOptions>()
            ?? throw new ArgumentException("JWT options are not configured");
        
        //minimal configuration
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false, //issuer validation
                    ValidateAudience = false, // getter/audience validation
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true, // issuer secret key
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
                
                 //checking the token
                 options.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context =>
                     {
                         context.Token = context.Request.Cookies["my-cookies"];

                         return Task.CompletedTask;
                     }
                 };
            });

        services.AddAuthorization();
    }
}