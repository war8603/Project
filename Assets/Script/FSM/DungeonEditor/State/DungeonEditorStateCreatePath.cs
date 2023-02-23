using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEditorStateCreatePath : DungeonEditorStateBase
{
    SquTile _startPathTile = null;
    SquTile _endPathTile = null;
    public DungeonEditorStateCreatePath() : base()
    {

    }

    public override bool OnEnter()
    {
        _startPathTile = null;
        _endPathTile = null;
        return base.OnEnter();
    }

    public override void OnUpdate()
    {
    }

    public override void OnMouseDown(SquTile selectedTile)
    {
        // ������ Ÿ���� �� �ȿ� ���� Ÿ���̶�� ��� ���� ����
        if (IsCreateablePath(selectedTile) == false)
            return;

        // ù��° Ŭ���ΰ�� -> ����
        if( _startPathTile == null)
        {
            _startPathTile = selectedTile.MainTile;
            _startPathTile.SetPath(true);
        }
        // �ι�° Ŭ���ΰ�� -> ��� ����
        else
        {
            if (selectedTile.MainTile.Point != _startPathTile.Point)
            {
                _endPathTile = selectedTile.MainTile;
                CreatePath();
                _manager.SetPathNumber();

                _startPathTile.SetPath(false);
                _endPathTile.SetPath(false);
                _startPathTile = null;
                _endPathTile = null;
            }
        }
    }

    bool IsCreateablePath(SquTile tile)
    {
        if (tile == null)
            return false;

        if (tile.TileType == SquTile.TileTypes.Room)
            return true;

        if (tile.TileType == SquTile.TileTypes.RoomSide)
            return true;

        return false;
    }

    public void CreatePath()
    {
        // ����, ���� Ÿ�� ���̿� (��, �Ʒ�, ��, ��)�߿��� ���� ����� Ÿ���� ���� Ž��
        DirectionType startPathDir = DirectionType.Down;
        DirectionType endPathDir = DirectionType.Up;
        GetPathDirectionType(_startPathTile.Point, _endPathTile.Point, ref startPathDir, ref endPathDir);

        // Path�� ������ Tile Ž��
        SquTile startPathTile = _manager.GetDirTile(_startPathTile, startPathDir);
        SquTile endPathTile = _manager.GetDirTile(_endPathTile, endPathDir);

        // Ž���� Ÿ���� �������� path ����
        if (startPathTile != null && endPathTile != null && startPathTile.IsPath == false && endPathTile.IsPath == false)
        {
            startPathTile.SetPath(true);
            endPathTile.SetPath(true);

            // Ÿ�Ͽ� Path �߰�
            _startPathTile.AddPath(startPathDir, _endPathTile.Point);
            _endPathTile.AddPath(endPathDir, _startPathTile.Point);
        }
    }

    void GetPathDirectionType(SquPoint startPoint, SquPoint endPoint, ref DirectionType startPathDir, ref DirectionType endPathDir)
    {
        if (Mathf.Abs(startPoint.x - endPoint.x) > Mathf.Abs(startPoint.y - endPoint.y))
        {
            // ���� ��ΰ� �� ���.
            // ������ġ�� ������ġ���� ������ ������� ���� ��ġ�� (��������Ʈ)���� ���� ��ġ�� (����������Ʈ)�� �̵�.
            // ������ġ�� ������ġ���� ������ ������� ���� ��ġ�� (����������Ʈ)���� ���� ��ġ�� (��������Ʈ)�� �̵�.
            bool isMoveRight = startPoint.x > endPoint.x;
            startPathDir = isMoveRight ? DirectionType.Left : DirectionType.Right;
            endPathDir = isMoveRight ? DirectionType.Right : DirectionType.Left;
        }
        else
        {
            // ���� ��ΰ� �� ���.
            // ������ġ�� ������ġ���� ������ ������� ���� ��ġ�� (��������Ʈ)���� ���� ��ġ�� (��������Ʈ)�� �̵�.
            // ������ġ�� ������ġ���� ������ ������� ���� ��ġ�� (��������Ʈ)���� ���� ��ġ�� (��������Ʈ)�� �̵�.
            bool isMoveUp = startPoint.y > endPoint.y;
            startPathDir = isMoveUp ? DirectionType.Down : DirectionType.Up;
            endPathDir = isMoveUp ? DirectionType.Up : DirectionType.Down;
        }
    }

    public override bool OnExit()
    {
        _startPathTile.SetPath(false);
        return base.OnExit();
    }
}