﻿using DAL.Interfaces;
using DAL.PostgresRepositories;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Configuration
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureDAL(this IServiceCollection services, string connection)
        {
            services.AddScoped<IClientRepository>(provider => new ClientRepository(connection));
            services.AddScoped<IWorkerRepository>(provider => new WorkerRepository(connection));
            services.AddScoped<IRoleRepository>(provider => new RoleRepository(connection));
            services.AddScoped<IDishRepository>(provider => new DishRepository(connection));
            services.AddScoped<IOrderRepository>(provider => new OrderRepository(connection));
            services.AddScoped<IOrderItemRepository>(provider => new OrderItemRepository(connection));
            services.AddScoped<IIngredientRepository>(provider => new IngredientRepository(connection));
            services.AddScoped<IRecipeRepository>(provider => new RecipeRepository(connection));    
        }
    }
}
