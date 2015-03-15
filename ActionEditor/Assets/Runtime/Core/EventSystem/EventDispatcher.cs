using System;
using System.Collections.Generic;

namespace Core
{
    public class EventDispatcher : IEventDispatcher
    {
        private Dictionary<string, Action<IEvent>> callBacks;

        public EventDispatcher()
        {
            callBacks = new Dictionary<string, Action<IEvent>>();
        }

        public void DispatchEvent(IEvent e)
        {
            if (callBacks.ContainsKey(e.type))
            {
                e.target = this;
                callBacks[e.type](e);
            }
        }

        public void AddEventListener(string type, Action<IEvent> handler)
        {
            if (!callBacks.ContainsKey(type) || callBacks[type] == null)
            {
                callBacks[type] = handler;
            }
            else
            {
                callBacks[type] += handler;
            }
        }

        public void RemoveEventListener(string type, Action<IEvent> handler)
        {
            if (callBacks.ContainsKey(type) && callBacks[type] != null)
            {
                callBacks[type] -= handler;
            }
        }

        public void RemoveEventListeners(string type)
        {
            if (callBacks.ContainsKey(type))
            {
                callBacks.Remove(type);
            }
        }

        public void RemoveAllEventListeners()
        {
            callBacks.Clear();
        }

        public void Dispose()
        {
            RemoveAllEventListeners();
        }
    }
}
