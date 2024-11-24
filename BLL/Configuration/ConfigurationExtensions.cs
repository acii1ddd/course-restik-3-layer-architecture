using BLL.Profiles;
using BLL.ServiceInterfaces;
using BLL.Services;
using DAL.Configuration;
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
                typeof(ClientProfile),
                typeof(RoleProfile),
                typeof(WorkerProfile),
                typeof(DishProfile),
                typeof(OrderProfile),
                typeof(OrderItemProfile)
            );

            // сервисы
            services.AddTransient<IClientService, ClientService>(); // с клиентами работает clientService
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IWorkerService, WorkerService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IClientInteractionService, ClientInteractionService>();
        }
    }
}
