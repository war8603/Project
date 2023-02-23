using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRMap
{
    int _index;
    string _mapName;
    MapData _mapItemData;

    public int Index
    {
        get { return _index; }
        set { _index = value; }
    }

    public string MapName
    {
        get { return _mapName; }
        set { _mapName = value; }
    }

    public MapData MapItemData
    {
        get { return _mapItemData; }
        set { _mapItemData = value; }
    }

}




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

public class TRHex
{
    int _index;
    string _name;
    string _objName;
    
    public int Index
    {
        get { return _index; }
        set { _index = value; }
    }

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
}

public class MapManager
{
    public enum DirectionType
    {
        Right,
        UpRight,
        UpLeft,
        Left,
        DownLeft,
        DownRight,
    }

    #region Singleton
    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        { 
            if (_instance == null)
            {
                _instance = new MapManager();
                Init();
            }
            return _instance;
        }
    }
    #endregion

    #region Map Parameter
    HexTile[][][] _map;
    List<HexTile> _hexLists;
    MapData _mapDataInfo;
    public MapData MapDataInfo
    {
        get { return _mapDataInfo; }
    }

    public List<HexTile> HexList
    {
        get { return _hexLists; }
    }
    #endregion

    #region GetPath parameter
    public static HexPoint[] Dirs;
    List<HexPath> _openList = new List<HexPath>();
    List<HexPath> _closedList = new List<HexPath>();
    #endregion

    #region Hex Parameter
    static string DefaultHexName = "Land";
    static string HexObjPath = "Prefabs/Hex/";

    bool _isSetHexSize = false;
    float _hexW = float.MaxValue;
    float _hexH = float.MaxValue;
    public float HexW
    {
        get { return _hexW; }
        set { _hexW = value; }
    }

    public float HexH
    {
        get { return _hexH; }
        set { _hexH = value; }
    }
    #endregion

    static void Init()
    {
        InitDirs();
    }

    void SetHexSize()
    {
        GameObject obj = ResourcesManager.Instance.Load<GameObject>(HexObjPath, DefaultHexName);
        _hexW = obj.transform.GetComponent<Renderer>().bounds.size.x;
        _hexH = obj.transform.GetComponent<Renderer>().bounds.size.z;
    }

    static void InitDirs()
    {
        Dirs = new HexPoint[6];

        Dirs[(int)DirectionType.Right] = new HexPoint(+1, -1, 0); // right
        Dirs[(int)DirectionType.UpRight] = new HexPoint(+1, 0, -1); // up right
        Dirs[(int)DirectionType.UpLeft] = new HexPoint(0, +1, -1); // up left
        Dirs[(int)DirectionType.Left] = new HexPoint(-1, +1, 0); // left
        Dirs[(int)DirectionType.DownLeft] = new HexPoint(-1, 0, +1); // down left
        Dirs[(int)DirectionType.DownRight] = new HexPoint(0, -1, +1); // down right
    }

    public Vector3 GetWorldPos(int x, int y, int z)
    {
        float xValue = 0f;
        float zValue = 0f;

        xValue = x * HexW + (z * HexW * 0.5f);
        zValue = (-z) * HexH * 0.75f;

        return new Vector3(xValue, 0f, zValue);
    }

    public Vector3 GetWorldPos(HexPoint point)
    {
        return GetWorldPos(point.x, point.y, point.z);
    }

    string GetHexObjName(HexTile.PossibleType type)
    {
        if (type == HexTile.PossibleType.AirPossible)
            return "AirPossible";
        else if (type == HexTile.PossibleType.Impossible)
            return "Impossible";
        else
            return DefaultHexName;
    }

    public GameObject CreateHexObj(int hexIndex, Transform root)
    {
        List<TRHex> hexInfos = DataManager.Instance.TRHexes;
        if (hexInfos == null || hexInfos.Count == 0)
            return null;
        TRHex hexInfo = hexInfos.Find(x => x.Index == hexIndex);

        GameObject baseObj = null;
        if (hexInfo != null)
            baseObj = ResourcesManager.Instance.Load<GameObject>(HexObjPath, hexInfo.ObjName);
        
        if (baseObj == null)
            baseObj = ResourcesManager.Instance.Load<GameObject>(HexObjPath, DefaultHexName);

        GameObject obj = GameObject.Instantiate(baseObj);
        obj.transform.parent = root;

        if (_isSetHexSize == false)
        {
            _isSetHexSize = true;
            _hexW = obj.transform.GetComponent<Renderer>().bounds.size.x;
            _hexH = obj.transform.GetComponent<Renderer>().bounds.size.z;
        }

        return obj;
    }

    public void CreateMap(Transform root, MapData mapDataInfo, bool isEditor = false)
    {
        List<TRHex> hexInfos = DataManager.Instance.TRHexes;
        // todo : hex 종류를 여러가지로 만들고, 오브젝트 풀을 만들어야함.
        if (_map != null)
        {
            for (int i = 0; i < _map.Length; i++)
            {
                for (int j = 0; j < _map[i].Length; j++)
                {
                    for (int z = 0; z < _map[i][j].Length; z++)
                    {
                        if (_map[i][j][z] != null)
                        {
                            GameObject.Destroy(_map[i][j][z].gameObject);
                            _map[i][j][z] = null;
                        }
                    }
                }
            }
        }
        
        _hexLists = new List<HexTile>();

        _mapDataInfo = mapDataInfo;
        _map = new HexTile[MapDataInfo.SizeX * 2 + 1][][];
        for (int x = -MapDataInfo.SizeX ; x <= MapDataInfo.SizeX; x++)
        {
            _map[x + MapDataInfo.SizeX] = new HexTile[MapDataInfo.SizeY * 2 + 1][];
            for (int y = -MapDataInfo.SizeY; y <= MapDataInfo.SizeY; y++)
            {
                _map[x + MapDataInfo.SizeX][y + MapDataInfo.SizeY] = new HexTile[MapDataInfo.SizeZ * 2 + 1];
                for (int z = -MapDataInfo.SizeZ; z <= MapDataInfo.SizeZ; z++)
                {
                    if (x + y + z == 0)
                    {
                        // 맵 오브젝트 생성
                        HexPoint mapPos = new HexPoint(x, y, z);
                        MapData.HexData hexData = mapDataInfo.HexDatas.Find(x => x.MapPos == mapPos);
                        int hexIndex = hexData == null ? 1 : hexData.HexInfoIndex;

                        TRHex hexInfo = hexInfos.Find(x => x.Index == hexIndex);
                        if (hexInfo == null)
                            Debug.Assert(hexInfo == null, string.Format("Not Found HexInfo index : " + 1));

                        GameObject obj = CreateHexObj(hexIndex, root);
                        obj.name = obj.name + _hexLists.Count.ToString();

                        HexTile hex = isEditor == true ? obj.AddComponent<HexEditor>() : obj.AddComponent<HexTile>();
                        hex.TileIndex = hexIndex;
                        SetHex(hex, x, y, z);
                        hex.MovingPossibleType = hexData == null ? HexTile.PossibleType.AllPossible : hexData.PossibleType;
                        hex.AreaType = hexData == null ? HexTile.SpawnAreaType.None : hexData.AreaType;
                        _hexLists.Add(hex);
                    }
                }
            }
        }
    }

    public void SetHex(HexTile hex, int x, int y, int z)
    {
        hex.gameObject.transform.position = GetWorldPos(x, y, z);
        _map[x + MapDataInfo.SizeX][y + MapDataInfo.SizeY][z + MapDataInfo.SizeZ] = hex;
        _map[x + MapDataInfo.SizeX][y + MapDataInfo.SizeY][z + MapDataInfo.SizeZ].SetPoint(new HexPoint(x, y, z));

        if (_hexLists != null)
        {
            HexPoint oldMapPos = new HexPoint(x, y, z);
            HexTile oldHex = _hexLists.Find(x => x.Point == oldMapPos);
            if (oldHex != null)
            {
                _hexLists.Remove(oldHex);
                _hexLists.Add(hex);
            }
        }
    }

    public void SetHex(HexTile hex, HexPoint mapPos)
    {
        SetHex(hex, mapPos.x, mapPos.y, mapPos.z);
    }

    public HexTile GetHex(int x, int y, int z)
    {
        return _map[x + MapDataInfo.SizeX][y + MapDataInfo.SizeY][z + MapDataInfo.SizeZ];
    }

    public HexTile GetHex(HexPoint p)
    {
        return _map[p.x + MapDataInfo.SizeX][p.y + MapDataInfo.SizeY][p.z + MapDataInfo.SizeZ];
    }

    public HexTile GetHex(Vector3 vec)
    {
        return GetHex((int)vec.x, (int)vec.y, (int)vec.z);
    }

    public bool HighLightMoveRange(HexTile start, int moveRange)
    {
        int moveableCount = 0;
        //for (int x = -MapDataInfo.SizeX; x <= MapDataInfo.SizeX; x++)
        //{
        //    for (int y = -MapDataInfo.SizeY; y <= MapDataInfo.SizeY; y++)
        //    {
        //        for (int z = -MapDataInfo.SizeZ; z <= MapDataInfo.SizeZ; z++)
        //        {
        //            if (x + y + z == 0)
        //            {
        //                Hex hex = _map[x + MapDataInfo.SizeX][y + MapDataInfo.SizeY][z + MapDataInfo.SizeZ];
        //                Point pos1 = start.MapPos;
        //                Point pos2 = hex.MapPos;
        //                int distance = GetDistance(pos1, pos2);
        //                if (distance <= moveRange && distance != 0)
        //                {
        //                    if (IsMoveable(start, hex, moveRange) == true)
        //                    {
        //                        moveableCount++;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        return moveableCount > 0;
    }

    public bool HighLightAttackRange(HexTile start, int attackRange)
    {
        int attackable = 0;
        for (int x = -MapDataInfo.SizeX; x <= MapDataInfo.SizeX; x++)
        {
            for (int y = -MapDataInfo.SizeY; y <= MapDataInfo.SizeY; y++)
            {
                for (int z = -MapDataInfo.SizeZ; z <= MapDataInfo.SizeZ; z++)
                {
                    if (x + y + z == 0)
                    {
                        HexTile hex = _map[x + MapDataInfo.SizeX][y + MapDataInfo.SizeY][z + MapDataInfo.SizeZ];
                        if (IsAttackable(start, hex, attackRange) == true)
                        {
                            attackable++;
                        }
                    }
                }
            }
        }
        return attackable > 0;
    }


    public bool IsAttackable(HexTile start, HexTile dest, int attackRange)
    {
        HexPoint pos1 = start.Point;
        HexPoint pos2 = dest.Point;

        // 공격 거리가 벗어낫을 경우
        int distance = GetDistance(pos1, pos2);
        if (distance > attackRange)
            return false;

        //공격할 개체가 없는경우
        bool isExistPlayer = false;
        foreach( BattlePlayerBase player in PlayerManager.Instance.Players)
        {
            if (player as AIPlayer)
            {
                if (player.CurHex.Point == dest.Point)
                {
                    isExistPlayer = true;
                }
            }
        }
        if (isExistPlayer == false)
            return false;

        return true;
    }

    public void OnChangeHexColor(HexPoint mapPos, Color defaultColor, PlayerType playerType)
    {
        for (int i = 0; i < _hexLists.Count; i++)
        {
            if (mapPos == _hexLists[i].Point)
            {
                _hexLists[i].GetComponent<Renderer>().material.color = Color.red;
            }
            else if (IsSpawnableByAreaType(_hexLists[i].AreaType, playerType) == true)
            {
                _hexLists[i].GetComponent<Renderer>().material.color = defaultColor;
            }
            else
            {
                _hexLists[i].GetComponent<Renderer>().material.color = _hexLists[i].OriginalColor;
            }
        }
    }

    public void OnResetHexColor()
    {
        for(int i = 0; i < _hexLists.Count; i++)
        {
            _hexLists[i].GetComponent<Renderer>().material.color = _hexLists[i].OriginalColor;
        }
    }

    public bool IsSpawnableByAreaType(HexTile.SpawnAreaType areaType, PlayerType playerType)
    {
        HexTile.SpawnAreaType spawnableType = HexTile.SpawnAreaType.None;
        switch (playerType)
        {
            case PlayerType.Friend:
                spawnableType = HexTile.SpawnAreaType.FriendArea;
                break;
            case PlayerType.Enemy:
                spawnableType = HexTile.SpawnAreaType.EnemyArea;
                break;
            default:
                spawnableType = HexTile.SpawnAreaType.None;
                break;
        }

        return areaType == spawnableType;
    }

    public HexTile GetRandomSpawnPos(MovingType movingType, PlayerType playerType)
    {
        List<HexTile> hexes = _hexLists.FindAll(x => IsSpawnableByAreaType(x.AreaType, playerType) == true);
        hexes = hexes.FindAll(x => PlayerManager.Instance.IsOverlapPos(x.Point) == false);
        while (true)
        {
            int randomValue = Random.Range(0, hexes.Count);

            if (IsMovable(hexes[randomValue], movingType) == false)
                continue;

            return hexes[randomValue];
        }
    }

    public HexTile GetRandomPos(MovingType movingType)
    {
        while (true)
        {
            int randomValue = Random.Range(0, _hexLists.Count);

            if (PlayerManager.Instance.IsOverlapPos(_hexLists[randomValue].Point) == true)
                continue;

            if (IsMovable(_hexLists[randomValue], movingType) == false)
                continue;

            return _hexLists[randomValue];
        }
    }

    public int GetDistance(HexTile start, HexTile Dest)
    {
        return GetDistance(start.Point, Dest.Point);
    }

    public int GetDistance(HexPoint p1, HexPoint p2)
    {
        return (Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y) + Mathf.Abs(p1.z - p2.z)) / 2;
    }

    public List<HexTile> GetPath(HexTile start, HexTile dest, MovingType movingType)
    {
        List<HexTile> paths = new List<HexTile>();
        _openList = new List<HexPath>();
        _closedList = new List<HexPath>();

        int H = GetDistance(start.Point, dest.Point);
        HexPath path = new HexPath(null, start, 0, H);
        _closedList.Add(path);


        HexPath result = RecursiveFindPath(path, dest, movingType);
        if (result == null)
        {
            return paths;
        }

        while (result.Parent != null)
        {
            paths.Insert(0, result.Tile);
            result = result.Parent;
        }
        return paths;
    }

    HexPath RecursiveFindPath(HexPath parent, HexTile dest, MovingType movingType)
    {
        if (parent.Tile.Point == dest.Point)
        {
            // 목적지를 찾은 경우
            return parent;
        }

        List<HexTile> neibhors = GetNeibhors(parent.Tile, movingType);     
        foreach (HexTile hex in neibhors)
        {
            HexPath newPath = new HexPath(parent, hex, parent.G + 1, GetDistance(hex, dest));
            AddOpenList(newPath);
        }

        if (_openList.Count == 0)
        { 
            // 목적지까지 가는 길이 없는 경우
            return null;
        }


        HexPath bestPath = _openList[0];
        foreach (HexPath path in _openList)
        {
            if (path.F <= bestPath.F)
            {
                bestPath = path;
            }
        }
        _openList.Remove(bestPath);
        _closedList.Add(bestPath);
        return RecursiveFindPath(bestPath, dest, movingType);
    }

    void AddOpenList(HexPath newPath)
    {
        foreach (HexPath path in _closedList)
        {
            if (path.Tile.Point == newPath.Tile.Point)
            {
                return;
            }
        }

        foreach (HexPath path in _openList)
        {
            if (newPath.Tile.Point == path.Tile.Point)
            {
                if (newPath.F < path.F)
                {
                    _openList.Remove(path);
                    _openList.Add(newPath);
                    return;
                }
            }
        }
        _openList.Add(newPath);
    }

    public List<HexTile> GetNeibhors(HexTile pos, MovingType movingType)
    {
        List<HexTile> neibhors = new List<HexTile>();
        HexPoint curPoint = pos.Point;
        foreach(HexPoint dir in Dirs)
        {
            HexPoint newPoint = curPoint + dir;
            if ( newPoint.x + newPoint.y + newPoint.z == 0)
            {
                // 갈수 있는 길인지 체크
                if (IsMovable(newPoint, movingType) == true)
                {
                    neibhors.Add(GetHex(newPoint.x, newPoint.y, newPoint.z));
                }
            }
        }
        return neibhors;
    }

    public List<HexTile> GetAttackAblePos(HexTile targetPos, int attackRange)
    {
        List<HexTile> neibhors = new List<HexTile>();
        HexPoint curPoint = targetPos.Point;
        foreach (HexPoint dir in Dirs)
        {
            for(int i = 0; i <= attackRange; i++)
            {
                HexPoint newPoint = curPoint + (dir * i);
                if (newPoint.x + newPoint.y + newPoint.z == 0)
                {
                    // 존재하는 위치인지 체크
                    if (IsContainsHex(newPoint) == true)
                    {
                        neibhors.Add(GetHex(newPoint.x, newPoint.y, newPoint.z));
                    }
                }
            }
        }
        return neibhors;
    }
    
    public bool IsMovable(HexPoint newPoint, MovingType movingType)
    {
        // 해당 위치가 실제로 존재하는 위치인지 체크
        if (IsContainsHex(newPoint) == false)
            return false;

        // 해당 위치가 갈수 있는 길인지 체크
        if (IsMovable(GetHex(newPoint).MovingPossibleType, movingType) == false)
            return false;

        return true;
    }

    public bool IsMovable(HexTile hex, MovingType movingType)
    {
        return IsMovable(hex.Point, movingType);
    }

    bool IsMovable(HexTile.PossibleType possibleType, MovingType movingType)
    {
        switch (movingType)
        {
            case MovingType.None:
                if (possibleType == HexTile.PossibleType.Impossible)
                    return false;
                if (possibleType == HexTile.PossibleType.Empty)
                    return false;
                return true;
            case MovingType.Walk:
                if (possibleType == HexTile.PossibleType.AllPossible)
                    return true;
                return false;
            case MovingType.Fly:
                if (possibleType == HexTile.PossibleType.AllPossible)
                    return true;
                if (possibleType == HexTile.PossibleType.AirPossible)
                    return true;
                return false;
            default:
                return true;
        }
    }

    bool IsContainsHex(HexPoint point)
    {
        if (Mathf.Abs(point.x) > MapDataInfo.SizeX)
            return false;

        if (Mathf.Abs(point.y) > MapDataInfo.SizeY)
            return false;

        if (Mathf.Abs(point.z) > MapDataInfo.SizeZ)
            return false;

        return true;
    }
}
