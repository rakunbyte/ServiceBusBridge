using EventModels;
using Serilog;

namespace BackingServices.Common;

public abstract class BasePassthroughBackingService<T> : BaseBackingService<T> where T : IPassThroughEvent
{
    public override Task HandleEvent(T workingEvent)
    {
        Log.Information("Passthrough Processing Event: {@Event}", workingEvent);
        return Task.CompletedTask;
    }
}