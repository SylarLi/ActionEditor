using Core;

namespace Local
{
    public class LocalDataEvent : Event
    {
        public const string LOAD_COMPLETE = "load_complete";

        public const string SAVE_COMPLETE = "save_complete";

        public LocalDataEvent(string type, object data = null) : base(type, data)
        {

        }
    }
}
