using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    private static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataManager();
            }
                
            return _instance;
        }
    }
    private FileManager _fileManager = null;
    private List<PlayerData> _playerDatas = new List<PlayerData>();
    private Tables _tables = new Tables();

    public void Init()
    {
        _fileManager = FileManager.Instance;
        _tables.InitTabeDatas();
    }

    public MapData GetMapData(int index)
    {
        TRMap trMap = Tables.Instance.GetRecord<TRMap>(x => x.Index == index);
        return FileManager.Instance.LoadMapData(trMap.MapName);
    }

    public List<PlayerData> GetPlayerData(PlayerType type)
    {
        return _playerDatas.FindAll(x => x.PlayerType == type);
    }

    public void SetPlayerData(List<PlayerData> playerDatas)
    {
        _playerDatas.Clear();
        for(int i = 0; i < playerDatas.Count; i++)
        {
            _playerDatas.Add(playerDatas[i]);
        }
    }
}
