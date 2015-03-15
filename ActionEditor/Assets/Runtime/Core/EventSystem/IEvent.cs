namespace Core
{
    public interface IEvent
    {
        string type { get; }

        IEventDispatcher target { get; set; }

        object data { get; }
    }
}
