

namespace Microservices.Common.Services
{
    public interface IMessageReceiver : IDisposable
    {
        Task StartProcessingAsync(CancellationToken cancellationToken);
    }
}
