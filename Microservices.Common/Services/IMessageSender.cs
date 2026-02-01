

namespace Microservices.Common.Services
{
    public interface IMessageSender
    {
        Task SendMessageAsync(string message);
    }
}
