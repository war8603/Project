using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEditorStateManager
{
    // �̱���
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

    // ���� ������ ����
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
            // ���� ������� ����� ����� �������� ���� ��� ����.
            List<StateOperation> deleteList = new List<StateOperation>();
            deleteList = _watingOperation.FindAll(x => x.IsEnded == true);
            _watingOperation = _watingOperation.Except(deleteList).ToList();
            _operations = _operations.Except(deleteList).ToList();
        }

        if (_operations.Count > 0)
        {
            // ���� ����� �����ϸ�, �ش� ����� �����Ѵ�.
            StateOperation oper = _operations[0];
            oper.OnAction();
            _watingOperation.Add(oper);
        }
        else
        {
            // ����� �������� ������� ���� ���¸� ������Ʈ
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
        // �׼��� ����Ѵ�.
        _action = action;
    }

    public void OnAction()
    {
        // �׼��� �����ϰ� ������� ���Ϲ޴´�.
        if (_isStarted == false)
            JobManager.Instance.CreateJob(Action());
        _isStarted = true;
    }

    IEnumerator Action()
    {
        // �׼��� �����ϰ�, ������� true�� �ɶ����� �õ��Ѵ�.
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
