using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace DDDCqrsEs.Persistance
{
    public interface ITableStorageConnection
    {
        public Task<CloudTable> CreateConnection(string tableName);
    }
}
