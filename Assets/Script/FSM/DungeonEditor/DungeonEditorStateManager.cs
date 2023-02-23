using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEditorStateManager
{
    // 싱글톤
    private static DungeonEditorStateManager _instance;
    public static DungeonEditorStateManager Instance
    {
        get 
        {
            if (_instance == null)
                _instance = new DungeonEditorStateManager();
            
            return _instance; 
        } 
    }

    // 현재 에디터 상태
    public enum StateType
    {
        None,
        Main,
        CreateRoom,
        DeleteRoom,
        CreatePath,
        DeletePath,
        SetDungeonType,
    }

    // FSM States
    Dictionary<StateType, DungeonEditorStateBase> _states = new Dictionary<StateType, DungeonEditorStateBase>();
    List<StateOperation> _operations = new List<StateOperation>();
    List<StateOperation> _watingOperation = new List<StateOperation>();

    public StateType CurStateType => _curStateType;
    StateType _curStateType;
    StateType _nextStateType;

    // Editor
    public DungeonEditorManager Manager => _manager;
    private DungeonEditorManager _manager;


    public void Init(DungeonEditorManager manager)
    {
        _manager = manager;
        CreateState();
    }

    void CreateState()
    {
        _states.Add(StateType.Main, new DungeonEditorStateMain());
        _states.Add(StateType.CreateRoom, new DungeonEditorStateCreateRoom());
        _states.Add(StateType.DeleteRoom, new DungeonEditorStateDeleteRoom());
        _states.Add(StateType.CreatePath, new DungeonEditorStateCreatePath());
        _states.Add(StateType.DeletePath, new DungeonEditorStateDeletePath());
        _states.Add(StateType.SetDungeonType, new DungeonEditorStateSetDungeonType());
        foreach (KeyValuePair<StateType, DungeonEditorStateBase> kv in _states)
        {
            kv.Value.SetManager(_manager);
            kv.Value.SetStateManager(this);
        }

        ChangeCurStateType(StateType.None);
        ChangeState(StateType.Main);
    }

    public void OnUpdate()
    {
        if(_watingOperation.Count > 0)
        {
            // 현재 대기중인 명령중 종료된 아이템이 있을 경우 삭제.
            List<StateOperation> deleteList = new List<StateOperation>();
            deleteList = _watingOperation.FindAll(x => x.IsEnded == true);
            _watingOperation = _watingOperation.Except(deleteList).ToList();
            _operations = _operations.Except(deleteList).ToList();
        }

        if (_operations.Count > 0)
        {
            // 만약 명령이 존재하면, 해당 명령을 수행한다.
            StateOperation oper = _operations[0];
            oper.OnAction();
            _watingOperation.Add(oper);
        }
        else
        {
            // 명령이 존재하지 않을경우 현재 상태를 업데이트
            _states[_curStateType].OnUpdate();
        }
    }

    bool ChangeCurStateType(StateType type)
    {
        _curStateType = type;
        return true;
    }
    bool ChangeCurStateType()
    {
        return true;
    }

    public void ChangeState(StateType type)
    {
        _nextStateType = type;
        if (_curStateType != StateType.None)
            _operations.Add(new StateOperation(_states[_curStateType].OnExit));
        _operations.Add(new StateOperation(() => ChangeCurStateType(type)));
        _operations.Add(new StateOperation(_states[_nextStateType].OnEnter));
    }

    bool IsCheckCurStateType(DungeonEditorStateManager.StateType type)
    {
        return _curStateType == type;
    }

    public bool IsCreateRoomState()
    {
        return IsCheckCurStateType(StateType.CreateRoom);
    }

    public void OnMouseDown(SquTile selectedTile)
    {
        _states[_curStateType].OnMouseDown(selectedTile);
    }

    public void SetDungeonType(DungeonTypes type)
    {
        if (_curStateType != StateType.SetDungeonType)
            return;

        (_states[_curStateType] as DungeonEditorStateSetDungeonType).SetDungeonType(type);
    }

    public DungeonTypes GetCurDungeonType()
    {
        return (_states[_curStateType] as DungeonEditorStateSetDungeonType).DungeonType;
    }
}

public class StateOperation
{
    private Func<bool> _action;

    public bool IsEnded => _isEnded;
    bool _isEnded = false;

    public bool IsStarted => _isStarted;
    bool _isStarted = false;

    public StateOperation(Func<bool> action)
    {
        // 액션을 등록한다.
        _action = action;
    }

    public void OnAction()
    {
        // 액션을 수행하고 결과값을 리턴받는다.
        if (_isStarted == false)
            JobManager.Instance.CreateJob(Action());
        _isStarted = true;
    }

    IEnumerator Action()
    {
        // 액션을 수행하고, 결과값이 true가 될때까지 시도한다.
        while(true)
        {
            if (_action.Invoke() == true)
            {
                _isEnded = true;
                break;
            }
            else
            {
                yield return null;
            }
        }
    }
}
