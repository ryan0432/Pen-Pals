//*!-------------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class storing graph infos such as [node positions],
//*!              [edge positions], and [edge normals].
//*!
//*! Last edit  : 21/08/2018
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

    public Node[,] BL_Nodes { get { return bl_nodes; } set { bl_nodes = value; } }
    public Node[,] LI_Nodes { get { return li_nodes; } set { li_nodes = value; } }

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//

    /*- Note: The reason to use 2D array for nodes is it is easier to find 
              connections between neighbor nodes than a list. As for edges
              only has two node refs, therefore using a list. -*/
    private Node[,] bl_nodes;
    private List<Edge> bl_edges;
    private Node[,] li_nodes;
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
