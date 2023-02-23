using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools
{
    public static void SetLayer(Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach(Transform child in trans)
        {
            SetLayer(child, name);
        }
    }
}
