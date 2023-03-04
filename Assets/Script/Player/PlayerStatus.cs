using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus
{
    int _hp = 30;

    float _moveSpeed = 0.3f;
    int _moveRange = 3;

    int _attackRange = 1;

    public int HP
    {
        get { return _hp; }
        set { _hp = value; }
    }

    public float MoveSpeed
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

    public PlayerStatus(int hp, float moveSpeed, int moveRanege, int attackRange)
    {
        _hp = hp;
        _moveSpeed = moveSpeed;
        _moveRange = moveRanege;
        _attackRange = attackRange;
    }
}
