using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager
{
    private static ResourcesManager _instance;
    public static ResourcesManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourcesManager();
            }
            return _instance;
        }
    }

    public T Load<T>(string path, string name) where T : Object
    {
        T obj = Resources.Load<T>(path + name);
        Debug.Assert(obj != null, "Not Found Obj : " + name);
        return obj;
    }
}
