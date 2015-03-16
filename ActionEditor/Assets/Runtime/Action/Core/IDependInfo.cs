using System.Collections.Generic;
using UnityEngine;

namespace Action
{
    public interface IDependInfo : IDictionary<DependType, object>
    {
        IList<GameObject> actors { get; }

        IList<Vector3> points { get; }

        int actorLimit { get; }

        int pointLimit { get; }

        object FindActor(string indexOrRange);

        void SetActor(string indexOrRange, object actor);

        object FindPoint(string indexOrRange);

        void SetPoint(string indexOrRange, object point);
    }
}
