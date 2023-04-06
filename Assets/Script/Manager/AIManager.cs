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
        // todo : �Լ��� �������� switch���� �ൿ�� �Լ��� �и� �ʿ�.
        switch (curAct)
        {
            case BattlePlayerActType.Idle:
                // Ÿ�Ͽ� ���� ���� ���� �Ÿ���ŭ ��ġ üũ
                BattlePlayerBase target = TargetManager.Instance.FindNearTarget(player);
                List<HexTile> dests = TargetManager.Instance.GetTargetPos(player, target);
                for(int i = 0; i < dests.Count; i++)
                {
                    // ���� ������ ������ �Ÿ��ȿ� ���� �����Ѵٸ� ����
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
                    // ��ǥ �������� �̵��Ҽ� �ִ��� üũ
                    if (MapManager.Instance.IsMovable(dests[i], player.ActorMovingType) == false)
                        continue;

                    // �ٸ��÷��̾��� ��ġ(curhex)�� ������(dest)�� ��ġ���� üũ
                    if (PlayerManager.Instance.IsOverlapPos(dests[i].Point) == true)
                        continue;

                    // ���� ������ ��ġ�� ���Ͽ� ���� ª�� �̵��Ÿ� üũ
                    List<HexTile> paths = mapManager.GetPath(player.CurHex, dests[i], player.ActorMovingType);
                    if (paths.Count < minCount)
                    {
                        minCount = paths.Count;
                        finPaths = paths;
                    }
                }

                // �����ϼ� �ִ� �Ÿ����� ũ�� �����ϼ� �ִ� �Ÿ���ŭ �̵�.
                if (finPaths.Count > player.MoveRange)
                {
                    finPaths.RemoveRange(player.MoveRange, finPaths.Count - player.MoveRange);
                }

                // ��ĭ�� �̵��ϰ� �˻��ϴ°� �´� �����ϰ�� �Ʒ�.
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
            // �����ϼ� �ִ� �Ÿ����� ũ�� �����ϼ� �ִ� �Ÿ���ŭ �̵�.
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
