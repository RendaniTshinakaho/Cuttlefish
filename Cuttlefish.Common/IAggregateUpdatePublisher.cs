namespace Cuttlefish.Common
{
    public interface IAggregateUpdatePublisher
    {
        void Dispose();
        void PublishUpdate(IAggregate aggregate);
    }
}