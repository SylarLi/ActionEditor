namespace Action
{
    public interface IActionPlayer
    {
        IActionInfo Build(IDependInfo dependInfo, string raw);

        IActionInfo Build(IDependInfo dependInfo, IActionInfo actionInfo);
    }
}
