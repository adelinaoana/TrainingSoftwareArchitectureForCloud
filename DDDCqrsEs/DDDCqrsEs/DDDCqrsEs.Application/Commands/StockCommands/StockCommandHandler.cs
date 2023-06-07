using MediatR;
using Newtonsoft.Json;
using DDDCqrsEs.Application.Common;
using DDDCqrsEs.Application.Services;
using DDDCqrsEs.Common;
using DDDCqrsEs.Domain.Aggregates;
using DDDCqrsEs.Domain.Entities;
using DDDCqrsEs.Domain.Events;
using DDDCqrsEs.Domain.Models;
using DDDCqrsEs.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DDDCqrsEs.Application.Commands.StockCommands
{

    [MapServiceDependency(Name: nameof(StockCommandHandler))]
    public class StockCommandHandler :  IRequestHandler<CreateStockCommand, CreateStockCommandResponse>,
                                        IRequestHandler<UpdateStockCommand, UpdateStockCommandResponse>,
                                        IRequestHandler<DeleteStockCommand, DeleteStockCommandResponse>
    {

        private IEventStore _eventRepository;
        private IBaseEventPublisher _serviceBusPublisher;

        public StockCommandHandler(IEventStore eventRepository, IBaseEventPublisher serviceBusPublisher)
        {
            _eventRepository = eventRepository;
            _serviceBusPublisher = serviceBusPublisher;
        }

        public async Task<CreateStockCommandResponse> Handle(CreateStockCommand request, CancellationToken cancellationToken)
        {
            var guid = Guid.NewGuid();
            var stockAggregate = new Stock(guid);

            var model = ModelMapper.ConvertCommandToModel(request);
            var stockCreatedEvent = stockAggregate.Create(model);

            await _eventRepository.SaveEvent(stockCreatedEvent);
            await _serviceBusPublisher.PublishEvent(stockCreatedEvent);

            return new CreateStockCommandResponse { AggregateId = guid, Version = stockCreatedEvent.Version };
        }

        public async Task<UpdateStockCommandResponse> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
        {
            var eventsSoFar = await _eventRepository.GetEventsByAggregateId(request.Id).ToListAsync();
            var baseEvents = EventMapper.ConvertListOfEntityEventsToBase(eventsSoFar);
            var stockAggregate = new Stock(request.Id);

            stockAggregate.ReconstituteFromEvents(baseEvents);

            var model = ModelMapper.ConvertCommandToModel(request);
            var stockUpdatedEvent = stockAggregate.Update(model);

            await _eventRepository.SaveEvent(stockUpdatedEvent);
            await _serviceBusPublisher.PublishEvent(stockUpdatedEvent);

            return new UpdateStockCommandResponse { AggregateId = stockUpdatedEvent.AggregateId, Version = stockUpdatedEvent.Version };
        }

        public async Task<DeleteStockCommandResponse> Handle(DeleteStockCommand request, CancellationToken cancellationToken)
        {
            var eventsSoFar = await _eventRepository.GetEventsByAggregateId(request.Id).ToListAsync();
            var baseEvents = EventMapper.ConvertListOfEntityEventsToBase(eventsSoFar);
            var stockAggregate = new Stock(request.Id);
            stockAggregate.ReconstituteFromEvents(baseEvents);

            var stockDeletedEvent = stockAggregate.Delete();

            await _eventRepository.SaveEvent(stockDeletedEvent);
            await _serviceBusPublisher.PublishEvent(stockDeletedEvent);

            return new DeleteStockCommandResponse {AggregateId = stockDeletedEvent.AggregateId, Version = stockDeletedEvent.Version };
        }
    }
}
