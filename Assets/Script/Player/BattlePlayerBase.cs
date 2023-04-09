using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

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
    ShowAttackRange,
    Attack,             // AIPlayer 제어
    Wait,
}

public class BattlePlayerBase : PlayerBase
{
    protected HexTile _curHex; // 플레이어의 현재 위치
    protected HexTile _destHex; // 이동시 도착위치
    protected HexTile attackHex; // 공격 위치

    protected float _attackSpeed = 0.1f; // todo : 공격시 속도. 나중에 바꿔야함.
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

    public override void OnPlayAnim(int trackIndex = 0, string name = "idle1", bool loop = true)
    {
        _anim?.OnPlayAnim(trackIndex, name, loop);
    }

    public override void OnUpdate()
    {

    }

    public virtual void OnStartMove(List<HexTile> paths)
    {
        /*
        SetActionType(BattlePlayerActType.Moving);
        OnPlayAnim(trackIndex: 0, "run", loop: true);
        _moveHexes = paths;

        if (paths.Count > 0)
            _destHex = paths[paths.Count - 1];

        if (paths.Count <= 0)
            return;
        */
        if (paths.Count <= 0)
            return;

        SetActionType(BattlePlayerActType.Moving);
        OnPlayAnim(trackIndex: 0, "run", loop: true);
        _moveHexes = paths;

        OnMoving();
    }

    void OnMoving()
    {
        _destHex = _moveHexes[0];
        _moveHexes.RemoveAt(0);

        OnChangeDirection(_destHex);
        transform.DOMove(_destHex.transform.position, MoveSpeed).SetEase(Ease.Linear).OnComplete(() =>
        {
            _curHex = _destHex;
            if (_moveHexes.Count > 0)
            {
                OnMoving();
            }
            else
            {
                if (this.Type == PlayerType.Friend && AIManager.Instance.IsAttackable(this) == false)
                    SetActionType(BattlePlayerActType.Wait);
                else if (this.Type == PlayerType.Enemy)
                    SetActionType(BattlePlayerActType.Wait);

            }
        });
    }

    public void SetDamage(int damage)
    {
        _status.HP -= damage;
        Debug.Log("Damage!! " + this.name);
        if (_status.HP <= 0)
        {
            Debug.Log("Dead " + this);
            PlayerManager.Instance.RemovePlayer(this);
        }
    }

    public virtual void OnStartAttack(BattlePlayerBase target)
    {
        SetActionType(BattlePlayerActType.Attack);
        OnPlayAnim(trackIndex: 0, "happy", loop: false);
        HexTile attackHex = MapManager.Instance.GetHex(CurHex.Point - (CurHex.Point - target.CurHex.Point).Normalize);
        GameObject effect = BattleManager.Instance.GetSkillEffect("ExplosionEffect");
        effect.transform.SetParent(attackHex.transform);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = Vector3.one;

        Sequence seq = DOTween.Sequence(effect);
        seq.AppendInterval(1f);
        seq.onComplete = () =>
        {
            target.SetDamage(1);
            BattleManager.Instance.SetReturnSkillEffect(effect);
            SetActionType(BattlePlayerActType.Wait);
        };
        /*
        SetActionType(BattlePlayerActType.Attack);

        HexTile attackHex = MapManager.Instance.GetHex(CurHex.Point - (CurHex.Point - target.CurHex.Point).Normalize);

        OnChangeDirection(attackHex);
        transform.DOMove(attackHex.transform.position, _attackSpeed / 2f).OnComplete(() =>
        {
            OnChangeDirection(CurHex);
            transform.DOMove(CurHex.transform.position, _attackSpeed / 2f).OnComplete(() => SetActionType(BattlePlayerActType.Idle));
            target.SetDamage(1);
        });
        */
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
