namespace BackingServices;

public interface IBackingService
{
    Task DoWork(string message);
}
