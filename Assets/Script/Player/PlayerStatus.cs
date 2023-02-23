using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus
{
    int _hp = 30;

    int _moveSpeed = 5;
    int _moveRange = 3;

    int _attackRange = 1;

    public int HP
    {
        get { return _hp; }
        set { _hp = value; }
    }

    public int MoveSpeed
    {
        get { return _moveSpeed; }
    }

    public int MoveRange
    {
        get { return _moveRange; }
    }

    public int AttackRange
    {
        get { return _attackRange; }
    }

    public PlayerStatus(int hp, int moveSpeed, int moveRanege, int attackRange)
    {
        _hp = hp;
        _moveSpeed = moveSpeed;
        _moveRange = moveRanege;
        _attackRange = attackRange;
    }
}
