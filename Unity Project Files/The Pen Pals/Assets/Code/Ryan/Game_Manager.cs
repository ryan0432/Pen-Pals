//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for Assigning [Node Type] to [Pencil Case]
//*!              [Edge Type] data.
//*!              This class in an experimental class to test using
//*!              editor with MonoBehavior.
//*!
//*! Last edit  : 12/09/2018
//*!--------------------------------------------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
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

    public bool Show_Gizmos;

    public GameObject Black_Pen;
    public GameObject HighLighter_Red;
    public GameObject Block_Blue_Goal;
    public GameObject Block_Red_Goal;
    public GameObject Line_Red_Goal;

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//

    [HideInInspector]
    private int row;
    [HideInInspector]
    private int col;

    [HideInInspector]
    private bool is_Pencil_Case;

    [SerializeField]
    [HideInInspector]
    private Mesh arrw_giz_mesh;
    [SerializeField]
    [HideInInspector]
    private Material bl_giz_mat;
    [SerializeField]
    [HideInInspector]
    private Material li_giz_mat;

    void Awake()
    {
        if (FindObjectOfType<Pencil_Case>() != null) is_Pencil_Case = true;
        Clean_Up_Symbols();
        Initializa_Level_Data();
    }

    void Update()
    {
        Clear_Pencil_Case_Gizmos();
        Render_Node_Traversability_Gizmos();
        Check_Node_Stickers_In_Update();

        //BL_U_Edges[0, 0].Is_Occupied = true;
        //BL_U_Edges[0, 0].Set_Traversability();
        //Debug.Log("BL_U_Edges[0, 0] UP_Node Can_DN: " + BL_U_Edges[0, 0].UP_Node.Can_DN);

        //BL_U_Edges[0, 0].Is_Occupied = false;
        //BL_U_Edges[0, 0].Set_Traversability();
        //Debug.Log("BL_U_Edges[0, 0] UP_Node Can_DN: " + BL_U_Edges[0, 0].UP_Node.Can_DN);
    }

    //*!----------------------------!*//
    //*!    Private Functions
    //*!----------------------------!*//

    [ContextMenu("Initializa_Level_Data")]
    private void Initializa_Level_Data()
    {
        //*! If there is no Level Data pluged in, show error message and return
        #region If there is no Level Data pluged in, show error message and return
        if (lvData == null)
        {
            Debug.Log("There is no Level Data loaded in [Game_Manager]. Please select a Level Data to edit.");
            return;
        }
        #endregion

        //*! Setup row/col for each [Node] & [Edge] array
        #region Setup row/col for each [Node] & [Edge] array
        row = lvData.row;
        col = lvData.col;

        int bl_node_row = row - 1;
        int bl_node_col = col - 1;

        int li_node_row = row;
        int li_node_col = col;

        int bl_U_edge_row = row;
        int bl_U_edge_col = col - 1;

        int bl_V_edge_row = row - 1;
        int bl_V_edge_col = col;

        int li_U_edge_row = row - 1;
        int li_U_edge_col = col;

        int li_V_edge_row = row;
        int li_V_edge_col = col - 1;
        #endregion

        //*! Setup new instances for each [Node] & [Edge] array
        #region Setup new instances for each [Node] & [Edge] array
        BL_Nodes = new Node[bl_node_row, bl_node_col];
        LI_Nodes = new Node[li_node_row, li_node_col];
        BL_U_Edges = new Edge[bl_U_edge_row, bl_U_edge_col];
        BL_V_Edges = new Edge[bl_V_edge_row, bl_V_edge_col];
        LI_U_Edges = new Edge[li_U_edge_row, li_U_edge_col];
        LI_V_Edges = new Edge[li_V_edge_row, li_V_edge_col];
        #endregion

        //*! Setup [Block] - [Node] data
        #region Setup [Block] Nodes and Preset Neighbor Nodes to Null
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                int colSize = BL_Nodes.GetLength(1);
                Node new_node = new Node();
                BL_Nodes[i, j] = new_node;
                BL_Nodes[i, j].Position = lvData.BL_Nodes[colSize * i + j].Position;
                BL_Nodes[i, j].Node_Type = lvData.BL_Nodes[colSize * i + j].Node_Type;
                BL_Nodes[i, j].UP_NODE = null;
                BL_Nodes[i, j].DN_NODE = null;
                BL_Nodes[i, j].LFT_NODE = null;
                BL_Nodes[i, j].RGT_NODE = null;

                //*! Check [Node] [Type] == [Block_Blue_Goal]
                if (BL_Nodes[i, j].Node_Type == Node_Type.Block_Blue_Goal)
                {
                    GameObject new_Gizmos_GO = Instantiate(Block_Blue_Goal, BL_Nodes[i, j].Position, Quaternion.identity, transform.Find("Symbols"));
                    BL_Nodes[i, j].Gizmos_GO = new_Gizmos_GO;
                }

                //* Check [Node] [Type] == [Block_Red_Goal]
                if (BL_Nodes[i, j].Node_Type == Node_Type.Block_Red_Goal)
                {
                    GameObject new_Gizmos_GO = Instantiate(Block_Red_Goal, BL_Nodes[i, j].Position, Quaternion.identity, transform.Find("Symbols"));
                    BL_Nodes[i, j].Gizmos_GO = new_Gizmos_GO;
                }
            }
        }
        #endregion

        #region Setup [Block] Nodes and Neighbor Nodes' linkage in UP & RGT Direction
        for (int i = 0; i < BL_Nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetUpperBound(1); ++j)
            {
                BL_Nodes[i, j].UP_NODE = BL_Nodes[i, j + 1];
                BL_Nodes[i, j].RGT_NODE = BL_Nodes[i + 1, j];
            }
        }

        for (int i = 0; i < BL_Nodes.GetUpperBound(0); ++i)
        {
            BL_Nodes[i, BL_Nodes.GetUpperBound(1)].RGT_NODE = BL_Nodes[i + 1, BL_Nodes.GetUpperBound(1)];
        }

        for (int i = 0; i < BL_Nodes.GetUpperBound(1); ++i)
        {
            BL_Nodes[BL_Nodes.GetUpperBound(0), i].UP_NODE = BL_Nodes[BL_Nodes.GetUpperBound(0), i + 1];
        }
        #endregion

        #region Setup [Block] Nodes and Neighbor Nodes' linkage in DN & LFT Direction
        for (int i = BL_Nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = BL_Nodes.GetUpperBound(1); j > 0; --j)
            {
                BL_Nodes[i, j].DN_NODE = BL_Nodes[i, j - 1];
                BL_Nodes[i, j].LFT_NODE = BL_Nodes[i - 1, j];
            }
        }

        for (int i = BL_Nodes.GetUpperBound(0); i > 0; --i)
        {
            BL_Nodes[i, BL_Nodes.GetLowerBound(1)].LFT_NODE = BL_Nodes[i - 1, BL_Nodes.GetLowerBound(1)];
        }

        for (int i = BL_Nodes.GetUpperBound(1); i > 0; --i)
        {
            BL_Nodes[BL_Nodes.GetLowerBound(0), i].DN_NODE = BL_Nodes[BL_Nodes.GetLowerBound(0), i - 1];
        }
        #endregion

        //*! Setup [Line] - [Node] data
        #region Setup [Line] Nodes and Preset Neighbor Nodes to Null
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {
                int colSize = LI_Nodes.GetLength(1);
                Node new_node = new Node();
                LI_Nodes[i, j] = new_node;
                LI_Nodes[i, j].Position = lvData.LI_Nodes[colSize * i + j].Position;
                LI_Nodes[i, j].Node_Type = lvData.LI_Nodes[colSize * i + j].Node_Type;
                LI_Nodes[i, j].UP_NODE = null;
                LI_Nodes[i, j].DN_NODE = null;
                LI_Nodes[i, j].LFT_NODE = null;
                LI_Nodes[i, j].RGT_NODE = null;

                //* Check [Node] [Type] == [Line_Red_Goal]
                if (LI_Nodes[i, j].Node_Type == Node_Type.Line_Red_Goal)
                {
                    GameObject new_Gizmos_GO = Instantiate(Line_Red_Goal, LI_Nodes[i, j].Position, Quaternion.identity, transform.Find("Symbols"));
                    LI_Nodes[i, j].Gizmos_GO = new_Gizmos_GO;
                }
            }
        }
        #endregion

        #region Setup [Line] Nodes and Neighbor Nodes' linkage in UP & RGT Direction
        for (int i = 0; i < LI_Nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetUpperBound(1); ++j)
            {
                LI_Nodes[i, j].UP_NODE = LI_Nodes[i, j + 1];
                LI_Nodes[i, j].RGT_NODE = LI_Nodes[i + 1, j];
            }
        }

        for (int i = 0; i < LI_Nodes.GetUpperBound(0); ++i)
        {
            LI_Nodes[i, LI_Nodes.GetUpperBound(1)].RGT_NODE = LI_Nodes[i + 1, LI_Nodes.GetUpperBound(1)];
        }

        for (int i = 0; i < LI_Nodes.GetUpperBound(1); ++i)
        {
            LI_Nodes[LI_Nodes.GetUpperBound(0), i].UP_NODE = LI_Nodes[LI_Nodes.GetUpperBound(0), i + 1];
        }
        #endregion

        #region Setup [Line] Nodes and Neighbor Nodes' linkage in DN & LFT Direction
        for (int i = LI_Nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = LI_Nodes.GetUpperBound(1); j > 0; --j)
            {
                LI_Nodes[i, j].DN_NODE = LI_Nodes[i, j - 1];
                LI_Nodes[i, j].LFT_NODE = LI_Nodes[i - 1, j];
            }
        }

        for (int i = LI_Nodes.GetUpperBound(0); i > 0; --i)
        {
            LI_Nodes[i, LI_Nodes.GetLowerBound(1)].LFT_NODE = LI_Nodes[i - 1, LI_Nodes.GetLowerBound(1)];
        }

        for (int i = LI_Nodes.GetUpperBound(1); i > 0; --i)
        {
            LI_Nodes[LI_Nodes.GetLowerBound(0), i].DN_NODE = LI_Nodes[LI_Nodes.GetLowerBound(0), i - 1];
        }
        #endregion

        //*! Setup [Line] - [Edge] data for [Block]
        #region Setup [Line] - [U] direction [Edge] for [Block]
        for (int i = 0; i < LI_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_U_Edges.GetLength(1); ++j)
            {
                int colSize = LI_U_Edges.GetLength(1);

                Edge new_edge_U = new Edge();
                new_edge_U.Edge_Type = lvData.LI_U_Edges[colSize * i + j].Edge_Type;
                new_edge_U.Boarder_Type = Boarder_Type.NONE;
                new_edge_U.Edge_Direction = lvData.LI_U_Edges[colSize * i + j].Edge_Direction;
                new_edge_U.LFT_Node = LI_Nodes[i, j];
                new_edge_U.RGT_Node = LI_Nodes[i + 1, j];

                if (j == 0)
                {
                    new_edge_U.Edge_Type = Edge_Type.Boarder;
                    new_edge_U.Boarder_Type = Boarder_Type.DN;
                    new_edge_U.UP_Node = BL_Nodes[i, j];
                    new_edge_U.DN_Node = null;
                }
                else if (j == LI_U_Edges.GetLength(1) - 1)
                {
                    new_edge_U.Edge_Type = Edge_Type.Boarder;
                    new_edge_U.Boarder_Type = Boarder_Type.UP;
                    new_edge_U.UP_Node = null;
                    new_edge_U.DN_Node = BL_Nodes[i, j - 1];
                }
                else
                {
                    new_edge_U.UP_Node = BL_Nodes[i, j];
                    new_edge_U.DN_Node = BL_Nodes[i, j - 1];
                }

                new_edge_U.Position = lvData.LI_U_Edges[colSize * i + j].Position;

                LI_U_Edges[i, j] = new_edge_U;

                //* Check [Edge] [Type] == [Black_Pen]
                if (LI_U_Edges[i, j].Edge_Type == Edge_Type.Black_Pen)
                {
                    GameObject new_Gizmos_GO = Instantiate(Black_Pen, LI_U_Edges[i, j].Position, LI_U_Edges[i, j].Rotation, transform.Find("Symbols"));
                    LI_U_Edges[i, j].Gizmos_GO = new_Gizmos_GO;
                }
            }
        }
        #endregion

        #region Setup [Line] - [V] direction [Edge] for [Block]
        for (int i = 0; i < LI_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_V_Edges.GetLength(1); ++j)
            {
                int colSize = LI_V_Edges.GetLength(1);

                Edge new_edge_V = new Edge();
                new_edge_V.Edge_Type = lvData.LI_V_Edges[colSize * i + j].Edge_Type;
                new_edge_V.Boarder_Type = Boarder_Type.NONE;
                new_edge_V.Edge_Direction = lvData.LI_V_Edges[colSize * i + j].Edge_Direction;
                new_edge_V.DN_Node = LI_Nodes[i, j];
                new_edge_V.UP_Node = LI_Nodes[i, j + 1];

                if (i == 0)
                {
                    new_edge_V.Edge_Type = Edge_Type.Boarder;
                    new_edge_V.Boarder_Type = Boarder_Type.LFT;
                    new_edge_V.LFT_Node = null;
                    new_edge_V.RGT_Node = BL_Nodes[i, j];
                }
                else if (i == LI_V_Edges.GetLength(0) - 1)
                {
                    new_edge_V.Edge_Type = Edge_Type.Boarder;
                    new_edge_V.Boarder_Type = Boarder_Type.RGT;
                    new_edge_V.LFT_Node = BL_Nodes[i - 1, j];
                    new_edge_V.RGT_Node = null;
                }
                else
                {
                    new_edge_V.LFT_Node = BL_Nodes[i - 1, j];
                    new_edge_V.RGT_Node = BL_Nodes[i, j];
                }

                new_edge_V.Position = lvData.LI_V_Edges[colSize * i + j].Position;
                LI_V_Edges[i, j] = new_edge_V;

                //* Check [Edge] [Type] = [Black_Pen]
                if (LI_V_Edges[i, j].Edge_Type == Edge_Type.Black_Pen)
                {
                    GameObject new_Gizmos_GO = Instantiate(Black_Pen, LI_V_Edges[i, j].Position, LI_V_Edges[i, j].Rotation, transform.Find("Symbols"));
                    LI_V_Edges[i, j].Gizmos_GO = new_Gizmos_GO;
                }
            }
        }
        #endregion

        //*! Setup [Block] - [Edge] data for [Line]
        #region Setup [Block] - [U] direction [Edge] for [Line]
        for (int i = 0; i < BL_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_U_Edges.GetLength(1); ++j)
            {
                int colSize = BL_U_Edges.GetLength(1);

                Edge new_edge_U = new Edge();
                new_edge_U.UP_Node = LI_V_Edges[i, j].UP_Node;
                new_edge_U.DN_Node = LI_V_Edges[i, j].DN_Node;
                new_edge_U.LFT_Node = LI_V_Edges[i, j].LFT_Node;
                new_edge_U.RGT_Node = LI_V_Edges[i, j].RGT_Node;
                new_edge_U.Position = LI_V_Edges[i, j].Position;
                new_edge_U.Edge_Type = lvData.BL_U_Edges[colSize * i + j].Edge_Type;
                new_edge_U.Boarder_Type = Boarder_Type.NONE;
                new_edge_U.Edge_Direction = lvData.BL_U_Edges[colSize * i + j].Edge_Direction;
                BL_U_Edges[i, j] = new_edge_U;

                //* Check [Edge] [Type] = [HighLighter_Red]
                if (BL_U_Edges[i, j].Edge_Type == Edge_Type.HighLighter_Red)
                {
                    GameObject new_Gizmos_GO = Instantiate(HighLighter_Red, BL_U_Edges[i, j].Position, BL_U_Edges[i, j].Rotation, transform.Find("Symbols"));
                    BL_U_Edges[i, j].Gizmos_GO = new_Gizmos_GO;

                }
            }
        }
        #endregion

        #region Setup [Block] - [V] direction [Edge] for [Line]
        for (int i = 0; i < BL_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_V_Edges.GetLength(1); ++j)
            {
                int colSize = BL_V_Edges.GetLength(1);
                Edge new_edge_V = new Edge();
                new_edge_V.UP_Node = LI_U_Edges[i, j].UP_Node;
                new_edge_V.DN_Node = LI_U_Edges[i, j].DN_Node;
                new_edge_V.LFT_Node = LI_U_Edges[i, j].LFT_Node;
                new_edge_V.RGT_Node = LI_U_Edges[i, j].RGT_Node;
                new_edge_V.Position = LI_U_Edges[i, j].Position;
                new_edge_V.Edge_Type = lvData.BL_V_Edges[colSize * i + j].Edge_Type;
                new_edge_V.Boarder_Type = Boarder_Type.NONE;
                new_edge_V.Edge_Direction = lvData.BL_V_Edges[colSize * i + j].Edge_Direction;
                BL_V_Edges[i, j] = new_edge_V;

                //* Check [Edge] [Type] = [HighLighter_Red]
                if (BL_V_Edges[i, j].Edge_Type == Edge_Type.HighLighter_Red)
                {
                    GameObject new_Gizmos_GO = Instantiate(HighLighter_Red, BL_V_Edges[i, j].Position, BL_V_Edges[i, j].Rotation, transform.Find("Symbols"));
                    BL_V_Edges[i, j].Gizmos_GO = new_Gizmos_GO;
                }
            }
        }
        #endregion

        //*! -------------------------------------------- !*//

        //*! Check [Edge] Enums flags to decide every [Node] tracersability to neighbor
        #region Check [Line] - [U-Edge] and set [Block] - [Node] Traversability
        for (int i = 0; i < li_U_edge_row; ++i)
        {
            for (int j = 0; j < li_U_edge_col; ++j)
            {
                Edge curEdge = LI_U_Edges[i, j];

                //*! Check if [U-Edge] is [UP] or [Dn] [Boarder]
                #region Check if [Line] - [U-Edge] is a [Boarder]
                if (curEdge.Edge_Type == Edge_Type.Boarder)
                {
                    if (curEdge.Boarder_Type == Boarder_Type.UP)
                    {
                        curEdge.DN_Node.UP_NODE = null;
                        curEdge.LFT_Node.UP_NODE = null;
                        curEdge.RGT_Node.UP_NODE = null;
                    }

                    if (curEdge.Boarder_Type == Boarder_Type.DN)
                    {
                        curEdge.UP_Node.DN_NODE = null;
                        curEdge.LFT_Node.DN_NODE = null;
                        curEdge.RGT_Node.DN_NODE = null;
                    }
                }
                #endregion

                //*! Check if [U-Edge] is [None]
                //*! If it is a [Boarder] assigned to [None],
                //*! DON'T REBUILD [Node] connection on its [Null] side 
                #region Check if [Line] [U-Edge] is [None]
                if (curEdge.Edge_Type == Edge_Type.NONE)
                {
                    if (curEdge.UP_Node == null || curEdge.DN_Node == null)
                    {
                        if (curEdge.UP_Node == null)
                        {
                            curEdge.DN_Node.UP_NODE = null;
                        }

                        if (curEdge.DN_Node == null)
                        {
                            curEdge.UP_Node.DN_NODE = null;
                        }
                    }
                    else
                    {
                        curEdge.UP_Node.DN_NODE = curEdge.DN_Node;
                        curEdge.DN_Node.UP_NODE = curEdge.UP_Node;
                    }
                }
                #endregion

                //*! Check if [U-Edge] is [Black_Pen]. If true, shut [UP] [DN] traversabilities
                #region Check if [Line] [U-Edge] is [Black_Pen]
                if (curEdge.Edge_Type == Edge_Type.Black_Pen)
                {
                    if (curEdge.UP_Node == null || curEdge.DN_Node == null)
                    {
                        if (curEdge.UP_Node == null)
                        {
                            curEdge.DN_Node.UP_NODE = null;
                        }

                        if (curEdge.DN_Node == null)
                        {
                            curEdge.UP_Node.DN_NODE = null;
                        }
                    }
                    else
                    {
                        curEdge.UP_Node.DN_NODE = null;
                        curEdge.DN_Node.UP_NODE = null;
                    }
                }
                #endregion
            }
        }
        #endregion

        #region Check [Line] - [V-Edge] and set [Block] - [Node] Traversability
        for (int i = 0; i < li_V_edge_row; ++i)
        {
            for (int j = 0; j < li_V_edge_col; ++j)
            {
                Edge curEdge = LI_V_Edges[i, j];

                //*! Check if [Line] [V-Edge] is [LFT] or RGT] [Boarder]
                #region Check if [Line] - [V-Edge] is a [Boarder]
                if (curEdge.Edge_Type == Edge_Type.Boarder)
                {
                    if (curEdge.Boarder_Type == Boarder_Type.LFT)
                    {
                        curEdge.RGT_Node.LFT_NODE = null;
                        curEdge.UP_Node.LFT_NODE = null;
                        curEdge.DN_Node.LFT_NODE = null;
                    }

                    if (curEdge.Boarder_Type == Boarder_Type.RGT)
                    {
                        curEdge.LFT_Node.RGT_NODE = null;
                        curEdge.UP_Node.RGT_NODE = null;
                        curEdge.DN_Node.RGT_NODE = null;
                    }
                }
                #endregion

                //*! Check if [Line] [V-Edge] is [None]
                //*! If it is a [Boarder] assigned to [None],
                //*! DON'T REBUILD [Node] connection on its [Null] side
                #region Check if [Line] [V-Edge] is [None]
                if (curEdge.Edge_Type == Edge_Type.NONE)
                {
                    if (curEdge.LFT_Node == null || curEdge.RGT_Node == null)
                    {
                        if (curEdge.LFT_Node == null)
                        {
                            curEdge.RGT_Node.LFT_NODE = null;
                        }

                        if (curEdge.RGT_Node == null)
                        {
                            curEdge.LFT_Node.RGT_NODE = null;
                        }
                    }
                    else
                    {
                        curEdge.LFT_Node.RGT_NODE = curEdge.RGT_Node;
                        curEdge.RGT_Node.LFT_NODE = curEdge.LFT_Node;
                    }
                }
                #endregion

                //*! Check if [V-Edge] is [Black_Pen]. If true, shut [LFT] [RGT] traversabilities
                #region Check if [Line] [V-Edge] is [Black_Pen]
                if (curEdge.Edge_Type == Edge_Type.Black_Pen)
                {
                    if (curEdge.LFT_Node == null || curEdge.RGT_Node == null)
                    {
                        if (curEdge.LFT_Node == null)
                        {
                            curEdge.RGT_Node.LFT_NODE = null;
                        }

                        if (curEdge.RGT_Node == null)
                        {
                            curEdge.LFT_Node.RGT_NODE = null;
                        }
                    }
                    else
                    {
                        curEdge.LFT_Node.RGT_NODE = null;
                        curEdge.RGT_Node.LFT_NODE = null;
                    }
                }
                #endregion
            }
        }
        #endregion

        #region Check [Block] - [U-Edge] and set [Line] - [Node] Traversability
        for (int i = 0; i < bl_U_edge_row; ++i)
        {
            for (int j = 0; j < bl_U_edge_col; ++j)
            {
                Edge curEdge = BL_U_Edges[i, j];

                //*! Check if [Block] [U-Edge] is [None]
                #region Check if [U-Edge] is [None]
                if (curEdge.Edge_Type == Edge_Type.NONE)
                {
                    curEdge.UP_Node.DN_NODE = curEdge.DN_Node;
                    curEdge.DN_Node.UP_NODE = curEdge.UP_Node;
                }
                #endregion

                //*! Check if [Block] [U-Edge] is [HighLighter_Red]
                #region Check if [Block] [U-Edge] is [HighLighter_Red]
                if (curEdge.Edge_Type == Edge_Type.HighLighter_Red)
                {
                    curEdge.UP_Node.DN_NODE = null;
                    curEdge.DN_Node.UP_NODE = null;
                }
                #endregion
            }
        }
        #endregion

        #region Check [Block] - [V-Edge] and set [Line] - [Node] Traversability
        for (int i = 0; i < bl_V_edge_row; ++i)
        {
            for (int j = 0; j < bl_V_edge_col; ++j)
            {
                Edge curEdge = BL_V_Edges[i, j];

                //*! Check if [Block] [V-Edge] is [None]
                #region Check if [V-Edge] is [None]
                if (curEdge.Edge_Type == Edge_Type.NONE)
                {

                    curEdge.LFT_Node.RGT_NODE = curEdge.RGT_Node;
                    curEdge.RGT_Node.LFT_NODE = curEdge.LFT_Node;

                }
                #endregion

                //*! Check if [Block] [V-Edge] is [HighLighter_Red]
                #region Check if [Block] [V-Edge] is [HighLighter_Red]
                if (curEdge.Edge_Type == Edge_Type.HighLighter_Red)
                {

                    curEdge.LFT_Node.RGT_NODE = null;
                    curEdge.RGT_Node.LFT_NODE = null;

                }
                #endregion
            }
        }
        #endregion
    }

    [ContextMenu("Clean_Up_Symbols")]
    private void Clean_Up_Symbols()
    {
        bool is_Symbol_List_Empty = (transform.Find("Symbols").childCount < 1);

        if (!is_Symbol_List_Empty)
        {
            for (int i = transform.Find("Symbols").childCount; i > 0; --i)
            {
                Destroy(transform.Find("Symbols").GetChild(0).gameObject);
            }
            Debug.Log("Clean Up Symbols");
        }
    }

    [ContextMenu("Clear_Pencil_Case_Gizmos")]
    private void Clear_Pencil_Case_Gizmos()
    {
        //*! If there is a [Pencil_Case_Handle] in the scene,
        //*! clean up gizmos by setting the mesh renderer to enable = false
        if (is_Pencil_Case)
        {
            bool is_BL_Node_Gizmos_List_Empty = (transform.Find("BL_Node_Gizmos").childCount < 1);
            bool is_LI_Node_Gizmos_List_Empty = (transform.Find("LI_Node_Gizmos").childCount < 1);
            bool is_BL_Edges_List_Empty = (transform.Find("BL_Edges_Handles").childCount < 1);
            bool is_LI_Edges_List_Empty = (transform.Find("LI_Edges_Handles").childCount < 1);

            if (!is_BL_Node_Gizmos_List_Empty)
            {
                for (int i  = 0; i < transform.Find("BL_Node_Gizmos").childCount; ++i)
                {
                    transform.Find("BL_Node_Gizmos").GetChild(i).gameObject.SetActive(false);
                }
            }

            if (!is_LI_Node_Gizmos_List_Empty)
            {
                for (int i = 0; i < transform.Find("LI_Node_Gizmos").childCount; ++i)
                {
                    transform.Find("LI_Node_Gizmos").GetChild(i).gameObject.SetActive(false);
                }
            }

            if (!is_BL_Edges_List_Empty)
            {
                for (int i = 0; i < transform.Find("BL_Edges_Handles").childCount; ++i)
                {
                    transform.Find("BL_Edges_Handles").GetChild(i).gameObject.SetActive(false);
                }
            }

            if (!is_LI_Edges_List_Empty)
            {
                for (int i = 0; i < transform.Find("LI_Edges_Handles").childCount; ++i)
                {
                    transform.Find("LI_Edges_Handles").GetChild(i).gameObject.SetActive(false);
                }
            }

            is_Pencil_Case = false;

            Debug.Log("Clear Pencil Case Gizmos in Play Mode!");
        }
    }

    [ContextMenu("Render_Node_Traversability_Gizmos")]
    private void Render_Node_Traversability_Gizmos()
    {
        if (!Show_Gizmos) return;

        #region Setup gizmos' spacing based on handle size
        float handle_size = 0.15f;
        float gizmos_spacing = handle_size * 0.8f;
        #endregion

        #region Setup Gizmos for [Block] Nodes and indication arrows
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                if (BL_Nodes[i, j].Can_UP)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, bl_giz_mat, 0);
                }

                if (BL_Nodes[i, j].Can_DN)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(180f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, bl_giz_mat, 0);
                }

                if (BL_Nodes[i, j].Can_LFT)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(90f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, bl_giz_mat, 0);
                }

                if (BL_Nodes[i, j].Can_RGT)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0)) *
                                           Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size)) *
                                           Matrix4x4.Rotate(Quaternion.AngleAxis(270f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, bl_giz_mat, 0);
                }
            }
        }
        #endregion

        #region Setup Gizmos for [Line] Nodes and indication arrows
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {
                if (LI_Nodes[i, j].Can_UP)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, li_giz_mat, 0);
                }

                if (LI_Nodes[i, j].Can_DN)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(180f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, li_giz_mat, 0);
                }

                if (LI_Nodes[i, j].Can_LFT)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(90f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, li_giz_mat, 0);
                }

                if (LI_Nodes[i, j].Can_RGT)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(270f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, li_giz_mat, 0);
                }
            }
        }
        #endregion
    }

    [ContextMenu("Check_Node_Stickers")]
    private void Check_Node_Stickers_In_Update()
    {
        //*! Check [Block] [Node] [Type]
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                if (BL_Nodes[i, j].Gizmos_GO == null) return;

                //*! If [Node] [Type] == [NONE], setActive to false
                if (BL_Nodes[i, j].Node_Type == Node_Type.NONE)
                {
                    BL_Nodes[i, j].Gizmos_GO.SetActive(false);
                }
                else
                {
                    BL_Nodes[i, j].Gizmos_GO.SetActive(true);
                }
            }
        }

        //*! Check [Line] [Node] [Type]
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {
                if (LI_Nodes[i, j].Gizmos_GO == null) return;

                if (LI_Nodes[i, j].Node_Type == Node_Type.NONE)
                {
                    LI_Nodes[i, j].Gizmos_GO.SetActive(false);
                }
                else
                {
                    LI_Nodes[i, j].Gizmos_GO.SetActive(false);
                }
            }
        }
    }

}
