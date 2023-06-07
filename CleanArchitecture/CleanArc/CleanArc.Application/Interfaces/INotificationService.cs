using System.Threading.Tasks;

namespace CleanArc.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyAsync(string message);
    }
}