//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for operating [Pencil Case] inspector.
//*!
//*! Last edit  : 28/09/2018
//*!--------------------------------------------------------------!*//

//*! Using namespaces
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

    SerializedProperty move_Graph_Direction;

    private void OnEnable()
    {
        initialRow = serializedObject.FindProperty("initialRow");
        initialCol = serializedObject.FindProperty("initialCol");
        startEditing = serializedObject.FindProperty("startEditing");

        row = serializedObject.FindProperty("row");
        col = serializedObject.FindProperty("col");

        isSaved = serializedObject.FindProperty("isSaved");
        isLoaded = serializedObject.FindProperty("isLoaded");

        move_Graph_Direction = serializedObject.FindProperty("move_Graph_Direction");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        #region [Initial Row/Col] Label, IntSlider
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Initial Row", GUILayout.Width(70f));
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
            GUILayout.Label("Initial Col", GUILayout.Width(70f));
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
        #endregion

        #region [Row/Col] Label, IntSlider
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Row", GUILayout.Width(35f));
            if (!startEditing.boolValue)
            {
                GUILayout.Label(row.intValue.ToString());
            }
            else
            {
                row.intValue = EditorGUILayout.IntSlider(row.intValue, 2, initialRow.intValue);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Col", GUILayout.Width(35f));
            if (!startEditing.boolValue)
            {
                GUILayout.Label(col.intValue.ToString());
            }
            else
            {
                col.intValue = EditorGUILayout.IntSlider(col.intValue, 2, initialCol.intValue);
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        #region [Save/Load] Buttons
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        {
            if (startEditing.boolValue)
            {
                if (GUILayout.Button("Save Level Data", GUILayout.MaxHeight(32)))
                {
                    isSaved.boolValue = true;
                }

                if (GUILayout.Button("Load Level Data", GUILayout.MaxHeight(32)))
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
        #endregion

        #region [Move Graph] Label and Arrow Buttons
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(85);
            if (startEditing.boolValue)
            {
                GUILayout.Label("Move Graph", EditorStyles.boldLabel);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        {
            if (startEditing.boolValue)
            {
                GUILayout.Space(112);
                if (GUILayout.Button("▲", GUILayout.MaxWidth(30), GUILayout.MaxHeight(30)))
                {
                    move_Graph_Direction.enumValueIndex = 1;
                }
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            if (startEditing.boolValue)
            {
                GUILayout.Space(80);
                if (GUILayout.Button("◄", GUILayout.MaxWidth(30), GUILayout.MaxHeight(30)))
                {
                    move_Graph_Direction.enumValueIndex = 3;
                }

                GUILayout.Space(30);
                if (GUILayout.Button("►", GUILayout.MaxWidth(30), GUILayout.MaxHeight(30)))
                {
                    move_Graph_Direction.enumValueIndex = 4;
                }
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(112);
            if (startEditing.boolValue)
            {
                if (GUILayout.Button("▼", GUILayout.MaxWidth(30), GUILayout.MaxHeight(30)))
                {
                    move_Graph_Direction.enumValueIndex = 2;
                }
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        #endregion

        serializedObject.ApplyModifiedProperties();
    }
}
