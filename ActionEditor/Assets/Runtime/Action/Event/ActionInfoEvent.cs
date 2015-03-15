using Core;

namespace Action
{
    public class ActionInfoEvent : Event
    {
        public const string ActionStateChange = "ActionStateChange";

        public const string ActionPauseChange = "ActionPauseChange";

        public const string ActionSpeedChange = "ActionSpeedChange";

        public const string ActionReadyChange = "ActionReadyChange";

        public const string ActionProgressChange = "ActionProgressChange";

        public ActionInfoEvent(string type, object data = null) : base(type, data)
        {
            
        }
    }
}
