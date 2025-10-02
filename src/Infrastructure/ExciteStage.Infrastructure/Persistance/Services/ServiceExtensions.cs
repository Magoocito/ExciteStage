using ExciteStage.Application.Repositories;
using ExciteStage.Infrastructure.Persistance.Context;
using ExciteStage.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace ExciteStage.Infrastructure.Persistance.Services
{
    public static class ServiceExtensions
    {
        public static void ConfigurePersistenceApp(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar todos los perfiles de AutoMapper en el ensamblado actual
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            var connectionString = configuration.GetConnectionString("Database");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped<IPortfolioRepository, PortfolioRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPortfolioBetRepository, PortfolioBetRepository>(); // Si lo usas



        }
    }
}