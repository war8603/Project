using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private static DungeonManager _instance;
    public static DungeonManager Instance
    {
        get { return _instance; }
    }

    [SerializeField]
    Camera _cameraUI;

    [SerializeField]
    Camera _cameraSpine;

    [SerializeField]
    Camera _camera3D;

    [SerializeField]
    Camera _cameraBattle;

    [SerializeField]
    GameObject _rootUI;

    [SerializeField]
    GameObject _rootSpine;

    [SerializeField]
    GameObject _root3D;

    [SerializeField]
    GameObject _rootBattleMap;

    [SerializeField]
    GameObject _rootBattlePlayer;

    Transform _flowTarget = null;
    BattleManager _battleManager;
    DungeonController _controller;

    public Camera CameraSpine
    {
        get { return _cameraSpine; }
    }

    public Camera Camera3D
    {
        get { return _camera3D; }
    }

    public Camera CameraBattle
    {
        get { return _cameraBattle; }
    }


    private void Awake()
    {
        _instance = this;

        CreateController();

        _cameraBattle.gameObject.SetActive(false);

        _battleManager = BattleManager.Instance;
    }

    public void Start()
    {
        _battleManager = BattleManager.Instance;
    }

    public void Init()
    {
        
    }

    private void CreateController()
    {
        _controller = new DungeonController(this);
        _controller.CreateView(_rootUI);
        _controller.Init(_rootSpine, _root3D);
    }

    public void SetBattleLoad(bool value)
    {
        _controller.SetBattleLoad(value);
    }


    public void Update()
    {
        _controller?.OnUpdate();
    }

    public void LateUpdate()
    {
        if (_flowTarget == null)
            return;

        Vector3 targetPos = _flowTarget.position;
        Vector3 cameraSpinePos = _cameraSpine.transform.position;
        Vector3 camera3DPos = _camera3D.transform.position;

        _cameraSpine.transform.position = new Vector3(targetPos.x, targetPos.y, cameraSpinePos.z);
        _camera3D.transform.position = new Vector3(targetPos.x, targetPos.y, camera3DPos.z);
    }

    public void GotoBattle()
    {
        _cameraUI.gameObject.SetActive(false);
        _camera3D.gameObject.SetActive(false);
        _cameraSpine.gameObject.SetActive(false);

        _cameraBattle.gameObject.SetActive(true);
        _battleManager.gameObject.SetActive(true);
        _battleManager.OnLoadingBattle(_rootBattleMap.transform, _rootBattlePlayer.transform, _cameraBattle);
    }

    public void GotoDungeon()
    {
        _cameraUI.gameObject.SetActive(true);
        _camera3D.gameObject.SetActive(true);
        _cameraSpine.gameObject.SetActive(true);

        _cameraBattle.gameObject.SetActive(false);
        SetBattleLoad(false);
    }

    public void SetFlowTarget(Transform target)
    {
        _flowTarget = target;
    }
}
