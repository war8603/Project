using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DungeonEditorStateDeletePath : DungeonEditorStateBase
{
    public DungeonEditorStateDeletePath() : base()
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
        List<Tuple<DirectionType, SquTile>> deleteList = new List<Tuple<DirectionType, SquTile>>();
        foreach (KeyValuePair<DirectionType, SquPoint> kv in selectedTile.MainTile.Paths)
        {
            // 선택한 타일이 Path 타일이라면 Paths에서 해당 정보 검색 후 삭제
            SquTile temp = _manager.GetDirTile(selectedTile.MainTile, kv.Key);
            if (selectedTile.IsPath == true && temp.Point == selectedTile.Point)
                DeletePath(selectedTile, kv.Key, kv.Value, ref deleteList);
            // 선택한 타일이 RoomTile이라면 보유한 모든 Path 삭제
            else if (selectedTile.TileType == SquTile.TileTypes.Room)
                DeletePath(selectedTile, kv.Key, kv.Value, ref deleteList);
        }

        for(int i = 0; i < deleteList.Count; i++)
        {
            deleteList[i].Item2.RemovePath(deleteList[i].Item1);
        }

        _manager.SetPathNumber();
    }

    void DeletePath(SquTile selectedTile, DirectionType dirType, SquPoint endRoomTilePoint, ref List<Tuple<DirectionType, SquTile>> deleteList)
    {
        // kv.key => 해당 타일에서 나가는 경로.
        // kv.value => 해당 타일에서 경로를 타고 도착하는 타일의 point
        // mainTile의 경로에서 선택한 타일의 path를 삭제.
        SquTile startRoomTile = selectedTile.MainTile;
        SquTile endRoomTile = _manager.GetTile(endRoomTilePoint);

        DirectionType startPathDir = dirType;
        DirectionType endPathDir = endRoomTile.GetPath(startRoomTile.Point).Item1;

        SquTile startPathTile = _manager.GetDirTile(startRoomTile, startPathDir);
        SquTile endPathTile = _manager.GetDirTile(endRoomTile, endPathDir);

        // path 설정 삭제
        startPathTile.SetPath(false);
        endPathTile.SetPath(false);

        // mailTile에서 path 삭제
        deleteList.Add(new Tuple<DirectionType, SquTile>(startPathDir, startRoomTile));
        deleteList.Add(new Tuple<DirectionType, SquTile>(endPathDir, endRoomTile));
        //startRoomTile.RemovePath(startPathDir);
        //endRoomTile.RemovePath(endPathDir);
    }

    public override bool OnExit()
    {
        return base.OnExit();
    }
}
