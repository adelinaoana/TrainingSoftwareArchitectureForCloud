using DDDCqrsEs.Application.EventHandlers;
using DDDCqrsEs.Domain.Events;
using DDDCqrsEs.Persistance;
using DDDCqrsEs.Persistance.Bootstrap;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DDDCqrsEs.WebJob.ConsoleApp
{
    class Program
    {
        static async Task Main()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new HostBuilder();
            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddServiceBus();
            });
            builder.ConfigureServices(services =>
            {
                services.AddLogging();
                services.AddMediatR(typeof(StockEventHandler).Assembly, typeof(BaseEvent).Assembly);
                services.AddDbContext<ToDoDbContext>(options => options.UseSqlServer(configurationBuilder.GetConnectionString("DbString")));
                services.RegisterRepositories();
            });
            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}