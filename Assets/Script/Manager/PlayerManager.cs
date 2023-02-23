using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class TRPlayer
{
    PlayerType _playerType;
    int _index;
    string _keyName;
    string _spineDataAsset;
    MovingType _movingType;
    List<string> _motions = new List<string>();
    int _hp;
    int _moveSpeed;
    int _moveRange;
    int _attackRange;

    public PlayerType Type
    {
        get { return _playerType; }
        set { _playerType = value; }
    }
    public int Index
    {
        get { return _index; }
        set { _index = value; }
    }

    public string KeyName
    {
        get { return _keyName; }
        set { _keyName = value; }
    }

    public string SpineDataAsset
    {
        get { return _spineDataAsset; }
        set { _spineDataAsset = value; }
    }

    public List<string> Motions
    {
        get { return _motions; }
        set { _motions = value; }
    }

    public MovingType PlayerMovingType
    {
        get { return _movingType; }
        set { _movingType = value; }
    }

    public int Hp
    {
        get { return _hp; }
        set { _hp = value; }
    }

    public int MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    public int MoveRange
    {
        get { return _moveRange; }
        set { _moveRange = value; }
    }

    public int AttackRange
    {
        get { return _attackRange; }
        set { _attackRange = value; }
    }
}

public enum PlayerType
{
    Friend,
    Enemy,
}
public class PlayerManager
{
    #region Singleton
    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new PlayerManager();
            return _instance; 
        }
    }
    #endregion

    #region Player
    const float PLAYERSIZE = 0.5f;
    List<BattlePlayerBase> _players = new List<BattlePlayerBase>();
    public List<BattlePlayerBase> Players
    {
        get { return _players; }
        set { _players = value; }
    }
    #endregion
    
    public void InitPlayers()
    {
        if( _players != null)
        {
            for(int i = 0; i < _players.Count; i++)
            {
                GameObject.Destroy(_players[i].gameObject);
                _players[i] = null;
            }
        }
        _players.Clear();
    }

    public void OnUpdate()
    {
        if (_players == null || _players.Count == 0)
            return;

        for(int i = 0; i < _players.Count; i++)
        {
            _players[i].OnUpdate();
        }
    }

    public void GenPlayerTest(Transform playerRoot, List<TRPlayer> playerData, List<TRPlayer> enemyData)
    {
        InitPlayers();    

        for(int i = 0; i < playerData.Count; i++)
        {
            // todo : dummy. 추후 변경.
            playerData[i].Type = PlayerType.Friend;
            BattlePlayerBase player = CreatePlayer(playerRoot, playerData[i]);
            _players.Add(player);
        }

        for (int i = 0; i < enemyData.Count; i++)
        {
            enemyData[i].Type = PlayerType.Enemy;
            BattlePlayerBase player = CreatePlayer(playerRoot, enemyData[i]);
            _players.Add(player);
        }
    }

    public BattlePlayerBase CreatePlayer(Transform root, TRPlayer playerData)
    {
        GameObject obj = CreatePlayerGameObject(root);
        obj.name = playerData.KeyName;

        BattlePlayerBase player = obj.AddComponent<AIPlayer>();
        player.SetPlayerType(playerData.Type);

        HexTile playerHex = MapManager.Instance.GetRandomSpawnPos(playerData.PlayerMovingType, playerData.Type);
        player.SetCurHex(playerHex);

        player.SetPlayerData(playerData);
        player.transform.position = MapManager.Instance.GetWorldPos(player.CurHex.Point);

        SkeletonAnimation anim = CreateSpineObject(obj.transform, playerData.KeyName, playerData.SpineDataAsset);
        SpineObject animObj = anim.gameObject.AddComponent<SpineObject>();
        player.SetAnim(animObj);
        player.OnPlayAnim();
        player.OnLookAtDirction();

        return player;
    }

    public DungeonPlayer CreateDungeonPlayer(Transform parent, string layerName, string keyName, string spineDataAsset)
    {
        GameObject obj = CreatePlayerGameObject(parent);
        obj.name = keyName;

        DungeonPlayer player = obj.AddComponent<DungeonPlayer>();

        SkeletonAnimation anim = CreateSpineObject(obj.transform, keyName, spineDataAsset);
        SpineObject animObj = anim.gameObject.AddComponent<SpineObject>();

        player.SetAnim(animObj);
        player.OnPlayAnim();

        Tools.SetLayer(obj.transform, layerName);
        return player;
    }

    GameObject CreatePlayerGameObject(Transform root)
    {
        GameObject obj = new GameObject();
        obj.AddComponent<RectTransform>();
        obj.transform.SetParent(root);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = new Vector3(PLAYERSIZE, PLAYERSIZE, PLAYERSIZE);
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        return obj;
    }

    SkeletonAnimation CreateSpineObject(Transform root, string keyName, string spineDataAssetName)
    {
        SkeletonDataAsset spineAsset = ResourcesManager.Instance.Load<SkeletonDataAsset>("Spines/" + keyName + "/", spineDataAssetName);
        SkeletonAnimation anim = SkeletonAnimation.NewSkeletonAnimationGameObject(spineAsset);
        anim.gameObject.name = keyName + "SpineObj";
        anim.gameObject.transform.SetParent(root);
        anim.gameObject.transform.localScale = Vector3.one;

        float height = 0f;
        if (anim.GetComponent<Renderer>() != null)
            height = anim.GetComponent<Renderer>().bounds.size.x;


        anim.gameObject.transform.localPosition = Vector3.zero;
        //anim.gameObject.transform.localPosition = new Vector3(0f, height/3, 0f);
        anim.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

        return anim;
    }

    public void RemovePlayer(BattlePlayerBase player)
    {
        PlayerManager.Instance.Players.Remove(player);
        GameObject.Destroy(player.gameObject);
    }

    public bool IsOverlapPos(HexPoint mapPos)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].CurHex != null && _players[i].CurHex.Point == mapPos)
                return true;

            if (_players[i].DestHex != null && _players[i].DestHex.Point == mapPos)
                return true;
        }
        return false;
    }

    public bool IsOverlapPos(HexPoint mapPos, HexPoint ignoreMapPos)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            HexTile curHex = _players[i].CurHex;
            if (curHex != null && curHex.Point == mapPos && curHex.Point != ignoreMapPos)
                return true;

            HexTile destHex = _players[i].DestHex;
            if (destHex != null && destHex.Point == mapPos && destHex.Point != ignoreMapPos)
                return true;
        }
        return false;
    }
}
