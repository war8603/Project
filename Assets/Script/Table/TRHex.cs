using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TRHex : TRFoundation
{
    string _name;
    string _objName;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public string ObjName
    {
        get { return _objName; }
        set { _objName = value; }
    }

    public override bool ReadRawData(XmlReader reader)
    {
        bool result = true;
        reader.ReadStartElement(GetTableName());
        base.ReadRawData(reader);
        result &= XmlHelper.ReadString(reader, "Name", ref _name);
        result &= XmlHelper.ReadString(reader, "ObjName", ref _objName);
        reader.ReadEndElement();
        return result;
    }
}