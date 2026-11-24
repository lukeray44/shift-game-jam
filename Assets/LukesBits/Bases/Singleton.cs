using System.Collections;
using UnityEngine;


//I know singletons are bad practise but I don't care
//Sue me
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{

    public static T main { get; protected set; }
    public void Awake()
    {
        if (main != null && main != this) Destroy(gameObject);
        else main = this as T;
    }
    bool initialised;
    private void Start()
    {
        init();
    }

    public void init()
    {
        if (!initialised)
        {
            initialised = true;
            onInitialise();
        }
    }
    public static T addInstance()
    {
        main = new GameObject("~" + typeof(T).ToString()).AddComponent<T>();
        main.init();
        return main;
    }
    protected virtual void onInitialise() { }
    public static bool InstanceExists => main != null;
}

//Singleton where it creates an instance if it doesn't exist
public abstract class AdditiveSingleton<T> : MonoBehaviour where T : AdditiveSingleton<T>
{
    protected static T mainInstance;
    public static T main
    {
        get
        {
            if (mainInstance == null && Application.isPlaying)
            {
                mainInstance = new GameObject("~" + typeof(T).ToString()).AddComponent<T>();
                mainInstance.onInitialise();
            }
            return mainInstance;
        }
    }
    public static T AddInstance()
    {
        if (mainInstance == null && Application.isPlaying)
        {
            mainInstance = new GameObject("~" + typeof(T).ToString()).AddComponent<T>();
            mainInstance.onInitialise();
        }
        return mainInstance;
    }
    public static T OverrideAddInstance()
    {
        if (Application.isPlaying)
        {
            mainInstance = new GameObject("~" + typeof(T).ToString()).AddComponent<T>();
            mainInstance.onInitialise();
            
        }
        return mainInstance;
    }
    public static T instance => mainInstance;
    private void Update()
    {
        if (mainInstance != this)
        {
            if (!Application.isPlaying) DestroyImmediate(gameObject);
            else Destroy(gameObject);
        }
    }

    public static bool InstanceExists => mainInstance != null;

    protected virtual void onInitialise() { }
}
