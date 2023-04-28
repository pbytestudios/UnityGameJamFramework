using UnityEngine;

[DefaultExecutionOrder(-200)]
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;
    public static T I
    {
        get
        {
            if (instance == null)
                Debug.LogError($"Instance of Type {typeof(T).Name} was not found!");
            return instance;
        }
    }

    public static bool Exists => instance != null;
    protected static bool applicationQuitting;

    [SerializeField] bool persistAcrossScenes = false;

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning($"Instance of Type {typeof(T).Name} ({instance.name}) is already in the scene! Deleting '{(this as T).name}'");
            Destroy((this as T).gameObject);
        }
        else
        {
            instance = this as T;
            applicationQuitting = false;
            //Set the name to the type name
            instance.gameObject.name = typeof(T).Name;
            if (persistAcrossScenes)
                DontDestroyOnLoad(instance.gameObject);
        }
    }

    protected virtual void OnDestroy() => instance = null;
    protected virtual void OnApplicationQuit() => applicationQuitting  = true;
}
