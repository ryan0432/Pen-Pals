//*!----------------------------!*//
//*! Programmer: Ryan Chung
//*!----------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_Node_Map : MonoBehaviour
{
    [Range(1, 500)]
    public int row;
    [Range(1, 500)]
    public int col;
    [Range(0.1f, 1.0f)]
    public float handle_size;

    //*! Note: The reason to use 2D array for nodes is it is easier to find 
    //*!       connections between neighbor nodes than a list. As for edges
    //*!       only has two node refs, therefore using a list.
    private Node[,] bl_nodes;
    private List<Edge> bl_edges;
    private Node[,] li_nodes;
    private List<Edge> li_edges;

    public Node[,] BL_Nodes { get { return bl_nodes; } set { bl_nodes = value; } }
    public Node[,] LI_Nodes { get { return li_nodes; } set { li_nodes = value; } }

    // Use this for initialization
    void Start ()
    {
        Initialize_Graph();
    }
	
	// Update is called once per frame
	void Update ()
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

        //*! Setup Block nodes by the number of row and col
        for (int i = 0; i < bl_nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < bl_nodes.GetLength(1); ++j)
            {
                bl_nodes[i, j] = new Node();
                bl_nodes[i, j].Position = new Vector3(i + 0.5f, j + 0.5f);
                bl_nodes[i, j].Node_Size = handle_size;
                
                bl_nodes[i, j].Can_UP = true;
                bl_nodes[i, j].Can_DN = true;
                bl_nodes[i, j].Can_LFT = true;
                bl_nodes[i, j].Can_RGT = true;

                if (i == bl_nodes.GetLowerBound(0))
                {
                    bl_nodes[i, j].Can_LFT = false;
                }

                if (i == bl_nodes.GetUpperBound(0))
                {
                    bl_nodes[i, j].Can_RGT = false;
                }

                if (j == bl_nodes.GetLowerBound(1))
                {
                    bl_nodes[i, j].Can_DN = false;
                }

                if (j == bl_nodes.GetUpperBound(1))
                {
                    bl_nodes[i, j].Can_UP = false;
                }
            }
        }

        // !Setup Line nodes by the number of row and col
        for (int i = 0; i < li_nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < li_nodes.GetLength(1); ++j)
            {
                li_nodes[i, j] = new Node();
                li_nodes[i, j].Position = new Vector3(i, j);
                li_nodes[i, j].Node_Size = handle_size;
                li_nodes[i, j].Can_UP = true;
                li_nodes[i, j].Can_DN = true;
                li_nodes[i, j].Can_LFT = true;
                li_nodes[i, j].Can_RGT = true;

                if (i == li_nodes.GetLowerBound(0))
                {
                    li_nodes[i, j].Can_LFT = false;
                }

                if (i == li_nodes.GetUpperBound(0))
                {
                    li_nodes[i, j].Can_RGT = false;
                }

                if (j == li_nodes.GetLowerBound(1))
                {
                    li_nodes[i, j].Can_DN = false;
                }

                if (j == li_nodes.GetUpperBound(1))
                {
                    li_nodes[i, j].Can_UP = false;
                }
            }
        }

        //*! Iterate from the buttom left node of Block 2D array to check UP & RIGHT direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup Block Edges in UP & Right Direction
        for (int i = 0; i < bl_nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < bl_nodes.GetUpperBound(1); ++j)
            {
                if (bl_nodes[i, j].Can_UP)
                {
                    Edge new_edge = new Edge();
                    new_edge.Start_Node = bl_nodes[i, j];
                    new_edge.End_Node = bl_nodes[i, j + 1];
                    bl_edges.Add(new_edge);
                }

                if (bl_nodes[i, j].Can_RGT)
                {
                    Edge new_edge = new Edge();
                    new_edge.Start_Node = bl_nodes[i, j];
                    new_edge.End_Node = bl_nodes[i + 1, j];
                    bl_edges.Add(new_edge);
                }
            }
        }

        for (int i = 0; i < bl_nodes.GetUpperBound(0); ++i)
        {
            if (bl_nodes[i, bl_nodes.GetUpperBound(1)].Can_RGT)
            {
                Edge new_edge = new Edge();
                new_edge.Start_Node = bl_nodes[i, bl_nodes.GetUpperBound(1)];
                new_edge.End_Node = bl_nodes[i + 1, bl_nodes.GetUpperBound(1)];
                bl_edges.Add(new_edge);
            }
        }

        for (int i = 0; i < bl_nodes.GetUpperBound(1); ++i)
        {
            if (bl_nodes[bl_nodes.GetUpperBound(0), i].Can_UP)
            {
                Edge new_edge = new Edge();
                new_edge.Start_Node = bl_nodes[bl_nodes.GetUpperBound(0), i];
                new_edge.End_Node = bl_nodes[bl_nodes.GetUpperBound(0), i + 1];
                bl_edges.Add(new_edge);
            }
        }
        #endregion
        #region Setup Line Edges in UP & Right Direction
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

    }

    //*!----------------------------!/
    //*!    Unity Functions
    //*!----------------------------!*//

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        //*! Iterate through both Block and Line's node array and draw the nodes
        for (int i = 0; i < bl_nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < bl_nodes.GetLength(1); ++j)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(bl_nodes[i,j].Position, handle_size);
            }
        }

        for (int i = 0; i < li_nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < li_nodes.GetLength(1); ++j)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(li_nodes[i, j].Position, handle_size);
            }
        }

        //*! Iterate through both Block and Line's edge list and draw the nodes
        for (int i = 0; i < bl_edges.Count; ++i)
        {
            Vector3 startPos = bl_edges[i].Start_Node.Position;
            Vector3 endPos = bl_edges[i].End_Node.Position;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(bl_edges[i].Start_Node.Position, endPos);
        }

        for (int i = 0; i < li_edges.Count; ++i)
        {
            Vector3 startPos = li_edges[i].Start_Node.Position;
            Vector3 endPos = li_edges[i].End_Node.Position;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(li_edges[i].Start_Node.Position, endPos);
        }
    }


    //*!----------------------------!*//
    //*!    Custom Subclass
    //*!----------------------------!*//

    //*! Structs for map elements [Node] and [Edge] 
    [System.Serializable]
    public class Node
    {
        //*! Getter, Setter of Node members
        public Vector3 Position { get; set; }
        public float Node_Size { get; set; }
        public bool Can_UP { get; set; }
        public bool Can_DN { get; set; }
        public bool Can_LFT { get; set; }
        public bool Can_RGT { get; set; }

    }

    [System.Serializable]
    public class Edge
    {
        //*! Getter, Setter of Edge members
        public Node Start_Node { get; set; }
        public Node End_Node { get; set; }
    }
}
