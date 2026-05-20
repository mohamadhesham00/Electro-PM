using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // 1. Validate the secret key signature to ensure the token hasn't been tampered with
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"] ?? throw new InvalidOperationException("JWT Secret Key is missing."))),

                        // 2. Validate the server that created the token (Issuer)
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JwtSettings:Issuer"],

                        // 3. Validate who the token is intended for (Audience / Client Application)
                        ValidateAudience = true,
                        ValidAudience = configuration["JwtSettings:Audience"],

                        // 4. Validate that the token has not expired yet
                        ValidateLifetime = true,

                        // 5. Set clock skew to zero so tokens expire *exactly* when they are supposed to, 
                        // instead of inheriting the default 5-minute grace period.
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }
    }
}
