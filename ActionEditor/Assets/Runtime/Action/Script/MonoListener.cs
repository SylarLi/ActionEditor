using UnityEngine;
using Core;

namespace Action
{
    public class MonoListener : MonoBehaviour, IEventDispatcher
    {
        private EventDispatcher dispatcher = new EventDispatcher();

        public void DispatchEvent(IEvent e)
        {
            dispatcher.DispatchEvent(e);
        }

        public void AddEventListener(string type, System.Action<IEvent> handler)
        {
            dispatcher.AddEventListener(type, handler);
        }

        public void RemoveEventListener(string type, System.Action<IEvent> handler)
        {
            dispatcher.RemoveEventListener(type, handler);
        }

        public void RemoveEventListeners(string type)
        {
            dispatcher.RemoveEventListeners(type);
        }

        public void RemoveAllEventListeners()
        {
            dispatcher.RemoveAllEventListeners();
        }

        public void Dispose()
        {
            dispatcher.Dispose();
        }
    }
}
