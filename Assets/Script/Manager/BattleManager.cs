using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    Camera _battleCamera;

    private static BattleManager _instance;
    public static BattleManager Instance
    {
        get { return _instance; }
    }

    DataManager _dataManager;
    MapManager _mapManager;
    FadeManager _fadeManager;
    PlayerManager _playerManager;

    BattlePlayerBase _selectedPlayer = null;
    HexTile _lastHex = null;
    int _selectedFingerID = -1;
    bool _isBattleStart = false;

    public void Awake()
    {
        _dataManager = DataManager.Instance;
        _mapManager = MapManager.Instance;
        _fadeManager = FadeManager.Instance;
        _playerManager = PlayerManager.Instance;

        _instance = this;
    }

    public void Start()
    {
        //OnLoadingBattle();
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.S) == true && _isBattleStart == false)
            _isBattleStart = true;

        if (_isBattleStart == true)
            _playerManager.OnUpdate();

        if (_isBattleStart == false)
        {
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

                    Vector3 position = new Vector3(oldTouch.Position.x, oldTouch.Position.y, _battleCamera.WorldToScreenPoint(_selectedPlayer.transform.position).z);
                    Vector3 worldPosition = _battleCamera.ScreenToWorldPoint(position);
                    _selectedPlayer.transform.position = new Vector3(worldPosition.x, _selectedPlayer.transform.position.y, worldPosition.z - 0.25f);
                }
            }
        }
    }

    private List<BattlePlayerBase> GetPlayersByRayCast(RaycastHit[] hits)
    {
        List<BattlePlayerBase> players = new List<BattlePlayerBase>();
        for (int i = hits.Length - 1; i >= 0; i--)
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

    private RaycastHit[] RaycastHits(Vector2 mousePos)
    {
        //Vector2 mousePos = TouchManager.Instance.GetTouchPosition();

        Vector3 screenMousePosFar = new Vector3(mousePos.x, mousePos.y, _battleCamera.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(mousePos.x, mousePos.y, _battleCamera.nearClipPlane);
        Vector3 worldMousePosFar = _battleCamera.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = _battleCamera.ScreenToWorldPoint(screenMousePosNear);

        Ray newRay = new Ray(worldMousePosNear, worldMousePosFar - worldMousePosNear);
        RaycastHit[] hits = Physics.RaycastAll(newRay, Vector3.Distance(worldMousePosNear, worldMousePosFar));
        return hits;
    }

    public void OnLoadingBattle(Transform mapRoot, Transform playerRoot, Camera billboardCamera)
    {
        // todo : dummy
        CreateBackImage(mapRoot, billboardCamera);

        // todo : dummy
        CreateMap(mapRoot);

        // todo : dummy
        CreatePlayer(playerRoot);

        //_fadeManager.OnStartFadeIn(null);
    }

    void CreateBackImage(Transform mapRoot, Camera billboardCamera)
    {
        // todo : dummy
        GameObject obj = new GameObject();
        obj.transform.SetParent(mapRoot);
        obj.transform.localPosition = new Vector3(0f, -15f, 15f);
        obj.transform.localScale = new Vector3(50f, 50f, 50f);
        obj.name = "BackImage";

        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = ResourcesManager.Instance.Load<Sprite>("Sprites/", "contents_kamibase_bg");

        BillboardHelper billboard = obj.AddComponent<BillboardHelper>();
        billboard.SetCamera(billboardCamera);
    }

    void CreateMap(Transform mapRoot)
    {
        // todo : dummy
        MapData mapInfo = _dataManager.TRMaps[Random.Range(0, _dataManager.TRMaps.Count)].MapItemData;

        _mapManager = MapManager.Instance;
        _mapManager.CreateMap(mapRoot, mapInfo);
    }

    void CreatePlayer(Transform playerRoot)
    {
        // todo : dummy
        List<TRPlayer> players = new List<TRPlayer>();
        players.Add(_dataManager.TRPlayers.Find(x => x.Index == 2));
        players.Add(_dataManager.TRPlayers.Find(x => x.Index == 1));
        players.Add(_dataManager.TRPlayers.Find(x => x.Index == 3));
        _playerManager = PlayerManager.Instance;

        List<TRPlayer> enemies = new List<TRPlayer>();
        enemies.Add(_dataManager.TRPlayers.Find(x => x.Index == 5));
        _playerManager.GenPlayerTest(playerRoot, players, enemies);
    }
}
