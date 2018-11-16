//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: This is the [Game Manager], handles [Level Data]
//*!              and convert it into playable levels
//*!
//*! Last edit  : 15/10/2018
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
    public Lv_Data[] lvData;
    //[HideInInspector]
    public int lvDataIndex = -1;

    public Node[,] BL_Nodes;
    public Node[,] LI_Nodes;

    public Edge[,] BL_U_Edges;
    public Edge[,] BL_V_Edges;

    public Edge[,] LI_U_Edges;
    public Edge[,] LI_V_Edges;

    public bool Show_Gizmos;
    [Range(0.5f, 1.5f)]
    public float Gizmos_Size;

    //*! Scene Objects References
    public GameObject Black_Pen;
    public GameObject HighLighter_Red;
    public GameObject Block_Blue_Goal;
    public GameObject Block_Red_Goal;
    public GameObject Line_Blue_Goal;
    public GameObject Line_Red_Goal;
    public GameObject Blocky;
    public GameObject Shadey;
    public GameObject Careline;
    public GameObject Linel;
    public GameObject[] Backgrounds;

    [HideInInspector]
    public int Blue_Sticker_Count = 0;
    [HideInInspector]
    public int Red_Sticker_Count = 0;

    [HideInInspector]
    public Node Block_Blue_Start_Node;
    [HideInInspector]
    public Node Block_Red_Start_Node;

    [HideInInspector]
    public List<Node> Line_Blue_Start_Nodes;
    [HideInInspector]
    public List<Node> Line_Red_Start_Nodes;


    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//

    [HideInInspector]
    private int row;
    [HideInInspector]
    private int col;

    [HideInInspector]
    private Camera cam;

    [HideInInspector]
    private bool is_pencil_case;

    [SerializeField]
    [HideInInspector]
    private Mesh arrw_giz_mesh;
    [SerializeField]
    [HideInInspector]
    private Material bl_giz_mat;
    [SerializeField]
    [HideInInspector]
    private Material li_giz_mat;

    [HideInInspector]
    private Sound_Manager snd;

    [HideInInspector]
    private Level_Availability lvlav;


    void Awake()
    {
        Initialize_Level(lvDataIndex + 1);
    }

    void Update()
    {
        Check_Shortcut_Input();
        Clear_Pencil_Case_Gizmos();
        Render_Node_Traversability_Gizmos();
        snd.Game_PlaySound(Game_Sound.BACKGROUND_SOUND);
    }


    //*!----------------------------!*//
    //*!    Private Functions
    //*!----------------------------!*//

    [ContextMenu("Setup_Level_Data")]
    private void Setup_Level_Data(int lvIndex)
    {
        //*! If there is no Level Data pluged in, show error message and return
        #region If there is no Level Data pluged in, show error message and return
        if (lvData == null)
        {
            Debug.Log("There is no Level Data loaded in [Game_Manager]. Please select a Level Data to play.");
            return;
        }
        #endregion

        //*! Setup row/col for each [Node] & [Edge] array
        #region Setup row/col for each [Node] & [Edge] array
        row = lvData[lvIndex].row;
        col = lvData[lvIndex].col;

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

        //*! Setup new instances for [Line Start Nodes] lists
        #region Setup new instances for [Line Start Nodes] lists
        Line_Blue_Start_Nodes = new List<Node>();
        Line_Red_Start_Nodes = new List<Node>();
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
                BL_Nodes[i, j].Position = lvData[lvIndex].BL_Nodes[colSize * i + j].Position;
                BL_Nodes[i, j].Node_Type = lvData[lvIndex].BL_Nodes[colSize * i + j].Node_Type;
                BL_Nodes[i, j].UP_NODE = null;
                BL_Nodes[i, j].DN_NODE = null;
                BL_Nodes[i, j].LFT_NODE = null;
                BL_Nodes[i, j].RGT_NODE = null;

                //*! Check [Node] [Type] == [Block_Blue_Goal]
                if (BL_Nodes[i, j].Node_Type == Node_Type.Block_Blue_Goal)
                {
                    float rand = Random.Range(10f, 270f);
                    GameObject new_Gizmos_GO = Instantiate(Block_Blue_Goal, BL_Nodes[i, j].Position, Quaternion.Euler(new Vector3(0, 0, rand)), transform.Find("Symbols"));
                    BL_Nodes[i, j].Gizmos_GO = new_Gizmos_GO;
                    Blue_Sticker_Count++;
                }

                //* Check [Node] [Type] == [Block_Red_Goal]
                if (BL_Nodes[i, j].Node_Type == Node_Type.Block_Red_Goal)
                {
                    float rand = Random.Range(10f, 270f);
                    GameObject new_Gizmos_GO = Instantiate(Block_Red_Goal, BL_Nodes[i, j].Position, Quaternion.Euler(new Vector3(0, 0, rand)), transform.Find("Symbols"));
                    BL_Nodes[i, j].Gizmos_GO = new_Gizmos_GO;
                    Red_Sticker_Count++;
                }

                //* Check [Node] [Type] == [Block_Blue_Start]
                if (BL_Nodes[i, j].Node_Type == Node_Type.Block_Blue_Start)
                {
                    Instantiate(Shadey, BL_Nodes[i, j].Position, Quaternion.identity, transform.Find("Players"));
                    Block_Blue_Start_Node = BL_Nodes[i, j];
                    BL_Nodes[i, j].Node_Type = Node_Type.NONE;
                }

                //* Check [Node] [Type] == [Block_Red_Start]
                if (BL_Nodes[i, j].Node_Type == Node_Type.Block_Red_Start)
                {
                    Instantiate(Blocky, BL_Nodes[i, j].Position, Quaternion.identity, transform.Find("Players"));
                    Block_Red_Start_Node = BL_Nodes[i, j];
                    BL_Nodes[i, j].Node_Type = Node_Type.NONE;
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
                BL_Nodes[i, j].Can_UP = true;
                BL_Nodes[i, j].Can_RGT = true;
            }
        }

        for (int i = 0; i < BL_Nodes.GetUpperBound(0); ++i)
        {
            BL_Nodes[i, BL_Nodes.GetUpperBound(1)].RGT_NODE = BL_Nodes[i + 1, BL_Nodes.GetUpperBound(1)];
            BL_Nodes[i, BL_Nodes.GetUpperBound(1)].Can_RGT = true;
        }

        for (int i = 0; i < BL_Nodes.GetUpperBound(1); ++i)
        {
            BL_Nodes[BL_Nodes.GetUpperBound(0), i].UP_NODE = BL_Nodes[BL_Nodes.GetUpperBound(0), i + 1];
            BL_Nodes[BL_Nodes.GetUpperBound(0), i].Can_UP = true;
        }
        #endregion

        #region Setup [Block] Nodes and Neighbor Nodes' linkage in DN & LFT Direction
        for (int i = BL_Nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = BL_Nodes.GetUpperBound(1); j > 0; --j)
            {
                BL_Nodes[i, j].DN_NODE = BL_Nodes[i, j - 1];
                BL_Nodes[i, j].LFT_NODE = BL_Nodes[i - 1, j];
                BL_Nodes[i, j].Can_DN = true;
                BL_Nodes[i, j].Can_LFT = true;
            }
        }

        for (int i = BL_Nodes.GetUpperBound(0); i > 0; --i)
        {
            BL_Nodes[i, BL_Nodes.GetLowerBound(1)].LFT_NODE = BL_Nodes[i - 1, BL_Nodes.GetLowerBound(1)];
            BL_Nodes[i, BL_Nodes.GetLowerBound(1)].Can_LFT = true;
        }

        for (int i = BL_Nodes.GetUpperBound(1); i > 0; --i)
        {
            BL_Nodes[BL_Nodes.GetLowerBound(0), i].DN_NODE = BL_Nodes[BL_Nodes.GetLowerBound(0), i - 1];
            BL_Nodes[BL_Nodes.GetLowerBound(0), i].Can_DN = true;
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
                LI_Nodes[i, j].Position = lvData[lvIndex].LI_Nodes[colSize * i + j].Position;
                LI_Nodes[i, j].Node_Type = lvData[lvIndex].LI_Nodes[colSize * i + j].Node_Type;
                LI_Nodes[i, j].UP_NODE = null;
                LI_Nodes[i, j].DN_NODE = null;
                LI_Nodes[i, j].LFT_NODE = null;
                LI_Nodes[i, j].RGT_NODE = null;

                //* Check [Node] [Type] == [Line_Blue_Goal]
                if (LI_Nodes[i, j].Node_Type == Node_Type.Line_Blue_Goal)
                {
                    float rand = Random.Range(10f, 270f);
                    GameObject new_Gizmos_GO = Instantiate(Line_Blue_Goal, LI_Nodes[i, j].Position, Quaternion.Euler(new Vector3(0, 0, rand)), transform.Find("Symbols"));
                    LI_Nodes[i, j].Gizmos_GO = new_Gizmos_GO;
                    Blue_Sticker_Count++;
                }

                //* Check [Node] [Type] == [Line_Red_Goal]
                if (LI_Nodes[i, j].Node_Type == Node_Type.Line_Red_Goal)
                {
                    float rand = Random.Range(10f, 270f);
                    GameObject new_Gizmos_GO = Instantiate(Line_Red_Goal, LI_Nodes[i, j].Position, Quaternion.Euler(new Vector3(0, 0, rand)), transform.Find("Symbols"));
                    LI_Nodes[i, j].Gizmos_GO = new_Gizmos_GO;
                    Red_Sticker_Count++;
                }

                //* Check [Node] [Type] == [Line_Blue_Head] or [Line_Blue_Segment]
                if (LI_Nodes[i, j].Node_Type == Node_Type.Line_Blue_Head || LI_Nodes[i, j].Node_Type == Node_Type.Line_Blue_Segment)
                {
                    if (LI_Nodes[i, j].Node_Type == Node_Type.Line_Blue_Head)
                    {
                        Instantiate(Linel, transform.Find("Players"));
                    }

                    Line_Blue_Start_Nodes.Add(LI_Nodes[i, j]);
                }

                //* Check [Node] [Type] == [Line_Red_Head] or [Line_Red_Segment]
                if (LI_Nodes[i, j].Node_Type == Node_Type.Line_Red_Head || LI_Nodes[i, j].Node_Type == Node_Type.Line_Red_Segment)
                {
                    if (LI_Nodes[i, j].Node_Type == Node_Type.Line_Red_Head)
                    {
                        Instantiate(Careline, transform.Find("Players"));
                    }

                    Line_Red_Start_Nodes.Add(LI_Nodes[i, j]);
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
                LI_Nodes[i, j].Can_UP = true;
                LI_Nodes[i, j].Can_RGT = true;
            }
        }

        for (int i = 0; i < LI_Nodes.GetUpperBound(0); ++i)
        {
            LI_Nodes[i, LI_Nodes.GetUpperBound(1)].RGT_NODE = LI_Nodes[i + 1, LI_Nodes.GetUpperBound(1)];
            LI_Nodes[i, LI_Nodes.GetUpperBound(1)].Can_RGT = true;
        }

        for (int i = 0; i < LI_Nodes.GetUpperBound(1); ++i)
        {
            LI_Nodes[LI_Nodes.GetUpperBound(0), i].UP_NODE = LI_Nodes[LI_Nodes.GetUpperBound(0), i + 1];
            LI_Nodes[LI_Nodes.GetUpperBound(0), i].Can_UP = true;
        }
        #endregion

        #region Setup [Line] Nodes and Neighbor Nodes' linkage in DN & LFT Direction
        for (int i = LI_Nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = LI_Nodes.GetUpperBound(1); j > 0; --j)
            {
                LI_Nodes[i, j].DN_NODE = LI_Nodes[i, j - 1];
                LI_Nodes[i, j].LFT_NODE = LI_Nodes[i - 1, j];
                LI_Nodes[i, j].Can_DN = true;
                LI_Nodes[i, j].Can_LFT = true;
            }
        }

        for (int i = LI_Nodes.GetUpperBound(0); i > 0; --i)
        {
            LI_Nodes[i, LI_Nodes.GetLowerBound(1)].LFT_NODE = LI_Nodes[i - 1, LI_Nodes.GetLowerBound(1)];
            LI_Nodes[i, LI_Nodes.GetLowerBound(1)].Can_LFT = true;
        }

        for (int i = LI_Nodes.GetUpperBound(1); i > 0; --i)
        {
            LI_Nodes[LI_Nodes.GetLowerBound(0), i].DN_NODE = LI_Nodes[LI_Nodes.GetLowerBound(0), i - 1];
            LI_Nodes[LI_Nodes.GetLowerBound(0), i].Can_DN = true;
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
                new_edge_U.Edge_Type = lvData[lvIndex].LI_U_Edges[colSize * i + j].Edge_Type;
                new_edge_U.Boarder_Type = Boarder_Type.NONE;
                new_edge_U.Edge_Direction = lvData[lvIndex].LI_U_Edges[colSize * i + j].Edge_Direction;
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

                new_edge_U.Position = lvData[lvIndex].LI_U_Edges[colSize * i + j].Position;
                LI_U_Edges[i, j] = new_edge_U;
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
                new_edge_V.Edge_Type = lvData[lvIndex].LI_V_Edges[colSize * i + j].Edge_Type;
                new_edge_V.Boarder_Type = Boarder_Type.NONE;
                new_edge_V.Edge_Direction = lvData[lvIndex].LI_V_Edges[colSize * i + j].Edge_Direction;
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

                new_edge_V.Position = lvData[lvIndex].LI_V_Edges[colSize * i + j].Position;
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
                int colSize = BL_U_Edges.GetLength(1);

                Edge new_edge_U = new Edge();
                new_edge_U.UP_Node = LI_V_Edges[i, j].UP_Node;
                new_edge_U.DN_Node = LI_V_Edges[i, j].DN_Node;
                new_edge_U.LFT_Node = LI_V_Edges[i, j].LFT_Node;
                new_edge_U.RGT_Node = LI_V_Edges[i, j].RGT_Node;
                new_edge_U.Position = LI_V_Edges[i, j].Position;
                new_edge_U.Edge_Type = lvData[lvIndex].BL_U_Edges[colSize * i + j].Edge_Type;
                new_edge_U.Boarder_Type = Boarder_Type.NONE;
                new_edge_U.Edge_Direction = lvData[lvIndex].BL_U_Edges[colSize * i + j].Edge_Direction;
                BL_U_Edges[i, j] = new_edge_U;
            }
        }
        #endregion

        #region Setup [Block] - [U] - [Edge]'s neighbor [Edge]
        for (int i = 0; i < BL_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_U_Edges.GetLength(1); ++j)
            {
                if (i == 0)
                {
                    BL_U_Edges[i, j].UP_or_RGT_Edge = BL_U_Edges[i + 1, j];
                    BL_U_Edges[i, j].DN_or_LFT_Edge = null;
                }
                else if (i == BL_U_Edges.GetUpperBound(0))
                {
                    BL_U_Edges[i, j].UP_or_RGT_Edge = null;
                    BL_U_Edges[i, j].DN_or_LFT_Edge = BL_U_Edges[i - 1, j];
                }
                else
                {
                    BL_U_Edges[i, j].UP_or_RGT_Edge = BL_U_Edges[i + 1, j];
                    BL_U_Edges[i, j].DN_or_LFT_Edge = BL_U_Edges[i - 1, j];
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
                new_edge_V.Edge_Type = lvData[lvIndex].BL_V_Edges[colSize * i + j].Edge_Type;
                new_edge_V.Boarder_Type = Boarder_Type.NONE;
                new_edge_V.Edge_Direction = lvData[lvIndex].BL_V_Edges[colSize * i + j].Edge_Direction;
                BL_V_Edges[i, j] = new_edge_V;
            }
        }
        #endregion

        #region Setup [Block] - [V] - [Edge]'s neighbor [Edge]
        for (int i = 0; i < BL_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_V_Edges.GetLength(1); ++j)
            {
                if (j == 0)
                {
                    BL_V_Edges[i, j].UP_or_RGT_Edge = BL_V_Edges[i, j + 1];
                    BL_V_Edges[i, j].DN_or_LFT_Edge = null;
                }
                else if (j == BL_V_Edges.GetUpperBound(1))
                {
                    BL_V_Edges[i, j].UP_or_RGT_Edge = null;
                    BL_V_Edges[i, j].DN_or_LFT_Edge = BL_V_Edges[i, j - 1];
                }
                else
                {
                    BL_V_Edges[i, j].UP_or_RGT_Edge = BL_V_Edges[i, j + 1];
                    BL_V_Edges[i, j].DN_or_LFT_Edge = BL_V_Edges[i, j - 1];
                }
            }
        }
        #endregion

        //*! Setup [Line] - [Node]'s neighbor [Edge]
        #region Setup [Line] - [Node]'s neighbor [Edge] - Non-Boarder
        for (int i = 1; i < LI_Nodes.GetLength(0) - 1; ++i)
        {
            for (int j = 1; j < LI_Nodes.GetLength(1) - 1; ++j)
            {
                LI_Nodes[i, j].UP_EDGE = LI_V_Edges[i, j];
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

        //*! -------------------------------------------- !*//

        //*! Setup [Block] - [Node] corner [Node]
        #region Setup [Block] - [Node]'s corner [Nodes]
        for (int i = 0; i < bl_node_row; ++i)
        {
            for (int j = 0; j < bl_node_col; ++j)
            {
                BL_Nodes[i, j].UP_LFT_NODE = LI_Nodes[i, j + 1];
                BL_Nodes[i, j].UP_RGT_NODE = LI_Nodes[i + 1, j + 1];
                BL_Nodes[i, j].DN_LFT_NODE = LI_Nodes[i, j];
                BL_Nodes[i, j].DN_RGT_NODE = LI_Nodes[i + 1, j];
            }
        }
        #endregion

        //*! -------------------------------------------- !*//

        //*! Check [Edge] Enums flags to decide every [Node] tracersability to neighbor
        //*! Also instantiate [Obstacle GameObject] according to [Edge] - [Type]
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
                //*! Also instantiate [Black_Pen] GameObject
                #region Check if [Line] [U-Edge] is [Black_Pen]
                if (curEdge.Edge_Type == Edge_Type.Black_Pen)
                {
                    //*! Set traversability
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

                    //*! Instantiate GameObject
                    if (Black_Pen.transform.childCount <= 0)
                    {
                        GameObject new_Gizmos_GO = Instantiate(Black_Pen, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                        curEdge.Gizmos_GO = new_Gizmos_GO;
                    }
                    else
                    {
                        int rand = Random.Range(0, Black_Pen.transform.childCount - 1);
                        GameObject new_Gizmos_GO = Instantiate(Black_Pen.transform.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                        curEdge.Gizmos_GO = new_Gizmos_GO;
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
                //*! Also instantiate [Black_Pen] GameObject
                #region Check if [Line] [V-Edge] is [Black_Pen]
                if (curEdge.Edge_Type == Edge_Type.Black_Pen)
                {
                    //*! Set traversability
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

                    //*! Instantiate GameObject
                    if (Black_Pen.transform.childCount <= 0)
                    {
                        GameObject new_Gizmos_GO = Instantiate(Black_Pen, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                        curEdge.Gizmos_GO = new_Gizmos_GO;
                    }
                    else
                    {
                        int rand = Random.Range(0, Black_Pen.transform.childCount - 1);
                        GameObject new_Gizmos_GO = Instantiate(Black_Pen.transform.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                        curEdge.Gizmos_GO = new_Gizmos_GO;
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
                //*! Also instantiate [HighLighter_Red] GameObject
                #region Check if [Block] [U-Edge] is [HighLighter_Red]
                if (curEdge.Edge_Type == Edge_Type.HighLighter_Red)
                {
                    //*! Set traversability
                    #region Set traversability
                    curEdge.UP_Node.DN_NODE = null;
                    curEdge.DN_Node.UP_NODE = null;
                    #endregion

                    //*! Instantiate GameObject
                    #region Instantiate GameObject
                    Transform HighLighter_Red_Head = HighLighter_Red.transform.Find("Head");
                    Transform HighLighter_Red_Body = HighLighter_Red.transform.Find("Body");
                    Transform HighLighter_Red_Tail = HighLighter_Red.transform.Find("Tail");
                    Transform HighLighter_Red_Sing = HighLighter_Red.transform.Find("Single");

                    if (curEdge.UP_or_RGT_Edge == null || curEdge.DN_or_LFT_Edge == null)
                    {
                        if (curEdge.DN_or_LFT_Edge == null)
                        {
                            if (curEdge.UP_or_RGT_Edge.Edge_Type == Edge_Type.NONE)
                            {
                                int rand = Random.Range(0, HighLighter_Red_Sing.childCount - 1);
                                GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Sing.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                                curEdge.Gizmos_GO = new_Gizmos_GO;
                            }

                            if (curEdge.UP_or_RGT_Edge.Edge_Type == Edge_Type.HighLighter_Red)
                            {
                                int rand = Random.Range(0, HighLighter_Red_Head.childCount - 1);
                                GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Head.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                                curEdge.Gizmos_GO = new_Gizmos_GO;
                            }
                        }

                        if (curEdge.UP_or_RGT_Edge == null)
                        {
                            if (curEdge.DN_or_LFT_Edge.Edge_Type == Edge_Type.NONE)
                            {
                                int rand = Random.Range(0, HighLighter_Red_Sing.childCount - 1);
                                GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Sing.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                                curEdge.Gizmos_GO = new_Gizmos_GO;
                            }

                            if (curEdge.DN_or_LFT_Edge.Edge_Type == Edge_Type.HighLighter_Red)
                            {
                                int rand = Random.Range(0, HighLighter_Red_Tail.childCount - 1);
                                GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Tail.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                                curEdge.Gizmos_GO = new_Gizmos_GO;
                            }
                        }
                    }
                    else
                    {
                        if (curEdge.UP_or_RGT_Edge.Edge_Type == Edge_Type.NONE && curEdge.DN_or_LFT_Edge.Edge_Type == Edge_Type.NONE)
                        {
                            int rand = Random.Range(0, HighLighter_Red_Sing.childCount - 1);
                            GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Sing.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                            curEdge.Gizmos_GO = new_Gizmos_GO;
                        }

                        if (curEdge.UP_or_RGT_Edge.Edge_Type == Edge_Type.HighLighter_Red && curEdge.DN_or_LFT_Edge.Edge_Type == Edge_Type.NONE)
                        {
                            int rand = Random.Range(0, HighLighter_Red_Head.childCount - 1);
                            GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Head.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                            curEdge.Gizmos_GO = new_Gizmos_GO;
                        }

                        if (curEdge.UP_or_RGT_Edge.Edge_Type == Edge_Type.NONE && curEdge.DN_or_LFT_Edge.Edge_Type == Edge_Type.HighLighter_Red)
                        {
                            int rand = Random.Range(0, HighLighter_Red_Tail.childCount - 1);
                            GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Tail.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                            curEdge.Gizmos_GO = new_Gizmos_GO;
                        }

                        if (curEdge.UP_or_RGT_Edge.Edge_Type == Edge_Type.HighLighter_Red && curEdge.DN_or_LFT_Edge.Edge_Type == Edge_Type.HighLighter_Red)
                        {
                            int rand = Random.Range(0, HighLighter_Red_Body.childCount - 1);
                            GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Body.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                            curEdge.Gizmos_GO = new_Gizmos_GO;
                        }
                    }
                    #endregion
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
                //*! Also instantiate [HighLighter_Red] GameObject
                #region Check if [Block] [V-Edge] is [HighLighter_Red]
                if (curEdge.Edge_Type == Edge_Type.HighLighter_Red)
                {
                    //*! Set traversability
                    #region Set traversability
                    curEdge.LFT_Node.RGT_NODE = null;
                    curEdge.RGT_Node.LFT_NODE = null;
                    #endregion

                    //*! Instantiate GameObject
                    #region Instantiate GameObject
                    Transform HighLighter_Red_Head = HighLighter_Red.transform.Find("Head");
                    Transform HighLighter_Red_Body = HighLighter_Red.transform.Find("Body");
                    Transform HighLighter_Red_Tail = HighLighter_Red.transform.Find("Tail");
                    Transform HighLighter_Red_Sing = HighLighter_Red.transform.Find("Single");

                    if (curEdge.UP_or_RGT_Edge == null || curEdge.DN_or_LFT_Edge == null)
                    {
                        if (curEdge.DN_or_LFT_Edge == null)
                        {
                            if (curEdge.UP_or_RGT_Edge.Edge_Type == Edge_Type.NONE)
                            {
                                int rand = Random.Range(0, HighLighter_Red_Sing.childCount - 1);
                                GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Sing.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                                curEdge.Gizmos_GO = new_Gizmos_GO;
                            }

                            if (curEdge.UP_or_RGT_Edge.Edge_Type == Edge_Type.HighLighter_Red)
                            {
                                int rand = Random.Range(0, HighLighter_Red_Tail.childCount - 1);
                                GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Tail.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                                curEdge.Gizmos_GO = new_Gizmos_GO;
                            }
                        }

                        if (curEdge.UP_or_RGT_Edge == null)
                        {
                            if (curEdge.DN_or_LFT_Edge.Edge_Type == Edge_Type.NONE)
                            {
                                int rand = Random.Range(0, HighLighter_Red_Sing.childCount - 1);
                                GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Sing.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                                curEdge.Gizmos_GO = new_Gizmos_GO;
                            }

                            if (curEdge.DN_or_LFT_Edge.Edge_Type == Edge_Type.HighLighter_Red)
                            {
                                int rand = Random.Range(0, HighLighter_Red_Head.childCount - 1);
                                GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Head.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                                curEdge.Gizmos_GO = new_Gizmos_GO;
                            }
                        }
                    }
                    else
                    {
                        if (curEdge.UP_or_RGT_Edge.Edge_Type == Edge_Type.NONE && curEdge.DN_or_LFT_Edge.Edge_Type == Edge_Type.NONE)
                        {
                            int rand = Random.Range(0, HighLighter_Red_Sing.childCount - 1);
                            GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Sing.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                            curEdge.Gizmos_GO = new_Gizmos_GO;
                        }

                        if (curEdge.UP_or_RGT_Edge.Edge_Type == Edge_Type.NONE && curEdge.DN_or_LFT_Edge.Edge_Type == Edge_Type.HighLighter_Red)
                        {
                            int rand = Random.Range(0, HighLighter_Red_Head.childCount - 1);
                            GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Head.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                            curEdge.Gizmos_GO = new_Gizmos_GO;
                        }

                        if (curEdge.UP_or_RGT_Edge.Edge_Type == Edge_Type.HighLighter_Red && curEdge.DN_or_LFT_Edge.Edge_Type == Edge_Type.NONE)
                        {
                            int rand = Random.Range(0, HighLighter_Red_Tail.childCount - 1);
                            GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Tail.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                            curEdge.Gizmos_GO = new_Gizmos_GO;
                        }

                        if (curEdge.UP_or_RGT_Edge.Edge_Type == Edge_Type.HighLighter_Red && curEdge.DN_or_LFT_Edge.Edge_Type == Edge_Type.HighLighter_Red)
                        {
                            int rand = Random.Range(0, HighLighter_Red_Body.childCount - 1);
                            GameObject new_Gizmos_GO = Instantiate(HighLighter_Red_Body.GetChild(rand).gameObject, curEdge.Position, curEdge.Rotation, transform.Find("Symbols"));
                            curEdge.Gizmos_GO = new_Gizmos_GO;
                        }
                    }
                    #endregion
                }
                #endregion
            }
        }
        #endregion

        //*! -------------------------------------------- !*//

        //*! Setup [Camera], load in settings from [Lv_Data]
        #region Setup [Camera], load in settings from [Lv_Data]
        cam.transform.position = lvData[lvIndex].Cam.Position;
        cam.orthographicSize = lvData[lvIndex].Cam.Size;
        #endregion

        //*! Setup [Backgrounds]. Instantiate a [Background] object from array
        #region Setup [Backgrounds]. Instantiate a [Background] object from array
        if (Backgrounds.Length == lvData.Length)
        {
            Instantiate(Backgrounds[lvIndex], transform.Find("Backgrounds"));
        }
        #endregion

        //*! Sort [Line Start Nodes] list for [Line] player to grab
        #region Sort [Line Start Nodes] list for [Line] player to grab
        Init_Line();
        #endregion

        //*! Clean up [Node] [Edge] Type if they are relevent to [Block] or [Line] initial positions
        #region Clean up [Node] [Edge] Type if they are relevent to [Block] or [Line] initial positions
        Clean_Up_Node_Edge_Type();
        #endregion
    }

    [ContextMenu("Init_Line")]
    private void Init_Line()
    {
        //*! If no [Line] player in the scene, return
        if (Line_Blue_Start_Nodes.Count == 0 && Line_Red_Start_Nodes.Count == 0) return;

        //*! Sort [Line Blue Start Nodes] List
        #region Sort [Line Blue Start Nodes] List
        for (int i = 0; i < Line_Blue_Start_Nodes.Count; ++i)
        {
            if (Line_Blue_Start_Nodes[i].Node_Type == Node_Type.Line_Blue_Head)
            {
                Swap_Nodes(Line_Blue_Start_Nodes, i, 0);
            }
        }
        
        for (int i = 0; i < Line_Blue_Start_Nodes.Count - 1; ++i)
        {
            for (int j = i + 1; j < Line_Blue_Start_Nodes.Count; ++j)
            {
                Node currNode = Line_Blue_Start_Nodes[i];
                Node examNode = Line_Blue_Start_Nodes[j];

                if (currNode.UP_NODE == examNode && currNode.UP_EDGE.Edge_Type == Edge_Type.Line_Blue_Edge && examNode.DN_EDGE.Edge_Type == Edge_Type.Line_Blue_Edge)
                {
                    Swap_Nodes(Line_Blue_Start_Nodes, i + 1, j);
                }

                if (currNode.DN_NODE == examNode && currNode.DN_EDGE.Edge_Type == Edge_Type.Line_Blue_Edge && examNode.UP_EDGE.Edge_Type == Edge_Type.Line_Blue_Edge)
                {
                    Swap_Nodes(Line_Blue_Start_Nodes, i + 1, j);
                }

                if (currNode.LFT_NODE == examNode && currNode.LFT_EDGE.Edge_Type == Edge_Type.Line_Blue_Edge && examNode.RGT_EDGE.Edge_Type == Edge_Type.Line_Blue_Edge)
                {
                    Swap_Nodes(Line_Blue_Start_Nodes, i + 1, j);
                }

                if (currNode.RGT_NODE == examNode && currNode.RGT_EDGE.Edge_Type == Edge_Type.Line_Blue_Edge && examNode.LFT_EDGE.Edge_Type == Edge_Type.Line_Blue_Edge)
                {
                    Swap_Nodes(Line_Blue_Start_Nodes, i + 1, j);
                }
            }
        }
        #endregion

        //*! Sort [Line Red Start Nodes] List
        #region Sort [Line Red Start Nodes] List
        for (int i = 0; i < Line_Red_Start_Nodes.Count; ++i)
        {
            if (Line_Red_Start_Nodes[i].Node_Type == Node_Type.Line_Red_Head)
            {
                Swap_Nodes(Line_Red_Start_Nodes, i, 0);
            }
        }

        for (int i = 0; i < Line_Red_Start_Nodes.Count - 1; ++i)
        {
            for (int j = i + 1; j < Line_Red_Start_Nodes.Count; ++j)
            {
                Node currNode = Line_Red_Start_Nodes[i];
                Node examNode = Line_Red_Start_Nodes[j];

                if (currNode.UP_NODE == examNode && currNode.UP_EDGE.Edge_Type == Edge_Type.Line_Red_Edge && examNode.DN_EDGE.Edge_Type == Edge_Type.Line_Red_Edge)
                {
                    Swap_Nodes(Line_Red_Start_Nodes, i + 1, j);
                }

                if (currNode.DN_NODE == examNode && currNode.DN_EDGE.Edge_Type == Edge_Type.Line_Red_Edge && examNode.UP_EDGE.Edge_Type == Edge_Type.Line_Red_Edge)
                {
                    Swap_Nodes(Line_Red_Start_Nodes, i + 1, j);
                }

                if (currNode.LFT_NODE == examNode && currNode.LFT_EDGE.Edge_Type == Edge_Type.Line_Red_Edge && examNode.RGT_EDGE.Edge_Type == Edge_Type.Line_Red_Edge)
                {
                    Swap_Nodes(Line_Red_Start_Nodes, i + 1, j);
                }

                if (currNode.RGT_NODE == examNode && currNode.RGT_EDGE.Edge_Type == Edge_Type.Line_Red_Edge && examNode.LFT_EDGE.Edge_Type == Edge_Type.Line_Red_Edge)
                {
                    Swap_Nodes(Line_Red_Start_Nodes, i + 1, j);
                }
            }
        }
        #endregion
    }

    [ContextMenu("Clean_Up_Node_Edge_Type")]
    private void Clean_Up_Node_Edge_Type()
    {
        //*! Clean up [Block] - [Node] Type
        #region Clean up [Block] - [Node] Type
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                Node currNode = BL_Nodes[i,j];

                if (currNode.Node_Type == Node_Type.Block_Blue_Start ||
                    currNode.Node_Type == Node_Type.Block_Red_Start)
                {
                    currNode.Node_Type = Node_Type.NONE;
                }
            }
        }
        #endregion

        //*! Clean up [Line] - [Node] Type
        #region Clean up [Line] - [Node] Type
        for (int i = 0; i < LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_Nodes.GetLength(1); ++j)
            {
                Node currNode = LI_Nodes[i, j];

                if (currNode.Node_Type == Node_Type.Line_Blue_Head ||
                    currNode.Node_Type == Node_Type.Line_Blue_Segment ||
                    currNode.Node_Type == Node_Type.Line_Red_Head ||
                    currNode.Node_Type == Node_Type.Line_Red_Segment)
                {
                    currNode.Node_Type = Node_Type.NONE;
                }
            }
        }
        #endregion

        //*! Clean up [Line - U] - [Edge] Type
        #region Clean up [Line - U] - [Edge] Type
        for (int i = 0; i < LI_U_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_U_Edges.GetLength(1); ++j)
            {
                Edge currEdge = LI_U_Edges[i, j];

                if (currEdge.Edge_Type == Edge_Type.Line_Blue_Edge ||
                    currEdge.Edge_Type == Edge_Type.Line_Red_Edge)
                {
                    currEdge.Edge_Type = Edge_Type.NONE;
                }
            }
        }
        #endregion

        //*! Clean up [Line - V] - [Edge] Type
        #region Clean up [Line - V] - [Edge] Type
        for (int i = 0; i < LI_V_Edges.GetLength(0); ++i)
        {
            for (int j = 0; j < LI_V_Edges.GetLength(1); ++j)
            {
                Edge currEdge = LI_V_Edges[i, j];

                if (currEdge.Edge_Type == Edge_Type.Line_Blue_Edge ||
                    currEdge.Edge_Type == Edge_Type.Line_Red_Edge)
                {
                    currEdge.Edge_Type = Edge_Type.NONE;
                }
            }
        }
        #endregion
    }

    [ContextMenu("Clean_Up_Child_Objects")]
    private void Clean_Up_Child_Objects(string name)
    {
        bool is_List_Empty = (transform.Find(name).childCount < 1);

        if (!is_List_Empty)
        {
            foreach (Transform child in transform.Find(name))
            {
                Destroy(child.gameObject);
            }

            //Debug.Log("Cleaned Up " + name);
        }
    }

    [ContextMenu("Clear_Pencil_Case_Gizmos")]
    private void Clear_Pencil_Case_Gizmos()
    {
        //*! If there is a [Pencil_Case_Handle] in the scene,
        //*! clean up gizmos by setting the mesh renderer to enable = false
        if (is_pencil_case)
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

            is_pencil_case = false;

            Debug.Log("Clear Pencil Case Gizmos in Play Mode!");
        }
    }

    [ContextMenu("Render_Node_Traversability_Gizmos")]
    private void Render_Node_Traversability_Gizmos()
    {
        if (!Show_Gizmos) return;

        #region Setup gizmos' spacing based on handle size
        float gizmos_spacing = Gizmos_Size * 0.1f;
        #endregion

        #region Setup Gizmos for [Block] Nodes and indication arrows
        for (int i = 0; i < BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < BL_Nodes.GetLength(1); ++j)
            {
                if (BL_Nodes[i, j].UP_NODE != null && BL_Nodes[i, j].Can_UP && !BL_Nodes[i, j].UP_NODE.Is_Occupied)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0)) *
                                              Matrix4x4.Scale(new Vector3(Gizmos_Size, Gizmos_Size, Gizmos_Size));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, bl_giz_mat, 0);
                }

                if (BL_Nodes[i, j].DN_NODE != null && BL_Nodes[i, j].Can_DN && !BL_Nodes[i, j].DN_NODE.Is_Occupied)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0)) *
                                              Matrix4x4.Scale(new Vector3(Gizmos_Size, Gizmos_Size, Gizmos_Size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(180f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, bl_giz_mat, 0);
                }

                if (BL_Nodes[i, j].LFT_NODE != null && BL_Nodes[i, j].Can_LFT && !BL_Nodes[i, j].LFT_NODE.Is_Occupied)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0)) *
                                              Matrix4x4.Scale(new Vector3(Gizmos_Size, Gizmos_Size, Gizmos_Size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(90f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, bl_giz_mat, 0);
                }

                if (BL_Nodes[i, j].RGT_NODE != null && BL_Nodes[i, j].Can_RGT && !BL_Nodes[i, j].RGT_NODE.Is_Occupied)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(BL_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0)) *
                                           Matrix4x4.Scale(new Vector3(Gizmos_Size, Gizmos_Size, Gizmos_Size)) *
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
                if (LI_Nodes[i, j].UP_NODE != null && LI_Nodes[i, j].Can_UP && !LI_Nodes[i, j].UP_NODE.Is_Occupied)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0)) *
                                              Matrix4x4.Scale(new Vector3(Gizmos_Size, Gizmos_Size, Gizmos_Size));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, li_giz_mat, 0);
                }

                if (LI_Nodes[i, j].DN_NODE != null && LI_Nodes[i, j].Can_DN && !LI_Nodes[i, j].DN_NODE.Is_Occupied)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0)) *
                                              Matrix4x4.Scale(new Vector3(Gizmos_Size, Gizmos_Size, Gizmos_Size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(180f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, li_giz_mat, 0);
                }

                if (LI_Nodes[i, j].LFT_NODE != null && LI_Nodes[i, j].Can_LFT && !LI_Nodes[i, j].LFT_NODE.Is_Occupied)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0)) *
                                              Matrix4x4.Scale(new Vector3(Gizmos_Size, Gizmos_Size, Gizmos_Size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(90f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, li_giz_mat, 0);
                }

                if (LI_Nodes[i, j].RGT_NODE != null && LI_Nodes[i, j].Can_RGT && !LI_Nodes[i, j].RGT_NODE.Is_Occupied)
                {
                    Matrix4x4 arrHandleMatx = Matrix4x4.Translate(LI_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0)) *
                                              Matrix4x4.Scale(new Vector3(Gizmos_Size, Gizmos_Size, Gizmos_Size)) *
                                              Matrix4x4.Rotate(Quaternion.AngleAxis(270f, Vector3.forward));

                    Graphics.DrawMesh(arrw_giz_mesh, arrHandleMatx, li_giz_mat, 0);
                }
            }
        }
        #endregion
    }

    [ContextMenu("Check_Shortcut_Input")]
    private void Check_Shortcut_Input()
    {
        //*! Press [Backspace] to go to [Level Selection] screen
        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            lvlav.Load_Available_Levels();
            Initialize_Level(1);
        }

        //*! Press [X] to skip current level
        if (lvDataIndex == 2 || lvDataIndex == 9 || lvDataIndex == 16 || lvDataIndex == lvData.Length - 1 || lvDataIndex == lvData.Length - 2)
        {
            if (Input.GetKeyUp(KeyCode.X) && lvDataIndex != lvData.Length - 1)
            {
                Initialize_Level(lvDataIndex + 1);
            }
            else if (Input.GetKeyUp(KeyCode.X) && lvDataIndex == lvData.Length - 1)
            {
                Initialize_Level(0);
            }
        }

        //*! Press [Keypad7] go to previous level
        if (Input.GetKeyUp(KeyCode.Keypad7))
        {
            Initialize_Level(lvDataIndex - 1);
        }

        //*! Press [Keypad8] restart current level
        if (Input.GetKeyUp(KeyCode.Keypad8))
        {
            Initialize_Level(lvDataIndex);
        }

        //*! Press [Keypad9] restart current level
        if (Input.GetKeyUp(KeyCode.Keypad9))
        {
            Initialize_Level(lvDataIndex + 1);
        }

        //*! Press [Keypad5] to show/hide gizmos
        if (Input.GetKeyUp(KeyCode.Keypad5))
        {
            switch (Show_Gizmos)
            {
                case true:
                    {
                        Show_Gizmos = false;
                        break;
                    }
                case false:
                    {
                        Show_Gizmos = true;
                        break;
                    }
            }
        }

        //*! Press [ESC] to quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    [ContextMenu("Swap_Nodes")]
    private void Swap_Nodes(List<Node> nodes, int indexA, int indexB)
    {
        Node tmp_node;
        tmp_node = nodes[indexA];
        nodes[indexA] = nodes[indexB];
        nodes[indexB] = tmp_node;
    }


    //*!----------------------------!*//
    //*!    Public Functions
    //*!----------------------------!*//

    //*! Initialize the level data and set level data index to current playing level index
    [ContextMenu("Initialize_Level")]
    public void Initialize_Level(int LvIndex)
    {
        lvDataIndex = LvIndex - 1;

        //*! Safe check if the next level index to load is out of bound
        //*! If next index is out of bound, display warning message and load the last level in array instead
        if (lvDataIndex >= lvData.GetUpperBound(0))
        {
            Debug.Log("Level Index Array is out of bound.\n Load the last level in the array instead.");
            lvDataIndex = lvData.GetUpperBound(0) - 1;
        }

        if (lvDataIndex < -1)
        {
            Debug.Log("Level Index Array is out of bound.\n Load the first level in the array instead.");
            lvDataIndex = -1;
        }

        if (FindObjectOfType<Pencil_Case>() != null) is_pencil_case = true;
        cam = FindObjectOfType<Camera>();
        snd = FindObjectOfType<Sound_Manager>();
        lvlav = FindObjectOfType<Level_Availability>();

        Blue_Sticker_Count = 0;
        Red_Sticker_Count = 0;

        Clean_Up_Child_Objects("Symbols");
        Clean_Up_Child_Objects("Players");
        Clean_Up_Child_Objects("Backgrounds");

        Setup_Level_Data(lvDataIndex + 1);

        lvDataIndex++;
    }

    [ContextMenu("Save_Players_Data")]
    public void Save_Players_Data()
    {
        #region Get Scene Player instances and Call Save()
        if (lvDataIndex == 0) return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject go in players)
        {
            if (go.GetComponent<Block_Control>() &&
                go.GetComponent<Block_Control>().playerType == Player_Type.RED)
            {
                go.GetComponent<Block_Control>().save_data.Completed_Level(lvDataIndex);
                go.GetComponent<Block_Control>().save_data.Save();
                Debug.Log("Player One [Block - Red] Data Saved!");
            }

            if (go.GetComponent<Line_Control>() &&
                go.GetComponent<Line_Control>().playerType == Player_Type.RED)
            {
                go.GetComponent<Line_Control>().save_data.Completed_Level(lvDataIndex);
                go.GetComponent<Line_Control>().save_data.Save();
                Debug.Log("Player Two [Line - Red] Data Saved!");
            }

            if (go.GetComponent<Block_Control>() &&
                go.GetComponent<Block_Control>().playerType == Player_Type.BLUE)
            {
                go.GetComponent<Block_Control>().save_data.Completed_Level(lvDataIndex);
                go.GetComponent<Block_Control>().save_data.Save();
                Debug.Log("Player One [Block - Blue] Data Saved!");
            }

            if (go.GetComponent<Line_Control>() &&
                go.GetComponent<Line_Control>().playerType == Player_Type.BLUE)
            {
                go.GetComponent<Line_Control>().save_data.Completed_Level(lvDataIndex);
                go.GetComponent<Line_Control>().save_data.Save();
                Debug.Log("Player Two [Line - Blue] Data Saved!");
            }
        }
        #endregion
    }
}


