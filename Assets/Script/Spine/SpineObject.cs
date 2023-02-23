using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SpineObject : MonoBehaviour
{
    SkeletonAnimation _anim;
    public SkeletonAnimation Anim => _anim;

    public void Init()
    {
        MeshCollider meshCol = this.gameObject.AddComponent<MeshCollider>();
        meshCol.convex = true;
        meshCol.isTrigger = true;
    }
    public void SetAnimation(SkeletonAnimation anim)
    {
        _anim = anim;
    }

    public void OnPlayAnim(int trackIndex = 0, string name = "idle1", bool loop = true)
    {
        _anim?.AnimationState.SetAnimation(trackIndex, name, loop);
    }

    public void OnMouseDown()
    {
        //Debug.Log("mouse down");
    }

    public void OnMouseDrag()
    {
        //Debug.Log("mouse Drag");
    }

    public void OnMouseUp()
    {
        //Debug.Log("mouse up");
    }
}
