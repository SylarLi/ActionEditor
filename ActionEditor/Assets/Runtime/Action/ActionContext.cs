namespace Action
{
    public class ActionContext : IActionContext
    {
        private IActionSetting _setting;

        private IActionPool _pool;

        private IActionPlayer _player;

        public ActionContext()
        {
            _setting = new ActionSetting();
            _pool = new ActionPool();
            _player = new ActionPlayer(_setting, _pool);
        }

        public IActionSetting setting
        {
            get
            {
                return _setting;
            }
        }

        public IActionPool pool
        {
            get
            {
                return _pool;
            }
        }

        public IActionPlayer player
        {
            get
            {
                return _player;
            }
        }
    }
}