//*!----------------------------!*//
//*!       Custom Classes
//*!----------------------------!*//

#region [Node] classes
//*! Classes for map elements [Node] and [Edge] 
public class Node
{
    //*! [Node] Position reference holder
    public Vector3 Position;

    //*! Getter, Setter of [Node] Traversability boolean properties of 4 neighbor [Node]
    //*! Return value according to if neighbor [Node] null && occupied
    //public bool Can_UP { get { return UP_NODE != null; } }
    //public bool Can_DN { get { return DN_NODE != null; } }
    //public bool Can_LFT { get { return LFT_NODE != null; } }
    //public bool Can_RGT { get { return RGT_NODE != null; } }

    public bool Can_UP;
    public bool Can_DN;
    public bool Can_LFT;
    public bool Can_RGT;

    //*! Neighbor [Node] reference holder
    public Node UP_NODE;
    public Node DN_NODE;
    public Node LFT_NODE;
    public Node RGT_NODE;

    //*! Corner [Node] reference holder
    public Node UP_LFT_NODE;
    public Node UP_RGT_NODE;
    public Node DN_LFT_NODE;
    public Node DN_RGT_NODE;

    //*! Connected [Edge] reference holder
    public Edge UP_EDGE;
    public Edge DN_EDGE;
    public Edge LFT_EDGE;
    public Edge RGT_EDGE;

