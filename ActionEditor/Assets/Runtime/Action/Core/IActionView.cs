namespace Action
{
    public interface IActionView
    {
        void Load();

        IActionSetting setting { get; set; }
    }
}
