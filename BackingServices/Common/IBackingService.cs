namespace BackingServices.Common;

public interface IBackingService
{
    string Name { get; }
    
    Task ProcessMessage(string message);
}
