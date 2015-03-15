namespace Action
{
    public interface IActionPool
    {
        IActionInfo GetActionInfo(string raw);
    }
}
