using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardHelper : MonoBehaviour
{
    Camera _camera = null;

    public void Update()
    {
        if(_camera != null)
        {
            transform.rotation = Quaternion.Euler(_camera.transform.rotation.eulerAngles);
        }
    }
    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }
}
