public class Singleton<T> where T : class, new()
{
    private static T _inst;

    public static T inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = new T();
            }
            return _inst;
        }
    }
}
