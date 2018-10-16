//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for operating [Pencil Case] in edit mode.
//*!
//*! Last edit  : 15/10/2018
//*!--------------------------------------------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public class Pencil_Case : MonoBehaviour
{
    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//

    #region Public Vars
    public bool startEditing = false;
    public bool showBlockNode = true;
    public bool showBlockEdge = true;
    public bool showLineNode = true;
    public bool showLineEdge = true;

    [Range(2, 50)]
    [HideInInspector]
    public int initialRow;
    [Range(2, 50)]
    [HideInInspector]
    public int initialCol;

    [Range(2, 50)]
    [HideInInspector]
    [SerializeField]
    public int row;

    [Range(2, 50)]
    [HideInInspector]
    [SerializeField]
    public int col;

    [Range(0.5f, 1.5f)]
    public float handle_size = 1.0f;

    public Node[,] BL_Nodes;
    public Node[,] LI_Nodes;

    public Edge[,] BL_U_Edges;
    public Edge[,] BL_V_Edges;

    public Edge[,] LI_U_Edges;
    public Edge[,] LI_V_Edges;

    [SerializeField]
    public Lv_Data lv_Data;
    #endregion

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//

    #region Private Vars
    //*! Gizmos prefabs for [Instantiate] mode
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
    private GameObject li_edge_giz;
    [SerializeField]
    [HideInInspector]
    private GameObject bl_edge_giz;

    //*! Gizmos meshes and materials for [DrawMesh] mode
    [SerializeField]
    [HideInInspector]
    private Mesh node_giz_mesh;
    [SerializeField]
    [HideInInspector]
    private Mesh edge_giz_mesh;
    [SerializeField]
    [HideInInspector]
    private Mesh arrw_giz_mesh;

    //*! Gizmos meshes for [Handles] in [Edit Mode]
    [SerializeField]
    [HideInInspector]
    private Mesh sticker_giz_mesh;

    [SerializeField]
    [HideInInspector]
    private Mesh obstacle_giz_mesh;

    //*! Gizmos materials for [Handles] in [Edit Mode]
    [SerializeField]
    [HideInInspector]
    private Material bl_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material li_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material boarder_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material block_blue_sticker_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material block_red_sticker_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material line_blue_sticker_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material line_red_sticker_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material block_blue_start_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material block_red_start_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material line_blue_head_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material line_blue_edge_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material line_blue_segment_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material line_red_head_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material line_red_edge_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material line_red_segment_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material black_pen_giz_mat;

    [SerializeField]
    [HideInInspector]
    private Material highlighter_red_giz_mat;

    //*! Save/Load Level Data based on booleans
    [HideInInspector]
    [SerializeField]
    private bool isSaved;
    [HideInInspector]
    [SerializeField]
    private bool isLoaded;

    //*! Main camera instance
    [HideInInspector]
    [SerializeField]
    private Camera cam;

    //*! Move Graph according to Enum
    [HideInInspector]
    [SerializeField]
    private Move_Graph move_Graph_Direction;
    #endregion


#if UNITY_EDITOR
    [ContextMenu("Editor_Awake")]
    public void Awake()
    {
        #region Hide [Gizmos], [Handle] and [Symbol] parent objects in Hierarchy
        transform.Find("BL_Node_Gizmos").hideFlags = HideFlags.HideInHierarchy;
        transform.Find("LI_Node_Gizmos").hideFlags = HideFlags.HideInHierarchy;
        transform.Find("BL_Edges_Handles").hideFlags = HideFlags.HideInHierarchy;
        transform.Find("LI_Edges_Handles").hideFlags = HideFlags.HideInHierarchy;
        //transform.Find("Players").hideFlags = HideFlags.HideInHierarchy;
        //transform.Find("Symbols").hideFlags = HideFlags.HideInHierarchy;
        //transform.Find("Backgrounds").hideFlags = HideFlags.HideInHierarchy;
        #endregion

        #region Get [Camera] reference
        Get_Camera();
        #endregion

        if (startEditing)
        {
            Get_Camera();
            Clear_Graph_Init_Mode();
            Layout_Graph_Init_Mode();
            Render_Node_Gizmos_Init_Mode();
            Render_Edges_Handles_Init_Mode();
            isLoaded = true;
            Load_Level_Data();
        }
    }

    //*! This [Update] only runs in edit mode
    [ContextMenu("Editor_Update")]
    public void Update()
    {
        #region Check if current state is in [Playing Mode] or [Edit Mode]
        //*! Call [Runtime Update] if editor is in test [Playing Mode]
        if (UnityEditor.EditorApplication.isPlaying) { Runtime_Update(); }

        //*! If current event is changing to [Playing Mode] to [Edit Mode] then return
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) { return; }
        #endregion

        /// Write your tools below. Only excuted in [Edit Mode] runtime ///

        //*! Get Camera reference
        Get_Camera();

        if (!startEditing)
        {
            //*! Destroys instantiated [Node] [Edge] - [Gizmos] [Handle] in [Initialization Mode]
            Clear_Graph_Init_Mode();

            //*! Initialize Graph. Setup [Block] and [Line] nodes [Data] by the number of row and col
            Layout_Graph_Init_Mode();

            //*! Render [Node] [Gizmos] by instantiating [GameObject]. Render arrows by [DrawMesh]
            Render_Node_Gizmos_Init_Mode();

            //*! Render [Edge] [Handle] by instantiating [GameObject]
            Render_Edges_Handles_Init_Mode();
        }
        else
        {
            //*! Load Level data from file
            Load_Level_Data();

            //*! Move Graph to designated direction
            Move_Graph_Edit_Mode(move_Graph_Direction);

            //*! Excute Refresh Graph function collection
            Refresh_Graph_Edit_Mode();

            //*! Save Level data to files
            Save_Level_Data();
        }
    }

    //*! [OnValidate] only works in [Editor Mode] changing related varibles in realtime
    [ContextMenu("OnValidate")]
    public void OnValidate()
    {
        //*! Limit the max number of [Row] and [Col] bound to [initialRow] and [initialCol]
        if (!startEditing)
        {
            row = initialRow;

            col = initialCol;
        }
        else
        {
            if (row > initialRow)
            {
                row = initialRow;
            }

            if (col > initialCol)
            {
                col = initialCol;
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
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Destroys instantiated [Node] [Edge] - [Gizmos] [Handle] in [Initialization Mode]
    [ContextMenu("Clear_Graph_Init_Mode")]
    private void Clear_Graph_Init_Mode()
    {
        //*! Check if parent GameObjects of [Gizmos] and [Handles] are empty
        #region Check if parent objects of [Gizmos] and [Handles] are empty
        bool is_BL_Node_Gizmos_List_Empty = (transform.Find("BL_Node_Gizmos").childCount < 1);
        bool is_LI_Node_Gizmos_List_Empty = (transform.Find("LI_Node_Gizmos").childCount < 1);
        bool is_BL_Edges_List_Empty = (transform.Find("BL_Edges_Handles").childCount < 1);
        bool is_LI_Edges_List_Empty = (transform.Find("LI_Edges_Handles").childCount < 1);
        #endregion

        //*! Destroy all remaining child [Gizmos] and [Handles] GameObjects immediately
        #region Destroy remaining child [Gizmos] and [Handles] GameObjects immediately
        if (!is_BL_Node_Gizmos_List_Empty)
        {
            for (int i = transform.Find("BL_Node_Gizmos").childCount; i > 0; --i)
            {
                DestroyImmediate(transform.Find("BL_Node_Gizmos").GetChild(0).gameObject, true);
            }
        }

        if (!is_LI_Node_Gizmos_List_Empty)
        {
            for (int i = transform.Find("LI_Node_Gizmos").childCount; i > 0; --i)
            {
                DestroyImmediate(transform.Find("LI_Node_Gizmos").GetChild(0).gameObject, true);
            }
        }

        if (!is_BL_Edges_List_Empty)
        {
            for (int i = transform.Find("BL_Edges_Handles").childCount; i > 0; --i)
            {
                DestroyImmediate(transform.Find("BL_Edges_Handles").GetChild(0).gameObject, true);
            }
        }

        if (!is_LI_Edges_List_Empty)
        {
            for (int i = transform.Find("LI_Edges_Handles").childCount; i > 0; --i)
            {
                DestroyImmediate(transform.Find("LI_Edges_Handles").GetChild(0).gameObject, true);
            }
        }
        #endregion
    }

    //*! Initialize Graph. Setup [Block] and [Line] nodes [Data] by the number of row and col
    [ContextMenu("Layout_Graph_Init_Mode")]
    private void Layout_Graph_Init_Mode()
    {
        //*! Setup instances and references for the graph data with [Node] & [Edge] default construction
        #region Setup instances for [Block] and [Line] Nodes and Edges
        BL_Nodes = new Node[initialRow - 1, initialCol - 1];
        LI_Nodes = new Node[initialRow, initialCol];

        BL_U_Edges = new Edge[initialRow, initialCol - 1];
        BL_V_Edges = new Edge[initialRow - 1, initialCol];
        LI_U_Edges = new Edge[initialRow - 1, initialCol];
        LI_V_Edges = new Edge[initialRow, initialCol - 1];
        #endregion

        //*! Setup [Block] - [Node] data
        #region Setup [Block] Nodes and Preset Neighbor Nodes to Null
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                Node new_node = new Node();
                BL_Nodes[i, j] = new_node;
                BL_Nodes[i, j].Position = new Vector3(i + 0.5f, j + 0.5f);
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

        //*! Setup [Line] - [Node] data
        #region Setup [Line] Nodes and Preset Neighbor Nodes to Null
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {
                Node new_node = new Node();
                LI_Nodes[i, j] = new_node;
                LI_Nodes[i, j].Position = new Vector3(i, j);
                LI_Nodes[i, j].UP_NODE = null;
                LI_Nodes[i, j].DN_NODE = null;
                LI_Nodes[i, j].LFT_NODE = null;
                LI_Nodes[i, j].RGT_NODE = null;

                LI_Nodes[i, j].UP_EDGE = null;
                LI_Nodes[i, j].DN_EDGE = null;
                LI_Nodes[i, j].LFT_EDGE = null;
                LI_Nodes[i, j].RGT_EDGE = null;
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
                Edge new_edge_U = new Edge();
                new_edge_U.Edge_Type = Edge_Type.NONE;
                new_edge_U.Boarder_Type = Boarder_Type.NONE;
                new_edge_U.Edge_Direction = Edge_Direction.Horizontal;
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

                new_edge_U.Position = new_edge_U.LFT_Node.Position + ((new_edge_U.RGT_Node.Position - new_edge_U.LFT_Node.Position) / 2);
                LI_U_Edges[i, j] = new_edge_U;
            }
        }
        #endregion

        #region Setup [Line] - [V] direction [Edge] for [Block]
        for (int i = 0; i < LI_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_V_Edges.GetLength(1); ++j)
            {
                Edge new_edge_V = new Edge();
                new_edge_V.Edge_Type = Edge_Type.NONE;
                new_edge_V.Boarder_Type = Boarder_Type.NONE;
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

                new_edge_V.Position = new_edge_V.DN_Node.Position + ((new_edge_V.UP_Node.Position - new_edge_V.DN_Node.Position) / 2);
                new_edge_V.Edge_Direction = Edge_Direction.Vertical;
                LI_V_Edges[i, j] = new_edge_V;
            }
        }
        #endregion

        //*! Setup [Block] - [Edge] data for [Line]
        #region Setup [Block] - [U] direction [Edge] for [Line]
        for (int i = 0; i < BL_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_U_Edges.GetLength(1); ++j)
            {
                Edge new_edge_U = new Edge();
                new_edge_U.UP_Node = LI_V_Edges[i, j].UP_Node;
                new_edge_U.DN_Node = LI_V_Edges[i, j].DN_Node;
                new_edge_U.LFT_Node = LI_V_Edges[i, j].LFT_Node;
                new_edge_U.RGT_Node = LI_V_Edges[i, j].RGT_Node;
                new_edge_U.Position = LI_V_Edges[i, j].Position;
                new_edge_U.Edge_Type = Edge_Type.NONE;
                new_edge_U.Boarder_Type = Boarder_Type.NONE;
                new_edge_U.Edge_Direction = Edge_Direction.Horizontal;
                BL_U_Edges[i, j] = new_edge_U;
            }
        }
        #endregion

        #region Setup [Block] - [V] direction [Edge] for [Line]
        for (int i = 0; i < BL_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_V_Edges.GetLength(1); ++j)
            {
                Edge new_edge_V = new Edge();
                new_edge_V.UP_Node = LI_U_Edges[i, j].UP_Node;
                new_edge_V.DN_Node = LI_U_Edges[i, j].DN_Node;
                new_edge_V.LFT_Node = LI_U_Edges[i, j].LFT_Node;
                new_edge_V.RGT_Node = LI_U_Edges[i, j].RGT_Node;
                new_edge_V.Position = LI_U_Edges[i, j].Position;
                new_edge_V.Edge_Type = Edge_Type.NONE;
                new_edge_V.Boarder_Type = Boarder_Type.NONE;
                new_edge_V.Edge_Direction = Edge_Direction.Vertical;
                BL_V_Edges[i, j] = new_edge_V;
            }
        }
        #endregion

        //*! Setup [Line] - [Node]'s neighbor [Edge]
        #region Setup [Line] - [Node]'s neighbor [Edge] - Non-Boarder
        for (int i = 1; i < LI_Nodes.GetLength(0) - 1; ++i)
        {
            for (int j = 1; j < LI_Nodes.GetLength(1) - 1; ++j)
            {
                LI_Nodes[i, j].UP_EDGE = LI_V_Edges[i,j];
                LI_Nodes[i, j].DN_EDGE = LI_V_Edges[i, j - 1];
                LI_Nodes[i, j].LFT_EDGE = LI_U_Edges[i - 1, j];
                LI_Nodes[i, j].RGT_EDGE = LI_U_Edges[i, j];
            }
        }
        #endregion

        #region Setup [Line] - [Node]'s neighbor [Edge] - UP-Boarder
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            int maxY = LI_Nodes.GetUpperBound(1);

            if (i == 0)
            {
                LI_Nodes[i, maxY].UP_EDGE = null;
                LI_Nodes[i, maxY].DN_EDGE = LI_V_Edges[i, maxY - 1];
                LI_Nodes[i, maxY].LFT_EDGE = null;
                LI_Nodes[i, maxY].RGT_EDGE = LI_U_Edges[i, maxY];
            }
            else if (i == LI_Nodes.GetLength(0) - 1)
            {
                LI_Nodes[i, maxY].UP_EDGE = null;
                LI_Nodes[i, maxY].DN_EDGE = LI_V_Edges[i, maxY - 1];
                LI_Nodes[i, maxY].LFT_EDGE = LI_U_Edges[i - 1, maxY];
                LI_Nodes[i, maxY].RGT_EDGE = null;
            }
            else
            {
                LI_Nodes[i, maxY].UP_EDGE = null;
                LI_Nodes[i, maxY].DN_EDGE = LI_V_Edges[i, maxY - 1];
                LI_Nodes[i, maxY].LFT_EDGE = LI_U_Edges[i - 1, maxY];
                LI_Nodes[i, maxY].RGT_EDGE = LI_U_Edges[i, maxY];
            }
        }
        #endregion

        #region Setup [Line] - [Node]'s neighbor [Edge] - DN-Boarder
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            if (i == 0)
            {
                LI_Nodes[i, 0].UP_EDGE = LI_V_Edges[i, 0];
                LI_Nodes[i, 0].DN_EDGE = null;
                LI_Nodes[i, 0].LFT_EDGE = null;
                LI_Nodes[i, 0].RGT_EDGE = LI_U_Edges[i, 0];
            }
            else if (i == LI_Nodes.GetLength(0) - 1)
            {
                LI_Nodes[i, 0].UP_EDGE = LI_V_Edges[i, 0];
                LI_Nodes[i, 0].DN_EDGE = null;
                LI_Nodes[i, 0].LFT_EDGE = LI_U_Edges[i - 1, 0];
                LI_Nodes[i, 0].RGT_EDGE = null;
            }
            else
            {
                LI_Nodes[i, 0].UP_EDGE = LI_V_Edges[i, 0];
                LI_Nodes[i, 0].DN_EDGE = null;
                LI_Nodes[i, 0].LFT_EDGE = LI_U_Edges[i - 1, 0];
                LI_Nodes[i, 0].RGT_EDGE = LI_U_Edges[i, 0];
            }
        }
        #endregion

        #region Setup [Line] - [Node]'s neighbor [Edge] - LFT-Boarder
        for (int i = 0; i < LI_Nodes.GetLength(1); ++i)
        {
            if (i == 0)
            {
                LI_Nodes[0, i].UP_EDGE = LI_V_Edges[0, i];
                LI_Nodes[0, i].DN_EDGE = null;
                LI_Nodes[0, i].LFT_EDGE = null;
                LI_Nodes[0, i].RGT_EDGE = LI_U_Edges[0, i];
            }
            else if (i == LI_Nodes.GetLength(1) - 1)
            {
                LI_Nodes[0, i].UP_EDGE = null;
                LI_Nodes[0, i].DN_EDGE = LI_V_Edges[0, i - 1];
                LI_Nodes[0, i].LFT_EDGE = null;
                LI_Nodes[0, i].RGT_EDGE = LI_U_Edges[0, i];
            }
            else
            {
                LI_Nodes[0, i].UP_EDGE = LI_V_Edges[0, i];
                LI_Nodes[0, i].DN_EDGE = LI_V_Edges[0, i - 1];
                LI_Nodes[0, i].LFT_EDGE = null;
                LI_Nodes[0, i].RGT_EDGE = LI_U_Edges[0, i];
            }
        }
        #endregion

        #region Setup [Line] - [Node]'s neighbor [Edge] - RGT-Boarder
        for (int i = 0; i < LI_Nodes.GetLength(1); ++i)
        {
            int maxX = LI_Nodes.GetUpperBound(0);

            if (i == 0)
            {
                LI_Nodes[maxX, i].UP_EDGE = LI_V_Edges[maxX, i];
                LI_Nodes[maxX, i].DN_EDGE = null;
                LI_Nodes[maxX, i].LFT_EDGE = LI_U_Edges[maxX - 1, i];
                LI_Nodes[maxX, i].RGT_EDGE = null;
            }
            else if (i == LI_Nodes.GetLength(1) - 1)
            {
                LI_Nodes[maxX, i].UP_EDGE = null;
                LI_Nodes[maxX, i].DN_EDGE = LI_V_Edges[maxX, i - 1];
                LI_Nodes[maxX, i].LFT_EDGE = LI_U_Edges[maxX - 1, i];
                LI_Nodes[maxX, i].RGT_EDGE = null;
            }
            else
            {
                LI_Nodes[maxX, i].UP_EDGE = LI_V_Edges[maxX, i];
                LI_Nodes[maxX, i].DN_EDGE = LI_V_Edges[maxX, i - 1];
                LI_Nodes[maxX, i].LFT_EDGE = LI_U_Edges[maxX - 1, i];
                LI_Nodes[maxX, i].RGT_EDGE = null;
            }
        }
        #endregion
    }

    //*! Render [Node] [Gizmos] by instantiating [GameObject]. Render arrows by [DrawMesh]
    [ContextMenu("Render_Node_Gizmos_Init_Mode")]
    private void Render_Node_Gizmos_Init_Mode()
    {
        #region Setup gizmos' spacing based on handle size
        //!* Gizmos Spacing for [DrawMesh] method
        float gizmos_spacing = handle_size * 0.125f;
        #endregion

        #region Setup Gizmos for [Block] Nodes and indication arrows
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                GameObject newNodeGiz = Instantiate(bl_node_giz, BL_Nodes[i, j].Position, Quaternion.identity, transform.Find("BL_Node_Gizmos"));
                newNodeGiz.transform.localScale = new Vector3 (handle_size, handle_size, handle_size);
                BL_Nodes[i, j].Gizmos_GO = newNodeGiz;

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

                if (!showBlockNode)
                {
                    newNodeGiz.GetComponentInChildren<MeshRenderer>().enabled = false;
                }
            }
        }
        #endregion

        #region Setup Gizmos for [Line] Nodes and indication arrows
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {
                GameObject newNodeGiz = Instantiate(li_node_giz, LI_Nodes[i, j].Position, Quaternion.identity, transform.Find("LI_Node_Gizmos"));
                newNodeGiz.transform.localScale = new Vector3(handle_size, handle_size, handle_size);
                LI_Nodes[i, j].Gizmos_GO = newNodeGiz;

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

                if (!showLineNode)
                {
                    newNodeGiz.GetComponentInChildren<MeshRenderer>().enabled = false;
                }
            }
        }
        #endregion
    }

    //*! Render [Edge] [Handle] by instantiating [GameObject]
    [ContextMenu("Render_Edges_Handles_Init_Mode")]
    private void Render_Edges_Handles_Init_Mode()
    {
        //*! Render [Line] [Edge] handles for [Block]
        #region Instantiate [Line] - [U_Edge] handles for [Block]
        for (int i = 0; i < LI_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_U_Edges.GetLength(1); ++j)
            {
                Vector3 pos = LI_U_Edges[i, j].Position;
                Quaternion rot = LI_U_Edges[i, j].Rotation;
                GameObject go = Instantiate(li_edge_giz, pos, rot, transform.Find("LI_Edges_Handles"));
                LI_U_Edges[i, j].Gizmos_GO = go;

                if (!showLineEdge)
                {
                    go.GetComponentInChildren<MeshRenderer>().enabled = false;
                }
                else
                {
                    go.GetComponentInChildren<MeshRenderer>().enabled = true;
                }
            }
        }
        #endregion

        #region Instantiate [Line] - [V_Edge] handles for [Block]
        for (int i = 0; i < LI_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_V_Edges.GetLength(1); ++j)
            {
                Vector3 pos = LI_V_Edges[i, j].Position;
                Quaternion rot = LI_V_Edges[i, j].Rotation;
                GameObject go = Instantiate(li_edge_giz, pos, rot, transform.Find("LI_Edges_Handles"));
                LI_V_Edges[i, j].Gizmos_GO = go;

                if (!showLineEdge)
                {
                    go.GetComponentInChildren<MeshRenderer>().enabled = false;
                }
                else
                {
                    go.GetComponentInChildren<MeshRenderer>().enabled = true;
                }
            }
        }
        #endregion

        //*! Render [Block] [Edge] handles for [Line]
        #region Instantiate [Block] - [U-Edge] handles for [Line]
        for (int i = 0; i < BL_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_U_Edges.GetLength(1); ++j)
            {
                Vector3 pos = BL_U_Edges[i, j].Position;
                Quaternion rot = BL_U_Edges[i, j].Rotation;
                GameObject go = Instantiate(bl_edge_giz, pos, rot, transform.Find("BL_Edges_Handles"));
                BL_U_Edges[i, j].Gizmos_GO = go;

                if (!showBlockEdge)
                {
                    go.GetComponentInChildren<MeshRenderer>().enabled = false;
                }
                else
                {
                    go.GetComponentInChildren<MeshRenderer>().enabled = true;
                }
            }
        }
        #endregion

        #region Instantiate [Block] - [V-Edge] handles for [Line]
        for (int i = 0; i < BL_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_V_Edges.GetLength(1); ++j)
            {
                Vector3 pos = BL_V_Edges[i, j].Position;
                Quaternion rot = BL_V_Edges[i, j].Rotation;
                GameObject go = Instantiate(bl_edge_giz, pos, rot, transform.Find("BL_Edges_Handles"));
                BL_V_Edges[i, j].Gizmos_GO = go;

                if (!showBlockEdge)
                {
                    go.GetComponentInChildren<MeshRenderer>().enabled = false;
                }
                else
                {
                    go.GetComponentInChildren<MeshRenderer>().enabled = true;
                }
            }
        }
        #endregion
    }

    //*! Update the Graph size in Edit Mode with [SetActive Method]
    [ContextMenu("Render_Edge_Handles_Edit_Mode")]
    private void Render_Edge_Handles_Edit_Mode()
    {
        //*! Get Child Count's references of both [BL_Edges_Handles] & [LI_Edges_Handles]
        #region Get Child Count's references of both [BL_Edges_Handles] & [LI_Edges_Handles]
        int bl_edge_Count = transform.Find("BL_Edges_Handles").childCount;
        int li_edge_Count = transform.Find("LI_Edges_Handles").childCount;
        #endregion

        //*! Switch OFF the whole list of [Edge] in both [Block] & [Line] for refresh purpose
        #region Switch OFF all [Edge] of [Block] & [Line] for refreshing purpose
        for (int i = 0; i < bl_edge_Count; ++i)
        {
            transform.Find("BL_Edges_Handles").GetChild(i).transform.GetComponentInChildren<MeshRenderer>().enabled = false;
        }

        for (int i = 0; i < li_edge_Count; ++i)
        {
            transform.Find("LI_Edges_Handles").GetChild(i).transform.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
        #endregion

        //*! Switch ON [Line] [Edge] according to row & col number
        //*! Set [Line] [Edge] [Type] to [Boarder] base on row & col
        #region Switch ON [Edge] of [Line] and set [Board] + [Board Type] base on row & col number 
        for (int i = 0; i < row - 1; ++i)
        {
            for (int j = 0; j < col; ++j)
            {
                if (showLineEdge)
                {
                    LI_U_Edges[i, j].Gizmos_GO.GetComponentInChildren<MeshRenderer>().enabled = true;
                }
                else
                {
                    LI_U_Edges[i, j].Gizmos_GO.GetComponentInChildren<MeshRenderer>().enabled = false;
                }
                
                if (j == 0)
                {
                    LI_U_Edges[i, j].Edge_Type = Edge_Type.Boarder;
                    LI_U_Edges[i, j].Boarder_Type = Boarder_Type.DN;
                    LI_U_Edges[i, j].Gizmos_GO.transform.GetChild(0).GetComponent<LI_Edge_Handle>().edgeType = LI_Edge_Handle_Type.Boarder;
                    LI_U_Edges[i, j].Gizmos_GO.transform.GetChild(0).GetComponent<LI_Edge_Handle>().boarderType = Boarder_Type.DN;
                }

                if (j == col - 1)
                {
                    LI_U_Edges[i, j].Edge_Type = Edge_Type.Boarder;
                    LI_U_Edges[i, j].Boarder_Type = Boarder_Type.UP;
                    LI_U_Edges[i, j].Gizmos_GO.transform.GetChild(0).GetComponent<LI_Edge_Handle>().edgeType = LI_Edge_Handle_Type.Boarder;
                    LI_U_Edges[i, j].Gizmos_GO.transform.GetChild(0).GetComponent<LI_Edge_Handle>().boarderType = Boarder_Type.UP;
                }
            }
        }

        for (int i = 0; i < row; ++i)
        {
            for (int j = 0; j < col - 1; ++j)
            {
                if (showLineEdge)
                {
                    LI_V_Edges[i, j].Gizmos_GO.GetComponentInChildren<MeshRenderer>().enabled = true;
                }
                else
                {
                    LI_V_Edges[i, j].Gizmos_GO.GetComponentInChildren<MeshRenderer>().enabled = false;
                }

                if (i == 0)
                {
                    LI_V_Edges[i, j].Edge_Type = Edge_Type.Boarder;
                    LI_V_Edges[i, j].Boarder_Type = Boarder_Type.LFT;
                    LI_V_Edges[i, j].Gizmos_GO.transform.GetChild(0).GetComponent<LI_Edge_Handle>().edgeType = LI_Edge_Handle_Type.Boarder;
                    LI_V_Edges[i, j].Gizmos_GO.transform.GetChild(0).GetComponent<LI_Edge_Handle>().boarderType = Boarder_Type.LFT;
                }

                if (i == row - 1)
                {
                    LI_V_Edges[i, j].Edge_Type = Edge_Type.Boarder;
                    LI_V_Edges[i, j].Boarder_Type = Boarder_Type.RGT;
                    LI_V_Edges[i, j].Gizmos_GO.transform.GetChild(0).GetComponent<LI_Edge_Handle>().edgeType = LI_Edge_Handle_Type.Boarder;
                    LI_V_Edges[i, j].Gizmos_GO.transform.GetChild(0).GetComponent<LI_Edge_Handle>().boarderType = Boarder_Type.RGT;
                }
            }
        }
        #endregion

        //*! Switch ON [Block] [Edge] according to row & col number
        #region Switch ON [Edge] of [Block] base on row & col number
        for (int i = 0; i < row; ++i)
        {
            for (int j = 0; j < col - 1; ++j)
            {
                if (showBlockEdge)
                {
                    BL_U_Edges[i, j].Gizmos_GO.GetComponentInChildren<MeshRenderer>().enabled = true;
                }
                else
                {
                    BL_U_Edges[i, j].Gizmos_GO.GetComponentInChildren<MeshRenderer>().enabled = false;
                }
            }
        }

        for (int i = 0; i < row - 1; ++i)
        {
            for (int j = 0; j < col; ++j)
            {
                if (showBlockEdge)
                {
                    BL_V_Edges[i, j].Gizmos_GO.GetComponentInChildren<MeshRenderer>().enabled = true;
                }
                else
                {
                    BL_V_Edges[i, j].Gizmos_GO.GetComponentInChildren<MeshRenderer>().enabled = false;
                }
            }
        }
        #endregion
    }

    //*! Check every [Edge] [Type] [Data]'s enum flag to switch [Nodes] traversability ON/OFF
    [ContextMenu("Update_Graph_Traversability_Edited_Mode")]
    private void Update_Graph_Traversability_Edited_Mode()
    {
        #region Check [Line] - [U-Edge] and set [Block] - [Node] Traversability
        for (int i = 0; i < row - 1; ++i)
        {
            for (int j = 0; j < col; ++j)
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
        for (int i = 0; i < row; ++i)
        {
            for (int j = 0; j < col - 1; ++j)
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
        for (int i = 0; i < row; ++i)
        {
            for (int j = 0; j < col - 1; ++j)
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
        for (int i = 0; i < row - 1; ++i)
        {
            for (int j = 0; j < col; ++j)
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

    //*! Render all [Node] and its arrow indicators ON/OFF according to the [Edge] check result
    [ContextMenu("Render_Node_Gizmos_Edit_Mode")]
    private void Render_Node_Gizmos_Edit_Mode()
    {
        #region Setup gizmos' spacing based on handle size
        //!* Gizmos Spacing for [DrawMesh] method
        float gizmos_spacing = handle_size * 0.125f;
        #endregion

        //*! Get Child Count's references of both [BL_Edges_Handles] & [LI_Edges_Handles]
        #region Get Child Count's references of both [BL_Edges_Handles] & [LI_Edges_Handles]
        int bl_node_Count = transform.Find("BL_Node_Gizmos").childCount;
        int li_node_Count = transform.Find("LI_Node_Gizmos").childCount;
        #endregion

        #region Setup row and col for [Block] & [Line] in [Edit Mode]
        int bl_row = row - 1;
        int bl_col = col - 1;
        int li_row = row;
        int li_col = col;
        #endregion

        #region Switch OFF all [Node] of [Block] & [Line] for refreshing purpose
        for (int i = 0; i < bl_node_Count; ++i)
        {
            transform.Find("BL_Node_Gizmos").GetChild(i).transform.GetComponentInChildren<MeshRenderer>().enabled = false;
        }

        for (int i = 0; i < li_node_Count; ++i)
        {
            transform.Find("LI_Node_Gizmos").GetChild(i).transform.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
        #endregion

        #region Setup Gizmos for [Block] Nodes and indication arrows
        for (int i = 0; i < bl_row; ++i)
        {
            for (int j = 0; j < bl_col; ++j)
            {
                if (showBlockNode)
                {
                    BL_Nodes[i, j].Gizmos_GO.transform.GetComponentInChildren<MeshRenderer>().enabled = true;
                }
                else
                {
                    BL_Nodes[i, j].Gizmos_GO.transform.GetComponentInChildren<MeshRenderer>().enabled = false;
                }

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
        for (int i = 0; i < li_row; ++i)
        {
            for (int j = 0; j < li_col; ++j)
            {
                if (showLineNode)
                {
                    LI_Nodes[i, j].Gizmos_GO.transform.GetComponentInChildren<MeshRenderer>().enabled = true;
                }
                else
                {
                    LI_Nodes[i, j].Gizmos_GO.transform.GetComponentInChildren<MeshRenderer>().enabled = false;
                }


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

    //*! Assign All [Handle] & [Gizmos]'s [Edge] & [Node] - [Type] to [Data]
    [ContextMenu("Assign_All_Handle_Type_To_Data_Edit_Mode")]
    private void Assign_All_Handle_Type_To_Data_Edit_Mode()
    {
        //*! Assign [Block] [Node] [Handle] [Type] to [Block] [Node] [Data] [Type]
        #region Assign [Block] [Node] [Handle] [Type] to [Block] [Node] [Data] [Type]
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                Node currNode = BL_Nodes[i, j];
                currNode.Node_Type = (Node_Type)(int)currNode.Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType;
            }
        }
        #endregion

        //*! Assign [Line] [Node] [Handle] [Type] to [Line] [Node] [Data] [Type]
        #region Assign [Line] [Node] [Handle] [Type] to [Line] [Node] [Data] [Type]
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {
                Node currNode = LI_Nodes[i, j];
                currNode.Node_Type = (Node_Type)(int)currNode.Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType;
            }
        }
        #endregion

        //*! Assign [Line] [U-Edge] [Handle] [Type] to [Line] [U-Edge] [Data] [Type]
        #region Assign [Line] [U-Edge] [Handle] [Type] to [Line] [U-Edge] [Data] [Type]
        for (int i = 0; i < LI_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_U_Edges.GetLength(1); ++j)
            {
                Edge currEdge = LI_U_Edges[i, j];
                currEdge.Edge_Type = (Edge_Type)(int)currEdge.Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType;
                currEdge.Boarder_Type = currEdge.Gizmos_GO.transform.GetChild(0).GetComponent<LI_Edge_Handle>().boarderType;
            }
        }
        #endregion

        //*! Assign [Line] [V-Edge] [Handle] [Type] to [Line] [V-Edge] [Data] [Type]
        #region Assign [Line] [V-Edge] [Handle] [Type] to [Line] [V-Edge] [Data] [Type]
        for (int i = 0; i < LI_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_V_Edges.GetLength(1); ++j)
            {
                Edge currEdge = LI_V_Edges[i, j];
                currEdge.Edge_Type = (Edge_Type)(int)currEdge.Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType;
                currEdge.Boarder_Type = currEdge.Gizmos_GO.transform.GetChild(0).GetComponent<LI_Edge_Handle>().boarderType;
            }
        }
        #endregion

        //*! Assign [Block] [U-Edge] [Handle] [Type] to [Block] [U-Edge] [Data] [Type]
        #region Assign [Block] [U-Edge] [Handle] [Type] to [Block] [U-Edge] [Data] [Type]
        for (int i = 0; i < BL_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_U_Edges.GetLength(1); ++j)
            {
                Edge currEdge = BL_U_Edges[i, j];
                currEdge.Edge_Type = (Edge_Type)(int)currEdge.Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType;
            }
        }
        #endregion

        //*! Assign [Block] [U-Edge] [Handle] [Type] to [Block] [U-Edge] [Data] [Type]
        #region Assign [Block] [U-Edge] [Handle] [Type] to [Block] [U-Edge] [Data] [Type]
        for (int i = 0; i < BL_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_V_Edges.GetLength(1); ++j)
            {
                Edge currEdge = BL_V_Edges[i, j];
                currEdge.Edge_Type = (Edge_Type)(int)currEdge.Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType;
            }
        }
        #endregion
    }

    //*! Assign [Data] [Node] & [Edge] [Type] to [Handle] & [Gizmos]'s [Node] & [Edge] [Type]
    [ContextMenu("Assign_All_Data_Type_To_Handle_Edit_Mode")]
    private void Assign_All_Data_Type_To_Handle_Edit_Mode()
    {
        //*! Assign [Block] [Node] [Data] [Type] to [Block] [Node] [Handle] [Type]
        #region Assign [Block] [Node] [Data] [Type] to [Block] [Node] [Handle] [Type]
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                Node currNode = BL_Nodes[i, j];
                currNode.Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType = (BL_Node_Handle_Type)(int)currNode.Node_Type;
                currNode.Gizmos_GO.transform.GetChild(0).position = currNode.Position;
                currNode.Gizmos_GO.transform.GetChild(0).rotation = Quaternion.identity;
                currNode.Gizmos_GO.transform.GetChild(0).localScale = new Vector3(handle_size, handle_size, handle_size);
            }
        }
        #endregion

        //*! Assign [Line] [Node] [Data] [Type] to [Line] [Node] [Handle] [Type]
        #region Assign [Line] [Node] [Data] [Type] to [Line] [Node] [Handle] [Type]
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {
                Node currNode = LI_Nodes[i, j];
                currNode.Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType = (LI_Node_Handle_Type)(int)currNode.Node_Type;
                currNode.Gizmos_GO.transform.GetChild(0).position = currNode.Position;
                currNode.Gizmos_GO.transform.GetChild(0).rotation = Quaternion.identity;
                currNode.Gizmos_GO.transform.GetChild(0).localScale = new Vector3(handle_size, handle_size, handle_size);
            }
        }
        #endregion

        //*! Assign [Line] [U-Edge] [Data] [Type] to [Line] [U-Edge] [Handle] [Type]
        #region Assign [Line] [U-Edge] [Data] [Type] to [Line] [U-Edge] [Handle] [Type]
        for (int i = 0; i < LI_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_U_Edges.GetLength(1); ++j)
            {
                Edge currEdge = LI_U_Edges[i, j];
                currEdge.Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = (LI_Edge_Handle_Type)(int)currEdge.Edge_Type;
                currEdge.Gizmos_GO.transform.GetChild(0).GetComponent<LI_Edge_Handle>().boarderType = currEdge.Boarder_Type;
                currEdge.Gizmos_GO.transform.GetChild(0).position = currEdge.Position;
                currEdge.Gizmos_GO.transform.GetChild(0).rotation = currEdge.Rotation;
                currEdge.Gizmos_GO.transform.GetChild(0).localScale = Vector3.one;
            }
        }
        #endregion

        //*! Assign [Line] [V-Edge] [Data] [Type] to [Line] [V-Edge] [Handle] [Type]
        #region Assign [Line] [V-Edge] [Data] [Type] to [Line] [V-Edge] [Handle] [Type]
        for (int i = 0; i < LI_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_V_Edges.GetLength(1); ++j)
            {
                Edge currEdge = LI_V_Edges[i, j];
                currEdge.Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = (LI_Edge_Handle_Type)(int)currEdge.Edge_Type;
                currEdge.Gizmos_GO.transform.GetChild(0).GetComponent<LI_Edge_Handle>().boarderType = currEdge.Boarder_Type;
                currEdge.Gizmos_GO.transform.GetChild(0).position = currEdge.Position;
                currEdge.Gizmos_GO.transform.GetChild(0).rotation = currEdge.Rotation;
                currEdge.Gizmos_GO.transform.GetChild(0).localScale = Vector3.one;
            }
        }
        #endregion

        //*! Assign [Block] [U-Edge] [Data] [Type] to [Block] [U-Edge] [Handle] [Type]
        #region Assign [Block] [U-Edge] [Data] [Type] to [Block] [U-Edge] [Handle] [Type]
        for (int i = 0; i < BL_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_U_Edges.GetLength(1); ++j)
            {
                Edge currEdge = BL_U_Edges[i, j];
                currEdge.Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = (BL_Edge_Handle_Type)(int)currEdge.Edge_Type;
                currEdge.Gizmos_GO.transform.GetChild(0).position = currEdge.Position;
                currEdge.Gizmos_GO.transform.GetChild(0).rotation = currEdge.Rotation;
                currEdge.Gizmos_GO.transform.GetChild(0).localScale = Vector3.one;
            }
        }
        #endregion

        //*! Assign [Block] [U-Edge] [Handle] [Type] to [Block] [U-Edge] [Data] [Type]
        #region Assign [Block] [U-Edge] [Handle] [Type] to [Block] [U-Edge] [Data] [Type]
        for (int i = 0; i < BL_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_V_Edges.GetLength(1); ++j)
            {
                Edge currEdge = BL_V_Edges[i, j];
                currEdge.Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = (BL_Edge_Handle_Type)(int)currEdge.Edge_Type;
                currEdge.Gizmos_GO.transform.GetChild(0).position = currEdge.Position;
                currEdge.Gizmos_GO.transform.GetChild(0).rotation = currEdge.Rotation;
                currEdge.Gizmos_GO.transform.GetChild(0).localScale = Vector3.one;
            }
        }
        #endregion
    }

    //*! Render all [Node] [Edge] handles' icon base on their [Type] - (Cosmatic Only)
    [ContextMenu("Render_All_Handles_By_Type_Edit_Mode")]
    private void Render_All_Handles_By_Type_Edit_Mode()
    {
        //*! Replace [Block] [Node] mesh base on it's [Node Type]
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                #region Get reference of current [Block] [Node]
                Node currNode = BL_Nodes[i, j];
                #endregion

                #region Check [Block] [Node] is [None]
                if (currNode.Node_Type == Node_Type.NONE)
                {
                    currNode.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = node_giz_mesh;
                    currNode.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = bl_giz_mat;
                }
                #endregion

                #region Check [Block] [Node] is [Block Blue Goal]
                if (currNode.Node_Type == Node_Type.Block_Blue_Goal)
                {
                    currNode.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = sticker_giz_mesh;
                    currNode.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = block_blue_sticker_giz_mat;
                }
                #endregion

                #region Check [Block] [Node] is [Block Red Goal]
                if (currNode.Node_Type == Node_Type.Block_Red_Goal)
                {
                    currNode.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = sticker_giz_mesh;
                    currNode.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = block_red_sticker_giz_mat;
                }
                #endregion

                #region Check [Block] [Node] is [Block Blue Start]
                if (currNode.Node_Type == Node_Type.Block_Blue_Start)
                {
                    currNode.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = sticker_giz_mesh;
                    currNode.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = block_blue_start_giz_mat;
                }
                #endregion

                #region Check [Block] [Node] is [Block Red Start]
                if (currNode.Node_Type == Node_Type.Block_Red_Start)
                {
                    currNode.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = sticker_giz_mesh;
                    currNode.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = block_red_start_giz_mat;
                }
                #endregion
            }
        }

        //*! Replace [Line] [Node] mesh base on it's [Node Type]
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {
                #region Get reference of current [Line] [Node]
                Node currNode = LI_Nodes[i, j];
                #endregion

                #region Check [Line] [Node] is [None]
                if (currNode.Node_Type == Node_Type.NONE)
                {
                    currNode.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = node_giz_mesh;
                    currNode.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = li_giz_mat;
                }
                #endregion

                #region Check [Line] [Node] is [Line Blue Goal]
                if (currNode.Node_Type == Node_Type.Line_Blue_Goal)
                {
                    currNode.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = sticker_giz_mesh;
                    currNode.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = line_blue_sticker_giz_mat;
                }
                #endregion

                #region Check [Line] [Node] is [Line Red Goal]
                if (currNode.Node_Type == Node_Type.Line_Red_Goal)
                {
                    currNode.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = sticker_giz_mesh;
                    currNode.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = line_red_sticker_giz_mat;
                }
                #endregion

                #region Check [Line] [Node] is [Line Blue Head]
                if (currNode.Node_Type == Node_Type.Line_Blue_Head)
                {
                    currNode.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = sticker_giz_mesh;
                    currNode.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = line_blue_head_giz_mat;
                }
                #endregion

                #region Check [Line] [Node] is [Line Blue Segment]
                if (currNode.Node_Type == Node_Type.Line_Blue_Segment)
                {
                    currNode.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = sticker_giz_mesh;
                    currNode.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = line_blue_segment_giz_mat;
                }
                #endregion

                #region Check [Line] [Node] is [Line Red Head]
                if (currNode.Node_Type == Node_Type.Line_Red_Head)
                {
                    currNode.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = sticker_giz_mesh;
                    currNode.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = line_red_head_giz_mat;
                }
                #endregion

                #region Check [Line] [Node] is [Line Red Segment]
                if (currNode.Node_Type == Node_Type.Line_Red_Segment)
                {
                    currNode.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = sticker_giz_mesh;
                    currNode.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = line_red_segment_giz_mat;
                }
                #endregion
            }
        }

        //*! Replace [Line] [U-Edge] mesh base on it's [Node Type]
        for (int i = 0; i < LI_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_U_Edges.GetLength(1); ++j)
            {
                #region Get reference of current [Line] [U-Edge]
                Edge currEdge = LI_U_Edges[i, j];
                #endregion

                #region Check [Line] [U-Edge] is [None]
                if (currEdge.Edge_Type == Edge_Type.NONE)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = edge_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = li_giz_mat;
                }
                #endregion

                #region Check [Line] [U-Edge] is [Black_Pen]
                if (currEdge.Edge_Type == Edge_Type.Black_Pen)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = obstacle_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = black_pen_giz_mat;
                }
                #endregion

                #region Check [Line] [U-Edge] is [Line_Blue_Edge]
                if (currEdge.Edge_Type == Edge_Type.Line_Blue_Edge)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = obstacle_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = line_blue_edge_giz_mat;
                }
                #endregion

                #region Check [Line] [U-Edge] is [Line_Red_Edge]
                if (currEdge.Edge_Type == Edge_Type.Line_Red_Edge)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = obstacle_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = line_red_edge_giz_mat;
                }
                #endregion

                #region Check [Line] [U-Edge] is [Boarder]
                if (currEdge.Edge_Type == Edge_Type.Boarder)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = edge_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<Renderer>().material = boarder_giz_mat;
                }
                #endregion
            }
        }

        //*! Replace [Line] [V-Edge] mesh base on it's [Edge Type]
        for (int i = 0; i < LI_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_V_Edges.GetLength(1); ++j)
            {
                #region Get reference of current [Line] [V-Edge]
                Edge currEdge = LI_V_Edges[i, j];
                #endregion

                #region Check [Line] [V-Edge] is [None]
                if (currEdge.Edge_Type == Edge_Type.NONE)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = edge_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<Renderer>().material = li_giz_mat;
                }
                #endregion

                #region Check [Line] [V-Edge] is [Black_Pen]
                if (currEdge.Edge_Type == Edge_Type.Black_Pen)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = obstacle_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = black_pen_giz_mat;
                }
                #endregion

                #region Check [Line] [V-Edge] is [Line_Blue_Edge]
                if (currEdge.Edge_Type == Edge_Type.Line_Blue_Edge)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = obstacle_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = line_blue_edge_giz_mat;
                }
                #endregion

                #region Check [Line] [V-Edge] is [Line_Red_Edge]
                if (currEdge.Edge_Type == Edge_Type.Line_Red_Edge)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = obstacle_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = line_red_edge_giz_mat;
                }
                #endregion

                #region Check [Line] [V-Edge] is [Boarder]
                if (currEdge.Edge_Type == Edge_Type.Boarder)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = edge_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = boarder_giz_mat;
                }
                #endregion
            }
        }

        //*! Replace [Block] [U-Edge] mesh base on it's [Edge Type]
        for (int i = 0; i < BL_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_U_Edges.GetLength(1); ++j)
            {
                #region Get reference of current [Block] [U-Edge]
                Edge currEdge = BL_U_Edges[i, j];
                #endregion

                #region Check [Block] [U-Edge] is [None]
                if (currEdge.Edge_Type == Edge_Type.NONE)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = edge_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<Renderer>().material = bl_giz_mat;
                }
                #endregion

                #region Check [Block] [U-Edge] is [Highlighter_Red]
                if (currEdge.Edge_Type == Edge_Type.HighLighter_Red)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = obstacle_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = highlighter_red_giz_mat;
                }
                #endregion
            }
        }

        //*! Replace [Block] [V-Edge] mesh base on it's [Edge Type]
        for (int i = 0; i < BL_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_V_Edges.GetLength(1); ++j)
            {
                #region Get reference of current [Block] [V-Edge]
                Edge currEdge = BL_V_Edges[i, j];
                #endregion

                #region Check [Block] [V-Edge] is [None]
                if (currEdge.Edge_Type == Edge_Type.NONE)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = edge_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<Renderer>().material = bl_giz_mat;
                }
                #endregion

                #region Check [Block] [V-Edge] is [Highlighter_Red]
                if (currEdge.Edge_Type == Edge_Type.HighLighter_Red)
                {
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshFilter>().mesh = obstacle_giz_mesh;
                    currEdge.Gizmos_GO.GetComponentInChildren<MeshRenderer>().material = highlighter_red_giz_mat;
                }
                #endregion
            }
        }
    }

    //*! Excute Refresh Graph function collection
    [ContextMenu("Refresh_Graph_Edit_Mode")]
    private void Refresh_Graph_Edit_Mode()
    {
        //*! Update the Graph size in Edit Mode with [SetActive Method]
        Render_Edge_Handles_Edit_Mode();

        //*! Check every [Edge] [Type] [Data]'s enum flag to switch [Nodes] traversability ON/OFF
        Update_Graph_Traversability_Edited_Mode();

        //*! Render all [Node] and its arrow indicators ON/OFF according to the [Edge] check result
        Render_Node_Gizmos_Edit_Mode();

        //*! Assign All [Handle] & [Gizmos]'s [Edge] & [Node] - [Type] to [Data]
        Assign_All_Handle_Type_To_Data_Edit_Mode();

        //*! Assign [Data] [Node] & [Edge] [Type] to [Handle] & [Gizmos]'s [Node] & [Edge] [Type]
        Assign_All_Data_Type_To_Handle_Edit_Mode();

        //*! Render all [Node] [Edge] handles' icon base on their [Type] - (Cosmatic Only)
        Render_All_Handles_By_Type_Edit_Mode();

        Debug.Log("Refresh Graph!");
    }

    //*! Move Graph to designated direction
    [ContextMenu("Move_Graph_Edit_Mode")]
    private void Move_Graph_Edit_Mode(Move_Graph direction)
    {
        #region Create boolean instance for checking
        bool notMove = direction == Move_Graph.NONE;
        bool moveUP = direction == Move_Graph.UP;
        bool moveDN = direction == Move_Graph.DN;
        bool moveLFT = direction == Move_Graph.LFT;
        bool moveRGT = direction == Move_Graph.RGT;
        #endregion

        #region Setup row/col for each [Node] & [Edge] array
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

        if (notMove) return;

        if (moveUP)
        {
            #region Check UP boundary [Node] & [Edge] - [Type] is NONE. If not, return
            for (int i = 0; i < bl_node_row; ++i)
            {
                if (BL_Nodes[i, bl_node_col - 1].Node_Type != Node_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                } 
            }

            for (int i = 0; i < li_node_row; ++i)
            {
                if (LI_Nodes[i, li_node_col - 1].Node_Type != Node_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < bl_U_edge_row; ++i)
            {
                if (BL_U_Edges[i, bl_U_edge_col - 1].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < bl_V_edge_row; ++i)
            {
                if (BL_V_Edges[i, bl_V_edge_col - 1].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < li_U_edge_row; ++i)
            {
                if (LI_U_Edges[i, li_U_edge_col - 2].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 1; i < li_V_edge_row - 1; ++i)
            {
                if (LI_V_Edges[i, li_V_edge_col - 1].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }
            #endregion

            #region Assign Lower [Node] or [Edge] - [Type] to Upper ones
            for (int i = 0; i < bl_node_row; ++i)
            {
                for (int j = bl_node_col - 1; j > 0 ; --j)
                {
                    BL_Nodes[i, j].Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType = BL_Nodes[i, j - 1].Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType;
                    BL_Nodes[i, j].Node_Type = BL_Nodes[i, j - 1].Node_Type;
                    BL_Nodes[i, j - 1].Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType = BL_Node_Handle_Type.NONE;
                    BL_Nodes[i, j - 1].Node_Type = Node_Type.NONE;
                }
            }

            for (int i = 0; i < li_node_row; ++i)
            {
                for (int j = li_node_col - 1; j > 0; --j)
                {
                    LI_Nodes[i, j].Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType = LI_Nodes[i, j - 1].Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType;
                    LI_Nodes[i, j].Node_Type = LI_Nodes[i, j - 1].Node_Type;
                    LI_Nodes[i, j - 1].Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType = LI_Node_Handle_Type.NONE;
                    LI_Nodes[i, j - 1].Node_Type = Node_Type.NONE;
                }
            }

            for (int i = 0; i < bl_U_edge_row; ++i)
            {
                for (int j = bl_U_edge_col - 1; j > 0; --j)
                {
                    BL_U_Edges[i, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_U_Edges[i, j - 1].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType;
                    BL_U_Edges[i, j].Edge_Type = BL_U_Edges[i, j - 1].Edge_Type;
                    BL_U_Edges[i, j - 1].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_Edge_Handle_Type.NONE;
                    BL_U_Edges[i, j - 1].Edge_Type = Edge_Type.NONE;
                }
            }

            for (int i = 0; i < bl_V_edge_row; ++i)
            {
                for (int j = bl_V_edge_col - 1; j > 0; --j)
                {
                    BL_V_Edges[i, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_V_Edges[i, j - 1].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType;
                    BL_V_Edges[i, j].Edge_Type = BL_V_Edges[i, j - 1].Edge_Type;
                    BL_V_Edges[i, j - 1].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_Edge_Handle_Type.NONE;
                    BL_V_Edges[i, j - 1].Edge_Type = Edge_Type.NONE;
                }
            }

            for (int i = 0; i < li_U_edge_row; ++i)
            {
                for (int j = li_U_edge_col - 2; j > 1; --j)
                {
                    LI_U_Edges[i, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_U_Edges[i, j - 1].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType;
                    LI_U_Edges[i, j].Edge_Type = LI_U_Edges[i, j - 1].Edge_Type;
                    LI_U_Edges[i, j - 1].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_Edge_Handle_Type.NONE;
                    LI_U_Edges[i, j - 1].Edge_Type = Edge_Type.NONE;
                }
            }

            for (int i = 1; i < li_V_edge_row - 1; ++i)
            {
                for (int j = li_V_edge_col - 1; j > 0; --j)
                {
                    LI_V_Edges[i, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_V_Edges[i, j - 1].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType;
                    LI_V_Edges[i, j].Edge_Type = LI_V_Edges[i, j - 1].Edge_Type;
                    LI_V_Edges[i, j - 1].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_Edge_Handle_Type.NONE;
                    LI_V_Edges[i, j - 1].Edge_Type = Edge_Type.NONE;
                }
            }
            #endregion
        }

        if (moveDN)
        {
            #region Check DN boundary [Node] & [Edge] - [Type] is NONE. If not, return
            for (int i = 0; i < bl_node_row; ++i)
            {
                if (BL_Nodes[i, 0].Node_Type != Node_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < li_node_row; ++i)
            {
                if (LI_Nodes[i, 0].Node_Type != Node_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < bl_U_edge_row; ++i)
            {
                if (BL_U_Edges[i, 0].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < bl_V_edge_row; ++i)
            {
                if (BL_V_Edges[i, 0].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < li_U_edge_row; ++i)
            {
                if (LI_U_Edges[i, 1].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 1; i < li_V_edge_row - 1; ++i)
            {
                if (LI_V_Edges[i, 0].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }
            #endregion

            #region Assign Upper [Node] or [Edge] - [Type] to Lower ones
            for (int i = 0; i < bl_node_row; ++i)
            {
                for (int j = 0; j < bl_node_col - 1; ++j)
                {
                    BL_Nodes[i, j].Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType = BL_Nodes[i, j + 1].Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType;
                    BL_Nodes[i, j].Node_Type = BL_Nodes[i, j + 1].Node_Type;
                    BL_Nodes[i, j + 1].Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType = BL_Node_Handle_Type.NONE;
                    BL_Nodes[i, j + 1].Node_Type = Node_Type.NONE;
                }
            }

            for (int i = 0; i < li_node_row; ++i)
            {
                for (int j = 0; j < li_node_col - 1; ++j)
                {
                    LI_Nodes[i, j].Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType = LI_Nodes[i, j + 1].Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType;
                    LI_Nodes[i, j].Node_Type = LI_Nodes[i, j + 1].Node_Type;
                    LI_Nodes[i, j + 1].Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType = LI_Node_Handle_Type.NONE;
                    LI_Nodes[i, j + 1].Node_Type = Node_Type.NONE;
                }
            }

            for (int i = 0; i < bl_U_edge_row; ++i)
            {
                for (int j = 0; j < bl_U_edge_col - 1; ++j)
                {
                    BL_U_Edges[i, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_U_Edges[i, j + 1].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType;
                    BL_U_Edges[i, j].Edge_Type = BL_U_Edges[i, j + 1].Edge_Type;
                    BL_U_Edges[i, j + 1].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_Edge_Handle_Type.NONE;
                    BL_U_Edges[i, j + 1].Edge_Type = Edge_Type.NONE;
                }
            }

            for (int i = 0; i < bl_V_edge_row; ++i)
            {
                for (int j = 0; j < bl_V_edge_col - 1; ++j)
                {
                    BL_V_Edges[i, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_V_Edges[i, j + 1].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType;
                    BL_V_Edges[i, j].Edge_Type = BL_V_Edges[i, j + 1].Edge_Type;
                    BL_V_Edges[i, j + 1].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_Edge_Handle_Type.NONE;
                    BL_V_Edges[i, j + 1].Edge_Type = Edge_Type.NONE;
                }
            }

            for (int i = 0; i < li_U_edge_row; ++i)
            {
                for (int j = 1; j < li_U_edge_col - 2; ++j)
                {
                    LI_U_Edges[i, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_U_Edges[i, j + 1].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType;
                    LI_U_Edges[i, j].Edge_Type = LI_U_Edges[i, j + 1].Edge_Type;
                    LI_U_Edges[i, j + 1].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_Edge_Handle_Type.NONE;
                    LI_U_Edges[i, j + 1].Edge_Type = Edge_Type.NONE;
                }
            }

            for (int i = 1; i < li_V_edge_row - 1; ++i)
            {
                for (int j = 0; j < li_V_edge_col - 1; ++j)
                {
                    LI_V_Edges[i, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_V_Edges[i, j + 1].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType;
                    LI_V_Edges[i, j].Edge_Type = LI_V_Edges[i, j + 1].Edge_Type;
                    LI_V_Edges[i, j + 1].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_Edge_Handle_Type.NONE;
                    LI_V_Edges[i, j + 1].Edge_Type = Edge_Type.NONE;
                }
            }
            #endregion
        }

        if (moveLFT)
        {
            #region Check LFT boundary [Node] & [Edge] - [Type] is NONE. If not, return
            for (int i = 0; i < bl_node_col; ++i)
            {
                if (BL_Nodes[0, i].Node_Type != Node_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < li_node_col; ++i)
            {
                if (LI_Nodes[0, i].Node_Type != Node_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < bl_U_edge_col; ++i)
            {
                if (BL_U_Edges[0, i].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < bl_V_edge_col; ++i)
            {
                if (BL_V_Edges[0, i].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 1; i < li_U_edge_col - 1; ++i)
            {
                if (LI_U_Edges[0, i].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < li_V_edge_col; ++i)
            {
                if (LI_V_Edges[1, i].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }
            #endregion

            #region Assign Right [Node] or [Edge] - [Type] to Left ones
            for (int i = 0; i < bl_node_row - 1; ++i)
            {
                for (int j = 0; j < bl_node_col; ++j)
                {
                    BL_Nodes[i, j].Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType = BL_Nodes[i + 1, j].Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType;
                    BL_Nodes[i, j].Node_Type = BL_Nodes[i + 1, j].Node_Type;
                    BL_Nodes[i + 1, j].Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType = BL_Node_Handle_Type.NONE;
                    BL_Nodes[i + 1, j].Node_Type = Node_Type.NONE;
                }
            }

            for (int i = 0; i < li_node_row - 1; ++i)
            {
                for (int j = 0; j < li_node_col; ++j)
                {
                    LI_Nodes[i, j].Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType = LI_Nodes[i + 1, j].Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType;
                    LI_Nodes[i, j].Node_Type = LI_Nodes[i + 1, j].Node_Type;
                    LI_Nodes[i + 1, j].Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType = LI_Node_Handle_Type.NONE;
                    LI_Nodes[i + 1, j].Node_Type = Node_Type.NONE;
                }
            }

            for (int i = 0; i < bl_U_edge_row - 1; ++i)
            {
                for (int j = 0; j < bl_U_edge_col; ++j)
                {
                    BL_U_Edges[i, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_U_Edges[i + 1, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType;
                    BL_U_Edges[i, j].Edge_Type = BL_U_Edges[i + 1, j].Edge_Type;
                    BL_U_Edges[i + 1, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_Edge_Handle_Type.NONE;
                    BL_U_Edges[i + 1, j].Edge_Type = Edge_Type.NONE;
                }
            }

            for (int i = 0; i < bl_V_edge_row - 1; ++i)
            {
                for (int j = 0; j < bl_V_edge_col; ++j)
                {
                    BL_V_Edges[i, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_V_Edges[i + 1, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType;
                    BL_V_Edges[i, j].Edge_Type = BL_V_Edges[i + 1, j].Edge_Type;
                    BL_V_Edges[i + 1, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_Edge_Handle_Type.NONE;
                    BL_V_Edges[i + 1, j].Edge_Type = Edge_Type.NONE;
                }
            }

            for (int i = 0; i < li_U_edge_row - 1; ++i)
            {
                for (int j = 1; j < li_U_edge_col - 2; ++j)
                {
                    LI_U_Edges[i, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_U_Edges[i + 1, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType;
                    LI_U_Edges[i, j].Edge_Type = LI_U_Edges[i + 1, j].Edge_Type;
                    LI_U_Edges[i + 1, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_Edge_Handle_Type.NONE;
                    LI_U_Edges[i + 1, j].Edge_Type = Edge_Type.NONE;
                }
            }

            for (int i = 1; i < li_V_edge_row - 2; ++i)
            {
                for (int j = 0; j < li_V_edge_col; ++j)
                {
                    LI_V_Edges[i, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_V_Edges[i + 1, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType;
                    LI_V_Edges[i, j].Edge_Type = LI_V_Edges[i + 1, j].Edge_Type;
                    LI_V_Edges[i + 1, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_Edge_Handle_Type.NONE;
                    LI_V_Edges[i + 1, j].Edge_Type = Edge_Type.NONE;
                }
            }
            #endregion
        }

        if (moveRGT)
        {
            #region Check RGT boundary [Node] & [Edge] - [Type] is NONE. If not, return
            for (int i = 0; i < bl_node_col; ++i)
            {
                if (BL_Nodes[bl_node_row - 1, i].Node_Type != Node_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < li_node_col; ++i)
            {
                if (LI_Nodes[li_node_row - 1, i].Node_Type != Node_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < bl_U_edge_col; ++i)
            {
                if (BL_U_Edges[bl_U_edge_row - 1, i].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < bl_V_edge_col; ++i)
            {
                if (BL_V_Edges[bl_V_edge_row - 1, i].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 1; i < li_U_edge_col - 1; ++i)
            {
                if (LI_U_Edges[li_U_edge_row - 1, i].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }

            for (int i = 0; i < li_V_edge_col; ++i)
            {
                if (LI_V_Edges[li_V_edge_row - 2, i].Edge_Type != Edge_Type.NONE)
                {
                    move_Graph_Direction = Move_Graph.NONE;
                    return;
                }
            }
            #endregion

            #region Assign Left [Node] or [Edge] - [Type] to Right ones
            for (int i = bl_node_row - 1; i > 0; --i)
            {
                for (int j = 0; j < bl_node_col; ++j)
                {
                    BL_Nodes[i, j].Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType = BL_Nodes[i - 1, j].Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType;
                    BL_Nodes[i, j].Node_Type = BL_Nodes[i - 1, j].Node_Type;
                    BL_Nodes[i - 1, j].Gizmos_GO.GetComponentInChildren<BL_Node_Handle>().nodeType = BL_Node_Handle_Type.NONE;
                    BL_Nodes[i - 1, j].Node_Type = Node_Type.NONE;
                }
            }

            for (int i = li_node_row - 1; i > 0; --i)
            {
                for (int j = 0; j < li_node_col; ++j)
                {
                    LI_Nodes[i, j].Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType = LI_Nodes[i - 1, j].Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType;
                    LI_Nodes[i, j].Node_Type = LI_Nodes[i - 1, j].Node_Type;
                    LI_Nodes[i - 1, j].Gizmos_GO.GetComponentInChildren<LI_Node_Handle>().nodeType = LI_Node_Handle_Type.NONE;
                    LI_Nodes[i - 1, j].Node_Type = Node_Type.NONE;
                }
            }

            for (int i = bl_U_edge_row - 1; i > 0; --i)
            {
                for (int j = 0; j < bl_U_edge_col; ++j)
                {
                    BL_U_Edges[i, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_U_Edges[i - 1, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType;
                    BL_U_Edges[i, j].Edge_Type = BL_U_Edges[i - 1, j].Edge_Type;
                    BL_U_Edges[i - 1, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_Edge_Handle_Type.NONE;
                    BL_U_Edges[i - 1, j].Edge_Type = Edge_Type.NONE;
                }
            }

            for (int i = bl_V_edge_row - 1; i > 0; --i)
            {
                for (int j = 0; j < bl_V_edge_col; ++j)
                {
                    BL_V_Edges[i, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_V_Edges[i - 1, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType;
                    BL_V_Edges[i, j].Edge_Type = BL_V_Edges[i - 1, j].Edge_Type;
                    BL_V_Edges[i - 1, j].Gizmos_GO.GetComponentInChildren<BL_Edge_Handle>().edgeType = BL_Edge_Handle_Type.NONE;
                    BL_V_Edges[i - 1, j].Edge_Type = Edge_Type.NONE;
                }
            }

            for (int i = li_U_edge_row - 1; i > 0; --i)
            {
                for (int j = 1; j < li_U_edge_col - 2; ++j)
                {
                    LI_U_Edges[i, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_U_Edges[i - 1, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType;
                    LI_U_Edges[i, j].Edge_Type = LI_U_Edges[i - 1, j].Edge_Type;
                    LI_U_Edges[i - 1, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_Edge_Handle_Type.NONE;
                    LI_U_Edges[i - 1, j].Edge_Type = Edge_Type.NONE;
                }
            }

            for (int i = li_V_edge_row - 2; i > 1; --i)
            {
                for (int j = 0; j < li_V_edge_col; ++j)
                {
                    LI_V_Edges[i, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_V_Edges[i - 1, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType;
                    LI_V_Edges[i, j].Edge_Type = LI_V_Edges[i - 1, j].Edge_Type;
                    LI_V_Edges[i - 1, j].Gizmos_GO.GetComponentInChildren<LI_Edge_Handle>().edgeType = LI_Edge_Handle_Type.NONE;
                    LI_V_Edges[i - 1, j].Edge_Type = Edge_Type.NONE;
                }
            }
            #endregion
        }

        move_Graph_Direction = Move_Graph.NONE;
    }

    //*! Save Level data to files
    [ContextMenu("Save_Level_Data")]
    private void Save_Level_Data()
    {
        if (isSaved)
        {
            //*! If there is no Lv_Data pluged in, show error message and return
            #region If there is no Lv_Data pluged in, show error message and return
            if (lv_Data == null)
            {
                Debug.Log("There is no Level Data loaded in [Pencil_Case]. Please select a Level Data to edit.");
                isSaved = false;
                return;
            }
            #endregion

            //*! Setup row/col for each [Node] & [Edge] array
            #region Setup row/col for each [Node] & [Edge] array
            lv_Data.row = row;
            lv_Data.col = col;

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

            //*! Create [Inter Node] and [Inter Edge] to save data
            #region Create [Inter Node] and [Inter Edge] to save data
            lv_Data.BL_Nodes = new Inter_Node[bl_node_row * bl_node_col];
            lv_Data.LI_Nodes = new Inter_Node[li_node_row * li_node_col];
            lv_Data.BL_U_Edges = new Inter_Edge[bl_U_edge_row * bl_U_edge_col];
            lv_Data.BL_V_Edges = new Inter_Edge[bl_V_edge_row * bl_V_edge_col];
            lv_Data.LI_U_Edges = new Inter_Edge[li_U_edge_row * li_U_edge_col];
            lv_Data.LI_V_Edges = new Inter_Edge[li_V_edge_row * li_V_edge_col];
            #endregion

            //*! Assign [Block] [Node] 2D Array to [Lv_Data] reflected 1D Array
            #region Assign [Block] [Node] 2D Array to Lv_Data 1D Array
            for (int i = 0; i < bl_node_row; ++i)
            {
                for (int j = 0; j < bl_node_col; ++j)
                {
                    int colSize = bl_node_col;
                    lv_Data.BL_Nodes[colSize * i + j] = new Inter_Node();
                    lv_Data.BL_Nodes[colSize * i + j].Position = BL_Nodes[i, j].Position;
                    lv_Data.BL_Nodes[colSize * i + j].Node_Type = BL_Nodes[i, j].Node_Type;
                }
            }
            #endregion

            //*! Assign [Line] [Node] 2D Array to [Lv_Data] reflected 1D Array
            #region Assign [Line] [Node] 2D Array to Lv_Data 1D Array
            for (int i = 0; i < li_node_row; ++i)
            {
                for (int j = 0; j < li_node_col; ++j)
                {
                    int colSize = li_node_col;
                    lv_Data.LI_Nodes[colSize * i + j] = new Inter_Node();
                    lv_Data.LI_Nodes[colSize * i + j].Position = LI_Nodes[i, j].Position;
                    lv_Data.LI_Nodes[colSize * i + j].Node_Type = LI_Nodes[i, j].Node_Type;
                }
            }
            #endregion

            //*! Assign [Line] [U-Edge] 2D Array to [Lv_Data] reflected 1D Array
            #region Assign [Line] [U-Edge] 2D Array to Lv_Data 1D Array
            for (int i = 0; i < li_U_edge_row; ++i)
            {
                for (int j = 0; j < li_U_edge_col; ++j)
                {
                    int colSize = li_U_edge_col;
                    lv_Data.LI_U_Edges[colSize * i + j] = new Inter_Edge();
                    lv_Data.LI_U_Edges[colSize * i + j].Position = LI_U_Edges[i, j].Position;
                    lv_Data.LI_U_Edges[colSize * i + j].Edge_Type = LI_U_Edges[i, j].Edge_Type;
                    lv_Data.LI_U_Edges[colSize * i + j].Edge_Direction = LI_U_Edges[i, j].Edge_Direction;
                }
            }
            #endregion

            //*! Assign [Line] [V-Edge] 2D Array to [Lv_Data] reflected 1D Array
            #region Assign [Line] [V-Edge] [Handle] [Type] to [Line] [V-Edge] [Data] [Type]
            for (int i = 0; i < li_V_edge_row; ++i)
            {
                for (int j = 0; j < li_V_edge_col; ++j)
                {
                    int colSize = li_V_edge_col;
                    lv_Data.LI_V_Edges[colSize * i + j] = new Inter_Edge();
                    lv_Data.LI_V_Edges[colSize * i + j].Position = LI_V_Edges[i, j].Position;
                    lv_Data.LI_V_Edges[colSize * i + j].Edge_Type = LI_V_Edges[i, j].Edge_Type;
                    lv_Data.LI_V_Edges[colSize * i + j].Edge_Direction = LI_V_Edges[i, j].Edge_Direction;

                }
            }
            #endregion

            //*! Assign [Block] [U-Edge] 2D Array to [Lv_Data] reflected 1D Array
            #region Assign [Block] [U-Edge] 2D Array to Lv_Data 1D Array
            for (int i = 0; i < bl_U_edge_row; ++i)
            {
                for (int j = 0; j < bl_U_edge_col; ++j)
                {
                    int colSize = bl_U_edge_col;
                    lv_Data.BL_U_Edges[colSize * i + j] = new Inter_Edge();
                    lv_Data.BL_U_Edges[colSize * i + j].Position = BL_U_Edges[i, j].Position;
                    lv_Data.BL_U_Edges[colSize * i + j].Edge_Type = BL_U_Edges[i, j].Edge_Type;
                    lv_Data.BL_U_Edges[colSize * i + j].Edge_Direction = BL_U_Edges[i, j].Edge_Direction;
                }
            }
            #endregion

            //*! Assign [Block] [V-Edge] 2D Array to [Lv_Data] reflected 1D Array
            #region Assign [Block] [V-Edge] 2D Array to Lv_Data 1D Array
            for (int i = 0; i < bl_V_edge_row; ++i)
            {
                for (int j = 0; j < bl_V_edge_col; ++j)
                {
                    int colSize = bl_V_edge_col;
                    lv_Data.BL_V_Edges[colSize * i + j] = new Inter_Edge();
                    lv_Data.BL_V_Edges[colSize * i + j].Position = BL_V_Edges[i, j].Position;
                    lv_Data.BL_V_Edges[colSize * i + j].Edge_Type = BL_V_Edges[i, j].Edge_Type;
                    lv_Data.BL_V_Edges[colSize * i + j].Edge_Direction = BL_V_Edges[i, j].Edge_Direction;
                }
            }
            #endregion

            //*! Assign Camera settings in scene to [Lv_Data]
            #region Assign Camera settings in scene to [Lv_Data]
            lv_Data.Cam.Position = cam.transform.position;
            lv_Data.Cam.Size = cam.orthographicSize;
            #endregion

            //*! UnityEditor - Refresh, SetDirty, SaveAssets
            #region Call UnityEditor - Refresh, SetDirty, SaveAssets
            UnityEditor.AssetDatabase.Refresh();

            UnityEditor.EditorUtility.SetDirty(lv_Data);

            UnityEditor.AssetDatabase.SaveAssets();
            #endregion

            Debug.Log("Level Data Saved!!");

            isSaved = false;
        }
    }

    //*! Load Level data from file
    [ContextMenu("Load_Level_Data")]
    private void Load_Level_Data()
    {
        if (isLoaded)
        {
            //*! If there is no Lv_Data pluged in, show error message and return
            #region If there is no Lv_Data pluged in, show error message and return
            if (lv_Data == null)
            {
                Debug.Log("There is no Level Data loaded in [Pencil_Case]. Please select a Level Data to edit.");
                isLoaded = false;
                return;
            }
            #endregion

            //*! If the editor's graph size is smaller than file' graph size
            //*! Print error message
            if (lv_Data.row > row || lv_Data.col > col)
            {
                //*! Print Error message if the Data Graph size is bigger than Pencil Case graph size
                #region Print Error message if the Data Graph size is bigger than Pencil Case graph size
                Debug.Log("This Level Data's [Row]: " + lv_Data.row + ", [Col]: " + lv_Data.col + "\n" +
                          "Current Pencil Case's [Row]: " + row + ", [Col]: " + col);

                Debug.Log("Current [Row]/[Col] size of Pencil Case is not big enough for the Level Data" +
                          "Please crank up the [Row] or [Col] to fit, thank you.");
                #endregion
            }
            else
            {
                //*! Setup row/col for each [Node] & [Edge] array
                #region Setup row/col for each [Node] & [Edge] array
                row = lv_Data.row;
                col = lv_Data.col;

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

                //*! Assign [Lv_Data] reflected 1D Array to [Block] [Node] 2D Array 
                #region Assign [Lv_Data] reflected 1D Array to [Block] [Node] 2D Array 
                for (int i = 0; i < bl_node_row; ++i)
                {
                    for (int j = 0; j < bl_node_col; ++j)
                    {
                        int colSize = bl_node_col;
                        BL_Nodes[i, j].Node_Type = lv_Data.BL_Nodes[colSize * i + j].Node_Type;
                    }
                }
                #endregion

                //*! Assign [Lv_Data] reflected 1D Array to [Line] [Node] 2D Array 
                #region Assign [Lv_Data] reflected 1D Array to [Line] [Node] 2D Array 
                for (int i = 0; i < li_node_row; ++i)
                {
                    for (int j = 0; j < li_node_col; ++j)
                    {
                        int colSize = li_node_col;
                        LI_Nodes[i, j].Node_Type = lv_Data.LI_Nodes[colSize * i + j].Node_Type;
                    }
                }
                #endregion

                //*! Assign [Lv_Data] reflected 1D Array to [Line] [U-Edge] 2D Array 
                #region Assign [Lv_Data] reflected 1D Array to [Line] [U-Edge] 2D Array 
                for (int i = 0; i < li_U_edge_row; ++i)
                {
                    for (int j = 0; j < li_U_edge_col; ++j)
                    {
                        int colSize = li_U_edge_col;
                        LI_U_Edges[i, j].Edge_Type = lv_Data.LI_U_Edges[colSize * i + j].Edge_Type;
                        LI_U_Edges[i, j].Edge_Direction = lv_Data.LI_U_Edges[colSize * i + j].Edge_Direction;
                    }
                }
                #endregion

                //*! Assign [Lv_Data] reflected 1D Array to [Line] [V-Edge] 2D Array 
                #region Assign [Lv_Data] reflected 1D Array to [Line] [V-Edge] 2D Array 
                for (int i = 0; i < li_V_edge_row; ++i)
                {
                    for (int j = 0; j < li_V_edge_col; ++j)
                    {
                        int colSize = li_V_edge_col;
                        LI_V_Edges[i, j].Edge_Type = lv_Data.LI_V_Edges[colSize * i + j].Edge_Type;
                        LI_V_Edges[i, j].Edge_Direction = lv_Data.LI_V_Edges[colSize * i + j].Edge_Direction;
                    }
                }
                #endregion

                //*! Assign [Lv_Data] reflected 1D Array to [BLock] [U-Edge] 2D Array 
                #region Assign [Lv_Data] reflected 1D Array to [BLock] [U-Edge] 2D Array 
                for (int i = 0; i < bl_U_edge_row; ++i)
                {
                    for (int j = 0; j < bl_U_edge_col; ++j)
                    {
                        int colSize = bl_U_edge_col;
                        BL_U_Edges[i, j].Edge_Type = lv_Data.BL_U_Edges[colSize * i + j].Edge_Type;
                        BL_U_Edges[i, j].Edge_Direction = lv_Data.BL_U_Edges[colSize * i + j].Edge_Direction;
                    }
                }
                #endregion

                //*! Assign [Lv_Data] reflected 1D Array to [BLock] [V-Edge] 2D Array 
                #region Assign [Lv_Data] reflected 1D Array to [BLock] [V-Edge] 2D Array 
                for (int i = 0; i < bl_V_edge_row; ++i)
                {
                    for (int j = 0; j < bl_V_edge_col; ++j)
                    {
                        int colSize = bl_V_edge_col;
                        BL_V_Edges[i, j].Edge_Type = lv_Data.BL_V_Edges[colSize * i + j].Edge_Type;
                        BL_V_Edges[i, j].Edge_Direction = lv_Data.BL_V_Edges[colSize * i + j].Edge_Direction;
                    }
                }
                #endregion

                //*! Assign [Lv_Data] camera settings to the camera in scene
                #region Assign [Lv_Data] camera settings to the camera in scene
                cam.transform.position = lv_Data.Cam.Position;
                cam.orthographicSize = lv_Data.Cam.Size;
                #endregion

                //*! Initiate Repaint Level Process initiate
                #region Initiate Repaint Level Process initiate
                //*! Render all [Edge] base on row/col and set [Boarder] data
                Render_Edge_Handles_Edit_Mode();

                //*! Check every [Edge] [Type] [Data]'s enum flag to turn the switch [Nodes] traversability ON/OFF
                Update_Graph_Traversability_Edited_Mode();

                //*! Render all [Node] and its [arrow Gizmos] according to the [Edge] check result
                Render_Node_Gizmos_Edit_Mode();

                //*! Assign [Data] [Node] & [Edge] [Type] to [Gizmos] [Node] & [Edge] [Type]
                Assign_All_Data_Type_To_Handle_Edit_Mode();

                //*! Render all [Node] [Edge] handles' icon base on their [Type] - (Cosmatic)
                Render_All_Handles_By_Type_Edit_Mode();
                #endregion

                Debug.Log("Level Data Loaded!!");
            }

            isLoaded = false;
        }
    }

    [ContextMenu("Get_Camera")]
    private void Get_Camera()
    {
        if (cam == null) cam = FindObjectOfType<Camera>();
    }

    //*! Only running this function when game runtime
    [ContextMenu("Runtime_Update")]
    void Runtime_Update()
    {
    }
}