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
            // ������ Ÿ���� Path Ÿ���̶�� Paths���� �ش� ���� �˻� �� ����
            SquTile temp = _manager.GetDirTile(selectedTile.MainTile, kv.Key);
            if (selectedTile.IsPath == true && temp.Point == selectedTile.Point)
                DeletePath(selectedTile, kv.Key, kv.Value, ref deleteList);
            // ������ Ÿ���� RoomTile�̶�� ������ ��� Path ����
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
        // kv.key => �ش� Ÿ�Ͽ��� ������ ���.
        // kv.value => �ش� Ÿ�Ͽ��� ��θ� Ÿ�� �����ϴ� Ÿ���� point
        // mainTile�� ��ο��� ������ Ÿ���� path�� ����.
        SquTile startRoomTile = selectedTile.MainTile;
        SquTile endRoomTile = _manager.GetTile(endRoomTilePoint);

        DirectionType startPathDir = dirType;
        DirectionType endPathDir = endRoomTile.GetPath(startRoomTile.Point).Item1;

        SquTile startPathTile = _manager.GetDirTile(startRoomTile, startPathDir);
        SquTile endPathTile = _manager.GetDirTile(endRoomTile, endPathDir);

        // path ���� ����
        startPathTile.SetPath(false);
        endPathTile.SetPath(false);

        // mailTile���� path ����
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
