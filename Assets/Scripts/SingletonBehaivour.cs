using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    public static T instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
        }
        else  if (instance != this)
            Destroy(this);
    }
}
