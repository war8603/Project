using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HexEditor : HexTile
{
    MapEditorManager _editorManager = null;

    private void OnMouseDown()
    {
        if (_editorManager == null)
        {
            _editorManager = MapEditorManager.Instance;
        }
        if (_editorManager == null)
            return;

        _editorManager.OnClickHex(this);
    }
}
