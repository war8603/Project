using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Func
{ 
    public static string GetProjectPath()
    {
        int index = Application.dataPath.IndexOf("Assets");
        return Application.dataPath.Substring(0, index - 1);
    }
}

#if UNITY_EDITOR
public static class EditorPrefsEx
{
    public static bool GetBoolOrSet(string key, bool value)
    {
        if (EditorPrefs.HasKey(key))
        {
            return EditorPrefsEx.GetBool(key);
        }
        else
        {
            EditorPrefsEx.SetBool(key, value);
        }
        return value;
    }

    public static int GetIntOrSet(string key, int value)
    {
        if (EditorPrefs.HasKey(key))
        {
            return EditorPrefs.GetInt(key);
        }
        else
        {
            EditorPrefs.SetInt(key, value);
        }
        return value;
    }

    public static void SetBool(string key, bool value)
    {
        int intValue = value ? 1 : 0;
        EditorPrefs.SetInt(key, intValue);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        int defaultValueInt = (int)System.Convert.ChangeType(defaultValue, typeof(int));
        int value = EditorPrefs.GetInt(key, defaultValueInt);
        bool result = (value == 1) ? true : false;
        return result;
    }
}
#endif

public class DelimiterHelper
{
    public static readonly char[] Comma = new char[] { ',' };
    public static readonly char[] Period = new char[] { '.' };
    public static readonly char[] Pipe = new char[] { '|' };
    public static readonly char[] Semicolon = new char[] { ';' };
    public static readonly char[] Colon = new char[] { ':' };
    public static readonly char[] Newline = new char[] { '\n' };
    public static readonly char[] Underscore = new char[] { '_' };
}
