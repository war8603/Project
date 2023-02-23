using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEditorStateBase : FSMStateBase
{
    protected DungeonEditorStateManager _stateManager;
    protected DungeonEditorManager _manager;
    protected DungeonEditorStateManager.StateType _stateType;

    #region Map Size Fond Style
    protected GUIStyle mapSizeFontStyle;
    #endregion
    public DungeonEditorStateBase()
    {
        mapSizeFontStyle = new GUIStyle();
        mapSizeFontStyle.fontSize = 12;
        mapSizeFontStyle.normal.textColor = Color.white;
    }

    public void SetManager(DungeonEditorManager manager)
    {
        _manager = manager;
    }

    public void SetStateManager(DungeonEditorStateManager stateManager)
    {
        _stateManager = stateManager;
    }

    public void SetStateType(DungeonEditorStateManager.StateType stateType)
    {
        _stateType = stateType;
    }

    public virtual void OnMouseDown(SquTile selectedTile)
    {

    }
}