    //*! Temp [Node] for storing current [Node] reference
    //public Node Curr_Node;

    //*! [Node] [Gizmos] reference holder
    public GameObject Gizmos_GO;

    //*! [Node] [Type] reference holder
    public Node_Type Node_Type;

    //*! [Node] is occupied boolean
    public bool Is_Occupied;

    //* Function to set neighbor [Node] traversability
    public void Set_Traversability(bool Can_Traverse)
    {
        #region Setup current [Node] references
        //Curr_Node = new Node();

        //Curr_Node = this;
        #endregion

        //*! Switch traversability according to [Can_Traverse] argument
        #region Switch traversability according to [Can_Traverse] argument
        if (!Can_Traverse)
        {
            //if (Can_UP) { UP_NODE.DN_NODE = null; }

            //if (Can_DN) { DN_NODE.UP_NODE = null; }

            //if (Can_LFT) { LFT_NODE.RGT_NODE = null; }

            //if (Can_RGT) { RGT_NODE.LFT_NODE = null; }

            if (UP_NODE != null) { UP_NODE.Can_DN = false; }

            if (DN_NODE != null) { DN_NODE.Can_UP = false; }

            if (LFT_NODE != null) { LFT_NODE.Can_RGT = false; }

            if (RGT_NODE != null) { RGT_NODE.Can_LFT = false; }

            Is_Occupied = true;
        }
        else
        {
            //if (Curr_Node.UP_NODE != null) { UP_NODE.DN_NODE = Curr_Node; }

            //if (Curr_Node.DN_NODE != null) { DN_NODE.UP_NODE = Curr_Node; }

            //if (Curr_Node.LFT_NODE != null) { LFT_NODE.RGT_NODE = Curr_Node; }

            //if (Curr_Node.RGT_NODE != null) { RGT_NODE.LFT_NODE = Curr_Node; }

            if (UP_NODE != null) { UP_NODE.Can_DN = true; }

            if (DN_NODE != null) { DN_NODE.Can_UP = true; }

            if (LFT_NODE != null) { LFT_NODE.Can_RGT = true; }

            if (RGT_NODE != null) { RGT_NODE.Can_LFT = true; }

            Is_Occupied = false;
        }

        #endregion
    }
}
#endregion

