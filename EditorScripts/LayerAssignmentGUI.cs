using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LayerAssignmentScript))]
[CanEditMultipleObjects]
public class LayerAssignmentGUI : Editor
{
    string[] worldOptions = { "1", "2", "3", "4" };
    SerializedProperty worldNum;

    string[] layerOptions = {
        "Foreground",
        "Primary Decorative Layer",
        "Player",
        "Enemies",
        "Collectables",
        "Secondaray Decorative Layer",
        "Worlds",
        "Skeleton",
        "Background"
    };
    SerializedProperty layerImportance;

    private void OnEnable()
    {
        worldNum = serializedObject.FindProperty("worldNum");
        layerImportance = serializedObject.FindProperty("layerImportance");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        //worldnum
        GUILayout.BeginHorizontal();

        GUILayout.Label("World:", GUILayout.Width(70f));
        worldNum.intValue = EditorGUILayout.Popup(worldNum.intValue, worldOptions, GUILayout.Width(100));

        GUILayout.EndHorizontal();

        //layer importance
        GUILayout.BeginHorizontal();

        GUILayout.Label("Render Layer:", GUILayout.Width(70f));
        layerImportance.intValue = EditorGUILayout.Popup(layerImportance.intValue, layerOptions, GUILayout.Width(100));

        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

}
