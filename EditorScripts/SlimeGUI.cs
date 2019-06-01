using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SlimeScript))]
public class SlimeGUI : Editor
{
    string[] options = new string[] { "Auto", "Point" };
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

            GUILayout.Label("Walk Type: ", GUILayout.Width(70));
            walkIndex.intValue = EditorGUILayout.Popup(walkIndex.intValue, options, GUILayout.Width(100));

        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
