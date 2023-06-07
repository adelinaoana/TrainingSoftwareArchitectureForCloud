using MediatR;
using Microsoft.Extensions.Logging;
using CleanArc.Application.Interfaces;
using CleanArc.Common;
using CleanArc.Domain.Entities;
using CleanArc.Domain.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArc.Application.Commands.ToDoCommands
{
    [MapServiceDependency(Name: nameof(ToDoCommandHandler))]
    public class ToDoCommandHandler : IRequestHandler<AddToDoCommand, AddToDoCommandResponse>,
        IRequestHandler<EditToDoCommand, EditToDoCommandResponse>,
        IRequestHandler<DeleteToDoCommand, Unit>
    {
        private readonly IRepository _repository;
        private readonly ILogger<ToDoCommandHandler> _logger;
        private readonly IEventDispatcher _eventDispatcher;

        public ToDoCommandHandler(IRepository repository, ILogger<ToDoCommandHandler> logger, IEventDispatcher eventDispatcher)
        {
            _repository = repository;
            _logger = logger;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<AddToDoCommandResponse> Handle(AddToDoCommand request, CancellationToken cancellationToken)
        {
            Guid newId = Guid.NewGuid();
            using (var uow = _repository.CreateUnitOfWork())
            {
                _logger.LogInformation("Entered Add task {0}", request.ToDo.Name);
                var toDo = new ToDo
                {
                    Id = newId,
                    Name = request.ToDo.Name,
                    UserId = request.User.UserId,
                    Description = request.ToDo.Description
                };

                uow.Add(toDo);
                await uow.SaveChangesAsync(cancellationToken);
            }

            if (request.ToDo.Notify)
                _eventDispatcher.Publish(new ToDoAdded(newId));

            _logger.LogInformation("Finished Add task {0}", request.ToDo.Name);
            return new AddToDoCommandResponse
            {
                ToDoId = newId
            };
        }

        public async Task<EditToDoCommandResponse> Handle(EditToDoCommand request, CancellationToken cancellationToken)
        {
            using (var uow = _repository.CreateUnitOfWork())
            {
                var toDo = uow.GetEntities<ToDo>().Where(x => x.Id == request.ToDo.Id).FirstOrDefault();

                toDo.Name = request.ToDo.Name;
                toDo.Description = request.ToDo.Description;

                await uow.SaveChangesAsync(cancellationToken);
            }
            return new EditToDoCommandResponse
            {
                ToDoId = request.ToDo.Id.Value,
            };
        }

        public async Task<Unit> Handle(DeleteToDoCommand request, CancellationToken cancellationToken)
        {
            using (var uow = _repository.CreateUnitOfWork())
            {
                var toRemoveToDo = uow.GetEntities<ToDo>().FirstOrDefault(x => x.Id == request.Id);
                uow.Delete(toRemoveToDo);
                await uow.SaveChangesAsync(cancellationToken);
            }
            
            return Unit.Value;
        }
    }
}
