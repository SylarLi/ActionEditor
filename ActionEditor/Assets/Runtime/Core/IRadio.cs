namespace Core
{
    public enum RadioState
    {
        None,
        Play,
        Stop
    }

    public interface IRadio
    {
        RadioState state { get; set; }

        bool pause { get; set; }

        float speed { get; set; }

        float progress { get; set; }
    }
}
