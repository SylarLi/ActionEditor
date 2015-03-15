namespace Action
{
    public interface IActionContext
    {
        IActionSetting setting { get; }

        IActionPool pool { get; }

        IActionPlayer player { get; }
    }
}