#region [Edge] classes
public class Edge
{
    //*! [Edge] neighbor [Node] reference holder
    public Node UP_Node;
    public Node DN_Node;
    public Node LFT_Node;
    public Node RGT_Node;

    //*! Temp [Edge] references for neighbor [Node] previous status
    //public Node Prev_UPsDN_Node;
    //public Node Prev_DNsUP_Node;
    //public Node Prev_LFTsRGT_Node;
    //public Node Prev_RGTsLFT_Node;

    ////*! [Edge] reference that corsses with current [Edge]
    //public Edge Cross_Edge;
    public Edge UP_or_RGT_Edge;
    public Edge DN_or_LFT_Edge;

    //*! [Edge] Position reference holder
    public Vector3 Position;
    public Quaternion Rotation
    {
        get
        {
            switch (Edge_Direction)
            {
                case Edge_Direction.Horizontal:
                    return Quaternion.AngleAxis(90, Vector3.forward);
                case Edge_Direction.Vertical:
                    return Quaternion.AngleAxis(0, Vector3.forward);
                default:
                    return Quaternion.AngleAxis(90, Vector3.forward);
            }
        }
    }

    //*! [Edge] [Gizmos] reference holder
    public GameObject Gizmos_GO;

    //*! [Edge] [Edge Type], [Boarder Type] and [Edge Direction] reference holder
    public Edge_Type Edge_Type;
    public Boarder_Type Boarder_Type;
    public Edge_Direction Edge_Direction;

