using Core;
using System.Collections.Generic;

namespace Action
{
    public interface IActionInfo : IEventDispatcher
    {
        ActionType type { get; }

        ActionState state { get; set; }

        bool paused { get; set; }

        float speed { get; set; }

        float progress { get; set; }

        bool ready { get; set; }

        DependType[] dependTypes { get; }

        string[] dependDescripts { get; }

        string[] dependActorIndexes { get; set; }

        void InjectDepends(object[] depends);

        bool IsDependValid();

        void InjectCondition(ConditionType type, float val = 0);

        IActionInfo Clone();

        ICollection<ICondition> Keys { get; }

        ICollection<IActionInfo> Values { get; }

        IActionInfo this[ICondition key] { get; set; }

        void Add(ICondition key, IActionInfo value);

        bool ContainsKey(ICondition key);

        bool Remove(ICondition key);

        void Clear();
    }
}
