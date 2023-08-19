using BackingServices.Common;
using EventModels;
using Serilog;

namespace BackingServices;

public class AccountCreatedService : BaseBackingService<AccountCreatedEvent>
{
    public override string Name => "AccountCreated";

    public override Task HandleEvent(AccountCreatedEvent workingEvent)
    {
        Log.Information("Account Created Processing Event: {@Event}", workingEvent);
        return Task.CompletedTask;
    }
}