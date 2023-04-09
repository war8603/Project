using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

public class AIPlayer : BattlePlayerBase
{
    public AIManager AIManager => _aiManager;
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
        if (ActType == BattlePlayerActType.Idle)
        {
            AIProc();
        }
        else if (ActType == BattlePlayerActType.Wait)
        {
            _actionType = BattlePlayerActType.Idle;
        }
    }

    public void AIProc()
    {
        if (PlayerType.Enemy == _playerType)
        {
            // todo : dummy �����̸� �����ٴ�
            _aiManager.MoveToRandomPos(this);
        }
        else
        {
            _aiManager.OnActionAIPlayer(ActType, this);
        }   
    }
}
