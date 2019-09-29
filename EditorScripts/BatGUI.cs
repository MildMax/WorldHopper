using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//SAYS FOR BAT SCRIPT, IS NOW ATTACHED TO BEE ENEMY
//BAT HAS NEW PATHFINDING-DEPENDENT BEHAVIOR

[CustomEditor(typeof(BatScript))]
public class BatGUI : Editor
{
    private bool _setValue;

    private GUIStyle ToggleButtonStyleNormal = null;
    private GUIStyle ToggleButtonStyleToggled = null;

    BatScript t;

    private void OnEnable()
    {
        t = (BatScript)base.target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (ToggleButtonStyleNormal == null)
        {
            ToggleButtonStyleNormal = "Button";
            ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
            ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Display Area", _setValue ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
        {
            _setValue = !_setValue;
        }

        GUILayout.EndHorizontal();
    }

    public void OnSceneGUI()
    {

        if (_setValue)
        {
            Vector3[] verts =
            {
                new Vector3(t.xMin, t.yMin, 0f),
                new Vector3(t.xMax, t.yMin, 0f),
                new Vector3(t.xMax, t.yMax, 0f),
                new Vector3(t.xMin, t.yMax, 0f)
            };

            //Handles.DrawSolidRectangleWithOutline(new Rect(new Vector2(t.xMax - t.xMin, t.yMax - t.yMin), new Vector2(t.xMax - t.xMin, t.yMax - t.yMin)), Color.red - new Color(0, 0, 0, 0.75f), Color.magenta);
            Handles.DrawSolidRectangleWithOutline(verts, Color.red - new Color(0, 0, 0, 0.75f), Color.magenta);
            
        }
    }
}
