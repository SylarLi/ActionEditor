namespace Action
{
    public class ActionPlayer : IActionPlayer
    {
        private IActionSetting _setting;

        private IActionPool _pool;

        public ActionPlayer(IActionSetting setting, IActionPool pool)
        {
            _setting = setting;
            _pool = pool;
        }

        public IActionInfo Build(IDependInfo dependInfo, string raw)
        {
            RootActionInfo rootInfo = new RootActionInfo(_pool.GetActionInfo(raw), dependInfo);
            RootAction rootAction = new RootAction(rootInfo);
            rootAction.setting = _setting;
            rootAction.Load();
            return rootInfo;
        }

        public IActionInfo Build(IDependInfo dependInfo, IActionInfo actionInfo)
        {
            RootActionInfo rootInfo = new RootActionInfo(actionInfo, dependInfo);
            RootAction rootAction = new RootAction(rootInfo);
            rootAction.setting = _setting;
            rootAction.Load();
            return rootInfo;
        }
    }
}
