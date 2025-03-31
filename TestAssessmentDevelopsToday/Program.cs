using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestAssessmentDevelopsToday.Config;
using TestAssessmentDevelopsToday.Infrastructure;
using TestAssessmentDevelopsToday.Repositories.Implementations;
using TestAssessmentDevelopsToday.Repositories.Interfaces;
using TestAssessmentDevelopsToday.Services.Implementations;
using TestAssessmentDevelopsToday.Services.Interfaces;

namespace TestAssessmentDevelopsToday
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var configPath = Path.Combine(Directory.GetCurrentDirectory(), "Config", "appsettings.json");
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile(configPath, optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    var connectionString = configuration.GetConnectionString("DefaultConnection");
                    var duplicatesFilePath = configuration.GetValue<string>("DuplicatesFilePath");

                    services.AddSingleton(new ApplicationConfig(connectionString, duplicatesFilePath));
                    services.AddSingleton<DatabaseInitializer>();
                    services.AddScoped<ITaxiTripRepository, TaxiTripRepository>();
                    services.AddScoped<ITaxiTripCsvProcessor, TaxiTripCsvProcessor>();
                })
                .Build();

            using var scope = builder.Services.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<ITaxiTripCsvProcessor>();

            Console.WriteLine("Enter path for the CSV-file: ");
            string filePath = Console.ReadLine();

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("File not found");
                return;
            }

            await processor.ProcessAsync(filePath);
        }
    }
}
