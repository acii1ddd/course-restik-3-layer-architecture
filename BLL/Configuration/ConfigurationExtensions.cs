﻿using BLL.Profiles;
using BLL.ServiceInterfaces;
using BLL.ServiceInterfaces.DTOs;
using BLL.ServiceInterfaces.LogicInterfaces;
using BLL.ServiceInterfaces.ValidatorInterfaces;
using BLL.ServiceInterfaces.ValidatorsInterfaces;
using BLL.Services;
using BLL.Services.ArchiveHandlers;
using BLL.Services.LogicServices;
using BLL.Services.Validators;
using BLL.Services.ValidatorsServices;
using DAL.ArchiveHandlers;
using DAL.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Configuration
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureBLL(this IServiceCollection services, string connection, string type, string databaseName)
        {
            services.ConfigureDAL(connection, type, databaseName);

            //профили
            services.AddAutoMapper(
                typeof(ClientProfile),
                typeof(RoleProfile),
                typeof(WorkerProfile),
                typeof(DishProfile),
                typeof(OrderProfile),
                typeof(OrderItemProfile),
                typeof(RecipeProfile),
                typeof(IngredientProfile)
            );

            // сервисы
            //services.AddTransient<IClientService, ClientService>(); // с клиентами работает clientService
            services.AddTransient<IRoleService, RoleService>();
            //services.AddTransient<IWorkerService, WorkerService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IClientInteractionService, ClientInteractionService>();
            services.AddTransient<ICookService, CookService>();
            services.AddTransient<IWaiterService, WaiterService>();
            services.AddTransient<IAdminService, AdminService>();

            // validators
            services.AddTransient<IClientValidatorService, ClientValidatorService>();
            services.AddTransient<ICookValidatorService, CookValidatorService>();
            services.AddTransient<IWaiterValidatorService, WaiterValidatorService>();
            services.AddTransient<IAdminValidatorService, AdminValidatorService>();

            if (type == "mongo")
            {
                services.AddTransient<IArchiveHandler, MongoArchiveHandler>();
            }
            else if (type == "postgres")
            {
                services.AddTransient<IArchiveHandler, PostgresArchiveHandler>();
            }
        }
    }
}
