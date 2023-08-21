using BackingServices.Common;
using EventModels;

namespace BackingServices;

public class DeliveryScheduledService : BasePassthroughBackingService<DeliveryScheduledEvent>
{
    public override string Name { get; } = "DeliveryScheduled";
}