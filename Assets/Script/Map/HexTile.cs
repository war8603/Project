using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : Tile
{
    public enum PossibleType
    {
        Empty,
        AllPossible,
        AirPossible,
        Impossible,
    }

    public enum SpawnAreaType
    {
        None,
        FriendArea,
        EnemyArea,
    }

    protected Color _originalColor = Color.white;
    PossibleType _possibleType;
    SpawnAreaType _areaType;
    int _tileIndex = 1;

    HexPoint _point;
    
    public HexPoint Point
    {
        get { return _point; }
    }

    public Color OriginalColor
    {
        get { return _originalColor; }
        set { _originalColor = value; }
    }

    public PossibleType MovingPossibleType
    {
        get { return _possibleType; }
        set { _possibleType = value; }
    }

    public SpawnAreaType AreaType
    {
        get { return _areaType; }
        set { _areaType = value; }
    }
    public int TileIndex
    {
        get { return _tileIndex; }
        set { _tileIndex = value; }
    }
    public void SetPoint(HexPoint point)
    {
        _point = new HexPoint(point.x, point.y, point.z);
    }
}
