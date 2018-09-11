using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Play : MonoBehaviour
{
    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//

    [SerializeField]
    public Lv_Data lvData;

    public Node[,] BL_Nodes;
    public Node[,] LI_Nodes;

    public Edge[,] BL_U_Edges;
    public Edge[,] BL_V_Edges;

    public Edge[,] LI_U_Edges;
    public Edge[,] LI_V_Edges;

    public int row;
    public int col;

    void Awake()
    {
        Initializa_Level_Data();
    }

    void Update()
    {
        Debug.Log(LI_Nodes[1,1].Position);
    }

    //*!----------------------------!*//
    //*!    Private Functions
    //*!----------------------------!*//

    private void Initializa_Level_Data()
    {
        row = lvData.row;
        col = lvData.col;

        //BL_Nodes = new Node[,];
        //LI_Nodes = new Node[,];
        //BL_U_Edges = new Edge[,];
        //BL_V_Edges = new Edge[,];
        //LI_U_Edges = new Edge[,];
        //LI_V_Edges = new Edge[,];

        //*! Assign [Block] [Node] 2D Array to [Lv_Data] reflected 1D Array
        #region Assign [Block] [Node] 2D Array to Lv_Data 1D Array
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {

            }
        }
        #endregion

        //*! Assign [Line] [Node] 2D Array to [Lv_Data] reflected 1D Array
        #region Assign [Line] [Node] 2D Array to Lv_Data 1D Array
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {

            }
        }
        #endregion

        //*! Assign [Line] [U-Edge] 2D Array to [Lv_Data] reflected 1D Array
        #region Assign [Line] [U-Edge] 2D Array to Lv_Data 1D Array
        for (int i = 0; i < LI_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_U_Edges.GetLength(1); ++j)
            {

            }
        }
        #endregion

        //*! Assign [Line] [V-Edge] 2D Array to [Lv_Data] reflected 1D Array
        #region Assign [Line] [V-Edge] [Handle] [Type] to [Line] [V-Edge] [Data] [Type]
        for (int i = 0; i < LI_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_V_Edges.GetLength(1); ++j)
            {

            }
        }
        #endregion

        //*! Assign [Block] [U-Edge] 2D Array to [Lv_Data] reflected 1D Array
        #region Assign [Block] [U-Edge] 2D Array to Lv_Data 1D Array
        for (int i = 0; i < BL_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_U_Edges.GetLength(1); ++j)
            {

            }
        }
        #endregion

        //*! Assign [Block] [V-Edge] 2D Array to [Lv_Data] reflected 1D Array
        #region Assign [Block] [V-Edge] 2D Array to Lv_Data 1D Array
        for (int i = 0; i < BL_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_V_Edges.GetLength(1); ++j)
            {

            }
        }
        #endregion
    }
}
