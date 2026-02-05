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
                    ValidateIssuer = true, //issuer validation
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true, // getter/audience validation
                    ValidAudience = jwtOptions.Audience,
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
                         var token = context.Request.Cookies["my-cookies"];
                         if (!string.IsNullOrWhiteSpace(token))
                         {
                             context.Token = token;
                         }

                         return Task.CompletedTask;
                     }
                 };
            });

        services.AddAuthorization();
    }
}