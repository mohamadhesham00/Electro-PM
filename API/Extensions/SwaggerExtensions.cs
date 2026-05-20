using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Electro PM API",
                    Version = "v1.0.0",
                    Description = "A comprehensive project management API built with Clean Architecture, CQRS pattern, and Entity Framework Core.",
                });

                // 1. Define the Security Scheme (The "Authorize" lock button)
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.\n\n" +
                                  "**How to use:**\n" +
                                  "1. Register a new user at POST /api/auth/register\n" +
                                  "2. Login at POST /api/auth/login to get your JWT token\n" +
                                  "3. Click 'Authorize' button below and paste the token (without 'Bearer ' prefix)\n" +
                                  "4. All subsequent requests will include your token automatically"
                });

                // 2. Apply the Security Requirement globally
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // 3. Add XML comments for method documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                // 4. Customize Swagger UI appearance and behavior
                options.OrderActionsBy(apiDesc => apiDesc.RelativePath);
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Electro PM API v1");
                    options.RoutePrefix = "swagger";

                    // Customize Swagger UI
                    options.DocumentTitle = "Electro PM API Documentation";
                    options.DefaultModelsExpandDepth(2);
                    options.DefaultModelExpandDepth(2);
                    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                    options.EnableFilter();
                    options.ShowExtensions();
                    options.EnableValidator();
                });
            }

            return app;
        }
    }
}
