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

    SerializedProperty BL_Nodes;
    SerializedProperty LI_Nodes;
    SerializedProperty BL_U_Edges;
    SerializedProperty BL_V_Edges;
    SerializedProperty LI_U_Edges;
    SerializedProperty LI_V_Edges;

    SerializedProperty isSaved;
    SerializedProperty isLoaded;


    private void OnEnable()
    {
        initialRow = serializedObject.FindProperty("initialRow");
        initialCol = serializedObject.FindProperty("initialCol");
        startEditing = serializedObject.FindProperty("startEditing");

        row = serializedObject.FindProperty("row");
        col = serializedObject.FindProperty("col");

        BL_Nodes = serializedObject.FindProperty("BL_Nodes");
        LI_Nodes = serializedObject.FindProperty("LI_Nodes");
        BL_U_Edges = serializedObject.FindProperty("BL_U_Edges");
        BL_V_Edges = serializedObject.FindProperty("BL_V_Edges");
        LI_U_Edges = serializedObject.FindProperty("LI_U_Edges");
        LI_V_Edges = serializedObject.FindProperty("LI_V_Edges");

        isSaved = serializedObject.FindProperty("isSaved");
        isLoaded = serializedObject.FindProperty("isLoaded");

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

        GUILayout.BeginHorizontal();
        {
            if (startEditing.boolValue)
            {
                if (GUILayout.Button("Save Level Data", GUILayout.MaxHeight(64), GUILayout.MaxHeight(32)))
                {
                    isSaved.boolValue = true;
                }

                if (GUILayout.Button("Load Level Data", GUILayout.MaxHeight(64), GUILayout.MaxHeight(32)))
                {
                    isLoaded.boolValue = true;
                }
            }
            else
            {
                GUILayout.Label("Pleasue Initialize Level Size", EditorStyles.boldLabel);
            }

        }
        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
