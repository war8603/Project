using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class AIPlayer : BattlePlayerBase
{
    AIManager _aiManager;
    private void Awake()
    {
        _aiManager = AIManager.Instance;
    }
    void Start()
    {

    }

    public override void OnUpdate()
    {
        if (ActType == BattlePlayerActType.Moving)
        {
            OnMoving();
        }
        else
        {
            AIProc();
        }
    }

    public void AIProc()
    {
        //_aiManager.OnActionAIPlayer(CurAct, this);
        //return;
        if (PlayerType.Enemy == _playerType)
        {
            // todo : dummy Àû±ºÀÌ¸é µµ¸Á´Ù´Ô
            _aiManager.MoveToRandomPos(this);
        }
        else
        {
            _aiManager.OnActionAIPlayer(ActType, this);
        }   
    }

    public void OnMoving()
    {
        if (_moveHexes.Count == 0)
        {
            SetCurHex(CurHex);
            OnEndMove(CurHex);
            return;
        }
        HexTile nextHex = _moveHexes[0];
        Vector3 playerPos = transform.position;
        Vector3 destPos = MapManager.Instance.GetWorldPos(nextHex.Point);
        float distance = Vector3.Distance(playerPos, destPos);
        if (distance > 0.1f)
        {               
            transform.position += (destPos - playerPos).normalized * Time.deltaTime * MoveSpeed;
        }
        else
        {
            transform.position = destPos;
            _moveHexes.RemoveAt(0);
            SetCurHex(nextHex);
            if (_moveHexes.Count == 0)
            {
                OnEndMove(nextHex);
            }
        }

        OnChangeDirection(nextHex);
    }

    void OnChangeDirection(HexTile targetHex)
    {
        for (int i = 0; i < MapManager.Dirs.Length; i++)
        {
            if (MapManager.Dirs[i] == (targetHex.Point - CurHex.Point).Normalize)
            {
                switch ((MapManager.DirectionType)i)
                {
                    case MapManager.DirectionType.Right:
                    case MapManager.DirectionType.UpRight:
                    case MapManager.DirectionType.DownRight:
                        OnLookAtRight();
                        return;
                    case MapManager.DirectionType.UpLeft:
                    case MapManager.DirectionType.Left:
                    case MapManager.DirectionType.DownLeft:
                        OnLookAtLeft();
                        return;
                    default:
                        return;
                }
            }
        }
    }
}
