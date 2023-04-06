using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Transactions;

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

        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].OnUpdate();
        }
    }

    public int IsGameOver()
    {
        int friendCount = 0;
        int enemyCount = 0;
        for(int i = 0; i < _players.Count; i++)
        {
            if (_players[i].Type == PlayerType.Enemy)
                enemyCount++;
            else if (_players[i].Type == PlayerType.Friend)
                friendCount++;
        }

        if (enemyCount != 0 && friendCount != 0)
            return 0;
        else
        {
            if (friendCount > 0)
                return 1;
            else 
                return 2;
        }    
    }

    public void GenPlayerTest(Transform playerRoot, List<PlayerData> friendPlayers, List<PlayerData> enemeyPlayers)
    {
        InitPlayers();    

        for(int i = 0; i < friendPlayers.Count; i++)
        {
            var trPlayer = Tables.Instance.GetRecord<TRPlayer>(x => x.Index == friendPlayers[i].Index);
            if (trPlayer == null)
                continue;

            BattlePlayerBase player = CreatePlayer(playerRoot, PlayerType.Friend, trPlayer);
            _players.Add(player);
        }

        for (int i = 0; i < enemeyPlayers.Count; i++)
        {
            var trPlayer = Tables.Instance.GetRecord<TRPlayer>(x => x.Index == enemeyPlayers[i].Index);
            if (trPlayer == null)
                continue;

            BattlePlayerBase player = CreatePlayer(playerRoot, PlayerType.Enemy, trPlayer);
            _players.Add(player);
        }
    }

    public BattlePlayerBase CreatePlayer(Transform root, PlayerType playerType, TRPlayer playerData)
    {
        GameObject obj = CreatePlayerGameObject(root);
        obj.name = playerData.KeyName;

        BattlePlayerBase player = obj.AddComponent<AIPlayer>();
        player.SetPlayerType(playerType);

        HexTile playerHex = MapManager.Instance.GetRandomSpawnPos(playerData.PlayerMovingType, playerType);
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
