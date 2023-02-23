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
        // 선택한 타일이 룸 안에 속한 타일이라면 경로 생성 가능
        if (IsCreateablePath(selectedTile) == false)
            return;

        // 첫번째 클릭인경우 -> 저장
        if( _startPathTile == null)
        {
            _startPathTile = selectedTile.MainTile;
            _startPathTile.SetPath(true);
        }
        // 두번째 클릭인경우 -> 경로 생성
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
        // 시작, 종료 타일 사이에 (위, 아래, 좌, 우)중에서 가장 가까운 타일의 방향 탐색
        DirectionType startPathDir = DirectionType.Down;
        DirectionType endPathDir = DirectionType.Up;
        GetPathDirectionType(_startPathTile.Point, _endPathTile.Point, ref startPathDir, ref endPathDir);

        // Path가 생성될 Tile 탐색
        SquTile startPathTile = _manager.GetDirTile(_startPathTile, startPathDir);
        SquTile endPathTile = _manager.GetDirTile(_endPathTile, endPathDir);

        // 탐색한 타일을 기준으로 path 생성
        if (startPathTile != null && endPathTile != null && startPathTile.IsPath == false && endPathTile.IsPath == false)
        {
            startPathTile.SetPath(true);
            endPathTile.SetPath(true);

            // 타일에 Path 추가
            _startPathTile.AddPath(startPathDir, _endPathTile.Point);
            _endPathTile.AddPath(endPathDir, _startPathTile.Point);
        }
    }

    void GetPathDirectionType(SquPoint startPoint, SquPoint endPoint, ref DirectionType startPathDir, ref DirectionType endPathDir)
    {
        if (Mathf.Abs(startPoint.x - endPoint.x) > Mathf.Abs(startPoint.y - endPoint.y))
        {
            // 가로 경로가 더 길다.
            // 시작위치가 종료위치보다 우측에 있을경우 시작 위치의 (왼쪽포인트)에서 종료 위치의 (오른쪽포인트)로 이동.
            // 시작위치가 종료위치보다 좌측에 있을경우 시작 위치의 (오른쪽포인트)에서 종료 위치의 (왼쪽포인트)로 이동.
            bool isMoveRight = startPoint.x > endPoint.x;
            startPathDir = isMoveRight ? DirectionType.Left : DirectionType.Right;
            endPathDir = isMoveRight ? DirectionType.Right : DirectionType.Left;
        }
        else
        {
            // 세로 경로가 더 길다.
            // 시작위치가 종료위치보다 상측에 있을경우 시작 위치의 (하측포인트)에서 종료 위치의 (상측포인트)로 이동.
            // 시작위치가 종료위치보다 하측에 있을경우 시작 위치의 (상측포인트)에서 종료 위치의 (하측포인트)로 이동.
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