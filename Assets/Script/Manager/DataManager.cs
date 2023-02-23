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
                Init();
            }
                
            return _instance;
        }
    }
    static FileManager _fileManager = null;
    static List<TRHex> _trHexes = null;
    static List<TRMap> _trMaps = null;
    static List<TRPlayer> _trPlayers = null;

    public List<TRHex> TRHexes => _trHexes;
    public List<TRMap> TRMaps => _trMaps;
    public List<TRPlayer> TRPlayers => _trPlayers;

    public static void Init()
    {
        _fileManager = FileManager.Instance;
        _trHexes = _fileManager.LoadHexTable();
        _trMaps = _fileManager.LoadMapTable();
        _trPlayers = _fileManager.LoadPlayerTable();

        for (int i = 0; i < _trMaps.Count; i++)
        {
            _trMaps[i].MapItemData = _fileManager.LoadMapData(_trMaps[i].MapName);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
