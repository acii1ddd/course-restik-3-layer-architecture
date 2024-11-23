using BLL.Profiles;
using BLL.ServiceInterfaces;
using BLL.Services;
using DAL.Configuration;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Configuration
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureBLL(this IServiceCollection services, string connection)
        {
            services.ConfigureDAL(connection);

            //профили
            services.AddAutoMapper(
                typeof(ClientProfile)
            );

            // сервисы
            services.AddTransient<IClientService, ClientService>(); // с клиентами работает clientService
            services.AddTransient<IRoleService, RoleService>();
        }
    }
}
