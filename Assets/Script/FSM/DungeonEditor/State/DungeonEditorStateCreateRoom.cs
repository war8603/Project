using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEditorStateCreateRoom : DungeonEditorStateBase
{
    public DungeonEditorStateCreateRoom() : base()
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
        // �� ���� Ÿ�� ����
        List<SquTile> roomTiles = new List<SquTile>();
        roomTiles.Add(selectedTile);
        roomTiles.AddRange(_manager.GetSideTiles(selectedTile));

        // Ŭ���� Ÿ�� �� �ֺ� Ÿ�Ͽ� ���� ������ �� �ִ��� üũ
        if (roomTiles.Find(x => IsCreateableRoom(x) == false) != null)
            return;
        
        for(int i = 0; i < roomTiles.Count; i++)
        {
            roomTiles[i].SetTileType(roomTiles[i].Point == selectedTile.Point ? SquTile.TileTypes.Room : SquTile.TileTypes.RoomSide);
            roomTiles[i].SetMainTile(selectedTile);
        }
    }

    bool IsCreateableRoom(SquTile tile)
    {
        if (tile == null)
            return false;

        if (tile.TileType == SquTile.TileTypes.Room)
            return false;

        if (tile.TileType == SquTile.TileTypes.RoomSide)
            return false;

        return true;
    }

    public override bool OnExit()
    {
        return base.OnExit();
    }
}
