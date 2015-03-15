using System.Collections.Generic;
using UnityEngine;

namespace Action
{
    public interface IDependInfo : IDictionary<DependType, object>
    {
        IList<GameObject> actors { get; }

        int actorLimit { get; }

        object FindActor(string indexOrRange);

        void SetActor(string indexOrRange, object actor);
    }
}
