using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SnailScript))]
public class SnailGUI : Editor
{
    string[] options = { "Auto", "Point", "Traverse" };
    SerializedProperty walkIndex;

    private void OnEnable()
    {
        walkIndex = serializedObject.FindProperty("walkIndex");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Walk Type:", GUILayout.Width(70f));
        walkIndex.intValue = EditorGUILayout.Popup(walkIndex.intValue, options, GUILayout.Width(100));

        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

}
