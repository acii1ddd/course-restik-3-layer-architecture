﻿using DAL.Interfaces;
using DAL.PostgresRepositories;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Configuration
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureDAL(this IServiceCollection services, string connection, string type)
        {
            switch (type)
            {
                case "postgres":
                    services.ConfigurePostgres(connection);
                    break;
                case "mongo":
                    services.ConfigureMongo(connection);
                    break;
                default:
                    throw new Exception("Неизвестный тип базы данных");
            }
        }

        public static void ConfigureMongo(this IServiceCollection services, string connection)
        {
            //services.AddScoped<IClientRepository>(provider => new DAL.MongoRepositories.ClientRepository(connection));
            //services.AddScoped<IWorkerRepository>(provider => new DAL.MongoRepositories.WorkerRepository(connection));
            //services.AddScoped<IRoleRepository>(provider => new DAL.MongoRepositories.RoleRepository(connection));
            //services.AddScoped<IDishRepository>(provider => new DAL.MongoRepositories.DAL.MongoRepositories.DishRepository(connection));
            //services.AddScoped<IOrderRepository>(provider => new DAL.MongoRepositories.OrderRepository(connection));
            //services.AddScoped<IOrderItemRepository>(provider => new DAL.MongoRepositories.OrderItemRepository(connection));
            //services.AddScoped<IIngredientRepository>(provider => new DAL.MongoRepositories.IngredientRepository(connection));
            //services.AddScoped<IRecipeRepository>(provider => new DAL.MongoRepositories.RecipeRepository(connection));
            //services.AddScoped<IPaymentRepository>(provider => new DAL.MongoRepositories.PaymentRepository(connection));
        }

        public static void ConfigurePostgres(this IServiceCollection services, string connection)
        {
            services.AddScoped<IClientRepository>(provider => new ClientRepository(connection));
            services.AddScoped<IWorkerRepository>(provider => new WorkerRepository(connection));
            services.AddScoped<IRoleRepository>(provider => new RoleRepository(connection));
            services.AddScoped<IDishRepository>(provider => new DishRepository(connection));
            services.AddScoped<IOrderRepository>(provider => new OrderRepository(connection));
            services.AddScoped<IOrderItemRepository>(provider => new OrderItemRepository(connection));
            services.AddScoped<IIngredientRepository>(provider => new IngredientRepository(connection));
            services.AddScoped<IRecipeRepository>(provider => new RecipeRepository(connection));
            services.AddScoped<IPaymentRepository>(provider => new PaymentRepository(connection));
        }
    }
}
