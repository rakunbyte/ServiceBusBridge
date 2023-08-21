namespace EventModels;

public class DeliveryScheduledEvent : IPassThroughEvent
{
    public string GetCcsV2Request()
    {
        return "This Event converts directly to a CCS V2 request";
    }
}