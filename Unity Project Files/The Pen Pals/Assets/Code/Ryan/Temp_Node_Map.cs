//*!----------------------------!*//
//*! Programmer: Ryan Chung
//*!----------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_Node_Map : MonoBehaviour
{
    private List<Node> nodes;
    private List<Edge> edges;
    public int row;
    public int col;
    // Use this for initialization
    void Start ()
    {
       nodes = new List<Node>();
       edges = new List<Edge>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		

	}

    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//

    private void OnDrawGizmos()
    {
        for (int i = 0; i < nodes.Count; ++i)
        {

        }
    }

    //*! Structs for map elements [Node] and [Edge] 
    struct Node
    {
        Vector3 position;
        float node_size;
        bool Can_UP;
        bool Can_DN;
        bool Can_LFT;
        bool Can_RGT;
    }

    struct Edge
    {
        Node start_node;
        Node end_node;
    }
}
