using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;
    public static bool HasInstance => instance != null;
    public static T TryGetInstance() => HasInstance ? instance : null;
    public bool DestroyObjectOnLoad = true;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        InitializeSingleton();
    }

    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying) return;

        if (instance != null)
        {
            Destroy(this);
        }
        else

        {
            if (!DestroyObjectOnLoad)
                DontDestroyOnLoad(this);
            instance = this as T;
        }
    }
}
