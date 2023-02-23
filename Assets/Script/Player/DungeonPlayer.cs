using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class DungeonPlayer : PlayerBase
{
    private Vector3 _destPos;
    private SquPoint _roomPoint;

    public Vector3 Position
    {
        get { return this.transform.position; }
        set { this.transform.position = value; }
    }
    private void Awake()
    {
        
    }

    public SquPoint RoomPoint
    {
        get { return _roomPoint; }
        set { _roomPoint = value; }
    }

    public override void OnUpdate()
    {
    }
}
