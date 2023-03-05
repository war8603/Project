using UnityEngine;
using System;
using System.Xml;
using System.IO;

public class TRFoundation
{
     protected int _index;

    public int Index { get => _index; set => _index = value; }
    protected bool _hasDevNote = true;

    public string GetTableName()
    {
        return this.GetType().ToString();
    }

    public virtual bool ReadRawData(XmlReader reader)
    {
        bool result = true;
        result &= XmlHelper.ReadInt(reader, "Index", ref _index);
        if (_hasDevNote)
        {
            string dev = String.Empty;
            result &= XmlHelper.ReadString(reader, "DevNote", ref dev);
        }
        return result;
    }

    public static void ReadStartRoot(XmlReader reader)
    {
        reader.ReadStartElement("Root");
    }

    public static void ReadEndRoot(XmlReader reader)
    {
        reader.ReadEndElement();
    }
}
