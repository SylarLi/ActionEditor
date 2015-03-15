using UnityEngine;

namespace Action
{
    public interface IActionLoader
    {
        void GetEffect(int id, System.Action<GameObject> callBack);

        void GetSound(int id, System.Action<GameObject> callBack);

        void ReturnEffect(int id, GameObject resource);

        void ReturnSound(int id, GameObject resource);
    }
}
