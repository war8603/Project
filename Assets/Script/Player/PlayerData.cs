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
