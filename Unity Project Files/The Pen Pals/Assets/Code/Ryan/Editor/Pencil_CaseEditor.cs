//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for operating [Pencil Case] inspector.
//*!
//*! Last edit  : 30/09/2018
//*!--------------------------------------------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pencil_Case))]
public class Pencil_CaseEditor : Editor
{
    Texture logo;

    SerializedProperty startEditing;

    SerializedProperty lv_Data;
    
    SerializedProperty showBlockNode;
    SerializedProperty showBlockEdge;
    SerializedProperty showLineNode;
    SerializedProperty showLineEdge;

    SerializedProperty handle_size;

    SerializedProperty initialRow;
    SerializedProperty initialCol;

    SerializedProperty row;
    SerializedProperty col;

    SerializedProperty isSaved;
    SerializedProperty isLoaded;

    SerializedProperty move_Graph_Direction;

    private void OnEnable()
    {
        logo = Resources.Load("Pencil_Case_UI/Pencil_Case_Logo") as Texture;

        lv_Data = serializedObject.FindProperty("lv_Data");

        startEditing = serializedObject.FindProperty("startEditing");

        showBlockNode = serializedObject.FindProperty("showBlockNode");
        showBlockEdge = serializedObject.FindProperty("showBlockEdge");
        showLineNode = serializedObject.FindProperty("showLineNode");
        showLineEdge = serializedObject.FindProperty("showLineEdge");

        handle_size = serializedObject.FindProperty("handle_size");

        initialRow = serializedObject.FindProperty("initialRow");
        initialCol = serializedObject.FindProperty("initialCol");

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

        #region Pencil Case Logo
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(logo, GUILayout.MinWidth(100f), GUILayout.MaxWidth(300f));
        }
        GUILayout.EndHorizontal();
        #endregion

        #region [Level Data] field
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PropertyField(lv_Data, new GUIContent("Insert Level Data"));
        }
        GUILayout.EndHorizontal();
        #endregion

        #region [Start Editing] toggle
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Toggle(startEditing.boolValue, " Start Editing"))
            {
                startEditing.boolValue = true;
            }
            else
            {
                startEditing.boolValue = false;
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        #region [Show/Hide Block Nodes] toggle
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Toggle(showBlockNode.boolValue, " Show/Hide [Block] Nodes"))
            {
                showBlockNode.boolValue = true;
            }
            else
            {
                showBlockNode.boolValue = false;
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        #region [Show/Hide Block Edges] toggle
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Toggle(showBlockEdge.boolValue, " Show/Hide [Block] Edges"))
            {
                showBlockEdge.boolValue = true;
            }
            else
            {
                showBlockEdge.boolValue = false;
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        #region [Show/Hide Line Nodes] toggle
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Toggle(showLineNode.boolValue, " Show/Hide [Line] Nodes"))
            {
                showLineNode.boolValue = true;
            }
            else
            {
                showLineNode.boolValue = false;
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        #region [Show/Hide Line Edges] toggle
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Toggle(showLineEdge.boolValue, " Show/Hide [Line] Edges"))
            {
                showLineEdge.boolValue = true;
            }
            else
            {
                showLineEdge.boolValue = false;
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        #region [Handle Size] Label, FloatSlider
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Gizmos Size", GUILayout.Width(80f));

            handle_size.floatValue = EditorGUILayout.Slider(handle_size.floatValue, 0.5f, 1.5f);
        }
        GUILayout.EndHorizontal();
        #endregion

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
                GUILayout.Label("Please Initialize Level Graph Size", EditorStyles.boldLabel);
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
