using DDDCqrsEs.Domain.Events;
using MediatR;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace DDDCqrsEs.WebJob.ConsoleApp
{
    public class WebJobFunction
    {
        private readonly IMediator _mediator;

        public WebJobFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Run([ServiceBusTrigger("stockevents",
                Connection = "ConnectionStrings:ServiceBusString")]
            string myQueueItem)
        {
            var _event = JsonConvert.DeserializeObject<BaseEvent>(myQueueItem);

            var assembly = typeof(BaseEvent).Assembly;
            var _namespace = typeof(BaseEvent).Namespace;
            var fullClassName = _namespace + "." + _event.EventType;
            var eventType = assembly.GetType(fullClassName);
            dynamic typedEvent = Activator.CreateInstance(eventType);

            typedEvent.Stock = _event.Stock;
            typedEvent.EventType = _event.EventType;
            typedEvent.AggregateId = _event.AggregateId;
            typedEvent.Version = _event.Version;

            await _mediator.Send(typedEvent);
        }
    }
}
