using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DungeonController
{
    public enum DungeonActType
    {
        Idle,
        MovePlayer,
    }

    public enum MoveType
    {
        Move,
        Teleport,
    }

    DungeonManager _manager;
    UIDungeonView _view;

    DungeonActType _actType;
    public static Dictionary<DirectionType, SquPoint> Dir => _dir;
    static Dictionary<DirectionType, SquPoint> _dir;
    SquPoint _curPoint;

    List<Tuple<MoveType, Vector3>> _path = new List<Tuple<MoveType, Vector3>>();

    List<DungeonItemInfo> _dungeonItemInfos = new List<DungeonItemInfo>();

    bool _isBattleLoad = false;

    public Camera SpineCamera
    {
        get { return _manager.CameraSpine; }
    }

    public DungeonActType ActType
    {
        get { return _actType; }
    }

    public DungeonController(DungeonManager manager)
    {
        _manager = manager;
        SetActType(DungeonActType.Idle);
        InitDirection();
        _isBattleLoad = false;
    }

    private void InitDirection()
    {
        _dir = new Dictionary<DirectionType, SquPoint>();
        _dir.Add(DirectionType.Left, new SquPoint(-1, 0));
        _dir.Add(DirectionType.Right, new SquPoint(1, 0));
        _dir.Add(DirectionType.Up, new SquPoint(0, 1));
        _dir.Add(DirectionType.Down, new SquPoint(0, -1));
    }

    public void Init(GameObject spineRoot, GameObject objRoot)
    {
        InitDungeonData();

        // todo : view에서 아이템을 생성해서 컨트롤러로 넘겨주는걸 바꿔야함
        DungeonData data = FileManager.Instance.LoadDungeonData("123");
        _view.CreateDungeon(data, spineRoot);
        _view.CreatePlayer(spineRoot);

        DungeonItemInfo startInfo = FindStartDungeon();
        SetCurPoint(startInfo.Point);

        _path = new List<Tuple<MoveType, Vector3>>();
        _path.Add(new Tuple<MoveType, Vector3>(MoveType.Teleport, startInfo.Item.Position));
        OnSetMovePlayer();
    }

    void InitDungeonData()
    {
        if (_dungeonItemInfos != null && _dungeonItemInfos.Count > 0)
        {
            for (int i = 0; i < _dungeonItemInfos.Count; i++)
            {
                GameObject.Destroy(_dungeonItemInfos[i].Item.gameObject);
            }
            _dungeonItemInfos.Clear();
        }
        else
        {
            _dungeonItemInfos = new List<DungeonItemInfo>();
        }
    }

    public void OnUpdate()
    {
        if (_isBattleLoad == true)
            return;

        if (_actType == DungeonActType.Idle)
        {
            DungeonItemInfo info = _dungeonItemInfos.Find(x => x.Point == _curPoint);
            if (info.IsUsed == false)
            {
                _isBattleLoad = true;
                _manager.GotoBattle();
            }

            if (Input.GetKeyUp(KeyCode.D) == true)
            {
                OnSetMovePlayer(DirectionType.Right);
            }
            else if (Input.GetKeyUp(KeyCode.A) == true)
            {
                OnSetMovePlayer(DirectionType.Left);
            }
            else if (Input.GetKeyUp(KeyCode.W) == true)
            {
                OnSetMovePlayer(DirectionType.Up);
            }
            else if (Input.GetKeyUp(KeyCode.S) == true)
            {
                OnSetMovePlayer(DirectionType.Down);
            }
        }
        else if (_actType == DungeonActType.MovePlayer)
        {
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        if (_path.Count > 0)
        {
            // 캐릭터 이동.
            // path의 type이 TeleportPlayer 일경우 즉시이동, Move일경우 등속이동이다.
            bool isTeleport = _path[0].Item1 == MoveType.Teleport ? true : false;
            _view.MovePlayer(_path[0].Item2, isTeleport);
        }
        else
        {
            _path = null;
            _actType = DungeonActType.Idle;
        }
    }

    public void OnEndMovePlayer()
    {
        _path.RemoveAt(0);
        if (_path.Count == 0)
        {
            _actType = DungeonActType.Idle;
        }
    }

    void OnSetMovePlayer()
    {
        if (_path.Count > 0)
        {
            _actType = DungeonActType.MovePlayer;
            _manager.SetFlowTarget(_view.Player.transform);
        }
    }

    public void OnSetMovePlayer(DirectionType type)
    {
        DungeonItemInfo startRoom = GetDungeonItemInfo(_curPoint);
        if (startRoom == null)
            return;
        foreach (KeyValuePair<DirectionType, SquPoint> kv in startRoom.Paths)
        {
            if (kv.Key == type)
            {
                // 시작타일의 경로 포지션 탐색
                Vector3 startPathPos = GetPathItemPos(startRoom, kv.Value);

                // 도착타일의 경로 포지션 탐색
                DungeonItemInfo endRoom = _dungeonItemInfos.Find(x => x.Point == kv.Value);
                Vector3 endPathPos = GetPathItemPos(endRoom, _curPoint);

                _path = new List<Tuple<MoveType, Vector3>>();
                _path.Add(new Tuple<MoveType, Vector3>(MoveType.Move, startPathPos));
                _path.Add(new Tuple<MoveType, Vector3>(MoveType.Teleport, endPathPos));
                _path.Add(new Tuple<MoveType, Vector3>(MoveType.Move, endRoom.Item.Position));
                OnSetMovePlayer();

                SetCurPoint(endRoom.Point);
                break;
            }
        }
    }

    Vector3 GetPathItemPos(DungeonItemInfo roomData, SquPoint endPoint)
    {
        foreach (KeyValuePair<DirectionType, SquPoint> kv in roomData.Paths)
        {
            if (kv.Value == endPoint)
            {
                return roomData.PathItems.Find(x => x.Item1 == kv.Key).Item2.transform.position;
            }
        }
        return Vector3.zero;
    }

    DungeonItemInfo GetDungeonItemInfo(SquPoint point)
    {
        return _dungeonItemInfos.Find(x => x.Point == point);
    }

    public void SetActType(DungeonActType actType)
    {
        _actType = actType;
    }

    public void SetCurPoint(SquPoint curPoint)
    {
        _curPoint = curPoint;
    }

    public void CreateView(GameObject uiRoot)
    {
        string viewName = "UIDungeonView";
        GameObject obj = GameObject.Instantiate(ResourcesManager.Instance.Load<GameObject>("Prefabs/UI/", viewName), uiRoot.transform);
        Debug.Assert(obj.GetComponent<UIDungeonView>() != null, "NullReference : " + viewName);
        _view = obj.GetComponent<UIDungeonView>();
        _view.SetController(this);
    }

    public void AddDungeonData(DungeonItemInfo data)
    {
        _dungeonItemInfos.Add(data);
    }

    public DungeonItemInfo FindStartDungeon()
    {
        return _dungeonItemInfos.Find(x => x.DungeonType == DungeonTypes.Start);
    }
}
