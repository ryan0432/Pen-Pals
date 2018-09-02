//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for operating shape editor in edit mode.
//*!              This class in an experimental class to test using
//*!              editor with MonoBehavior.
//*!
//*! Last edit  : 02/09/2018
//*!--------------------------------------------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Pencil_Case : MonoBehaviour
{
    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//

    #region Public Vars
    [Range(2, 50)]
    public int initialRow;
    [Range(2, 50)]
    public int initialCol;

    [Range(2, 50)]
    public int row;
    [Range(2, 50)]
    public int col;

    [Range(0.1f, 1.0f)]
    public float handle_size;

    public Node[,] BL_Nodes { get; set; }
    public Node[,] LI_Nodes { get; set; }

    public List<Edge> BL_Edges { get; set; }
    public List<Edge> LI_Edges { get; set; }

    /// [Deprecated] varibles in this section ///
    /// ------------------------------------- ///
    #endregion

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//

    #region Private Vars
    //!* Gizmos prefabs for [Instantiate] mode
    [SerializeField]
    [HideInInspector]
    private GameObject bl_node_giz;
    [SerializeField]
    [HideInInspector]
    private GameObject bl_arrw_giz;
    [SerializeField]
    [HideInInspector]
    private GameObject li_node_giz;
    [SerializeField]
    [HideInInspector]
    private GameObject li_arrw_giz;

    [SerializeField]
    [HideInInspector]
    private GameObject edge_giz;


    //!* Gizmos meshes and materials for [DrawMesh] mode
    [SerializeField]
    [HideInInspector]
    private Mesh node_giz;
    [SerializeField]
    [HideInInspector]
    private Mesh arrw_giz;
    [SerializeField]
    [HideInInspector]
    private Material bl_giz_mat;
    [SerializeField]
    [HideInInspector]
    private Material li_giz_mat;
    #endregion


#if UNITY_EDITOR
    //*! This [Update] only runs in edit mode
    [ContextMenu("Editor_Update")]
    private void Update()
    {
        #region Check if current state is in [Playing Mode] or [Edit Mode]
        //*! Call [Runtime Update] if editor is in test [Playing Mode]
        if (UnityEditor.EditorApplication.isPlaying) { Runtime_Update(); }

        //*! If current event is changing from [Playing Mode] to [Edit Mode] then return
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) { return; }
        #endregion

        /// Write your tools below. Only excuted in [Edit Mode] runtime ///

        Update_Graph();
        Layout_Graph();
        Render_Graph();
    }

    //*! [OnValidate] only works in [Editor Mode] changing related varibles in realtime
    [ContextMenu("OnValidate")]
    private void OnValidate()
    {
        //*! Limit the max number of [Row] and [Col] bound to [initialRow] and [initialCol]
        if (row > initialRow)
        {
            row = initialRow;
        }

        if (col > initialCol)
        {
            col = initialCol;
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

    //*! Setup [Block] and [Line] nodes by the number of row and col
    [ContextMenu("Layout_Graph")]
    private void Layout_Graph()
    {
        //*! Populate the graph with [Node] default construction
        #region Setup instances for [Block] and [Line] Nodes and Edges
        BL_Nodes = new Node[row - 1, col - 1];
        LI_Nodes = new Node[row, col];
        BL_Edges = new List<Edge>();
        LI_Edges = new List<Edge>();

        /// [Deprecated] varibles in this section ///
        /// ------------------------------------- ///

        #endregion

        #region Setup [Block] Nodes and Preset Neighbor Nodes to Null
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                Node new_node = new Node();
                BL_Nodes[i, j] = new_node;
                BL_Nodes[i, j].Position = new Vector3(i + 0.5f, j + 0.5f);
                BL_Nodes[i, j].Node_Size = handle_size;
                BL_Nodes[i, j].UP_NODE = null;
                BL_Nodes[i, j].DN_NODE = null;
                BL_Nodes[i, j].LFT_NODE = null;
                BL_Nodes[i, j].RGT_NODE = null;
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

        #region Setup [Line] Nodes and Preset Neighbor Nodes to Null
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {
                Node new_node = new Node();
                LI_Nodes[i, j] = new_node;
                LI_Nodes[i, j].Position = new Vector3(i, j);
                LI_Nodes[i, j].Node_Size = handle_size;
                LI_Nodes[i, j].UP_NODE = null;
                LI_Nodes[i, j].DN_NODE = null;
                LI_Nodes[i, j].LFT_NODE = null;
                LI_Nodes[i, j].RGT_NODE = null;
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

        #region Setup Edges for [Block] Nodes
        for (int i = 0; i < BL_Nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetUpperBound(1); ++j)
            {
                //*! Setup [Up] edge for Block Nodes
                Edge new_edge_U = new Edge();
                new_edge_U.Type = Edge_Type.NONE;
                new_edge_U.Start_Node = BL_Nodes[i, j];
                new_edge_U.End_Node = BL_Nodes[i, j + 1];
                new_edge_U.Position = new_edge_U.Start_Node.Position + ((new_edge_U.End_Node.Position - new_edge_U.Start_Node.Position)/2);
                new_edge_U.Normal = Vector3.Cross(Vector3.Normalize(new_edge_U.End_Node.Position - new_edge_U.Start_Node.Position), Vector3.forward);
                BL_Edges.Add(new_edge_U);

                //*! Setup [RGT] edge for Block Nodes
                Edge new_edge_R = new Edge();
                new_edge_R.Type = Edge_Type.NONE;
                new_edge_R.Start_Node = BL_Nodes[i, j];
                new_edge_R.End_Node = BL_Nodes[i + 1, j];
                new_edge_R.Position = new_edge_R.Start_Node.Position + ((new_edge_R.End_Node.Position - new_edge_R.Start_Node.Position) / 2);
                new_edge_R.Normal = Vector3.Cross(Vector3.Normalize(new_edge_R.End_Node.Position - new_edge_R.Start_Node.Position), Vector3.forward);
                BL_Edges.Add(new_edge_R);
            }
        }

        for (int i = 0; i < BL_Nodes.GetUpperBound(0); ++i)
        {
            int arrayYBound = BL_Nodes.GetUpperBound(1);
            Edge new_edge_R = new Edge();
            new_edge_R.Type = Edge_Type.NONE;
            new_edge_R.Start_Node = BL_Nodes[i, arrayYBound];
            new_edge_R.End_Node = BL_Nodes[i + 1, arrayYBound];
            new_edge_R.Position = new_edge_R.Start_Node.Position + ((new_edge_R.End_Node.Position - new_edge_R.Start_Node.Position) / 2);
            new_edge_R.Normal = Vector3.Cross(Vector3.Normalize(new_edge_R.End_Node.Position - new_edge_R.Start_Node.Position), Vector3.forward);
            BL_Edges.Add(new_edge_R);

        }

        for (int i = 0; i < BL_Nodes.GetUpperBound(1); ++i)
        {
            int arrayXBound = BL_Nodes.GetUpperBound(0);
            Edge new_edge_U = new Edge();
            new_edge_U.Type = Edge_Type.NONE;
            new_edge_U.Start_Node = BL_Nodes[arrayXBound, i];
            new_edge_U.End_Node = BL_Nodes[arrayXBound, i + 1];
            new_edge_U.Position = new_edge_U.Start_Node.Position + ((new_edge_U.End_Node.Position - new_edge_U.Start_Node.Position) / 2);
            new_edge_U.Normal = Vector3.Cross(Vector3.Normalize(new_edge_U.End_Node.Position - new_edge_U.Start_Node.Position), Vector3.forward);
            BL_Edges.Add(new_edge_U);
        }
        #endregion

        /// [Deprecated] code in this section ///
        #region Layout [Edge] in graph (Two Direction Old Way [Deprecated])
        //*! Iterate from the buttom left node of [Block] 2D array to check [UP] & [RGT] direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Block] Edges in UP & RGT Direction
        //for (int i = 0; i < BL_Nodes.GetUpperBound(0); ++i)
        //{
        //    for (int j = 0; j < BL_Nodes.GetUpperBound(1); ++j)
        //    {
        //        if (BL_Nodes[i, j].Can_UP)
        //        {
        //            Edge new_edge = new Edge();
        //            new_edge.Start_Node = BL_Nodes[i, j];
        //            new_edge.End_Node = BL_Nodes[i, j + 1];
        //            BL_Edges.Add(new_edge);
        //        }

        //        if (BL_Nodes[i, j].Can_RGT)
        //        {
        //            Edge new_edge = new Edge();
        //            new_edge.Start_Node = BL_Nodes[i, j];
        //            new_edge.End_Node = BL_Nodes[i + 1, j];
        //            BL_Edges.Add(new_edge);
        //        }
        //    }
        //}

        //for (int i = 0; i < BL_Nodes.GetUpperBound(0); ++i)
        //{
        //    if (BL_Nodes[i, BL_Nodes.GetUpperBound(1)].Can_RGT)
        //    {
        //        Edge new_edge = new Edge();
        //        new_edge.Start_Node = BL_Nodes[i, BL_Nodes.GetUpperBound(1)];
        //        new_edge.End_Node = BL_Nodes[i + 1, BL_Nodes.GetUpperBound(1)];
        //        BL_Edges.Add(new_edge);
        //    }
        //}

        //for (int i = 0; i < BL_Nodes.GetUpperBound(1); ++i)
        //{
        //    if (BL_Nodes[BL_Nodes.GetUpperBound(0), i].Can_UP)
        //    {
        //        Edge new_edge = new Edge();
        //        new_edge.Start_Node = BL_Nodes[BL_Nodes.GetUpperBound(0), i];
        //        new_edge.End_Node = BL_Nodes[BL_Nodes.GetUpperBound(0), i + 1];
        //        BL_Edges.Add(new_edge);
        //    }
        //}
        #endregion

        //*! Iterate from the upper right node of [Block] 2D array to check [DN] & [LFT] direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Block] Edges in DN & LFT Direction (Two Direction Old Way [Deprecated])
        //for (int i = BL_Nodes.GetUpperBound(0); i > 0; --i)
        //{
        //    for (int j = BL_Nodes.GetUpperBound(1); j > 0; --j)
        //    {
        //        if (BL_Nodes[i, j].Can_DN)
        //        {
        //            Edge new_edge = new Edge();
        //            new_edge.Start_Node = BL_Nodes[i, j];
        //            new_edge.End_Node = BL_Nodes[i, j - 1];
        //            BL_Edges.Add(new_edge);
        //        }

        //        if (BL_Nodes[i, j].Can_LFT)
        //        {
        //            Edge new_edge = new Edge();
        //            new_edge.Start_Node = BL_Nodes[i, j];
        //            new_edge.End_Node = BL_Nodes[i - 1, j];
        //            BL_Edges.Add(new_edge);
        //        }
        //    }
        //}

        //for (int i = BL_Nodes.GetUpperBound(0); i > 0; --i)
        //{
        //    if (BL_Nodes[i, BL_Nodes.GetLowerBound(1)].Can_LFT)
        //    {
        //        Edge new_edge = new Edge();
        //        new_edge.Start_Node = BL_Nodes[i, BL_Nodes.GetLowerBound(1)];
        //        new_edge.End_Node = BL_Nodes[i - 1, BL_Nodes.GetLowerBound(1)];
        //        BL_Edges.Add(new_edge);
        //    }
        //}

        //for (int i = BL_Nodes.GetUpperBound(1); i > 0; --i)
        //{
        //    if (BL_Nodes[BL_Nodes.GetLowerBound(0), i].Can_DN)
        //    {
        //        Edge new_edge = new Edge();
        //        new_edge.Start_Node = BL_Nodes[BL_Nodes.GetLowerBound(0), i];
        //        new_edge.End_Node = BL_Nodes[BL_Nodes.GetLowerBound(0), i - 1];
        //        BL_Edges.Add(new_edge);
        //    }
        //}
        #endregion

        //*! Iterate from the upper right node of [Line] 2D array to check [UP] & [RGT] direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Line] Edges in UP & Right Direction (Two Direction Old Way [Deprecated])
        //for (int i = 0; i < LI_Nodes.GetUpperBound(0); ++i)
        //{
        //    for (int j = 0; j < LI_Nodes.GetUpperBound(1); ++j)
        //    {
        //        if (LI_Nodes[i, j].Can_UP)
        //        {
        //            Edge new_edge = new Edge();
        //            new_edge.Start_Node = LI_Nodes[i, j];
        //            new_edge.End_Node = LI_Nodes[i, j + 1];
        //            LI_Edges.Add(new_edge);
        //        }

        //        if (LI_Nodes[i, j].Can_RGT)
        //        {
        //            Edge new_edge = new Edge();
        //            new_edge.Start_Node = LI_Nodes[i, j];
        //            new_edge.End_Node = LI_Nodes[i + 1, j];
        //            LI_Edges.Add(new_edge);
        //        }
        //    }
        //}

        //for (int i = 0; i < LI_Nodes.GetUpperBound(0); ++i)
        //{
        //    if (LI_Nodes[i, LI_Nodes.GetUpperBound(1)].Can_RGT)
        //    {
        //        Edge new_edge = new Edge();
        //        new_edge.Start_Node = LI_Nodes[i, LI_Nodes.GetUpperBound(1)];
        //        new_edge.End_Node = LI_Nodes[i + 1, LI_Nodes.GetUpperBound(1)];
        //        LI_Edges.Add(new_edge);
        //    }
        //}

        //for (int i = 0; i < LI_Nodes.GetUpperBound(1); ++i)
        //{
        //    if (LI_Nodes[LI_Nodes.GetUpperBound(0), i].Can_UP)
        //    {
        //        Edge new_edge = new Edge();
        //        new_edge.Start_Node = LI_Nodes[LI_Nodes.GetUpperBound(0), i];
        //        new_edge.End_Node = LI_Nodes[LI_Nodes.GetUpperBound(0), i + 1];
        //        LI_Edges.Add(new_edge);
        //    }
        //}
        #endregion

        //*! Iterate from the upper right node of [Line] 2D array to check [DN] & [LFT] direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Line] Edges in DN & LFT Direction (Two Direction Old Way [Deprecated])
        //for (int i = LI_Nodes.GetUpperBound(0); i > 0; --i)
        //{
        //    for (int j = LI_Nodes.GetUpperBound(1); j > 0; --j)
        //    {
        //        if (LI_Nodes[i, j].Can_DN)
        //        {
        //            Edge new_edge = new Edge();
        //            new_edge.Start_Node = LI_Nodes[i, j];
        //            new_edge.End_Node = LI_Nodes[i, j - 1];
        //            LI_Edges.Add(new_edge);
        //        }

        //        if (LI_Nodes[i, j].Can_LFT)
        //        {
        //            Edge new_edge = new Edge();
        //            new_edge.Start_Node = LI_Nodes[i, j];
        //            new_edge.End_Node = LI_Nodes[i - 1, j];
        //            LI_Edges.Add(new_edge);
        //        }
        //    }
        //}

        //for (int i = LI_Nodes.GetUpperBound(0); i > 0; --i)
        //{
        //    if (LI_Nodes[i, LI_Nodes.GetLowerBound(1)].Can_LFT)
        //    {
        //        Edge new_edge = new Edge();
        //        new_edge.Start_Node = LI_Nodes[i, LI_Nodes.GetLowerBound(1)];
        //        new_edge.End_Node = LI_Nodes[i - 1, LI_Nodes.GetLowerBound(1)];
        //        LI_Edges.Add(new_edge);
        //    }
        //}

        //for (int i = LI_Nodes.GetUpperBound(1); i > 0; --i)
        //{
        //    if (LI_Nodes[LI_Nodes.GetLowerBound(0), i].Can_DN)
        //    {
        //        Edge new_edge = new Edge();
        //        new_edge.Start_Node = LI_Nodes[LI_Nodes.GetLowerBound(0), i];
        //        new_edge.End_Node = LI_Nodes[LI_Nodes.GetLowerBound(0), i - 1];
        //        LI_Edges.Add(new_edge);
        //    }
        //}
        #endregion
        #endregion
        /// --------------------------------- ///
    }

    //*! Render the graph by instantiating [GameObjects]
    [ContextMenu("Render_Graph")]
    private void Render_Graph()
    {
        #region Setup gizmos' spacing based on handle size
        //!* Gizmos Spacing for [Instantiate] method
        //float gizmos_spacing = handle_size * 0.5f;

        //!* Gizmos Spacing for [DrawMesh] method
        float gizmos_spacing = handle_size * 0.8f;
        #endregion

        #region Setup Gizmos for [Block] Nodes and indication arrows
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                //GameObject newNodeGiz = Instantiate(bl_node_giz, BL_Nodes[i, j].Position, Quaternion.identity, transform.GetChild(0));
                //newNodeGiz.transform.localScale *= handle_size;

                Matrix4x4 handleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position) *
                Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size) * 1.5f);

                Graphics.DrawMesh(node_giz, handleMatx, bl_giz_mat, 0);

                if (BL_Nodes[i, j].Can_UP)
                {
                    //Vector3 arrowPos = BL_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0);
                    //GameObject newNodeArrwGiz = Instantiate(bl_arrw_giz, arrowPos, Quaternion.identity, newNodeGiz.transform);

                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size));

                    Graphics.DrawMesh(arrw_giz, arrHandleMatx, bl_giz_mat, 0);
                }

                if (BL_Nodes[i, j].Can_DN)
                {
                    //Vector3 arrowPos = BL_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0);
                    //GameObject newNodeArrwGiz = Instantiate(bl_arrw_giz, arrowPos, Quaternion.AngleAxis(180f, Vector3.forward), newNodeGiz.transform);

                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(180f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz, arrHandleMatx, bl_giz_mat, 0);
                }

                if (BL_Nodes[i, j].Can_LFT)
                {
                    //Vector3 arrowPos = BL_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0);
                    //GameObject newNodeArrwGiz = Instantiate(bl_arrw_giz, arrowPos, Quaternion.AngleAxis(90f, Vector3.forward), newNodeGiz.transform);

                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(90f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz, arrHandleMatx, bl_giz_mat, 0);
                }

                if (BL_Nodes[i, j].Can_RGT)
                {
                    //Vector3 arrowPos = BL_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0);
                    //GameObject newNodeArrwGiz = Instantiate(bl_arrw_giz, arrowPos, Quaternion.AngleAxis(270f, Vector3.forward), newNodeGiz.transform);

                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0)) *
                                           Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size)) *
                                           Matrix4x4.Rotate(Quaternion.AngleAxis(270f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz, arrHandleMatx, bl_giz_mat, 0);
                }
            }
        }
        #endregion

        #region Setup Gizmos for [Line] Nodes and indication arrows
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {
                //GameObject newNodeGiz = Instantiate(li_node_giz, LI_Nodes[i, j].Position, Quaternion.identity, transform.GetChild(1));
                //newNodeGiz.transform.localScale *= handle_size;

                Matrix4x4 handleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position) *
                Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size) * 1.5f);

                Graphics.DrawMesh(node_giz, handleMatx, li_giz_mat, 0);

                if (LI_Nodes[i, j].Can_UP)
                {
                    //Vector3 arrowPos = LI_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0);
                    //GameObject newNodeArrwGiz = Instantiate(li_arrw_giz, arrowPos, Quaternion.identity, newNodeGiz.transform);

                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size));

                    Graphics.DrawMesh(arrw_giz, arrHandleMatx, li_giz_mat, 0);
                }

                if (LI_Nodes[i, j].Can_DN)
                {
                    //Vector3 arrowPos = LI_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0);
                    //GameObject newNodeArrwGiz = Instantiate(li_arrw_giz, arrowPos, Quaternion.AngleAxis(180f, Vector3.forward), newNodeGiz.transform);

                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(180f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz, arrHandleMatx, li_giz_mat, 0);
                }

                if (LI_Nodes[i, j].Can_LFT)
                {
                    //Vector3 arrowPos = LI_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0);
                    //GameObject newNodeArrwGiz = Instantiate(li_arrw_giz, arrowPos, Quaternion.AngleAxis(90f, Vector3.forward), newNodeGiz.transform);

                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(90f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz, arrHandleMatx, li_giz_mat, 0);
                }

                if (LI_Nodes[i, j].Can_RGT)
                {
                    //Vector3 arrowPos = LI_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0);
                    //GameObject newNodeArrwGiz = Instantiate(li_arrw_giz, arrowPos, Quaternion.AngleAxis(270f, Vector3.forward), newNodeGiz.transform);

                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0)) *
                                              Matrix4x4.Scale(new Vector3(handle_size, handle_size, handle_size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(270f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz, arrHandleMatx, li_giz_mat, 0);
                }
            }
        }
        #endregion
    }

    [ContextMenu("Render_Edges")]
    private void Render_Edges()
    {
        for (int i = 0; i < BL_Edges.Count; ++i)
        {
            
        }
    }

    //*! Destroys instantiated node prefabs
    [ContextMenu("Update_Graph")]
    private void Update_Graph()
    {
        bool is_BL_Node_Gizmos_List_Empty = (transform.GetChild(0).childCount < 1);
        bool is_LI_Node_Gizmos_List_Empty = (transform.GetChild(1).childCount < 1);

        if (!is_BL_Node_Gizmos_List_Empty)
        {
            for (int i = transform.GetChild(0).childCount; i > 0; --i)
            {
                DestroyImmediate(transform.GetChild(0).GetChild(0).gameObject, true); 
            }
            Debug.Log("Clear [Block] Node Gizmos");
        }

        if (!is_LI_Node_Gizmos_List_Empty)
        {
            for (int i = transform.GetChild(1).childCount; i > 0; --i)
            {
                DestroyImmediate(transform.GetChild(1).GetChild(0).gameObject, true);
            }
            Debug.Log("Clear [Line] Node Gizmos");
        }
    }

    //*! Only running this function when game runtime
    [ContextMenu("Runtime_Update")]
    void Runtime_Update()
    {
    }


    //*!----------------------------!*//
    //*!    Custom Subclasses
    //*!----------------------------!*//

    #region [Node] and [Edge] classes

    //*! Classes for map elements [Node] and [Edge] 
    public class Node
    {
        //*! Getter, Setter of Node members
        public Vector3 Position { get; set; }
        public float Node_Size { get; set; }

        //*! Getter, Setter of Node members
        public bool Can_UP { get { return UP_NODE != null; } }
        public bool Can_DN { get { return DN_NODE != null; } }
        public bool Can_LFT { get { return LFT_NODE != null; } }
        public bool Can_RGT { get { return RGT_NODE != null; } }

        //*! Neighbor Node reference holder
        public Node UP_NODE { get; set; }
        public Node DN_NODE { get; set; }
        public Node LFT_NODE { get; set; }
        public Node RGT_NODE { get; set; }
    }

    public class Edge
    {
        //*! Getter, Setter of Edge members
        public Node Start_Node { get; set; }
        public Node End_Node { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Edge_Type Type { get; set; }
    }

    #endregion

    #region [Edge_Type] Enum class
    public enum Edge_Type
    {
        NONE = 0,
        Pencil,
        HighLighter_Blue,
        HighLighter_Red,
        Colour_Pencil
    }
    #endregion
}