    //* Function to set neighbor [Node] traversability
    public void Set_Traversability(bool Can_Traverse)
    {
        //*! Setup temp [Node] references for 4 directions
        #region Setup temp [Node] references for 4 directions
        //if (UP_Node != null && UP_Node.Can_DN) { Prev_UPsDN_Node = new Node(); Prev_UPsDN_Node = UP_Node.DN_NODE; }

        //if (DN_Node != null && DN_Node.Can_UP) { Prev_DNsUP_Node = new Node(); Prev_DNsUP_Node = DN_Node.UP_NODE; }

        //if (LFT_Node != null && LFT_Node.Can_RGT) { Prev_LFTsRGT_Node = new Node(); Prev_LFTsRGT_Node = LFT_Node.RGT_NODE; }

        //if (RGT_Node != null && RGT_Node.Can_LFT) { Prev_RGTsLFT_Node = new Node(); Prev_RGTsLFT_Node = RGT_Node.LFT_NODE; }
        #endregion

        //*! Switch traversability according to [Can_Traverse] argument
        #region Switch traversability according to [Can_Traverse] argument
        if (!Can_Traverse)
        {
            if (Edge_Direction == Edge_Direction.Horizontal && Edge_Type == Edge_Type.NONE)
            {
                //if (UP_Node != null && UP_Node.Can_DN) { UP_Node.DN_NODE = null; }

                //if (DN_Node != null && DN_Node.Can_UP) { DN_Node.UP_NODE = null; }

                if (UP_Node != null) { UP_Node.Can_DN = false; }

                if (DN_Node != null) { DN_Node.Can_UP = false; }
            }

            if (Edge_Direction == Edge_Direction.Vertical && Edge_Type == Edge_Type.NONE)
            {
                //if (LFT_Node != null && LFT_Node.Can_RGT) { LFT_Node.RGT_NODE = null; }

                //if (RGT_Node != null && RGT_Node.Can_LFT) { RGT_Node.LFT_NODE = null; }

                if (LFT_Node != null) { LFT_Node.Can_RGT = false; }
                
                if (RGT_Node != null) { RGT_Node.Can_LFT = false; }
            }
        }
        else
        {
            if (Edge_Direction == Edge_Direction.Horizontal && Edge_Type == Edge_Type.NONE)
            {
                //if (Prev_UPsDN_Node != null) Prev_UPsDN_Node.Is_Occupied = false;
                //if (UP_Node != null) { UP_Node.DN_NODE = Prev_UPsDN_Node; }

                //if (Prev_DNsUP_Node != null) Prev_DNsUP_Node.Is_Occupied = false;
                //if (DN_Node != null) { DN_Node.UP_NODE = Prev_DNsUP_Node; }

                if (UP_Node != null) { UP_Node.Can_DN = true; }

                if (DN_Node != null) { DN_Node.Can_UP = true; }
            }

            if (Edge_Direction == Edge_Direction.Vertical && Edge_Type == Edge_Type.NONE)
            {
                //if (Prev_LFTsRGT_Node != null) Prev_LFTsRGT_Node.Is_Occupied = false;
                //if (LFT_Node != null) {  LFT_Node.RGT_NODE = Prev_LFTsRGT_Node; }

                //if (Prev_RGTsLFT_Node != null) Prev_RGTsLFT_Node.Is_Occupied = false;
                //if (RGT_Node != null) {  RGT_Node.LFT_NODE = Prev_RGTsLFT_Node; }

                if (LFT_Node != null) { LFT_Node.Can_RGT = true; }

                if (RGT_Node != null) { RGT_Node.Can_LFT = true; }
            }
        }
        #endregion
    }
}
#endregion


