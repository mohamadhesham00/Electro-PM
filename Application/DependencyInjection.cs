
using Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // 1. Standard MediatR registration
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(assembly);

                // 2. Attach our Validation interceptor into the MediatR pipeline!
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            // 3. Automatically registers all classes inheriting from AbstractValidator in this assembly
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
