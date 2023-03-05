using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public static class XmlHelper
{
    static XmlReaderSettings defaultReadsettings;

    public static XmlWriterSettings GetDefaultSetting()
    {
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.OmitXmlDeclaration = false;
        settings.IndentChars = ("\t");
        settings.NewLineChars = "\r\n";
        return settings;
    }

    public static XmlReaderSettings GetDefaultReadSetting()
    {
        if (defaultReadsettings == null)
        {
            defaultReadsettings = new XmlReaderSettings();
            defaultReadsettings.IgnoreComments = true;
            defaultReadsettings.IgnoreWhitespace = true;
            defaultReadsettings.IgnoreProcessingInstructions = true;
        }
        return defaultReadsettings;
    }

    public static bool ReadInt(XmlReader reader, string elementName, ref int result)
    {
        bool readResult = false;
        try
        {
            var desc = elementName;
            reader.ReadStartElement(elementName);
            readResult = ReadXML<int>(reader, ref result, desc);
            reader.ReadEndElement();
        }
        catch (XmlException e)
        {
            Debug.LogError("[Failed] invalid xml node name: " + elementName + ", " + e.Message);
        }

        return readResult;
    }

    public static bool ReadFloat(XmlReader reader, string elementName, ref float result)
    {
        bool readResult = false;
        try
        {
            string desc = elementName;
            reader.ReadStartElement(elementName);
            readResult = ReadXML<float>(reader, ref result, desc);
            reader.ReadEndElement();
        }
        catch (XmlException e)
        {
            Debug.LogError("[Failed] invalid xml node name: " + elementName + ", " + e.Message);
        }

        return readResult;
    }

    public static bool ReadString(XmlReader reader, string elementName, ref string result)
    {
        string desc = elementName;
        bool readResult = false;

        if (reader.IsEmptyElement)
        {
            reader.ReadStartElement();
            result = string.Empty;
            Debug.Log($"[Failed] empty element: {elementName}");
            reader.ReadEndElement();
            return true;
        }
        else
        {
            reader.ReadStartElement(elementName);
            readResult = ReadXML<string>(reader, ref result, desc);
            reader.ReadEndElement();
            return readResult;
        }
    }

    public static bool ReadStringArray(XmlReader reader, string elementName, ref string[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        readResult &= IOHelper.ReadArray2<string>(desc, splits, ref value);
        return readResult;
    }

    public static bool ReadEnumInt<T>(XmlReader reader, string elementName, ref T result) where T : IConvertible
    {
        int tempInt = 0;
        bool readResult = ReadInt(reader, elementName, ref tempInt);
        result = (T)(Enum.ToObject(typeof(T), tempInt));

        return readResult;
    }

    public static bool ReadXML<T>(XmlReader reader, ref T result, string desc = "")
    {
        try
        {
            string elementValue = reader.ReadContentAsString();
            if (string.IsNullOrEmpty(elementValue))
            {
                //DebugHelper.Log(string.Format("[Failed] empty value: {0}, type: {1}", desc, typeof(T).ToString()));
                //return false;
            }

            result = (T)System.Convert.ChangeType(elementValue, typeof(T));
        }
        catch (System.FormatException e)
        {
            if (string.IsNullOrEmpty(desc))
                desc = reader.Name;

            Debug.LogError($"[Failed] invalid value, Name:{desc}, Message:{e.Message}");
            return false;
        }
        catch (System.InvalidCastException e)
        {
            if (string.IsNullOrEmpty(desc))
                desc = reader.Name;

            Debug.LogError("[Failed] invalid value type: " + desc + ", " + e.Message);
            return false;
        }

        return true;
    }
}
