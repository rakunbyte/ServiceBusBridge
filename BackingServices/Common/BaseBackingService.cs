using Newtonsoft.Json;
using Serilog;

namespace BackingServices.Common;

public abstract class BaseBackingService<T> : IBackingService
{
    public abstract string Name { get; }
    
    public async Task ProcessMessage(string message)
    {
        Log.Information("AccountCreatedService start processing message");
        var workingEvent = DeserializeMessage(message);
        await HandleEvent(workingEvent);
    }

    private T DeserializeMessage(string message)
    {
        if (message == null) throw new ArgumentException();

        var result = JsonConvert.DeserializeObject<T>(message);
        return result ?? throw new ArgumentException($"Json Deserialized as null for {message} for BackingService");
    }

    public abstract Task HandleEvent(T workingEvent);


}