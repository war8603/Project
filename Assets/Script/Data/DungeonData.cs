using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData
{
    public class Info
    {
        public int index;
        public DungeonTypes dungeonType;
        public SquPoint Point;
        public Dictionary<DirectionType, SquPoint> Path;
    }
    string _dungeonName = string.Empty;
    List<Info> _infos = new List<Info>();

    public string DungeonName
    {
        get { return _dungeonName; }
        set { _dungeonName = value; }
    }

    public List<Info> Infos
    {
        get { return _infos; }
        set { _infos = value; }
    }
}
