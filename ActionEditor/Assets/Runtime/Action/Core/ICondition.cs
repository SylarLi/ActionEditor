namespace Action
{
    public interface ICondition
    {
        ConditionType type { get; }

        float progress { get; set; }

        bool Meet(float value);

        ICondition Clone();
    }
}
