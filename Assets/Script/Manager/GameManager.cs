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

    DataManager _dataManager = null;
    FadeManager _fadeManager = null;
    TouchManager _touchManager = null;

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

    void Update()
    {
        if (_touchManager != null)
            _touchManager.OnUpdate();
    }

    private RaycastHit[] RaycastHits(Vector2 mousePos)
    {
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
}
