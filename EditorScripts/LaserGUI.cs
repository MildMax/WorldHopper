using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LaserScript))]
[CanEditMultipleObjects]
public class LaserGUI : Editor
{

    string[] dirs = { "Left", "Right", "Up", "Down" };
    SerializedProperty direction;

    string[] options = { "1", "2", "3", "4" };
    SerializedProperty worldNum;

    string[] colors = { "Blue", "Green", "Red", "Yellow" };
    SerializedProperty laserColor;

    private void OnEnable()
    {
        direction = serializedObject.FindProperty("direction");
        worldNum = serializedObject.FindProperty("worldNum");
        laserColor = serializedObject.FindProperty("laserColor");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Direction:", GUILayout.Width(70f));
        direction.intValue = EditorGUILayout.Popup(direction.intValue, dirs, GUILayout.Width(100));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("World Num:", GUILayout.Width(70f));
        worldNum.intValue = EditorGUILayout.Popup(worldNum.intValue, options, GUILayout.Width(100));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Color:", GUILayout.Width(70f));
        laserColor.intValue = EditorGUILayout.Popup(laserColor.intValue, colors, GUILayout.Width(100));

        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

    }
}
