namespace Action
{
    public class ActionSetting : IActionSetting
    {
        private IActionLoader _loader;

        private string _defaultActorAction;

        public IActionLoader loader
        {
            get
            {
                return _loader;
            }
            set
            {
                _loader = value;
            }
        }

        public string defaultActorAction
        {
            get
            {
                return _defaultActorAction;
            }
            set
            {
                _defaultActorAction = value;
            }
        }
    }
}
