using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager
{
    private static TargetManager _instance;
    public static TargetManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new TargetManager();
            return _instance;
        }
    }
    bool IsOtherPlayerType(PlayerType playerType, PlayerType targetType)
    {
        if (playerType == PlayerType.Friend)
        {
            switch (targetType)
            {
                case PlayerType.Friend:
                    return false;
                case PlayerType.Enemy:
                    return true;
            }
        }
        else if (playerType == PlayerType.Enemy)
        {
            switch (targetType)
            {
                case PlayerType.Friend:
                    return false;
                case PlayerType.Enemy:
                    return true;
                default:
                    return true;
            }
        }
        return true;
    }

    public BattlePlayerBase FindNearTarget(BattlePlayerBase player)
    {
        PlayerManager playerManager = PlayerManager.Instance;
        MapManager mapManager = MapManager.Instance;
        int nearDistance = int.MaxValue;
        BattlePlayerBase target = null;
        foreach (BattlePlayerBase p in playerManager.Players)
        {
            if (IsOtherPlayerType(player.Type, p.Type) == true)
            {
                int distance = mapManager.GetDistance(p.CurHex, player.CurHex);
                if (distance < nearDistance)
                {
                    target = p;
                    nearDistance = distance;
                }
            }
        }
        return target;
    }

    public List<HexTile> GetTargetPos(BattlePlayerBase player)
    {
        // 가까운적 탐색.
        BattlePlayerBase target = player;
        PlayerManager playerManager = PlayerManager.Instance;
        for (int i = 0; i < playerManager.Players.Count; i++)
        {
            if (IsOtherPlayerType(player.Type, playerManager.Players[i].Type) == true)
            {
                target = playerManager.Players[i];
            }
        }

        // 가까운적에 대해 attackRange 체크
        List<HexTile> attackAblePos = MapManager.Instance.GetAttackAblePos(target.CurHex, player.AttackRange);
        return attackAblePos;
    }

    public List<HexTile> GetTargetPos(BattlePlayerBase player, BattlePlayerBase target)
    {
        // 타켓에 대해 attackRange 체크
        List<HexTile> attackAblePos = MapManager.Instance.GetAttackAblePos(target.CurHex, player.AttackRange);
        return attackAblePos;
    }
}
