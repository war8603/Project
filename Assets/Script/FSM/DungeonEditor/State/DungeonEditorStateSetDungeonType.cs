using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEditorStateSetDungeonType : DungeonEditorStateBase
{
    public DungeonTypes DungeonType => _dungeonType;
    DungeonTypes _dungeonType;

    
    public DungeonEditorStateSetDungeonType() : base()
    {

    }

    public override bool OnEnter()
    {
        _dungeonType = DungeonTypes.Normal;
        return base.OnEnter();
    }

    public override void OnUpdate()
    {
    }

    public void SetDungeonType(DungeonTypes type)
    {
        _dungeonType = type;
    }

    public override void OnMouseDown(SquTile selectedTile)
    {
        // 선택한 타일이 룸 안에 속한 타일이라면 경로 생성 가능
        if (selectedTile.TileType != SquTile.TileTypes.Room && selectedTile.TileType != SquTile.TileTypes.RoomSide)
            return;

        SquTile mainTile = selectedTile.MainTile;
        mainTile.SetDungeonType(_dungeonType);
        if (_dungeonType == DungeonTypes.Start)
        {
            for(int i = 0; i < _manager.Tiles.Count; i++)
            {
                if (_manager.Tiles[i].Point == mainTile.Point || _manager.Tiles[i].DungeonType != DungeonTypes.Start)
                    continue;

                _manager.Tiles[i].SetDungeonType(DungeonTypes.Normal);
            }
        }
    }

    public override bool OnExit()
    {
        return base.OnExit();
    }
}
