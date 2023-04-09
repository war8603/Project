using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    Camera _battleCamera;

    [SerializeField]
    GameObject _effectPoolRoot;

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

    Dictionary<GameObject, bool> _effectPool = new Dictionary<GameObject, bool>();

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
        {
            if (_playerManager.IsGameOver() == 1 || _playerManager.IsGameOver() == 2)
            {
                string result = _playerManager.IsGameOver() == 1 ? "Victory!!" : "Defeat!!";
                Debug.Log(result);
                _isBattleStart = false;
                DungeonManager.Instance.GotoDungeon();
                this.gameObject.SetActive(false);
            }
            else
                _playerManager.OnUpdate();
        }   

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

        List<string> skillEffectName = Enumerable.Repeat("ExplosionEffect", 5).ToList<string>();
        CreateSkillEffectPool(skillEffectName);
    }

    void CreateSkillEffectPool(List<string> effectNames)
    {
        for (int i = 0; i < effectNames.Count; i++)
        {
            CreateSkillEffect(effectNames[i]);
        }
    }

    public GameObject GetSkillEffect(string effectName)
    {
        GameObject effectObj = null;
        foreach(var effect in _effectPool)
        {
            if (effect.Key.name == effectName && effect.Value == false)
            {
                effectObj = effect.Key;
                break;
            }
        }

        if (effectObj == null)
        {
            effectObj = CreateSkillEffect(effectName);
        }
        
        SetUseSkillEffect(effectObj);
        return effectObj;
    }

    GameObject CreateSkillEffect(string effectName)
    {
        GameObject effectObj = GameObject.Instantiate(ResourcesManager.Instance.Load<GameObject>("Prefabs/Effect/", effectName), _effectPoolRoot.transform);
        effectObj.name = effectName;
        effectObj.gameObject.SetActive(false);
        _effectPool.Add(effectObj, false);
        return effectObj;
    }

    void SetUseSkillEffect(GameObject key)
    {
        _effectPool[key] = true;
        key.gameObject.SetActive(true);
    }

    public void SetReturnSkillEffect(GameObject key)
    {
        key.transform.SetParent(_effectPoolRoot.transform);
        key.gameObject.SetActive(false);
        _effectPool[key] = false;
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
        //MapData mapInfo = _dataManager.TRMaps[Random.Range(0, _dataManager.TRMaps.Count)].MapItemData;
        var trMaps = Tables.Instance.GetTable<TRMap>();
        int randomTRMaps = trMaps[UnityEngine.Random.Range(0, trMaps.Count)].Index;

        var mapData = DataManager.Instance.GetMapData(randomTRMaps);
        MapData mapInfo = mapData;

        _mapManager = MapManager.Instance;
        _mapManager.CreateMap(mapRoot, mapInfo);
    }

    void CreatePlayer(Transform playerRoot)
    {
        var friendPlayers = DataManager.Instance.GetPlayerData(PlayerType.Friend);
        friendPlayers.RemoveRange(0, 2);
        var enemyPlayers = DataManager.Instance.GetPlayerData(PlayerType.Enemy);
        
        _playerManager.GenPlayerTest(playerRoot, friendPlayers, enemyPlayers);
    }
}
