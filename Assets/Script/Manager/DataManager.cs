using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    private PlayerType _playerType;
    private int _index;

    public int Index => _index;
    public PlayerType PlayerType => _playerType;

    public PlayerData(PlayerType type, int index)
    {
        _playerType = type;
        _index = index;
    }
}

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
    private List<TRHex> _trHexes = null;
    private List<TRMap> _trMaps = null;
    private List<PlayerData> _playerDatas = new List<PlayerData>();
    private Tables _tables = new Tables();

    public List<TRHex> TRHexes => _trHexes;
    public List<TRMap> TRMaps => _trMaps;

    public void Init()
    {
        _fileManager = FileManager.Instance;
        _trHexes = _fileManager.LoadHexTable();
        _trMaps = _fileManager.LoadMapTable();
        _tables.InitTabeDatas();

        for (int i = 0; i < _trMaps.Count; i++)
        {
            _trMaps[i].MapItemData = _fileManager.LoadMapData(_trMaps[i].MapName);
        }
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
