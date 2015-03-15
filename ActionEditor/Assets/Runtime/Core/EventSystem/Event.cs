using System;

namespace Core
{
    public class Event : EventArgs, IEvent
    {
        private string _type;

        private object _data;

        private IEventDispatcher _target;
        
        public Event(string type, object data = null) : base()
        {
            _type = type;
            _data = data;
        }
        
        public string type
        {
            get { return _type; }
        }

        public IEventDispatcher target
        {
            get { return _target; }
            set { _target = value; }
        }

        public object data
        {
            get { return _data; }
        }
    }
}
