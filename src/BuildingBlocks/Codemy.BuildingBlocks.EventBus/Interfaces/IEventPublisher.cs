namespace Codemy.BuildingBlocks.EventBus.Interfaces
{
    public interface IEventPublisher
    {
        void Publish<T>(T @event, string queueName);
    }
}
