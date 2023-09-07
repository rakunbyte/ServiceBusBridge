using Newtonsoft.Json;
using Serilog;

namespace BackingServices.Common;

public abstract class BaseBackingService<T> : IBackingService
{
    public abstract string Name { get; }
    
    public async Task ProcessMessage(string message)
    {
        Log.Information("{Service} start processing message", GetType());
        var workingEvent = DeserializeMessage(message);
        await HandleEvent(workingEvent);
    }

    private static T DeserializeMessage(string message)
    {
        if (message == null) throw new ArgumentException();

        var result = JsonConvert.DeserializeObject<T>(message);
        return result ?? throw new ArgumentException($"Json Deserialized as null for {message} for BackingService");
    }

    protected abstract Task HandleEvent(T workingEvent);
}