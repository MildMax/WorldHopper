using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SnakePokerScript))]
public class SnakePokerGUI : Editor
{
    string[] options = { "Continuous", "Timed", "Proximity", "Up" };
    SerializedProperty actionIndex;

    private void OnEnable()
    {
        actionIndex = serializedObject.FindProperty("actionIndex");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Action:", GUILayout.Width(45));
        actionIndex.intValue = EditorGUILayout.Popup(actionIndex.intValue, options, GUILayout.Width(75));

        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
