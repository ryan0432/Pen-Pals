//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for operating shape editor in edit mode.
//*!              This class in an experimental class to test using
//*!              editor with MonoBehavior.
//*!
//*! Last edit  : 28/08/2018
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

    public int init_row;
    public int init_col;

    [Range(2, 50)]
    public int row;
    [Range(2, 50)]
    public int col;


    [Range(0.1f, 1.0f)]
    public float handle_size;
    public Node[,] BL_Nodes { get; set; }
    public Node[,] LI_Nodes { get { return li_nodes; } set { li_nodes = value; } }
    public List<Edge> BL_Edges { get; set; }
    public List<Edge> LI_Edges { get { return li_edges; } set { li_edges = value; } }

    #endregion
    #region Private Vars

    [HideInInspector]
    private Node[,] li_nodes;
    [HideInInspector]
    private List<Edge> li_edges;

    [SerializeField]
    private GameObject bl_node_giz;
    [SerializeField]
    private GameObject bl_arrw_giz;
    [SerializeField]
    private GameObject li_node_giz;
    [SerializeField]
    private GameObject li_arrw_giz;

    [SerializeField]
    private Mesh node_giz;
    [SerializeField]
    private Mesh arrw_giz;
    [SerializeField]
    private Material bl_giz_mat;
    [SerializeField]
    private Material li_giz_mat;

    #endregion

#if UNITY_EDITOR
    //*! This [Update] only runs in edit mode
    [ContextMenu("Update")]
    private void Update()
    {
        #region Check if current state is in [Playing Mode] or [Edit Mode]
        //*! Call [Runtime Update] if editor is in test [Playing Mode]
        if (UnityEditor.EditorApplication.isPlaying) { RuntimeUpdate(); }

        //*! If current event is changing from [Playing Mode] to [Edit Mode] then return
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) { return; }
        #endregion

        /// Write your tools below. Only excuted in [Edit Mode] runtime ///

        Update_Graph();
        Layout_Graph();
        Render_Graph();
    }

#else

    #region Native Unity Update(), calls RuntimeUpdate()
	//*! Update is called once per frame
	private void Update ()
    {
        //*! Ingame runtime update
		RuntimeUpdate();
	}
    #endregion

