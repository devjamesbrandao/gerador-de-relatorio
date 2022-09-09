namespace Relatorio.Core.Interfaces
{
    public interface IMessageBusPublishService
    {
        void Publish(string queue, byte[] message);
    }
}