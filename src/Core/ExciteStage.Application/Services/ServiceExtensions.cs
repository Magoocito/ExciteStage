using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ExciteStage.Application.Services
{
    public static class ServiceExtensions
    {
        public static void ConfigureApplicationApp(this IServiceCollection services)
        {
            // Registrar todos los mapeos de AutoMapper en el ensamblado de Application
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Registrar todos los handlers de MediatR en el ensamblado de Application
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        }
    }
}
