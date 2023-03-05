using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TRPlayer : TRFoundation
{
    string _keyName;
    string _spineDataAsset;
    string[] _motions;
    MovingType _movingType;
    
    int _hp;
    float _moveSpeed;
    int _moveRange;
    int _attackRange;

    public string KeyName
    {
        get { return _keyName; }
        set { _keyName = value; }
    }

    public string SpineDataAsset
    {
        get { return _spineDataAsset; }
        set { _spineDataAsset = value; }
    }

    public string[] Motions
    {
        get { return _motions; }
        set { _motions = value; }
    }

    public MovingType PlayerMovingType
    {
        get { return _movingType; }
        set { _movingType = value; }
    }

    public int Hp
    {
        get { return _hp; }
        set { _hp = value; }
    }

    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    public int MoveRange
    {
        get { return _moveRange; }
        set { _moveRange = value; }
    }

    public int AttackRange
    {
        get { return _attackRange; }
        set { _attackRange = value; }
    }

    public override bool ReadRawData(XmlReader reader)
    {
        bool result = true;
        reader.ReadStartElement(GetTableName());
        base.ReadRawData(reader);
        result &= XmlHelper.ReadString(reader, "KeyName", ref _keyName);
        result &= XmlHelper.ReadString(reader, "SpineDataAsset", ref _spineDataAsset);
        result &= XmlHelper.ReadStringArray(reader, "Motion", ref _motions);
        result &= XmlHelper.ReadEnumInt(reader, "MovingType", ref _movingType);
        result &= XmlHelper.ReadInt(reader, "Hp", ref _hp);
        result &= XmlHelper.ReadFloat(reader, "MoveSpeed", ref _moveSpeed);
        result &= XmlHelper.ReadInt(reader, "MoveRange", ref _moveRange);
        result &= XmlHelper.ReadInt(reader, "AttackRange", ref _attackRange);
        reader.ReadEndElement();
        return result;
    }
}