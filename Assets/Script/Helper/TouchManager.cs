using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TouchInfo
{
    public int FingerID;
    public Vector2 Position;
    public TouchPhase Phase;
    public bool IsUpdated;
}
public class TouchManager
{
    
    private static TouchManager _instance;
    public static TouchManager Instance
    {
        get { if (_instance == null) _instance = new TouchManager(); return _instance; }
    }

    List<TouchInfo> _touchInfos = new List<TouchInfo>();

    public List<TouchInfo> TouchInfos => _touchInfos;

    public void OnUpdate()
    {
#if UNITY_EDITOR
        if (_touchInfos.Count == 0)
        {
            //입력된 터치 정보가 없을 경우.
            //터치가 시작되었는지 체크 후 터치 정보를 기입한다
            if (Input.GetMouseButtonDown(0) == true)
            {
                TouchInfo newTouch = new TouchInfo();
                newTouch.FingerID = 0;
                newTouch.Position = Input.mousePosition;
                newTouch.Phase = TouchPhase.Began;
                _touchInfos.Add(newTouch);
            }
        }
        else
        {
            // 만약 터치 정보가 있는 경우.
            TouchInfo preTouchInfo = _touchInfos[0];
            if (Input.GetMouseButtonUp(0) == true)
            {
                preTouchInfo.Position = Input.mousePosition;
                preTouchInfo.Phase = TouchPhase.Ended;
            }
            else if (Input.GetMouseButton(0) == true)
            {
                Vector2 prePos = preTouchInfo.Position;
                preTouchInfo.Position = Input.mousePosition;
                preTouchInfo.Phase = Vector2.Distance(prePos, Input.mousePosition) > 0.1f ? TouchPhase.Moved : TouchPhase.Stationary;
            }
            else
            {
                _touchInfos.Clear();
            }
        }
        // UNITY_EDITOR 가 아닌 경우       
#else
        _touchInfos.RemoveAll(x => x.Phase == TouchPhase.Ended);

        // updated 초기화
        for (int i = 0; i < _touchInfos.Count; i++)
        {
            _touchInfos[i].IsUpdated = false;
        }

        if (_touchInfos.Count == 0)
        {
            // 저장된 터치 정보가 없고 터치입력이 있을 경우 전부 등록
            if (Input.touchCount > 0)
            {
                for(int i = 0; i < Input.touches.Length; i++)
                {
                    Touch touch = Input.touches[i];
                    TouchInfo newTouch = new TouchInfo();
                    newTouch.FingerID = touch.fingerId;
                    newTouch.Position = touch.position;
                    newTouch.Phase = touch.phase;
                    newTouch.IsUpdated = true;
                    _touchInfos.Add(newTouch);
                }
            }
        }
        else
        {
            List<Touch> newTouches = new List<Touch>();
            for(int i = 0; i < Input.touches.Length; i++)
            {
                TouchInfo oldInfo = _touchInfos.Find(x => x.FingerID == Input.touches[i].fingerId);
                if (oldInfo == null)
                {
                    // 이전 정보가 없을경우 새로 등록.
                    TouchInfo newTouch = new TouchInfo();
                    newTouch.FingerID = Input.touches[i].fingerId;
                    newTouch.Position = Input.touches[i].position;
                    newTouch.Phase = Input.touches[i].phase;
                    newTouch.IsUpdated = true;
                    _touchInfos.Add(newTouch);
                }
                else
                {
                    // 이전 정보가 존재할 경우 갱신
                    // 같은 fingerID가 있고, 이전 정보가 Ended인 경우?
                    // 
                    oldInfo.Position = Input.touches[i].position;
                    oldInfo.Phase = Input.touches[i].phase;
                    oldInfo.IsUpdated = true;
                }
            }
        }

        // 새로 등록되지 않거나 갱신되지 않은 터치는 삭제.
        _touchInfos.RemoveAll(x => x.IsUpdated == false);
#endif
    }
    public TouchInfo GetTouch(int fingerID = 0)
    {
        return _touchInfos.Find(x => x.FingerID == fingerID);
    }

    public Vector2 GetTouchPosition(int fingerID = 0)
    {
        TouchInfo touchInfo = _touchInfos.Find(x => x.FingerID == fingerID);
        return touchInfo == null ? Vector2.zero : touchInfo.Position;
    }
}
