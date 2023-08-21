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
        //This is where you make your api calls, etc
        
        //send CCS V2 request
        return Task.CompletedTask;
    }
}