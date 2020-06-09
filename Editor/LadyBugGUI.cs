using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LadyBugScript))]
public class LadyBugGUI : Editor
{
    string[] options = { "Auto", "Point", "Traverse" };
    SerializedProperty walkIndex;

    string[] worldOptions = { "1", "2", "3", "4" };
    SerializedProperty worldNum;

    private void OnEnable()
    {
        walkIndex = serializedObject.FindProperty("walkIndex");
        worldNum = serializedObject.FindProperty("worldNum");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Walk Type:", GUILayout.Width(70f));
        walkIndex.intValue = EditorGUILayout.Popup(walkIndex.intValue, options, GUILayout.Width(100));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("World Number:", GUILayout.Width(70f));
        worldNum.intValue = EditorGUILayout.Popup(worldNum.intValue, worldOptions, GUILayout.Width(100));

        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
