using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEditorStateDeleteRoom : DungeonEditorStateBase
{
    public DungeonEditorStateDeleteRoom() : base()
    {

    }

    public override bool OnEnter()
    {
        return base.OnEnter();
    }

    public override void OnUpdate()
    {
    }

    public override void OnMouseDown(SquTile selectedTile)
    {
        // 선택된 타일의 메인타일로 변경
        selectedTile = selectedTile.MainTile;
        if (selectedTile == null)
            return;

        // 룸 범위 타일 생성
        List<SquTile> roomTiles = new List<SquTile>();
        roomTiles.Add(selectedTile);
        roomTiles.AddRange(_manager.GetSideTiles(selectedTile));

        // 범위 타일들에 대해 삭제가 가능한지 체크 
        if (roomTiles.Find(x => IsDeleteable(x) == false) != null)
            return;

        for (int i = 0; i < roomTiles.Count; i++)
        {
            roomTiles[i].SetTileType(SquTile.TileTypes.None);
            roomTiles[i].SetMainTile(selectedTile);
        }
    }

    bool IsDeleteable(SquTile tile)
    {
        if (tile == null)
            return false;

        if (tile.TileType == SquTile.TileTypes.Room)
            return true;

        if (tile.TileType == SquTile.TileTypes.RoomSide)
            return true;

        return false;
    }


    public override bool OnExit()
    {
        return base.OnExit();
    }
}
