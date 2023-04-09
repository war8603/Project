using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    Dictionary<GameObject, bool> _objectPool = new Dictionary<GameObject, bool>();
    GameObject _objRoot;

    public void CreateObjectPool(List<string> effectNames, GameObject root)
    {
        _objRoot = root;
        for (int i = 0; i < effectNames.Count; i++)
        {
            CreateObject(effectNames[i]);
        }
    }

    public GameObject GetObject(string objName)
    {
        GameObject obj = null;
        foreach (var item in _objectPool)
        {
            if (item.Key.name == objName && item.Value == false)
            {
                obj = item.Key;
                break;
            }
        }

        if (obj == null)
        {
            obj = CreateObject(objName);
        }

        SetUseObject(obj);
        return obj;
    }

    GameObject CreateObject(string effectName)
    {
        GameObject effectObj = GameObject.Instantiate(ResourcesManager.Instance.Load<GameObject>("Prefabs/Effect/", effectName), _objRoot.transform);
        effectObj.name = effectName;
        effectObj.gameObject.SetActive(false);
        _objectPool.Add(effectObj, false);
        return effectObj;
    }


    void SetUseObject(GameObject key)
    {
        _objectPool[key] = true;
        key.gameObject.SetActive(true);
    }

    public void SetReturnObject(GameObject key)
    {
        key.transform.SetParent(_objRoot.transform);
        key.gameObject.SetActive(false);
        _objectPool[key] = false;
    }

    public void ClearObjectPool()
    {
        foreach(var item in _objectPool)
        {
            GameObject.Destroy(item.Key);
        }
        _objectPool.Clear();
    }
}
