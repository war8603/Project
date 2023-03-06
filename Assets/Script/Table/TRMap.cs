using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TRMap : TRFoundation
{
    string _mapName;

    public string MapName
    {
        get { return _mapName; }
        set { _mapName = value; }
    }

    public override bool ReadRawData(XmlReader reader)
    {
        bool result = true;
        reader.ReadStartElement(GetTableName());
        base.ReadRawData(reader);
        result &= XmlHelper.ReadString(reader, "MapName", ref _mapName);
        reader.ReadEndElement();
        return result;
    }
}