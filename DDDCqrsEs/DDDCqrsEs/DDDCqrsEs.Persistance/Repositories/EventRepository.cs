using DDDCqrsEs.Domain.Events;
using System;
using System.Collections.Generic;
using DDDCqrsEs.Domain.Repositories;
using DDDCqrsEs.Common;
using Newtonsoft.Json;
using DDDCqrsEs.Persistance.DataModel;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace DDDCqrsEs.Persistance.Repositories
{
    [MapServiceDependency(Name: nameof(EventStore))]

    public class EventStore : IEventStore
    {

        private readonly ITableStorageConnection _connectionCreator; 

        public EventStore(ITableStorageConnection connection)
        {
            _connectionCreator = connection;
        }

        public async Task SaveEvent(BaseEvent _event)
        {
            string data = JsonConvert.SerializeObject(_event.Stock);

            EventEntity eventEnity = new EventEntity(_event.AggregateId, _event.Version)
            {
                EventType = _event.EventType,
                Data = data,
                TimeCreated =_event.TimeStamp
            };

            var cloudTable = await _connectionCreator.CreateConnection(nameof(EventStore));
            var insertOperation = TableOperation.Insert(eventEnity);

            await cloudTable.ExecuteAsync(insertOperation);
            
        }

        public async IAsyncEnumerable<EventEntity> GetAllEvents()
        {
            var cloudTable = await _connectionCreator.CreateConnection(nameof(EventStore));

            var query = new TableQuery<EventEntity>();

            var result = cloudTable.ExecuteQuerySegmentedAsync(query, null);
            foreach (var item in result.Result)
            {
                yield return item;
            }
        }

        public async IAsyncEnumerable<EventEntity> GetEventsByAggregateId(Guid id)
        {
            var cloudTable = await _connectionCreator.CreateConnection(nameof(EventStore));

            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString());
            var query = new TableQuery<EventEntity>().Where(filter);

            var result = cloudTable.ExecuteQuerySegmentedAsync(query, null);
            foreach (var item in result.Result)
            {
                yield return item;
            }
        }

    }
}
