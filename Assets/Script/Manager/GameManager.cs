using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    [SerializeField]
    Camera _cameraUI;

    [SerializeField]
    GameObject _mapRoot;

    [SerializeField]
    GameObject _playerRoot;

    [SerializeField]
    GameObject _uiRoot;

    PlayerManager _playerManager = null;
    MapManager _mapManager = null;
    DataManager _dataManager = null;
    FadeManager _fadeManager = null;
    TouchManager _touchManager = null;
    DungeonManager _dungeonManager = null;

    bool _isGameStart = false;

    public Camera CameraUI
    {
        get { return _cameraUI; }
    }

    public GameObject UIRoot
    {
        get { return _uiRoot; }
    }
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(_uiRoot.transform.parent);
    }

    void Start()
    {
        CreateFadeManager();
        _touchManager = TouchManager.Instance;

        _fadeManager.OnStartFadeOut(() => StartCoroutine(GotoDungeon(() => _fadeManager.OnStartFadeIn(null))), isImmediately:true);

        _dataManager = DataManager.Instance;
        _dataManager.Init();

        // todo : Dummy
        _dataManager.SetPlayerData(new List<PlayerData>
        {
            new PlayerData(PlayerType.Friend, 1),
            new PlayerData(PlayerType.Friend, 2),
            new PlayerData(PlayerType.Friend, 3),
            new PlayerData(PlayerType.Friend, 4),
            new PlayerData(PlayerType.Enemy, 5),
        });
    }

    IEnumerator GotoDungeon(Action callback)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Dungeon");
        while(!asyncLoad.isDone)
        {
            yield return null;
        }
        callback?.Invoke();
    }

    /*
    void OnLoadingBattle()
    {
        // todo : dummy
        CreateMap();

        // todo : dummy
        CreatePlayer();

        _fadeManager.OnStartFadeIn(null);
    }
    */

    void CreateMap()
    {
        // todo : dummy
        MapData mapInfo = _dataManager.TRMaps[UnityEngine.Random.Range(0, _dataManager.TRMaps.Count)].MapItemData;

        _mapManager = MapManager.Instance;
        _mapManager.CreateMap(_mapRoot.transform, mapInfo);
    }

    void Update()
    {
        if (_touchManager != null)
            _touchManager.OnUpdate();

        if (_isGameStart == true)
            return;
        /*
        List<TouchInfo> touchInfos = TouchManager.Instance.TouchInfos;
        if (_selectedFingerID == -1 && touchInfos.Count > 0)
        {
            // 현재 체크중인 id가 없을경우.
            // 상태가 Began인것들만 골라서 플레이어를 클릭했는지 체크
            touchInfos = touchInfos.FindAll(x => x.Phase == TouchPhase.Began);

            for (int i = 0; i < touchInfos.Count; i++)
            {
                RaycastHit[] hits = RaycastHits(touchInfos[i].Position);

                List<BattlePlayerBase> players = GetPlayersByRayCast(hits);
                players = players.FindAll(x => x.Type == PlayerType.Friend);
                if (players.Count > 0)
                {
                    _selectedPlayer = players[0];
                    _lastHex = players[0].CurHex;
                    _selectedFingerID = touchInfos[i].FingerID;
                    break;
                }
            }
        }
        else if (_selectedFingerID != -1)
        {
            // 현재 체크중인 id로 체크
            TouchInfo oldTouch = TouchManager.Instance.GetTouch(_selectedFingerID);
            if (oldTouch == null || oldTouch.Phase == TouchPhase.Ended)
            {
                _selectedPlayer.SetCurHex(_lastHex);
                _selectedPlayer.transform.position = MapManager.Instance.GetWorldPos(_lastHex.Point);
                _selectedPlayer = null;
                _lastHex = null;
                _selectedFingerID = -1;

                _mapManager.OnResetHexColor();
            }
            else if (oldTouch.Phase == TouchPhase.Moved)
            {
                RaycastHit[] hits = RaycastHits(oldTouch.Position);
                List<HexTile> hexes = GetHexByRayCast(hits);
                hexes = hexes.FindAll(x => _mapManager.IsSpawnableByAreaType(x.AreaType, _selectedPlayer.Type) == true);
                hexes = hexes.FindAll(x => _playerManager.IsOverlapPos(x.Point, _selectedPlayer.CurHex.Point) == false);
                hexes = hexes.FindAll(x => _mapManager.IsMovable(x.Point, _selectedPlayer.ActorMovingType) == true);
                _lastHex = hexes.Count > 0 ? hexes[0] : _lastHex;
                if (_lastHex != null)
                    _mapManager.OnChangeHexColor(_lastHex.Point, Color.gray, _selectedPlayer.Type);

                Vector3 position = new Vector3(oldTouch.Position.x, oldTouch.Position.y, Camera.main.WorldToScreenPoint(_selectedPlayer.transform.position).z);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                _selectedPlayer.transform.position = new Vector3(worldPosition.x, _selectedPlayer.transform.position.y, worldPosition.z - 0.25f);
            }
        }
        */
        /*
        TouchInfo touchInfo = TouchManager.Instance.GetTouch();
        if (touchInfo != null)
        {
            if (touchInfo.Phase == TouchPhase.Began)
            {
                RaycastHit[] hits = RaycastHits();
                List<PlayerBase> players = GetPlayersByRayCast(hits);
                players = players.FindAll(x => x.Type == PlayerType.Friend);
                _selectedPlayer = players.Count > 0 ? players[0] : null;

                List<Hex> hexes = GetHexByRayCast(hits);
                _lastHex = hexes.Count > 0 ? hexes[0] : null;
            }
            
            if (_selectedPlayer != null)
            {
                if (touchInfo.Phase == TouchPhase.Ended)
                {
                    _selectedPlayer.SetCurHex(_lastHex);
                    _selectedPlayer.transform.position = MapManager.Instance.GetWorldPos(_lastHex.MapPos);
                    _selectedPlayer = null;
                    _lastHex = null;

                    _mapManager.OnResetHexColor();
                }
                else if (touchInfo.Phase == TouchPhase.Moved)
                {
                    RaycastHit[] hits = RaycastHits();
                    List<Hex> hexes = GetHexByRayCast(hits);
                    hexes = hexes.FindAll(x => _mapManager.IsSpawnableByAreaType(x.AreaType, _selectedPlayer.Type) == true);
                    hexes = hexes.FindAll(x => _playerManager.IsOverlapPos(x.MapPos, _selectedPlayer.CurHex.MapPos) == false);
                    hexes = hexes.FindAll(x => _mapManager.IsMovable(x.MapPos, _selectedPlayer.ActorMovingType) == true);
                    _lastHex = hexes.Count > 0 ? hexes[0] : _lastHex;
                    if (_lastHex != null)
                        _mapManager.OnChangeHexColor(_lastHex.MapPos, Color.gray, _selectedPlayer.Type);

                    Vector3 position = new Vector3(touchInfo.Position.x, touchInfo.Position.y, Camera.main.WorldToScreenPoint(_selectedPlayer.transform.position).z);
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                    _selectedPlayer.transform.position = new Vector3(worldPosition.x, _selectedPlayer.transform.position.y, worldPosition.z - 0.25f);
                }
            }
        }
        */
    }

    private RaycastHit[] RaycastHits(Vector2 mousePos)
    {
        //Vector2 mousePos = TouchManager.Instance.GetTouchPosition();

        Vector3 screenMousePosFar = new Vector3(mousePos.x, mousePos.y, Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);

        Ray newRay = new Ray(worldMousePosNear, worldMousePosFar - worldMousePosNear);
        RaycastHit[] hits = Physics.RaycastAll(newRay, Vector3.Distance(worldMousePosNear, worldMousePosFar));
        return hits;
    }

    private List<BattlePlayerBase> GetPlayersByRayCast(RaycastHit[] hits)
    {
        List<BattlePlayerBase> players = new List<BattlePlayerBase>();
        for(int i = hits.Length -1 ; i >= 0; i--)
        {
            if (hits[i].transform == null)
                continue;

            if (hits[i].transform.parent == null)
                continue;

            if (hits[i].transform.parent.GetComponent<PlayerBase>() == null)
                continue;

            players.Add(hits[i].transform.parent.GetComponent<BattlePlayerBase>());
        }
        return players;
    }

    private List<HexTile> GetHexByRayCast(RaycastHit[] hits)
    {
        List<HexTile> hexes = new List<HexTile>();
        for (int i = hits.Length - 1; i >= 0; i--)
        {
            if (hits[i].transform == null)
                continue;

            if (hits[i].transform.GetComponent<HexTile>() == null)
                continue;

            hexes.Add(hits[i].transform.GetComponent<HexTile>());
        }
        return hexes;
    }

    void CreateFadeManager()
    {
        GameObject obj = GameObject.Instantiate(ResourcesManager.Instance.Load<GameObject>("Prefabs/UI/", "FadeManager"));
        obj.transform.SetParent(_uiRoot.transform);
        DontDestroyOnLoad(obj);

        _fadeManager = obj.GetComponent<FadeManager>();
        _fadeManager.Init();
    }
    
    private void OnGUI()
    {
        //if (_isGameStart == false)
        //{
        //    Rect rect = new Rect(0, 0, 200, 50);
        //    if (GUI.Button(rect, "Game Start"))
        //    {
        //        OnBattleStart();
        //    }
        //}
    }

    private void OnBattleStart()
    {
        _isGameStart = true;
    }
}
