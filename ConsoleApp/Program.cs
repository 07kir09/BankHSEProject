using System;
using Microsoft.Extensions.DependencyInjection;
using AccountingForFinances;
using AccountingForFinances.Analytics;
using AccountingForFinances.data;
using AccountingForFinances.DataManagement;

namespace BankHSEProject.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Настройка DI контейнера
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Получаем главный класс приложения (AppRunner) и запускаем его
            var app = serviceProvider.GetService<AppRunner>();
            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Регистрируем RealRepository для каждого типа через прокси
            services.AddSingleton<IRepository<BankAccount>>(provider =>
                new RepositoryProxy<BankAccount>(new RealRepository<BankAccount>()));
            services.AddSingleton<IRepository<Category>>(provider =>
                new RepositoryProxy<Category>(new RealRepository<Category>()));
            services.AddSingleton<IRepository<Operation>>(provider =>
                new RepositoryProxy<Operation>(new RealRepository<Operation>()));

            // Регистрируем сервисы
            services.AddSingleton<AnalyticsService>();
            services.AddSingleton<DataManagementService>();

            // Регистрируем импортеры (Transient, т.к. они не сохраняют состояние)
            services.AddTransient<CsvImporter>();
            services.AddTransient<JsonImporter>();
            services.AddTransient<YamlImporter>();

            // Регистрируем главный класс приложения
            services.AddSingleton<AppRunner>();
        }
    }
}