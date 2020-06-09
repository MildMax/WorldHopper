using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TriggerEventListener))]
public class TriggerEventListenerGUI : Editor
{
    string[] worlds = { "1", "2", "3", "4" };
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

        GUILayout.Label("World Num:", GUILayout.Width(70));
        worldNum.intValue = EditorGUILayout.Popup(worldNum.intValue, worlds, GUILayout.Width(100));

        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
