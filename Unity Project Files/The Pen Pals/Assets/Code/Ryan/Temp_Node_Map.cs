//*!----------------------------!*//
//*! Programmer: Ryan Chung
//*!----------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_Node_Map : MonoBehaviour
{
    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//

    [Range(1, 500)]
    public int row;
    [Range(1, 500)]
    public int col;
    [Range(0.1f, 1.0f)]
    public float handle_size;
    public Node[,] BL_Nodes { get { return bl_nodes; } set { bl_nodes = value; } }
    public Node[,] LI_Nodes { get { return li_nodes; } set { li_nodes = value; } }


    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//

    /*- Note: The reason to use 2D array for nodes is it is easier to find 
              connections between neighbor nodes than a list. As for edges
              only has two node refs, therefore using a list. -*/
    [HideInInspector]
    private Node[,] bl_nodes;
    [HideInInspector]
    private List<Edge> bl_edges;
    [HideInInspector]
    private Node[,] li_nodes;
    [HideInInspector]
    private List<Edge> li_edges;

    // Use this for initialization
    void Awake ()
    {
        Initialize_Graph();
        //Debug_Graph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Initialize the graphs for Block and Line entities
    private void Initialize_Graph()
    {
        bl_nodes = new Node[row - 1, col - 1];
        bl_edges = new List<Edge>();
        li_nodes = new Node[row, col];
        li_edges = new List<Edge>();

        //*! ------------------ Old method WITHOUT neighbor nodes ------------------ !*//

        //*! Setup Block nodes by the number of row and col (currently disabled)
        #region Setup [Block] Nodes and limit the boarders' Traversability
        //for (int i = 0; i < bl_nodes.GetLength(0); ++i)
        //{
        //    for (int j = 0; j < bl_nodes.GetLength(1); ++j)
        //    {
        //        bl_nodes[i, j] = new Node();
        //        bl_nodes[i, j].Position = new Vector3(i + 0.5f, j + 0.5f);
        //        bl_nodes[i, j].Node_Size = handle_size;

        //        bl_nodes[i, j].Can_UP = true;
        //        bl_nodes[i, j].Can_DN = true;
        //        bl_nodes[i, j].Can_LFT = true;
        //        bl_nodes[i, j].Can_RGT = true;

        //        if (i == bl_nodes.GetLowerBound(0))
        //        {
        //            bl_nodes[i, j].Can_LFT = false;
        //        }

        //        if (i == bl_nodes.GetUpperBound(0))
        //        {
        //            bl_nodes[i, j].Can_RGT = false;
        //        }

        //        if (j == bl_nodes.GetLowerBound(1))
        //        {
        //            bl_nodes[i, j].Can_DN = false;
        //        }

        //        if (j == bl_nodes.GetUpperBound(1))
        //        {
        //            bl_nodes[i, j].Can_UP = false;
        //        }
        //    }
        //}
        #endregion

        //*! Setup Line nodes by the number of row and col (currently disabled)
        #region Setup [Line] Nodes and limit the boarders' Traversability
        //for (int i = 0; i < li_nodes.GetLength(0); ++i)
        //{
        //    for (int j = 0; j < li_nodes.GetLength(1); ++j)
        //    {
        //        li_nodes[i, j] = new Node();
        //        li_nodes[i, j].Position = new Vector3(i, j);
        //        li_nodes[i, j].Node_Size = handle_size;
        //        li_nodes[i, j].Can_UP = true;
        //        li_nodes[i, j].Can_DN = true;
        //        li_nodes[i, j].Can_LFT = true;
        //        li_nodes[i, j].Can_RGT = true;

        //        if (i == li_nodes.GetLowerBound(0))
        //        {
        //            li_nodes[i, j].Can_LFT = false;
        //        }

        //        if (i == li_nodes.GetUpperBound(0))
        //        {
        //            li_nodes[i, j].Can_RGT = false;
        //        }

        //        if (j == li_nodes.GetLowerBound(1))
        //        {
        //            li_nodes[i, j].Can_DN = false;
        //        }

        //        if (j == li_nodes.GetUpperBound(1))
        //        {
        //            li_nodes[i, j].Can_UP = false;
        //        }
        //    }
        //}
        #endregion

        //*! ------------------ New method WITH neighbor nodes ------------------ !*//

        //*! Setup [Block] and [Line] nodes by the number of row and col
        #region Setup [Block] Nodes and Preset Neighbor Nodes to Null
        for (int i = 0; i < bl_nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < bl_nodes.GetLength(1); ++j)
            {
                Node new_node = new Node();
                bl_nodes[i, j] = new_node;
                bl_nodes[i, j].Position = new Vector3(i + 0.5f, j + 0.5f);
                bl_nodes[i, j].Node_Size = handle_size;
                bl_nodes[i, j].UP_NODE = null;
                bl_nodes[i, j].DN_NODE = null;
                bl_nodes[i, j].LFT_NODE = null;
                bl_nodes[i, j].RGT_NODE = null;
            }
        }
        #endregion

        #region Setup [Block] Nodes and Neighbor Nodes' linkage in UP & RGT Direction
        for (int i = 0; i < bl_nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < bl_nodes.GetUpperBound(1); ++j)
            {
                bl_nodes[i, j].UP_NODE = bl_nodes[i, j + 1];
                bl_nodes[i, j].RGT_NODE = bl_nodes[i + 1, j];
            }
        }

        for (int i = 0; i < bl_nodes.GetUpperBound(0); ++i)
        {
            bl_nodes[i, bl_nodes.GetUpperBound(1)].RGT_NODE = bl_nodes[i + 1, bl_nodes.GetUpperBound(1)];
        }

        for (int i = 0; i < bl_nodes.GetUpperBound(1); ++i)
        {
            bl_nodes[bl_nodes.GetUpperBound(0), i].UP_NODE = bl_nodes[bl_nodes.GetUpperBound(0), i + 1];
        }
        #endregion

        #region Setup [Block] Nodes and Neighbor Nodes' linkage in DN & LFT Direction
        for (int i = bl_nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = bl_nodes.GetUpperBound(1); j > 0; --j)
            {
                bl_nodes[i, j].DN_NODE = bl_nodes[i, j - 1];
                bl_nodes[i, j].LFT_NODE = bl_nodes[i - 1, j];
            }
        }

        for (int i = bl_nodes.GetUpperBound(0); i > 0; --i)
        {
            bl_nodes[i, bl_nodes.GetLowerBound(1)].LFT_NODE = bl_nodes[i - 1, bl_nodes.GetLowerBound(1)];
        }

        for (int i = bl_nodes.GetUpperBound(1); i > 0; --i)
        {
            bl_nodes[bl_nodes.GetLowerBound(0), i].DN_NODE = bl_nodes[bl_nodes.GetLowerBound(0), i - 1];
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

        //*! Iterate from the buttom left node of Block 2D array to check UP & RIGHT direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Block] Edges in UP & RGT Direction
        for (int i = 0; i < bl_nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < bl_nodes.GetUpperBound(1); ++j)
            {
                if (bl_nodes[i, j].Can_UP)
                {
                    Edge new_edge = new Edge
                    {
                        Start_Node = bl_nodes[i, j],
                        End_Node = bl_nodes[i, j + 1]
                    };
                    bl_edges.Add(new_edge);
                }

                if (bl_nodes[i, j].Can_RGT)
                {
                    Edge new_edge = new Edge
                    {
                        Start_Node = bl_nodes[i, j],
                        End_Node = bl_nodes[i + 1, j]
                    };
                    bl_edges.Add(new_edge);
                }
            }
        }

        for (int i = 0; i < bl_nodes.GetUpperBound(0); ++i)
        {
            if (bl_nodes[i, bl_nodes.GetUpperBound(1)].Can_RGT)
            {
                Edge new_edge = new Edge
                {
                    Start_Node = bl_nodes[i, bl_nodes.GetUpperBound(1)],
                    End_Node = bl_nodes[i + 1, bl_nodes.GetUpperBound(1)]
                };
                bl_edges.Add(new_edge);
            }
        }

        for (int i = 0; i < bl_nodes.GetUpperBound(1); ++i)
        {
            if (bl_nodes[bl_nodes.GetUpperBound(0), i].Can_UP)
            {
                Edge new_edge = new Edge
                {
                    Start_Node = bl_nodes[bl_nodes.GetUpperBound(0), i],
                    End_Node = bl_nodes[bl_nodes.GetUpperBound(0), i + 1]
                };
                bl_edges.Add(new_edge);
            }
        }
        #endregion

        #region Setup [Block] Edges in DN & LFT Direction
        for (int i = bl_nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = bl_nodes.GetUpperBound(1); j > 0; --j)
            {
                if (bl_nodes[i, j].Can_DN)
                {
                    Edge new_edge = new Edge
                    {
                        Start_Node = bl_nodes[i, j],
                        End_Node = bl_nodes[i, j - 1]
                    };
                    bl_edges.Add(new_edge);
                }

                if (bl_nodes[i, j].Can_LFT)
                {
                    Edge new_edge = new Edge
                    {
                        Start_Node = bl_nodes[i, j],
                        End_Node = bl_nodes[i - 1, j]
                    };
                    bl_edges.Add(new_edge);
                }
            }
        }

        for (int i = bl_nodes.GetUpperBound(0); i > 0 ; --i)
        {
            if (bl_nodes[i, bl_nodes.GetLowerBound(1)].Can_LFT)
            {
                Edge new_edge = new Edge
                {
                    Start_Node = bl_nodes[i, bl_nodes.GetLowerBound(1)],
                    End_Node = bl_nodes[i - 1, bl_nodes.GetLowerBound(1)]
                };
                bl_edges.Add(new_edge);
            }
        }

        for (int i = bl_nodes.GetUpperBound(1); i > 0; --i)
        {
            if (bl_nodes[bl_nodes.GetLowerBound(0), i].Can_DN)
            {
                Edge new_edge = new Edge
                {
                    Start_Node = bl_nodes[bl_nodes.GetLowerBound(0), i],
                    End_Node = bl_nodes[bl_nodes.GetLowerBound(0), i - 1]
                };
                bl_edges.Add(new_edge);
            }
        }
        #endregion

        #region Setup [Line] Edges in UP & Right Direction
        for (int i = 0; i < li_nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < li_nodes.GetUpperBound(1); ++j)
            {
                if (li_nodes[i, j].Can_UP)
                {
                    Edge new_edge = new Edge
                    {
                        Start_Node = li_nodes[i, j],
                        End_Node = li_nodes[i, j + 1]
                    };
                    li_edges.Add(new_edge);
                }

                if (li_nodes[i, j].Can_RGT)
                {
                    Edge new_edge = new Edge
                    {
                        Start_Node = li_nodes[i, j],
                        End_Node = li_nodes[i + 1, j]
                    };
                    li_edges.Add(new_edge);
                }
            }
        }

        for (int i = 0; i < li_nodes.GetUpperBound(0); ++i)
        {
            if (li_nodes[i, li_nodes.GetUpperBound(1)].Can_RGT)
            {
                Edge new_edge = new Edge
                {
                    Start_Node = li_nodes[i, li_nodes.GetUpperBound(1)],
                    End_Node = li_nodes[i + 1, li_nodes.GetUpperBound(1)]
                };
                li_edges.Add(new_edge);
            }
        }

        for (int i = 0; i < li_nodes.GetUpperBound(1); ++i)
        {
            if (li_nodes[li_nodes.GetUpperBound(0), i].Can_UP)
            {
                Edge new_edge = new Edge
                {
                    Start_Node = li_nodes[li_nodes.GetUpperBound(0), i],
                    End_Node = li_nodes[li_nodes.GetUpperBound(0), i + 1]
                };
                li_edges.Add(new_edge);
            }
        }
        #endregion

        #region Setup [Line] Edges in DN & LFT Direction
        for (int i = li_nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = li_nodes.GetUpperBound(1); j > 0; --j)
            {
                if (li_nodes[i, j].Can_DN)
                {
                    Edge new_edge = new Edge
                    {
                        Start_Node = li_nodes[i, j],
                        End_Node = li_nodes[i, j - 1]
                    };
                    li_edges.Add(new_edge);
                }

                if (li_nodes[i, j].Can_LFT)
                {
                    Edge new_edge = new Edge
                    {
                        Start_Node = li_nodes[i, j],
                        End_Node = li_nodes[i - 1, j]
                    };
                    li_edges.Add(new_edge);
                }
            }
        }

        for (int i = li_nodes.GetUpperBound(0); i > 0; --i)
        {
            if (li_nodes[i, li_nodes.GetLowerBound(1)].Can_LFT)
            {
                Edge new_edge = new Edge
                {
                    Start_Node = li_nodes[i, li_nodes.GetLowerBound(1)],
                    End_Node = li_nodes[i - 1, li_nodes.GetLowerBound(1)]
                };
                li_edges.Add(new_edge);
            }
        }

        for (int i = li_nodes.GetUpperBound(1); i > 0; --i)
        {
            if (li_nodes[li_nodes.GetLowerBound(0), i].Can_DN)
            {
                Edge new_edge = new Edge
                {
                    Start_Node = li_nodes[li_nodes.GetLowerBound(0), i],
                    End_Node = li_nodes[li_nodes.GetLowerBound(0), i - 1]
                };
                li_edges.Add(new_edge);
            }
        }
        #endregion
    }

    //*! Debug the graph. Display all member values of nodes'
    private void Debug_Graph()
    {
        Debug.Log("------ Start [Block] Node Array Info ------");
        Debug.Log("Block Row Number: " + bl_nodes.GetLength(0));
        Debug.Log("Block Col Number: " + bl_nodes.GetLength(1));

        for (int i = 0; i < bl_nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < bl_nodes.GetLength(1); ++j)
            {
                Debug.Log("Block Node" + "[" + i + " , " + j + "]" + " Pos  : " + "( " + bl_nodes[i, j].Position.x + " , " + bl_nodes[i, j].Position.y + " )");
                Debug.Log("Block Node" + "[" + i + " , " + j + "]" + " C_UP : " + "( " + bl_nodes[i, j].Can_UP + " )");
                Debug.Log("Block Node" + "[" + i + " , " + j + "]" + " C_DN : " + "( " + bl_nodes[i, j].Can_DN + " )");
                Debug.Log("Block Node" + "[" + i + " , " + j + "]" + " C_LFT: " + "( " + bl_nodes[i, j].Can_LFT + " )");
                Debug.Log("Block Node" + "[" + i + " , " + j + "]" + " C_RGT: " + "( " + bl_nodes[i, j].Can_RGT + " )");
            }
        }

        Debug.Log("------ End [Block] Node Array Info ------");

        Debug.Log("------ Start [Line] Node Array Info ------");
        Debug.Log("Line Row Number: " + li_nodes.GetLength(0));
        Debug.Log("Line Col Number: " + li_nodes.GetLength(1));
        for (int i = 0; i < li_nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < li_nodes.GetLength(1); ++j)
            {
                Debug.Log("Line Node" + "[" + i + " , " + j + "]" + " Pos  : " + "( " + li_nodes[i, j].Position.x + " , " + li_nodes[i, j].Position.y + " )");
                Debug.Log("Line Node" + "[" + i + " , " + j + "]" + " C_UP : " + "( " + li_nodes[i, j].Can_UP + " )");
                Debug.Log("Line Node" + "[" + i + " , " + j + "]" + " C_DN : " + "( " + li_nodes[i, j].Can_DN + " )");
                Debug.Log("Line Node" + "[" + i + " , " + j + "]" + " C_LFT: " + "( " + li_nodes[i, j].Can_LFT + " )");
                Debug.Log("Line Node" + "[" + i + " , " + j + "]" + " C_RGT: " + "( " + li_nodes[i, j].Can_RGT + " )");
            }
        }
        Debug.Log("------ End [Line] Node Array Info ------");

    }


    //*!----------------------------!/
    //*!    Unity Functions
    //*!----------------------------!/

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        float gizmos_spacing = 0.15f + handle_size * 0.5f;

        //*! Iterate through both Block and Line's node array and draw the nodes
        for (int i = 0; i < bl_nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < bl_nodes.GetLength(1); ++j)
            {
                
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(bl_nodes[i, j].Position, handle_size);

                if (bl_nodes[i, j].Can_UP)
                {
                    Vector3 arrowPos = bl_nodes[i, j].Position + new Vector3(0f, gizmos_spacing, 0);
                    Gizmos.DrawSphere(arrowPos, handle_size * 0.35f);
                }

                if (bl_nodes[i, j].Can_DN)
                {
                    Vector3 arrowPos = bl_nodes[i, j].Position + new Vector3(0f, -gizmos_spacing, 0);
                    Gizmos.DrawSphere(arrowPos, handle_size * 0.35f);

                }

                if (bl_nodes[i, j].Can_LFT)
                {
                    Vector3 arrowPos = bl_nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0);
                    Gizmos.DrawSphere(arrowPos, handle_size * 0.35f);
                }

                if (bl_nodes[i, j].Can_RGT)
                {
                    Vector3 arrowPos = bl_nodes[i, j].Position + new Vector3((0.15f + handle_size * 0.5f), 0, 0);
                    Gizmos.DrawSphere(arrowPos, handle_size * 0.35f);
                }
            }
        }

        //*! Iterate through both Block and Line's node arrays and draw the nodes
        for (int i = 0; i < li_nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < li_nodes.GetLength(1); ++j)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(li_nodes[i, j].Position, handle_size);

                if (li_nodes[i, j].Can_UP)
                {
                    Vector3 arrowPos = li_nodes[i, j].Position + new Vector3(0f, gizmos_spacing, 0);
                    Gizmos.DrawSphere(arrowPos, handle_size * 0.35f);
                }

                if (li_nodes[i, j].Can_DN)
                {
                    Vector3 arrowPos = li_nodes[i, j].Position + new Vector3(0f, -gizmos_spacing, 0);
                    Gizmos.DrawSphere(arrowPos, handle_size * 0.35f);

                }

                if (li_nodes[i, j].Can_LFT)
                {
                    Vector3 arrowPos = li_nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0);
                    Gizmos.DrawSphere(arrowPos, handle_size * 0.35f);
                }

                if (li_nodes[i, j].Can_RGT)
                {
                    Vector3 arrowPos = li_nodes[i, j].Position + new Vector3((0.15f + handle_size * 0.5f), 0, 0);
                    Gizmos.DrawSphere(arrowPos, handle_size * 0.35f);
                }
            }
        }

        //*! Iterate through both Block and Line's edge lists and draw the nodes
        for (int i = 0; i < bl_edges.Count; ++i)
        {
            Vector3 startPos = bl_edges[i].Start_Node.Position;
            Vector3 endPos = bl_edges[i].End_Node.Position;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(startPos, endPos);
        }

        for (int i = 0; i < li_edges.Count; ++i)
        {
            Vector3 startPos = li_edges[i].Start_Node.Position;
            Vector3 endPos = li_edges[i].End_Node.Position;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPos, endPos);
        }
    }


    //*!----------------------------!*//
    //*!    Custom Subclasses
    //*!----------------------------!*//

    //*! Structs for map elements [Node] and [Edge] 
    //[System.Serializable]
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
    }
}
