using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorManager : MonoBehaviour
{
    private static MapEditorManager _instance;
    public static MapEditorManager Instance
    {
        get
        {
            return _instance; 
        }
    }

    public enum BtnType
    {
        PossibleType,
        AreaType,
        HexType,
    }

    #region COLOR
    static Color EMPTY_COLOR = Color.black;
    static Color ALLPOSSIBLE_COLOR = Color.white;
    static Color AIRPOSSIBLE_COLOR = Color.blue;
    static Color IMPOSSIBLE_COLOR = Color.red;

    static Color FRIENDAREA_COLOR = Color.green;
    static Color ENEMYAREA_COLOR = Color.gray;
    #endregion

    #region Camera Option
    [SerializeField]
    Camera _camera;

    float _zoomSpeed = 5f;
    float _moveSpeed = 0.5f;
    #endregion
    
    #region Map Parameter
    int _mapSizeX;
    int _mapSizeY;
    int _mapSizeZ;

    string _mapName;
    #endregion 

    public List<TRHex> HexInfos
    {
        get { return DataManager.Instance.TRHexes; }
    }

    public List<TRMap> MapItems
    {
        get { return DataManager.Instance.TRMaps; }
    }
    
    BtnType _curSelectedBtnType = BtnType.PossibleType;

    HexTile.PossibleType _curPossibleType = HexTile.PossibleType.AllPossible;
    HexTile.SpawnAreaType _curAreaType = HexTile.SpawnAreaType.None;
    int _curHexIndex = 0;

    public HexTile.PossibleType SelectedHexType
    {
        get { return _curPossibleType; }
    }

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckMouseWheel();
        CheckMove();
    }

    void CheckMouseWheel()
    {
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0)
        {
            _camera.orthographicSize -= wheel * _zoomSpeed;
            if (_camera.orthographicSize > 8f)
                _camera.orthographicSize = 8f;
            if (_camera.orthographicSize < 2f)
                _camera.orthographicSize = 2f;
        }
    }

    void CheckMove()
    {
        float vertical = Input.GetAxis("Vertical") * _moveSpeed;
        float horizontal = Input.GetAxis("Horizontal") * _moveSpeed;

        if (vertical == 0 && horizontal == 0)
        {
            return;
        }
        Vector3 camPos = _camera.transform.position;
        _camera.transform.position = new Vector3(camPos.x + horizontal, camPos.y, camPos.z + vertical);
    }

    private void OnGUI()
    {
        DrawLeftLayout();
    }

    void DrawLeftLayout()
    {
        if (HexInfos == null)
            return;

        #region Map Size Field Options
        GUILayoutOption[] mapSizeFieldOptions = new GUILayoutOption[2];
        mapSizeFieldOptions[0] = GUILayout.Width(100f);
        mapSizeFieldOptions[1] = GUILayout.Height(30);
        #endregion

        #region Map Name Field Options
        GUILayoutOption[] mapNameFieldOption = new GUILayoutOption[2];
        mapNameFieldOption[0] = GUILayout.Width(150f);
        mapNameFieldOption[1] = GUILayout.Height(30);
        #endregion

        #region Map Size Fond Style
        GUIStyle mapSizeFontStyle = new GUIStyle();
        mapSizeFontStyle.fontSize = 12;
        mapSizeFontStyle.normal.textColor = Color.white;
        #endregion

        #region Btn Label Option
        GUIStyle btnLabelStyle = new GUIStyle();
        btnLabelStyle.fontSize = 15;
        btnLabelStyle.normal.textColor = Color.white;
        #endregion

        GUILayout.BeginArea(new Rect(0, 0, 300f, Screen.height), "MapInfo", GUI.skin.window);
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Map Name : ", mapSizeFontStyle);
        string mapName = GUILayout.TextField(_mapName, mapNameFieldOption);
        _mapName = mapName;
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Save"))
        {
            SaveMap();
        }

        if (GUILayout.Button("Load"))
        {
            LoadMap();
        }

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Size X : ", mapSizeFontStyle);
        string sizeXStr = GUILayout.TextField("" + _mapSizeX, mapSizeFieldOptions);
        if (sizeXStr != string.Empty)
            _mapSizeX = int.Parse(sizeXStr);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Size Y : ", mapSizeFontStyle);
        string sizeYStr = GUILayout.TextField("" + _mapSizeY, mapSizeFieldOptions);
        if (sizeYStr != string.Empty)
            _mapSizeY = int.Parse(sizeYStr);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Size Z : ", mapSizeFontStyle);
        string sizeZStr = GUILayout.TextField("" + _mapSizeZ, mapSizeFieldOptions);
        if (sizeZStr != string.Empty)
            _mapSizeZ = int.Parse(sizeZStr);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Create"))
        {
            CreateMap();
        }

        GUILayout.Space(20);
        GUILayout.Label("Selected Btn Type : " + _curSelectedBtnType.ToString(), btnLabelStyle);
        GUILayout.Space(20);

        GUILayout.Label("Selected Possible Type : " + _curPossibleType.ToString(), btnLabelStyle);
        foreach(HexTile.PossibleType type in Enum.GetValues(typeof(HexTile.PossibleType)))
        {
            if (_curSelectedBtnType == BtnType.PossibleType && _curPossibleType == type)
                GUI.color = Color.yellow;
            else
                GUI.color = Color.white;
            if (GUILayout.Button(type.ToString()))
            {
                OnSelectPossibleType(type);
            }
            GUI.color = Color.white;
        }

        GUILayout.Space(20);
        GUILayout.Label("Selected Area Type : " + _curAreaType.ToString(), btnLabelStyle);
        foreach (HexTile.SpawnAreaType type in Enum.GetValues(typeof(HexTile.SpawnAreaType)))
        {
            if (_curSelectedBtnType == BtnType.AreaType && _curAreaType == type)
                GUI.color = Color.yellow;
            else
                GUI.color = Color.white;
            if (GUILayout.Button(type.ToString()))
            {
                OnSelectAreaType(type);
            }
            GUI.color = Color.white;
        }

        GUIStyle boxStyle = new GUIStyle();
        boxStyle.normal.background = MakeTex(50, 30, GetHexColorByPossible(_curPossibleType));
        GUILayout.Box("", boxStyle);

        GUILayout.Space(20);

        GUILayout.Label("Selected Hex Type : " + HexInfos.Find(x => x.Index == _curHexIndex).Name.ToString(), btnLabelStyle);
        for(int i = 0; i < HexInfos.Count; i++)
        {
            if (_curSelectedBtnType == BtnType.HexType && _curHexIndex == i)
                GUI.color = Color.yellow;
            else
                GUI.color = Color.white;
            if (GUILayout.Button(HexInfos[i].Name))
            {
                OnSelectHexData(HexInfos[i].Index);
            }
            GUI.color = Color.white;
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
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

    void CreateMap()
    {
        if (_mapSizeX <= 0 || _mapSizeY <= 0 || _mapSizeZ <= 0)
            return;

        Debug.Log("Create Map");
        MapData mapDataInfo = new MapData();
        mapDataInfo.SizeX = _mapSizeX;
        mapDataInfo.SizeY = _mapSizeY;
        mapDataInfo.SizeZ = _mapSizeZ;

        MapManager.Instance.CreateMap(root:null, mapDataInfo, isEditor:true);
    }

    Color GetHexColorByPossible(HexTile.PossibleType type)
    {
        switch (type)
        {
            case HexTile.PossibleType.Empty:
                return EMPTY_COLOR;
            case HexTile.PossibleType.AllPossible:
                return ALLPOSSIBLE_COLOR;
            case HexTile.PossibleType.AirPossible:
                return AIRPOSSIBLE_COLOR;
            case HexTile.PossibleType.Impossible:
                return IMPOSSIBLE_COLOR;
            default:
                return ALLPOSSIBLE_COLOR;
        }
    }

    Color GetHexColorByArea(HexTile.SpawnAreaType type)
    {
        switch (type)
        {
            case HexTile.SpawnAreaType.None:
                return ALLPOSSIBLE_COLOR;
            case HexTile.SpawnAreaType.FriendArea:
                return FRIENDAREA_COLOR;
            case HexTile.SpawnAreaType.EnemyArea:
                return ENEMYAREA_COLOR;
            default:
                return ALLPOSSIBLE_COLOR;
        }
    }

    void LoadMap()
    {
        if (_mapName == string.Empty)
        {
            Debug.Log("Empty Map");
        }
        else
        {
            MapData mapData = FileManager.Instance.LoadMapData(_mapName);
            _mapSizeX = mapData.SizeX;
            _mapSizeY = mapData.SizeY;
            _mapSizeZ = mapData.SizeZ;
            MapManager.Instance.CreateMap(root: null, mapData, isEditor: true);
            List<HexTile> hexList = MapManager.Instance.HexList;
            for (int i = 0; i < hexList.Count; i++)
            {
                hexList[i].GetComponent<Renderer>().material.color = GetHexColorByPossible(hexList[i].MovingPossibleType);
            }

            for (int i = 0; i < hexList.Count; i++)
            {
                if (hexList[i].MovingPossibleType == HexTile.PossibleType.AllPossible)
                    hexList[i].GetComponent<Renderer>().material.color = GetHexColorByArea(hexList[i].AreaType);
            }
        }
    }

    void SaveMap()
    {
        if (_mapName == string.Empty || MapManager.Instance.HexList == null || MapManager.Instance.HexList.Count == 0)
        {
            Debug.Log("Empty Map");
        }
        else
        {
            MapData mapData = new MapData();
            mapData.MapName = _mapName;
            mapData.SizeX = _mapSizeX;
            mapData.SizeY = _mapSizeY;
            mapData.SizeZ = _mapSizeZ;
            mapData.HexDatas = new List<MapData.HexData>();
            List<HexTile> mapList = MapManager.Instance.HexList;
            for (int i = 0; i < mapList.Count; i++)
            {
                MapData.HexData hexData = new MapData.HexData();
                hexData.HexInfoIndex = mapList[i].TileIndex;
                hexData.MapPos = mapList[i].Point;
                hexData.PossibleType = mapList[i].MovingPossibleType;
                hexData.AreaType = mapList[i].AreaType;
                mapData.HexDatas.Add(hexData);
            }
            FileManager.Instance.SaveMapData(mapData);
        }
    }

    void OnSelectPossibleType(HexTile.PossibleType type)
    {
        _curSelectedBtnType = BtnType.PossibleType;
        _curPossibleType = type;
    }

    void OnSelectAreaType(HexTile.SpawnAreaType type)
    {
        _curSelectedBtnType = BtnType.AreaType;
        _curAreaType = type;
    }

    void OnSelectHexData(int index)
    {
        _curSelectedBtnType = BtnType.HexType;
        _curHexIndex = index;
    }

    public void OnClickHex(HexTile hex)
    {
        if (_curSelectedBtnType == BtnType.HexType)
        {
            GameObject obj = MapManager.Instance.CreateHexObj(_curHexIndex, null);
            HexEditor hexEditor = obj.AddComponent<HexEditor>();
            MapManager.Instance.SetHex(hexEditor, hex.Point);
            hexEditor.MovingPossibleType = hex.MovingPossibleType;
            hexEditor.TileIndex = _curHexIndex;

            hexEditor.GetComponent<Renderer>().material.color = GetHexColorByPossible(hex.MovingPossibleType);
            hexEditor.MovingPossibleType = hex.MovingPossibleType;
            hexEditor.TileIndex = _curHexIndex;
            GameObject.Destroy(hex.gameObject);
        }
        else if (_curSelectedBtnType == BtnType.AreaType)
        {
            hex.GetComponent<Renderer>().material.color = GetHexColorByArea(_curAreaType);
            hex.AreaType = _curAreaType;
        }
        else
        {
            hex.GetComponent<Renderer>().material.color = GetHexColorByPossible(_curPossibleType);
            hex.MovingPossibleType = _curPossibleType;
        }
    }
}
