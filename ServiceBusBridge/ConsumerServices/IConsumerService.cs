namespace KafkaBridge.ConsumerServices;

public interface IConsumerService
{
    Task StartConsumer(CancellationToken cancellationToken);
}