using DAL.Interfaces;
using DAL.PostgresRepositories;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Configuration
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureDAL(this IServiceCollection services, string connection)
        {   
            services.AddScoped<IClientRepository>(provider => new ClientRepository(connection));
            services.AddScoped<IRoleRepository>(provider => new RoleRepository(connection));
            services.AddScoped<IWorkerRepository>(provider => new WorkerRepository(connection));
        }
    }
}
