using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using DDDCqrsEs.Domain.Events;

namespace DDDCqrsEs.Application.Services
{
    public class ServiceBusPublisher : IBaseEventPublisher
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public ServiceBusPublisher(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public async Task PublishEvent(BaseEvent _event)
        {
            await using (var client = new ServiceBusClient(_connectionString))
            {
                var sender = client.CreateSender(_queueName);

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };

                var serializedMessage = JsonConvert.SerializeObject(_event);
                var message = new ServiceBusMessage(serializedMessage);
                await sender.SendMessageAsync(message);
            } 
        }
    }
}
