using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    public static BattleManager Instance
    {
        get { return _instance; }
    }

    DataManager _dataManager;
    MapManager _mapManager;
    FadeManager _fadeManager;
    PlayerManager _playerManager;

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
