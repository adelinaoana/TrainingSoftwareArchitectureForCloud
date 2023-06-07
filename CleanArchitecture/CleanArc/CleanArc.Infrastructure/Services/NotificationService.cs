using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CleanArc.Application.Interfaces;
using CleanArc.Common;
using Newtonsoft.Json;

namespace CleanArc.Infrastructure.Services
{
    [MapServiceDependency(nameof(INotificationService))]
    public class NotificationService : INotificationService
    {
        public async Task NotifyAsync(string message)
        {
            var json = JsonConvert.SerializeObject(message);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "https://httpbin.org/post";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            var result = await response.Content.ReadAsStringAsync();
        }
    }
}