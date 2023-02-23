using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class PlayerBase : MonoBehaviour
{
    //protected SkeletonAnimation _anim;
    protected SpineObject _anim;
    public SpineObject Anim => _anim;
    
    public void SetAnim(SpineObject anim)
    {
        _anim = anim;
        _anim.Init();
        _anim.SetAnimation(anim.GetComponent<SkeletonAnimation>());
    }

    public virtual void OnPlayAnim(int trackIndex = 0, string name = "idle1", bool loop = true)
    {
        _anim?.OnPlayAnim(trackIndex, name, loop);
    }

    public virtual void OnUpdate()
    {
        
    }
}
