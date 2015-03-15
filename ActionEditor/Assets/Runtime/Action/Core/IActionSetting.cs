namespace Action
{
    public interface IActionSetting
    {
        IActionLoader loader { get; set; }

        string defaultActorAction { get; set; }
    }
}
