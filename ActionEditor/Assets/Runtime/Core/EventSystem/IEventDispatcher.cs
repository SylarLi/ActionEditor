using System;

namespace Core
{
    public interface IEventDispatcher : IDispose
    {
        void DispatchEvent(IEvent e);

        void AddEventListener(string type, Action<IEvent> handler);

        void RemoveEventListener(string type, Action<IEvent> handler);

        void RemoveEventListeners(string type);

        void RemoveAllEventListeners();
    }
}
