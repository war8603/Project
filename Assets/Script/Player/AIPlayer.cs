using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

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
        if (ActType == BattlePlayerActType.Idle)
        {
            AIProc();
        }
    }

    public void AIProc()
    {
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
}
