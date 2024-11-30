using BLL.Profiles;
using BLL.ServiceInterfaces;
using BLL.ServiceInterfaces.DTOs;
using BLL.ServiceInterfaces.LogicInterfaces;
using BLL.ServiceInterfaces.ValidatorsInterfaces;
using BLL.Services;
using BLL.Services.Validators;
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
            services.AddTransient<ICookService, CookService>();


            // validators
            services.AddTransient<IOrderValidatorService, OrderValidatorService>();
        }
    }
}
