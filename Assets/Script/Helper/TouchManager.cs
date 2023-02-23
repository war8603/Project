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
            //�Էµ� ��ġ ������ ���� ���.
            //��ġ�� ���۵Ǿ����� üũ �� ��ġ ������ �����Ѵ�
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
            // ���� ��ġ ������ �ִ� ���.
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
        // UNITY_EDITOR �� �ƴ� ���       
#else
        _touchInfos.RemoveAll(x => x.Phase == TouchPhase.Ended);

        // updated �ʱ�ȭ
        for (int i = 0; i < _touchInfos.Count; i++)
        {
            _touchInfos[i].IsUpdated = false;
        }

        if (_touchInfos.Count == 0)
        {
            // ����� ��ġ ������ ���� ��ġ�Է��� ���� ��� ���� ���
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
                    // ���� ������ ������� ���� ���.
                    TouchInfo newTouch = new TouchInfo();
                    newTouch.FingerID = Input.touches[i].fingerId;
                    newTouch.Position = Input.touches[i].position;
                    newTouch.Phase = Input.touches[i].phase;
                    newTouch.IsUpdated = true;
                    _touchInfos.Add(newTouch);
                }
                else
                {
                    // ���� ������ ������ ��� ����
                    // ���� fingerID�� �ְ�, ���� ������ Ended�� ���?
                    // 
                    oldInfo.Position = Input.touches[i].position;
                    oldInfo.Phase = Input.touches[i].phase;
                    oldInfo.IsUpdated = true;
                }
            }
        }

        // ���� ��ϵ��� �ʰų� ���ŵ��� ���� ��ġ�� ����.
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