//*!----------------------------!*//
//*!    Custom Enum Classes
//*!----------------------------!*//

#region [Edge_Type] Enum class
public enum Edge_Type
{
    NONE = 0,
    Black_Pen = 1,
    HighLighter_Red = 2,
    HighLighter_Blue = 3,
    Pencil = 4,
    Colour_Pencil = 5,
    Boarder = 6,
    Line_Blue_Edge = 7,
    Line_Red_Edge = 8
}
#endregion

#region [Boarder_Type] Enum class
public enum Boarder_Type
{
    NONE,
    UP,
    DN,
    LFT,
    RGT
}
#endregion

#region [Edge_Direction] Enum class
public enum Edge_Direction
{
    Horizontal = 0,
    Vertical = 1
}
#endregion

#region [Node_Type] Enum class
public enum Node_Type
{
    NONE = 0,
    Block_Blue_Goal = 1,
    Block_Red_Goal = 2,
    Line_Blue_Goal = 3,
    Line_Red_Goal = 4,
    Block_Blue_Start = 5,
    Block_Red_Start = 6,
    Line_Blue_Head = 7,
    Line_Blue_Segment = 8,
    Line_Red_Head = 9,
    Line_Red_Segment = 10
}
#endregion

#region [Move_Graph] Enum class
public enum Move_Graph
{
    NONE = 0,
    UP = 1,
    DN = 2,
    LFT = 3,
    RGT = 4
}
#endregion
