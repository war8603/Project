using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public class HexData
    {
        int _hexIndex;
        HexPoint _mapPos;
        HexTile.PossibleType _possibleType;
        HexTile.SpawnAreaType _areaType;
        public int HexInfoIndex
        {
            get { return _hexIndex; }
            set { _hexIndex = value; }
        }

        public HexPoint MapPos
        {
            get { return _mapPos; }
            set { _mapPos = value; }
        }

        public HexTile.PossibleType PossibleType
        {
            get { return _possibleType; }
            set { _possibleType = value; }
        }

        public HexTile.SpawnAreaType AreaType
        {
            get { return _areaType; }
            set { _areaType = value; }
        }
    }

    int _sizeX;
    int _sizeY;
    int _sizeZ;
    string _mapName;

    List<HexData> _hexItems = new List<HexData>();
    List<TRMap> _mapItems = new List<TRMap>();

    public int SizeX
    {
        get { return _sizeX; }
        set { _sizeX = value; }
    }

    public int SizeY
    {
        get { return _sizeY; }
        set { _sizeY = value; }
    }

    public int SizeZ
    {
        get { return _sizeZ; }
        set { _sizeZ = value; }
    }

    public string MapName
    {
        get { return _mapName; }
        set { _mapName = value; }
    }

    public List<HexData> HexDatas
    {
        get { return _hexItems; }
        set { _hexItems = value; }
    }
}

