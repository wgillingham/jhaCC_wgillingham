using System.Threading;
using System.Threading.Tasks;

namespace jhaCC.Services.Monitor
{
    // defines method for accessing raw 'message' data (twitter stream)
    public interface IMessageMonitor
    {
        public Task RetrieveMessagesAsync(CancellationToken cancellationToken);
    }
}