#endif


    //*!----------------------------!*//
    //*!    Private Functions
    //*!----------------------------!*//

    //*! Setup [Block] and [Line] nodes by the number of row and col
    private void Layout_Graph()
    {
        //*! Populate the graph with [Node] default construction
        #region Setup instances for [Block] and [Line] Nodes and Edges
        BL_Nodes = new Node[row - 1, col - 1];
        BL_Edges = new List<Edge>();
        li_nodes = new Node[row, col];
        li_edges = new List<Edge>();
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
        for (int i = 0; i < li_nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < li_nodes.GetLength(1); ++j)
            {
                Node new_node = new Node();
                li_nodes[i, j] = new_node;
                li_nodes[i, j].Position = new Vector3(i, j);
                li_nodes[i, j].Node_Size = handle_size;
                li_nodes[i, j].UP_NODE = null;
                li_nodes[i, j].DN_NODE = null;
                li_nodes[i, j].LFT_NODE = null;
                li_nodes[i, j].RGT_NODE = null;
            }
        }
        #endregion

        #region Setup [Line] Nodes and Neighbor Nodes' linkage in UP & RGT Direction
        for (int i = 0; i < li_nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < li_nodes.GetUpperBound(1); ++j)
            {
                li_nodes[i, j].UP_NODE = li_nodes[i, j + 1];
                li_nodes[i, j].RGT_NODE = li_nodes[i + 1, j];
            }
        }

        for (int i = 0; i < li_nodes.GetUpperBound(0); ++i)
        {
            li_nodes[i, li_nodes.GetUpperBound(1)].RGT_NODE = li_nodes[i + 1, li_nodes.GetUpperBound(1)];
        }

        for (int i = 0; i < li_nodes.GetUpperBound(1); ++i)
        {
            li_nodes[li_nodes.GetUpperBound(0), i].UP_NODE = li_nodes[li_nodes.GetUpperBound(0), i + 1];
        }
        #endregion

        #region Setup [Line] Nodes and Neighbor Nodes' linkage in DN & LFT Direction
        for (int i = li_nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = li_nodes.GetUpperBound(1); j > 0; --j)
            {
                li_nodes[i, j].DN_NODE = li_nodes[i, j - 1];
                li_nodes[i, j].LFT_NODE = li_nodes[i - 1, j];
            }
        }

        for (int i = li_nodes.GetUpperBound(0); i > 0; --i)
        {
            li_nodes[i, li_nodes.GetLowerBound(1)].LFT_NODE = li_nodes[i - 1, li_nodes.GetLowerBound(1)];
        }

        for (int i = li_nodes.GetUpperBound(1); i > 0; --i)
        {
            li_nodes[li_nodes.GetLowerBound(0), i].DN_NODE = li_nodes[li_nodes.GetLowerBound(0), i - 1];
        }
        #endregion

        //*! Iterate from the buttom left node of [Block] 2D array to check [UP] & [RGT] direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Block] Edges in UP & RGT Direction
        for (int i = 0; i < BL_Nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetUpperBound(1); ++j)
            {
                if (BL_Nodes[i, j].Can_UP)
                {
                    Edge new_edge = new Edge();
                    new_edge.Start_Node = BL_Nodes[i, j];
                    new_edge.End_Node = BL_Nodes[i, j + 1];
                    BL_Edges.Add(new_edge);
                }

                if (BL_Nodes[i, j].Can_RGT)
                {
                    Edge new_edge = new Edge();
                    new_edge.Start_Node = BL_Nodes[i, j];
                    new_edge.End_Node = BL_Nodes[i + 1, j];
                    BL_Edges.Add(new_edge);
                }
            }
        }

        for (int i = 0; i < BL_Nodes.GetUpperBound(0); ++i)
        {
            if (BL_Nodes[i, BL_Nodes.GetUpperBound(1)].Can_RGT)
            {
                Edge new_edge = new Edge();
                new_edge.Start_Node = BL_Nodes[i, BL_Nodes.GetUpperBound(1)];
                new_edge.End_Node = BL_Nodes[i + 1, BL_Nodes.GetUpperBound(1)];
                BL_Edges.Add(new_edge);
            }
        }

        for (int i = 0; i < BL_Nodes.GetUpperBound(1); ++i)
        {
            if (BL_Nodes[BL_Nodes.GetUpperBound(0), i].Can_UP)
            {
                Edge new_edge = new Edge();
                new_edge.Start_Node = BL_Nodes[BL_Nodes.GetUpperBound(0), i];
                new_edge.End_Node = BL_Nodes[BL_Nodes.GetUpperBound(0), i + 1];
                BL_Edges.Add(new_edge);
            }
        }
        #endregion

        //*! Iterate from the upper right node of [Block] 2D array to check [DN] & [LFT] direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Block] Edges in DN & LFT Direction
        for (int i = BL_Nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = BL_Nodes.GetUpperBound(1); j > 0; --j)
            {
                if (BL_Nodes[i, j].Can_DN)
                {
                    Edge new_edge = new Edge();
                    new_edge.Start_Node = BL_Nodes[i, j];
                    new_edge.End_Node = BL_Nodes[i, j - 1];
                    BL_Edges.Add(new_edge);
                }

                if (BL_Nodes[i, j].Can_LFT)
                {
                    Edge new_edge = new Edge();
                    new_edge.Start_Node = BL_Nodes[i, j];
                    new_edge.End_Node = BL_Nodes[i - 1, j];
                    BL_Edges.Add(new_edge);
                }
            }
        }

        for (int i = BL_Nodes.GetUpperBound(0); i > 0; --i)
        {
            if (BL_Nodes[i, BL_Nodes.GetLowerBound(1)].Can_LFT)
            {
                Edge new_edge = new Edge();
                new_edge.Start_Node = BL_Nodes[i, BL_Nodes.GetLowerBound(1)];
                new_edge.End_Node = BL_Nodes[i - 1, BL_Nodes.GetLowerBound(1)];
                BL_Edges.Add(new_edge);
            }
        }

        for (int i = BL_Nodes.GetUpperBound(1); i > 0; --i)
        {
            if (BL_Nodes[BL_Nodes.GetLowerBound(0), i].Can_DN)
            {
                Edge new_edge = new Edge();
                new_edge.Start_Node = BL_Nodes[BL_Nodes.GetLowerBound(0), i];
                new_edge.End_Node = BL_Nodes[BL_Nodes.GetLowerBound(0), i - 1];
                BL_Edges.Add(new_edge);
            }
        }
        #endregion

        //*! Iterate from the upper right node of [Line] 2D array to check [UP] & [RGT] direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Line] Edges in UP & Right Direction
        for (int i = 0; i < li_nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < li_nodes.GetUpperBound(1); ++j)
            {
                if (li_nodes[i, j].Can_UP)
                {
                    Edge new_edge = new Edge();
                    new_edge.Start_Node = li_nodes[i, j];
                    new_edge.End_Node = li_nodes[i, j + 1];
                    li_edges.Add(new_edge);
                }

                if (li_nodes[i, j].Can_RGT)
                {
                    Edge new_edge = new Edge();
                    new_edge.Start_Node = li_nodes[i, j];
                    new_edge.End_Node = li_nodes[i + 1, j];
                    li_edges.Add(new_edge);
                }
            }
        }

        for (int i = 0; i < li_nodes.GetUpperBound(0); ++i)
        {
            if (li_nodes[i, li_nodes.GetUpperBound(1)].Can_RGT)
            {
                Edge new_edge = new Edge();
                new_edge.Start_Node = li_nodes[i, li_nodes.GetUpperBound(1)];
                new_edge.End_Node = li_nodes[i + 1, li_nodes.GetUpperBound(1)];
                li_edges.Add(new_edge);
            }
        }

        for (int i = 0; i < li_nodes.GetUpperBound(1); ++i)
        {
            if (li_nodes[li_nodes.GetUpperBound(0), i].Can_UP)
            {
                Edge new_edge = new Edge();
                new_edge.Start_Node = li_nodes[li_nodes.GetUpperBound(0), i];
                new_edge.End_Node = li_nodes[li_nodes.GetUpperBound(0), i + 1];
                li_edges.Add(new_edge);
            }
        }
        #endregion

        //*! Iterate from the upper right node of [Line] 2D array to check [DN] & [LFT] direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Line] Edges in DN & LFT Direction
        for (int i = li_nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = li_nodes.GetUpperBound(1); j > 0; --j)
            {
                if (li_nodes[i, j].Can_DN)
                {
                    Edge new_edge = new Edge();
                    new_edge.Start_Node = li_nodes[i, j];
                    new_edge.End_Node = li_nodes[i, j - 1];
                    li_edges.Add(new_edge);
                }

                if (li_nodes[i, j].Can_LFT)
                {
                    Edge new_edge = new Edge();
                    new_edge.Start_Node = li_nodes[i, j];
                    new_edge.End_Node = li_nodes[i - 1, j];
                    li_edges.Add(new_edge);
                }
            }
        }

        for (int i = li_nodes.GetUpperBound(0); i > 0; --i)
        {
            if (li_nodes[i, li_nodes.GetLowerBound(1)].Can_LFT)
            {
                Edge new_edge = new Edge();
                new_edge.Start_Node = li_nodes[i, li_nodes.GetLowerBound(1)];
                new_edge.End_Node = li_nodes[i - 1, li_nodes.GetLowerBound(1)];
                li_edges.Add(new_edge);
            }
        }

        for (int i = li_nodes.GetUpperBound(1); i > 0; --i)
        {
            if (li_nodes[li_nodes.GetLowerBound(0), i].Can_DN)
            {
                Edge new_edge = new Edge();
                new_edge.Start_Node = li_nodes[li_nodes.GetLowerBound(0), i];
                new_edge.End_Node = li_nodes[li_nodes.GetLowerBound(0), i - 1];
                li_edges.Add(new_edge);
            }
        }
        #endregion
    }

    //*! Render the graph by instantiating [GameObjects]
    private void Render_Graph()
    {
        float gizmos_spacing = handle_size * 0.5f;

        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                GameObject newNodeGiz = Instantiate(bl_node_giz, BL_Nodes[i, j].Position, Quaternion.identity, transform.GetChild(0));
                newNodeGiz.transform.localScale *= handle_size;

                if (BL_Nodes[i, j].Can_UP)
                {
                    Vector3 arrowPos = BL_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0);
                    GameObject newNodeArrwGiz = Instantiate(bl_arrw_giz, arrowPos, Quaternion.identity, newNodeGiz.transform);
                }

                if (BL_Nodes[i, j].Can_DN)
                {
                    Vector3 arrowPos = BL_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0);
                    GameObject newNodeArrwGiz = Instantiate(bl_arrw_giz, arrowPos, Quaternion.AngleAxis(180f, Vector3.forward), newNodeGiz.transform);
                }

                if (BL_Nodes[i, j].Can_LFT)
                {
                    Vector3 arrowPos = BL_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0);
                    GameObject newNodeArrwGiz = Instantiate(bl_arrw_giz, arrowPos, Quaternion.AngleAxis(90f, Vector3.forward), newNodeGiz.transform);
                }

                if (BL_Nodes[i, j].Can_RGT)
                {
                    Vector3 arrowPos = BL_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0);
                    GameObject newNodeArrwGiz = Instantiate(bl_arrw_giz, arrowPos, Quaternion.AngleAxis(270f, Vector3.forward), newNodeGiz.transform);
                }
            }
        }

        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {
                GameObject newNodeGiz = Instantiate(li_node_giz, LI_Nodes[i, j].Position, Quaternion.identity, transform.GetChild(1));
                newNodeGiz.transform.localScale *= handle_size;

                if (LI_Nodes[i, j].Can_UP)
                {
                    Vector3 arrowPos = LI_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0);
                    GameObject newNodeArrwGiz = Instantiate(li_arrw_giz, arrowPos, Quaternion.identity, newNodeGiz.transform);
                }

                if (LI_Nodes[i, j].Can_DN)
                {
                    Vector3 arrowPos = LI_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0);
                    GameObject newNodeArrwGiz = Instantiate(li_arrw_giz, arrowPos, Quaternion.AngleAxis(180f, Vector3.forward), newNodeGiz.transform);
                }

                if (LI_Nodes[i, j].Can_LFT)
                {
                    Vector3 arrowPos = LI_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0);
                    GameObject newNodeArrwGiz = Instantiate(li_arrw_giz, arrowPos, Quaternion.AngleAxis(90f, Vector3.forward), newNodeGiz.transform);
                }

                if (LI_Nodes[i, j].Can_RGT)
                {
                    Vector3 arrowPos = LI_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0);
                    GameObject newNodeArrwGiz = Instantiate(li_arrw_giz, arrowPos, Quaternion.AngleAxis(270f, Vector3.forward), newNodeGiz.transform);
                }
            }
        }
    }

    //*! Destroys instantiated node prefabs
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


    #region Custom RuntimeUpdate()
    //*! Only running this block when game runtime
    [ContextMenu("RuntimeUpdate")]
    void RuntimeUpdate()
    {

    }
    #endregion

    //*!----------------------------!*//
    //*!    Custom Subclasses
    //*!----------------------------!*//

    #region [Node] and [Edge] class

    //*! Classes for map elements [Node] and [Edge] 
    [System.Serializable]
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

    [System.Serializable]
    public class Edge
    {
        //*! Getter, Setter of Edge members
        public Node Start_Node { get; set; }
        public Node End_Node { get; set; }
        public Vector3 Edge_Normal { get; set; }
    }

    #endregion

}
