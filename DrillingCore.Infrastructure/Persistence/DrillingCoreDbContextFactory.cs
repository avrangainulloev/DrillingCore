using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DrillingCore.Infrastructure.Persistence
{
    public class DrillingCoreDbContextFactory : IDesignTimeDbContextFactory<DrillingCoreDbContext>
    {
        public DrillingCoreDbContext CreateDbContext(string[] args)
        {
            // Получаем путь к файлу конфигурации
            var basePath = Directory.GetCurrentDirectory();

            // Построение конфигурации. Обратите внимание, что файл appsettings.json должен находиться в корне проекта.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Чтение строки подключения из конфигурации
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Настройка параметров для DbContext
            var optionsBuilder = new DbContextOptionsBuilder<DrillingCoreDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new DrillingCoreDbContext(optionsBuilder.Options);
        }
    }
}
