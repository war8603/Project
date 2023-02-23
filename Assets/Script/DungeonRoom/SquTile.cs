using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SquTile : Tile
{
    [SerializeField]
    TextMeshPro _text;
    public enum TileTypes
    {
        None,
        Room,
        RoomSide,
    }

    DungeonEditorManager _manager;
    Material _mat;
    public SquPoint Point => _point;
    SquPoint _point;

    public TileTypes TileType => _tileType;
    TileTypes _tileType;

    public SquTile MainTile => _mainTile;
    SquTile _mainTile;

    public bool IsPath => _isPath;
    bool _isPath = false;

#if UNITY_EDITOR
    public bool IsOver => _isOver;
    bool _isOver = false;
#endif
    // Path정보. 방향과 도착점 정보를 들고있다.
    Dictionary<DirectionType, SquPoint> _paths = new Dictionary<DirectionType, SquPoint>();

    public DungeonTypes DungeonType => _dungeonType;
    DungeonTypes _dungeonType = DungeonTypes.Normal;
    public Dictionary<DirectionType, SquPoint> Paths
    {
        get { return _paths; }
        set { _paths = value; }
    }

    public void Awake()
    {
        _text.text = string.Empty;
        _mat = this.gameObject.GetComponent<Renderer>().material;
    }

    public void SetManager(DungeonEditorManager manager)
    {
        _manager = manager;
    }

    public void SetRoomPos(int x, int y)
    {
        _point = new SquPoint(x, y);
    }

    public void SetTileType(TileTypes tileType)
    {
        _tileType = tileType;
    }

    public void SetDungeonType(DungeonTypes type)
    {
        _dungeonType = type;
    }

    public Tuple<DirectionType, SquPoint> GetPath(SquPoint point)
    {
        foreach(KeyValuePair<DirectionType, SquPoint> kv in _paths)
        {
            if (kv.Value == point)
                return new Tuple<DirectionType, SquPoint>(kv.Key, kv.Value);
        }
        return null;
    }



    public void SetPath(bool enable)
    {
        _isPath = enable;
        if (enable == false)
        {
            _text.text = "";
        }
    }

    public void SetOver(bool enable)
    {
        _isOver = enable;
    }

    public void AddPath(DirectionType dirType, SquPoint point)
    {
        if (_paths.ContainsKey(dirType) == false)
        {
            _paths.Add(dirType, point);
        }
    }

    public void RemovePath(DirectionType dirType)
    {
        foreach(KeyValuePair<DirectionType, SquPoint> kv in _paths)
        {
            if (dirType == kv.Key)
            {
                _paths.Remove(kv.Key);
                return;
            }
        }
    }

    public void RemovePath(SquPoint point)
    {
        foreach(KeyValuePair<DirectionType, SquPoint> kv in _paths)
        {
            if (kv.Value == point)
            {
                _paths.Remove(kv.Key);
                return;
            }
        }
    }

    public void SetPathNumber(string number)
    {
        _text.text = number;
    }

    public void InitPathTile()
    {
        _isPath = false;
    }
    public void SetMainTile(SquTile mainTile)
    {
        _mainTile = mainTile;
    }

    public void OnMouseOver()
    {
        if (_mat == null)
            return;

        _manager.OnMouseOverItem(_point);        
    }

    public void OnMouseDown()
    {
        _manager.OnMouseDown(_point);
    }

    public void SetColor(Color color)
    {
        if (_mat == null)
            return;

        _mat.color = color;
    }
}
