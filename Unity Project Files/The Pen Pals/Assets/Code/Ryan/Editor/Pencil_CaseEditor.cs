using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pencil_Case))]
public class Pencil_CaseEditor : Editor
{
    SerializedProperty initialRow;
    SerializedProperty initialCol;
    SerializedProperty startEditing;

    SerializedProperty row;
    SerializedProperty col;

    private void OnEnable()
    {
        initialRow = serializedObject.FindProperty("initialRow");
        initialCol = serializedObject.FindProperty("initialCol");
        startEditing = serializedObject.FindProperty("startEditing");
        row = serializedObject.FindProperty("row");
        col = serializedObject.FindProperty("col");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Initial Row");
            if (startEditing.boolValue)
            {
                GUILayout.Label(initialRow.intValue.ToString());
            }
            else
            {
                initialRow.intValue = EditorGUILayout.IntSlider(initialRow.intValue, 2, 50);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Initial Col");
            if (startEditing.boolValue)
            {
                GUILayout.Label(initialCol.intValue.ToString());
            }
            else
            {
                initialCol.intValue = EditorGUILayout.IntSlider(initialCol.intValue, 2, 50);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Row");
            row.intValue = EditorGUILayout.IntSlider(row.intValue, 2, initialRow.intValue);

        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Col");
            col.intValue = EditorGUILayout.IntSlider(col.intValue, 2, initialCol.intValue);

        }
        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
