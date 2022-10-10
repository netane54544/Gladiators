using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
    where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                var objs = FindObjectsOfType(typeof(T)) as T[];

                if (objs.Length > 0)
                    instance = objs[0];
                if (objs.Length > 1)
                    Debug.LogWarning("More than one" + typeof(T).Name + "is in the scene");

                if(instance == null) //Still haven't found
                {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    instance = obj.AddComponent<T>();
                }
            }

            return instance;
        }
    }

}
