using Azure.Messaging.ServiceBus;
using MediatR;
using Newtonsoft.Json;
using DDDCqrsEs.Domain.Events;
using System;
using System.Threading.Tasks;

namespace DDDCqrsEs.WebJob.Services
{
    public class ServiceBusQueueHandler : IServiceBusQueueHandler
    {
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly IMediator _mediator;
    
        public ServiceBusQueueHandler(string connectionString, string queueName, IMediator mediator)
        {
            _connectionString = connectionString;
            _queueName = queueName;
            _mediator = mediator;
        }

        public async Task HandleMessage(ProcessMessageEventArgs message)
        {
            var messageBody = message.Message.Body.ToString();
            var _event = JsonConvert.DeserializeObject<BaseEvent>(messageBody);

            var assembly = typeof(BaseEvent).Assembly;
            var _namespace = typeof(BaseEvent).Namespace;
            var fullClassName = _namespace + "." + _event.EventType;
            var eventType = assembly.GetType(fullClassName);
            dynamic typedEvent = Activator.CreateInstance(eventType);

            typedEvent.Stock = _event.Stock;
            typedEvent.AggregateId = _event.AggregateId;
            typedEvent.Version = _event.Version;

            await _mediator.Send(typedEvent);

            Console.WriteLine($"Received {_event.EventType} for stock with id {_event.AggregateId}");

            await message.CompleteMessageAsync(message.Message);
        }

        public async Task HandleError(ProcessErrorEventArgs message)
        {

        }

        public async Task ReceiveMessages()
        {
            await using (var client = new ServiceBusClient(_connectionString))
            { 
                ServiceBusProcessor processor = client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

                processor.ProcessMessageAsync += HandleMessage;
                processor.ProcessErrorAsync += HandleError;

                await processor.StartProcessingAsync();

                while (true)
                {
                }
                //Console.ReadKey();
                //await processor.StopProcessingAsync();
            }
        }

    }
}
