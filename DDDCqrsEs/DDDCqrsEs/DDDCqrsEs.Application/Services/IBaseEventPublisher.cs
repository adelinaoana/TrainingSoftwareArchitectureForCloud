using DDDCqrsEs.Domain.Events;
using System.Threading.Tasks;

namespace DDDCqrsEs.Application.Services
{
    public interface IBaseEventPublisher
    {
        public Task PublishEvent(BaseEvent _event);
    }
}
