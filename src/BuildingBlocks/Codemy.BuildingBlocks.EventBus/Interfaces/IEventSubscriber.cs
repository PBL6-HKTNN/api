namespace Codemy.BuildingBlocks.EventBus.Interfaces
{
    public interface IEventSubscriber
    {
        void Subscribe<T>(string queueName, Func<T, Task> handler);
    }
}
