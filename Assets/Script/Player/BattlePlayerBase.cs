using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public enum MovingType
{
    None,
    Walk,
    Fly,
}
public enum BattlePlayerActType
{
    Idle,               // AIManager 명령
    ShowMoveRange,
    Moving,             // AIPlayer 제어
    MoveEnd,
    ShowAttackRange,
    Attack,             // AIPlayer 제어
    AttackEnd,          // AIPlayer 제어
}

public class BattlePlayerBase : PlayerBase
{
    protected HexTile _curHex; // 플레이어의 현재 위치
    protected HexTile _destHex; // 이동시 도착위치
    protected HexTile _attackHex; // 공격 위치

    protected float _attackSpeed = 20f; // todo : 공격시 속도. 나중에 바꿔야함.
    protected const float ATTACKSPEED = 20f;

    protected List<HexTile> _moveHexes; // 이동할 위치
    protected BattlePlayerActType _actionType = BattlePlayerActType.Idle;
    protected MovingType _movingType;

    protected PlayerType _playerType;

    #region PlayerData
    public PlayerType Type => _playerType;
    protected PlayerStatus _status;
    public PlayerStatus Status => _status;
    public int MoveRange => _status.MoveRange;
    public int AttackRange => _status.AttackRange;
    public float MoveSpeed => _status.MoveSpeed;
    public int HP => _status.HP;
    public MovingType ActorMovingType => _movingType;
    #endregion

    public HexTile CurHex => _curHex;
    public HexTile DestHex => _destHex;
    public HexTile AttackHex => _attackHex;
    public List<HexTile> MoveHexes => _moveHexes;

    public BattlePlayerActType ActType => _actionType;

    public virtual void SetActionType(BattlePlayerActType actionType)
    {
        _actionType = actionType;
    }

    public virtual void SetMoveHexes(List<HexTile> hexes)
    {
        _moveHexes = hexes;
    }

    public virtual void SetPlayerType(PlayerType type)
    {
        _playerType = type;
    }
    public virtual void SetPlayerData(TRPlayer playerData)
    {
        _status = new PlayerStatus(playerData.Hp, playerData.MoveSpeed, playerData.MoveRange, playerData.AttackRange);
        _movingType = playerData.PlayerMovingType;
    }

    public virtual void SetCurHex(HexTile hex)
    {
        _curHex = hex;
    }

    public virtual void OnPlayAnim(int trackIndex = 0, string name = "idle1", bool loop = true)
    {
        _anim?.OnPlayAnim(trackIndex, name, loop);
    }

    public override void OnUpdate()
    {

    }

    public virtual void OnStartMove(List<HexTile> paths)
    {
        SetActionType(BattlePlayerActType.Moving);
        OnPlayAnim(trackIndex: 0, "run", loop: true);
        _moveHexes = paths;

        if (paths.Count > 0)
            _destHex = paths[paths.Count - 1];
    }

    public void OnEndMove(HexTile nextHex)
    {
        SetActionType(BattlePlayerActType.MoveEnd);
        OnPlayAnim(trackIndex: 0, "idle2", loop: true);
        _curHex = nextHex;
    }

    public void GetDamage(int damage)
    {
        _status.HP -= damage;
        if (_status.HP <= 0)
        {
            Debug.Log("Dead " + this);
            PlayerManager.Instance.RemovePlayer(this);
        }
    }

    public virtual void OnStartAttack(BattlePlayerBase target)
    {
        SetActionType(BattlePlayerActType.Attack);

        HexPoint attackPos = (CurHex.Point - target.CurHex.Point).Normalize;
        _attackHex = MapManager.Instance.GetHex(CurHex.Point - (CurHex.Point - target.CurHex.Point).Normalize);
    }

    public virtual void OnEndAttack()
    {
        SetActionType(BattlePlayerActType.Idle);
    }

    public void OnLookAtLeft()
    {
        _anim.transform.rotation = DungeonManager.Instance.CameraBattle.transform.rotation;
    }

    public void OnLookAtRight()
    {
        _anim.transform.rotation = DungeonManager.Instance.CameraBattle.transform.rotation;
        _anim.transform.Rotate(0, 180f, 0f);
    }

    public void OnLookAtDirction()
    {
        if (_playerType == PlayerType.Friend)
            OnLookAtRight();
        else
            OnLookAtLeft();
    }
}
