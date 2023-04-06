using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager
{
    private static AIManager _instance;
    public static AIManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new AIManager();
            }
            return _instance;
        }
    }

    public void OnActionAIPlayer(BattlePlayerActType curAct, BattlePlayerBase player)
    {
        MapManager mapManager = MapManager.Instance;
        // todo : 함수가 길어질경우 switch내의 행동을 함수로 분리 필요.
        switch (curAct)
        {
            case BattlePlayerActType.Idle:
                // 타켓에 대해 공격 가능 거리만큼 위치 체크
                BattlePlayerBase target = TargetManager.Instance.FindNearTarget(player);
                List<HexTile> dests = TargetManager.Instance.GetTargetPos(player, target);
                for(int i = 0; i < dests.Count; i++)
                {
                    // 만약 공격이 가능한 거리안에 내가 존재한다면 공격
                    if (dests[i].Point == player.CurHex.Point)
                    {
                        player.OnStartAttack(target);
                        return;
                    }
                }

                int minCount = int.MaxValue;
                List<HexTile> finPaths = new List<HexTile>();
                for (int i = 0; i < dests.Count; i++)
                {
                    // 목표 지점으로 이동할수 있는지 체크
                    if (MapManager.Instance.IsMovable(dests[i], player.ActorMovingType) == false)
                        continue;

                    // 다른플레이어의 위치(curhex)나 도착점(dest)와 겹치는지 체크
                    if (PlayerManager.Instance.IsOverlapPos(dests[i].Point) == true)
                        continue;

                    // 공격 가능한 위치에 대하여 가장 짧은 이동거리 체크
                    List<HexTile> paths = mapManager.GetPath(player.CurHex, dests[i], player.ActorMovingType);
                    if (paths.Count < minCount)
                    {
                        minCount = paths.Count;
                        finPaths = paths;
                    }
                }

                // 움직일수 있는 거리보다 크면 움직일수 있는 거리만큼 이동.
                if (finPaths.Count > player.MoveRange)
                {
                    finPaths.RemoveRange(player.MoveRange, finPaths.Count - player.MoveRange);
                }

                // 한칸씩 이동하고 검색하는게 맞는 로직일경우 아래.
                if (finPaths.Count > 1)
                    finPaths.RemoveRange(1, finPaths.Count - 1);

                player.OnStartMove(finPaths);
                break;
            case BattlePlayerActType.MoveEnd:
                player.SetActionType(BattlePlayerActType.Idle);
                break;
            default:
                break;
        }
    }

    public void MoveToRandomPos(BattlePlayerBase player)
    {
        MapManager mapManager = MapManager.Instance;
        HexTile tarGetHex = MapManager.Instance.GetRandomPos(player.ActorMovingType);
        List<HexTile> paths = mapManager.GetPath(player.CurHex, tarGetHex, player.ActorMovingType);
        //if (paths.Count > player.MoveRange)
        {
            // 움직일수 있는 거리보다 크면 움직일수 있는 거리만큼 이동.
            //paths.RemoveRange(player.MoveRange, paths.Count - player.MoveRange);
        }

        if (paths.Count > 1)
            paths.RemoveRange(1, paths.Count - 1);

        player.OnStartMove(paths);
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

    bool IsOtherPlayerType(PlayerType playerType, PlayerType targetType)
    {
        if (playerType == PlayerType.Friend)
        {
            switch(targetType)
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

    public void StartAttack(BattlePlayerBase player, BattlePlayerBase target)
    {
        Debug.Log("Attack <attcker : " + player + "> <target : " + target + ">");
        target.SetDamage(0);
    }
}
