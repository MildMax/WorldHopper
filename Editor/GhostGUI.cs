using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GhostScript))]
public class GhostGUI : Editor
{

    string[] worldOptions = { "1", "2", "3", "4" };
    SerializedProperty worldNum;

    private void OnEnable()
    {
        worldNum = serializedObject.FindProperty("worldNum");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.BeginHorizontal();

        GUILayout.Label("World Number:", GUILayout.Width(70f));
        worldNum.intValue = EditorGUILayout.Popup(worldNum.intValue, worldOptions, GUILayout.Width(100));

        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
