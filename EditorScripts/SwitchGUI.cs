using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SwitchScript))]
[CanEditMultipleObjects]
public class SwitchGUI : Editor
{
    string[] colors = { "Blue", "Green", "Red", "Yellow" };
    SerializedProperty colorIndex;

    private void OnEnable()
    {
        colorIndex = serializedObject.FindProperty("colorIndex");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Color:", GUILayout.Width(70f));
        colorIndex.intValue = EditorGUILayout.Popup(colorIndex.intValue, colors, GUILayout.Width(100f));

        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
