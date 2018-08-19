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

    private Node[,] bl_nodes;
    private List<Edge> bl_edges;
    private Node[,] li_nodes;
    private List<Edge> li_edges;

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
        for (int i = 0; i < row - 1; ++i)
        {
            for (int j = 0; j < col - 1; ++j)
            {
                bl_nodes[i, j] = new Node();
                bl_nodes[i, j].Position = new Vector3(i + 0.5f, j + 0.5f);
                bl_nodes[i, j].Node_Size = handle_size;
                bl_nodes[i, j].Can_UP = true;
                bl_nodes[i, j].Can_DN = true;
                bl_nodes[i, j].Can_LFT = true;
                bl_nodes[i, j].Can_RGT = true;
            }
        }

        //*! Setup Line nodes by the number of row and col
        for (int i = 0; i < row; ++i)
        {
            for (int j = 0; j < col; ++j)
            {
                li_nodes[i, j] = new Node();
                li_nodes[i, j].Position = new Vector3(i, j);
                li_nodes[i, j].Node_Size = handle_size;
                li_nodes[i, j].Can_UP = true;
                li_nodes[i, j].Can_DN = true;
                li_nodes[i, j].Can_LFT = true;
                li_nodes[i, j].Can_RGT = true;
            }
        }

        //*! Iterate from the buttom left node of Block 2D array to check UP & RIGHT direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        for (int i = 0; i < bl_nodes.GetUpperBound(0) - 1; ++i)
        {
            for (int j = 0; j < bl_nodes.GetUpperBound(1) - 1; ++j)
            {
                if (bl_nodes[i, j].Can_UP && bl_nodes[i, j + 1].Can_DN)
                {
                    Edge new_edge = new Edge();
                    new_edge.Start_Node = bl_nodes[i, j];
                    new_edge.End_Node = bl_nodes[i, j + 1];
                    bl_edges.Add(new_edge);
                }

                if (bl_nodes[i, j].Can_RGT && bl_nodes[i + 1, j].Can_LFT)
                {
                    Edge new_edge = new Edge();
                    new_edge.Start_Node = bl_nodes[i, j];
                    new_edge.End_Node = bl_nodes[i + 1, j];
                    bl_edges.Add(new_edge);
                }
            }
        }
    }

    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        for (int i = 0; i < bl_nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < bl_nodes.GetUpperBound(1); ++j)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(bl_nodes[i,j].Position, handle_size);
            }
        }

        for (int i = 0; i < li_nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < li_nodes.GetUpperBound(1); ++j)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(li_nodes[i, j].Position, handle_size);
            }
        }
    }


    //*!----------------------------!*//
    //*!    Custom Subclass
    //*!----------------------------!*//

    //*! Structs for map elements [Node] and [Edge] 
    class Node
    {
        //*! Getter, Setter of Node members
        public Vector3 Position { get; set; }
        public float Node_Size { get; set; }
        public bool Can_UP { get; set; }
        public bool Can_DN { get; set; }
        public bool Can_LFT { get; set; }
        public bool Can_RGT { get; set; }

    }

    class Edge
    {
        //*! Getter, Setter of Edge members
        public Node Start_Node { get; set; }
        public Node End_Node { get; set; }
    }
}
