//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for Assigning [Edge Type] to [Pencil Case]
//*!              [Edge Type] data.
//*!              This class in an experimental class to test using
//*!              editor with MonoBehavior.
//*!
//*! Last edit  : 08/09/2018
//*!--------------------------------------------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LI_Edge_Type : MonoBehaviour
{
    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    public Edge_Type edgeType;

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    private Pencil_Case pc;

#if UNITY_EDITOR
    // Update is called once per frame
    [ContextMenu("Editor_Update")]
    void Update ()
	{
        #region Check if current state is in [Playing Mode] or [Edit Mode]
        //*! Call [Runtime Update] if editor is in test [Playing Mode]
        if (UnityEditor.EditorApplication.isPlaying) { Runtime_Update(); }

        //*! If current event is changing from [Playing Mode] to [Edit Mode] then return
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) { return; }
        #endregion

        /// Write your tools below. Only excuted in [Edit Mode] runtime ///

        pc = FindObjectOfType<Pencil_Case>();

        if (pc.startEditing)
        {
            for (int i = 0; i < pc.LI_U_Edges.GetLength(0); ++i)
            {
                for (int j = 0; j < pc.LI_U_Edges.GetLength(1); ++j)
                {
                    if (gameObject == pc.LI_U_Edges[i, j].Gizmos_GO)
                    {
                        int currType = (int)edgeType;
                        int dataType = (int)pc.LI_U_Edges[i, j].Edge_Type;
                        dataType = currType;
                    }
                }
            }

            for (int i = 0; i < pc.LI_V_Edges.GetLength(0); ++i)
            {
                for (int j = 0; j < pc.LI_V_Edges.GetLength(1); ++j)
                {
                    if (gameObject == pc.LI_V_Edges[i, j].Gizmos_GO)
                    {
                        int currType = (int)edgeType;
                        int dataType = (int)pc.LI_V_Edges[i, j].Edge_Type;
                        dataType = currType;
                    }
                }
            }
        }
	}
#else
    #region Native Unity Update(), calls RuntimeUpdate()
	//*! Update is called once per frame
	private void Update ()
    {
        //*! Ingame runtime update
		Runtime_Update();
	}
    #endregion
#endif

    //*!----------------------------!*//
    //*!    Private Functions
    //*!----------------------------!*//

    //*! Only running this function when game runtime
    [ContextMenu("Runtime_Update")]
    void Runtime_Update()
    {
    }

    //*!----------------------------!*//
    //*!    Custom Subclasses
    //*!----------------------------!*//
    #region [Edge_Type] Enum class
    public enum Edge_Type
    {
        NONE = 0,
        Black_Pen = 1,
    }
    #endregion
}
