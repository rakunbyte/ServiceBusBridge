using System.Text.Json.Nodes;
using EventModels;
using Newtonsoft.Json;

namespace BackingServices;

public class AccountCreatedService : IBackingService
{
    
    public Task DoWork(string message)
    {
        var xxx = JsonConvert.DeserializeObject<AccountCreatedEvent>(message);
        Console.WriteLine("Hey I got an event " + xxx.UserId );
        return Task.CompletedTask;
    }
}