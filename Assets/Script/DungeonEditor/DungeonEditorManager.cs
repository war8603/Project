using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEditorManager : MonoBehaviour
{
    [SerializeField]
    Transform _mapRoot;

    Vector3 _itemSize;
    int _sizeX = 20;
    int _sizeY = 20;

    Dictionary<DirectionType, SquPoint> _dir = new Dictionary<DirectionType, SquPoint>();

    // 타일 관련 매개변수
    public List<SquTile> Tiles => _tiles;
    List<SquTile> _tiles = new List<SquTile>();

    // FSM 관련 매개변수
    DungeonEditorStateManager _fsmManager;

    // GUI 옵션
    public GUILayoutOption[] DungeonNameFileOption => _dungeonNameFileOption;
    GUILayoutOption[] _dungeonNameFileOption;
    GUIStyle _mapSizeFontStyle;

    #region COLOR
    static Color COLOR_DUNGEONTYPE_START = Color.magenta;
    static Color COLOR_DUNGEONTYPE_NORMAL = Color.blue;
    static Color COLOR_DUNGEONTYPE_BOSS = Color.yellow;
    static Color IMPOSSIBLE_COLOR = Color.red;

    static Color FRIENDAREA_COLOR = Color.green;
    static Color ENEMYAREA_COLOR = Color.gray;
    #endregion

    string _dungeonName;
    public void Awake()
    {
        InitDir();
        SetGUIOption();
        CreateTiles();

        _fsmManager = DungeonEditorStateManager.Instance;
        _fsmManager.Init(this);
    }

    public void Start()
    {
        StartCoroutine(RefreshTileColor());
    }

    IEnumerator RefreshTileColor()
    {
        while(true)
        {
            if (_tiles != null)
            {
                for (int i = 0; i < _tiles.Count; i++)
                {
                    SetTileColor(_tiles[i]);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        _fsmManager?.OnUpdate();
    }

    void CreateDungeon(DungeonData data)
    {
        CreateTiles();
        _dungeonName = data.DungeonName;
        for(int i = 0; i < data.Infos.Count; i++)
        {
            // 룸 생성
            SquTile mainTile = _tiles.Find(x => x.Point == data.Infos[i].Point);
            if (mainTile != null)
            {
                mainTile.SetDungeonType(data.Infos[i].dungeonType);
                mainTile.SetTileType(SquTile.TileTypes.Room);
                mainTile.SetMainTile(mainTile);

                // path 설정
                mainTile.Paths = data.Infos[i].Path;
                foreach(KeyValuePair<DirectionType, SquPoint> kv in mainTile.Paths)
                {
                    SquTile pathTile = GetDirTile(mainTile, kv.Key);
                    if (pathTile != null)
                        pathTile.SetPath(true);
                }

                // 사이드 룸 생성
                foreach (DirectionType type in Enum.GetValues(typeof(DirectionType)))
                {
                    SquPoint sidePoint = mainTile.Point + _dir[type];
                    SquTile sideTile = _tiles.Find(x => x.Point == sidePoint);
                    if (sideTile != null)
                    {
                        sideTile.SetTileType(SquTile.TileTypes.RoomSide);
                        sideTile.SetMainTile(mainTile);
                    }
                }
            }
        }
        SetPathNumber();
    }

    void CreateTiles()
    {
        if (_tiles != null && _tiles.Count > 0)
        {
            for (int i = 0; i < _tiles.Count; i++)
            {
                Destroy(_tiles[i].gameObject);
            }
            _tiles.Clear();
        }
        GameObject baseObj = ResourcesManager.Instance.Load<GameObject>("Prefabs/Hex/", "Cube");
        _itemSize = baseObj.GetComponent<Renderer>().bounds.size;
        _itemSize = new Vector3(_itemSize.x + _itemSize.x / 10, _itemSize.y + _itemSize.y / 10, _itemSize.z + _itemSize.z / 10);

        for (int i = 0; i < _sizeX; i++)
        {
            for (int j = 0; j < _sizeY; j++)
            {
                GameObject itemObj = GameObject.Instantiate(baseObj);
                itemObj.transform.SetParent(_mapRoot.transform);
                SquTile item = itemObj.GetComponent<SquTile>();
                item.SetManager(this);
                item.SetRoomPos(i, j);
                item.transform.position = GetWorldPosition(i, j);
                _tiles.Add(item);
            }
        }
    }

    void InitDir()
    {
        _dir.Add(DirectionType.Left, new SquPoint(-1, 0));
        _dir.Add(DirectionType.LeftUp, new SquPoint(-1, 1));
        _dir.Add(DirectionType.Up, new SquPoint(0, 1));
        _dir.Add(DirectionType.RightUp, new SquPoint(1, 1));
        _dir.Add(DirectionType.Right, new SquPoint(1, 0));
        _dir.Add(DirectionType.RightDown, new SquPoint(1, -1));
        _dir.Add(DirectionType.Down, new SquPoint(0, -1));
        _dir.Add(DirectionType.LeftDown, new SquPoint(-1, -1));
    }

    void SetGUIOption()
    {
        #region Dungeon Name Field Options
        _dungeonNameFileOption = new GUILayoutOption[2];
        _dungeonNameFileOption[0] = GUILayout.Width(150f);
        _dungeonNameFileOption[1] = GUILayout.Height(20);
        #endregion

        #region Map Size Fond Style
        _mapSizeFontStyle = new GUIStyle();
        _mapSizeFontStyle.fontSize = 12;
        _mapSizeFontStyle.normal.textColor = Color.white;
        #endregion
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        float xPos = _itemSize.x * (x + 1) - _itemSize.x / 2;
        xPos = xPos - ((float)_sizeX / (float)2 * _itemSize.x);

        float yPos = _itemSize.y * (y + 1) - _itemSize.y / 2;
        yPos = yPos - ((float)_sizeY / (float)2 * _itemSize.y);
        return new Vector3(xPos, 0f, yPos);
    }

    public void OnMouseOverItem(SquPoint point)
    {
        SquTile overCenterTile = _tiles.Find(x => x.Point == point);
        for (int i = 0; i < _tiles.Count; i++)
        {
            // 마우스 오버된 타일이거나, 오버된 타일의 반경안에 있는 타일인지 체크
            bool isOver = (overCenterTile.Point == _tiles[i].Point) || (IsSideTile(overCenterTile, _tiles[i]) == true);
            _tiles[i].SetOver(isOver && _fsmManager.IsCreateRoomState());
        }
    }

    public SquTile GetTile(SquPoint point)
    {
        return _tiles.Find(x => x.Point == point);
    }

    public List<SquTile> GetSideTiles(SquTile centerTile)
    {
        List<SquTile> sideTiles = new List<SquTile>();
        for(int i = 0; i < _tiles.Count; i++)
        {
            if (IsSideTile(centerTile, _tiles[i]) == true)
            {
                sideTiles.Add(_tiles[i]);
            }
        }
        return sideTiles;
    }

    bool IsSideTile(SquTile centerTile, SquTile otherTile)
    {
        foreach (KeyValuePair<DirectionType, SquPoint> kv in _dir)
        {
            SquTile checkTile = _tiles.Find(x => x.Point == centerTile.Point + kv.Value);
            if (checkTile != null && checkTile.Point == otherTile.Point)
                return true;
        }
        return false;
    }

    public void OnMouseDown(SquPoint point)
    {
        SquTile selectedTile = _tiles.Find(x => x.Point == point);
        if (selectedTile == null)
            return;

        _fsmManager.OnMouseDown(selectedTile);
    }

    public SquTile GetDirTile(SquTile tile, DirectionType dir)
    {
        return _tiles.Find(x => x.Point == tile.Point + _dir[dir]);
    }

    public void SetPathNumber()
    {
        List<SquTile> tiles = new List<SquTile>();
        int index = 1;
        for(int i = 0; i < _tiles.Count; i++)
        {
            if (_tiles[i].TileType == SquTile.TileTypes.Room)
            {
                Dictionary<DirectionType, SquPoint> paths = _tiles[i].Paths;
                foreach(KeyValuePair<DirectionType, SquPoint> kv in paths)
                {
                    SquTile startRoomTile = _tiles[i];
                    SquTile endRoomTile = _tiles.Find(x => x.Point == kv.Value);

                    SquTile startPathTile = GetDirTile(_tiles[i], kv.Key);
                    SquTile endPathTile = GetDirTile(endRoomTile, endRoomTile.GetPath(_tiles[i].Point).Item1);

                    if (tiles.Find(x => x.Point == startPathTile.Point) == null)
                    {
                        startPathTile.SetPathNumber("Path : " + index.ToString());
                        endPathTile.SetPathNumber("Path : " + index.ToString());
                        index++;

                        tiles.Add(startPathTile);
                        tiles.Add(endPathTile);
                    }
                }
            }
            else if (tiles.Find(x => x.Point == _tiles[i].Point) == null)
            {
                _tiles[i].SetPathNumber("");
            }
        }
    }

    void SetTileColor(SquTile tile)
    {
        if (tile.IsPath == true)
        {
            tile.SetColor(Color.red);
            return;
        }

        switch (tile.TileType)
        {
            case SquTile.TileTypes.None:
                tile.SetColor(tile.IsOver == true ? Color.gray : Color.white);
                break;
            case SquTile.TileTypes.Room:
                switch (tile.DungeonType)
                {
                    case DungeonTypes.Start:
                        tile.SetColor(Color.magenta);
                        break;
                    case DungeonTypes.Normal:
                        tile.SetColor(Color.blue);
                        break;
                    case DungeonTypes.Boss:
                        tile.SetColor(Color.yellow);
                        break;
                    default:
                        break;
                }
                break;
            case SquTile.TileTypes.RoomSide:
                tile.SetColor(Color.green);
                break;
            default:
                break;
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, 300f, Screen.height), "", GUI.skin.window);
        GUILayout.BeginVertical();
        switch (_fsmManager.CurStateType)
        {
            case DungeonEditorStateManager.StateType.None:
                OnGUIStateMain();
                break;
            case DungeonEditorStateManager.StateType.Main:
                OnGUIStateMain();
                break;
            case DungeonEditorStateManager.StateType.CreateRoom:
                OnGUIStateCreateRoom();
                break;
            case DungeonEditorStateManager.StateType.DeleteRoom:
                OnGUIStateDeleteRoom();
                break;
            case DungeonEditorStateManager.StateType.CreatePath:
                OnGUIStateCreatePath();
                break;
            case DungeonEditorStateManager.StateType.DeletePath:
                OnGUIStateDeletePath();
                break;
            case DungeonEditorStateManager.StateType.SetDungeonType:
                OnGUIStateSetDungeonType();
                break;
            default:
                break;
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void OnGUIStateMain()
    {
        GUILayout.Space(20);
        if (GUILayout.Button("CreateRoom"))
        {
            _fsmManager.ChangeState(DungeonEditorStateManager.StateType.CreateRoom);
        }

        GUILayout.Space(20);
        if (GUILayout.Button("DeleteRoom"))
        {
            _fsmManager.ChangeState(DungeonEditorStateManager.StateType.DeleteRoom);
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Create Path"))
        {
            _fsmManager.ChangeState(DungeonEditorStateManager.StateType.CreatePath);
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Delete Path"))
        {
            _fsmManager.ChangeState(DungeonEditorStateManager.StateType.DeletePath);
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Set Dungeon Type"))
        {
            _fsmManager.ChangeState(DungeonEditorStateManager.StateType.SetDungeonType);
        }

        GUILayout.Space(100);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Dungeon Name : ", _mapSizeFontStyle);
        string dungeonName = GUILayout.TextField(_dungeonName, _dungeonNameFileOption);
        _dungeonName = dungeonName;
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        if (GUILayout.Button("Save"))
        {
            SaveDungeonData();
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Load"))
        {
            LoadDungeonDdata();
        }
    }

    void OnGUIStateCreateRoom()
    {
        if (GUILayout.Button("End"))
        {
            _fsmManager.ChangeState(DungeonEditorStateManager.StateType.Main);
        }
    }

    void OnGUIStateDeleteRoom()
    {
        if (GUILayout.Button("End"))
        {
            _fsmManager.ChangeState(DungeonEditorStateManager.StateType.Main);
        }
    }

    void OnGUIStateCreatePath()
    {
        if (GUILayout.Button("End"))
        {
            _fsmManager.ChangeState(DungeonEditorStateManager.StateType.Main);
        }
    }

    void OnGUIStateDeletePath()
    {
        if (GUILayout.Button("End"))
        {
            _fsmManager.ChangeState(DungeonEditorStateManager.StateType.Main);
        }
    }

    void OnGUIStateSetDungeonType()
    {
        
        foreach (DungeonTypes type in Enum.GetValues(typeof(DungeonTypes)))
        {
            GUILayout.Space(20);
            if (_fsmManager.GetCurDungeonType() == type)
                GUI.color = Color.yellow;
            else
                GUI.color = Color.white;
            if (GUILayout.Button(type.ToString()))
            {
                _fsmManager.SetDungeonType(type);
            }
            GUI.color = Color.white;
        }

        GUILayout.Space(20);
        GUIStyle boxStyle = new GUIStyle();
        boxStyle.normal.background = MakeTex(50, 30, GetTileColorByDungeonType(_fsmManager.GetCurDungeonType()));
        GUILayout.Box("", boxStyle);

        GUILayout.Space(50);
        if (GUILayout.Button("End"))
        {
            _fsmManager.ChangeState(DungeonEditorStateManager.StateType.Main);
        }
    }

    Color GetTileColorByDungeonType(DungeonTypes type)
    {
        switch (type)
        {
            case DungeonTypes.Start:
                return COLOR_DUNGEONTYPE_START;
            case DungeonTypes.Normal:
                return COLOR_DUNGEONTYPE_NORMAL;
            case DungeonTypes.Boss:
                return COLOR_DUNGEONTYPE_BOSS;
            default:
                return COLOR_DUNGEONTYPE_NORMAL;
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    public void SaveDungeonData()
    {
        if (_dungeonName == null || _dungeonName == string.Empty)
            return;

        if (_tiles.Find(x => x.DungeonType == DungeonTypes.Start) == null)
        {
            Debug.LogError(string.Format("Not Found StartType Tile"));
            return;
        }
            

        DungeonData data = new DungeonData();
        data.DungeonName = _dungeonName;
        data.Infos = new List<DungeonData.Info>();
        for (int i = 0; i < _tiles.Count; i++)
        {
            if (_tiles[i].TileType == SquTile.TileTypes.Room)
            {
                DungeonData.Info newInfo = new DungeonData.Info();
                newInfo.Point = _tiles[i].Point;
                newInfo.dungeonType = _tiles[i].DungeonType;
                newInfo.Path = new Dictionary<DirectionType, SquPoint>();
                foreach(KeyValuePair<DirectionType, SquPoint> kv in _tiles[i].Paths)
                {
                    newInfo.Path.Add(kv.Key, kv.Value);
                }
                data.Infos.Add(newInfo);
            }
        }
        FileManager.Instance.SaveDungeonData(data);
    }

    public void LoadDungeonDdata()
    {
        if (_dungeonName == null || _dungeonName == string.Empty)
            return;

        DungeonData data = FileManager.Instance.LoadDungeonData(_dungeonName);
        CreateDungeon(data);
    }
}
