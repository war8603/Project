using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IOHelper
{
    public static bool ReadArray2<T>(string desc, string[] splits, ref T[] array)
    {
        try
        {
            int count = splits.Length;
            array = new T[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = (T)System.Convert.ChangeType(splits[i], typeof(T));
            }
        }
        catch
        {
            return false;
        }
        return true;
    }
}
