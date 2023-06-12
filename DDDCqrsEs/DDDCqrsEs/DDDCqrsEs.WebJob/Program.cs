using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using DDDCqrsEs.Application.EventHandlers;
using System.Reflection;
using DDDCqrsEs.Domain.Events;
using DDDCqrsEs.Persistance;
using Microsoft.EntityFrameworkCore;
using DDDCqrsEs.Persistance.Bootstrap;
using DDDCqrsEs.WebJob.Services;

namespace DDDCqrsEs.WebJob
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();
            var connectionString = config.GetConnectionString("ServiceBusString");
            var queueName = config.GetConnectionString("QueueName");
            var dbString = config.GetConnectionString("DbString"); 

            var services = new ServiceCollection();
            services.AddMediatR(typeof(StockEventHandler).GetTypeInfo().Assembly, typeof(BaseEvent).GetTypeInfo().Assembly);
            services.AddDbContext<ToDoDbContext>(options => options.UseSqlServer(dbString));
            services.RegisterRepositories();
            var serviceProvider = services.BuildServiceProvider();
            
            var mediator = serviceProvider.GetService<IMediator>();
            var messageHandler = new ServiceBusQueueHandler(connectionString, queueName, mediator);
            await messageHandler.ReceiveMessages();
        }
    }
}
