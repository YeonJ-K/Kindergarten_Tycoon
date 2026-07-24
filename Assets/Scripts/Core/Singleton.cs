using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        protected virtual bool IsPersistent => true;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = GameObject.Find(typeof(T).Name);
                    if (obj == null)
                    {
                        obj = new GameObject(typeof(T).Name);
                        instance = obj.AddComponent<T>();
                    }
                    else
                    {
                        instance = obj.GetComponent<T>();
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
           if (IsPersistent)
               DontDestroyOnLoad(gameObject);   
        }

        public static T GetInstance() => instance;
    }

    public class Singleton<T> where T : class, new()
    {
        public static T Instance { get; } = new T();
    }
}