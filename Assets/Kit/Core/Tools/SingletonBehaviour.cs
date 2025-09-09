using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 싱글톤 클래스
/// 사용 예
///     public class MyClass : NexusSingletonBehaviour<MyClass> { public void func(){} }
///     MyClass.Instance.func();
/// Resources 폴더 하위에 클래스 이름으로 된 프리팹이 있으면 이를 인스턴스화해서 사용하고 그렇지 않으면 새롭게 생성한다.
public partial class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if (null != instance)
                return instance;

            if (!Application.isPlaying)
                return null;

            string typeName = typeof(T).Name;

            var prefab = Resources.Load(typeName);

            if (null != prefab)
                instance = (Instantiate(prefab) as GameObject).GetComponent<T>();

            if (null == instance && Time.timeScale != 0)
                instance = new GameObject(typeof(T).Name).AddComponent<T>();

            if (null == instance.transform.parent)
                DontDestroyOnLoad(instance.gameObject);

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (null != instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this as T;

        if (null == transform.parent)
            DontDestroyOnLoad(gameObject);
    }

    public static bool HasInstance { get { return instance != null; } }

    public static void DestroyInstance()
    {
        if (null != instance)
        {
            if (Application.isPlaying) 
                Object.Destroy(instance.gameObject);
            else 
                Object.DestroyImmediate(instance.gameObject);

            instance = null;
        }
    }
}

public partial class SingletonBehaviour<T>
{
    static T instance;
}