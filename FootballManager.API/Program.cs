using FootballManager.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FootballManager.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            PrepareDatabase(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void PrepareDatabase(IHost host)
        {
            var config = host.Services.GetRequiredService<IConfiguration>();
            var databaseConfig = config.GetConnectionString(nameof(FootballManagerContext));
            MigrateDatabase(databaseConfig);
        }

        private static void MigrateDatabase(string databaseConfig)
        {
            var services = new ServiceCollection();
            services.AddDbContext<FootballManagerContext>(options =>
                options.UseSqlServer(databaseConfig)
                    .EnableSensitiveDataLogging());

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (var context = scope.ServiceProvider.GetService<FootballManagerContext>())
                    {
                        context.Database.Migrate();
                    }
                }
            }
        }
    }
}