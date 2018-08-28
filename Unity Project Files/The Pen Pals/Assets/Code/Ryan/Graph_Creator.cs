//*!-------------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class storing graph infos such as [node positions],
//*!              [edge positions], and [edge normals].
//*!              This class in an experimental class to test using
//*!              editor with Editor class combined with Node systme.
//*!
//*! Last edit  : 27/08/2018
//*!-------------------------------------------------------------------!*//

//*! Using namespaces
using System.Collections.Generic;
using UnityEngine;

public class Graph_Creator : MonoBehaviour
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

    public Mesh node_giz;
    public Mesh arrw_giz;
    public Material bl_giz_mat;
    public Material li_giz_mat;

    public Node[,] BL_Nodes { get { return bl_nodes; } set { bl_nodes = value; } }
    public Node[,] LI_Nodes { get { return li_nodes; } set { li_nodes = value; } }
    public List<Edge> BL_Edges { get { return bl_edges; } set { bl_edges = value; } }
    public List<Edge> LI_Edges { get { return li_edges; } set { li_edges = value; } }

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

    //*!----------------------------!*//
    //*!    Custom Subclasses
    //*!----------------------------!*//

    //*! Structs for map elements [Node] and [Edge] 
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
}
