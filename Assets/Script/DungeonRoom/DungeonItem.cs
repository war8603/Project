using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DungeonDirectionType
{
    Left,
    Right,
    Up,
    Down,
}

public class DungeonItemInfo
{
    DungeonTypes _dungeonType;      // 던전의 타입
    DungeonItem _item;              // 던전의 GameObject
    SquPoint _point;                // 던전의 위치 point
    
    bool _isUsed = false;           // 던전을 진입하였는지 체크하는 변수

    List<Tuple<DirectionType, GameObject>> _pathItems = new List<Tuple<DirectionType, GameObject>>();
    Dictionary<DirectionType, SquPoint> _paths = new Dictionary<DirectionType, SquPoint>();

    public DungeonTypes DungeonType
    {
        get { return _dungeonType; }
        set { _dungeonType = value; }
    }

    public DungeonItem Item
    {
        get { return _item; }
        set { _item = value; }
    }

    public SquPoint Point
    {
        get { return _point; }
        set { _point = value; }
    }

    public bool IsUsed
    {
        get { return _isUsed; }
        set { _isUsed = value; }
    }

    public Dictionary<DirectionType, SquPoint> Paths
    {
        get { return _paths; }
        set { _paths = value; }
    }

    public List<Tuple<DirectionType, GameObject>> PathItems
    {
        get { return _pathItems; }
        set { _pathItems = value; }
    }
}

public class DungeonItem : MonoBehaviour
{
    [SerializeField]
    Transform _playerDestPos;

    [SerializeField]
    SpriteRenderer _renderer;

    public Transform PlayerDestPos => _playerDestPos;
    public Vector2 RectSize
    {
        get { return _renderer.GetComponent<Renderer>().bounds.size; }
    }

    public Vector3 Position
    {
        get { return transform.position; }
    }
}
