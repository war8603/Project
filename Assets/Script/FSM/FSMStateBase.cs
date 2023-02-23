using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

interface IStateBase
{
    bool OnEnter();
    void OnUpdate();
    bool OnExit();

}

public class FSMStateBase : IStateBase
{
    public virtual bool OnEnter()
    {
        Debug.Log("OnEndter state : " + this);
        return true;
    }

    public virtual void OnUpdate()
    {

    }

    public virtual bool OnExit()
    {
        Debug.Log("OnExit state : " + this);
        return true;
    }
}