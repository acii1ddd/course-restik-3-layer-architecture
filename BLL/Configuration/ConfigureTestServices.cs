using BLL.Profiles;
using BLL.ServiceInterfaces;
using BLL.ServiceInterfaces.DTOs;
using BLL.ServiceInterfaces.LogicInterfaces;
using BLL.ServiceInterfaces.ValidatorInterfaces;
using BLL.ServiceInterfaces.ValidatorsInterfaces;
using BLL.Services;
using BLL.Services.LogicServices;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Configuration
{
    public static class ConfigurationTests
    {
        public static void ConfigureBLLServices(this IServiceCollection services)
        {
            // сервисы
            //services.AddTransient<IClientService, ClientService>(); // с клиентами работает clientService
            //services.AddTransient<IRoleService, RoleService>();
            //services.AddTransient<IWorkerService, WorkerService>();
            //services.AddTransient<IAuthService, AuthService>();
            //services.AddTransient<IClientInteractionService, ClientInteractionService>();
            //services.AddTransient<ICookService, CookService>();
            //services.AddTransient<IAdminService, AdminService>();

            services.AddTransient<IWaiterService, WaiterService>();
        }
    }
}
