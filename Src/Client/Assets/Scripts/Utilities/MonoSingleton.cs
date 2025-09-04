using UnityEngine;


public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool global = true;
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance =(T)FindObjectOfType<T>();
            }
            return instance;
        }

    }

    void Start()
    {
        if (global)
        {
            if (instance != null &&instance!=gameObject.GetComponent<T>())//不为空说明已经存在单例 
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
            instance = gameObject.GetComponent<T>();
        }
        this.OnStart();
    }

    protected virtual void OnStart()
    {

    }
}