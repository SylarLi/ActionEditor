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
            throw new System.NotImplementedException();
        }

        public IActionInfo Build(IDependInfo dependInfo, IActionInfo actionInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}
