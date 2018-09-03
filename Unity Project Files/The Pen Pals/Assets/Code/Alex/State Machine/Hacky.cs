using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacky : MonoBehaviour {


    //*! Graph Container
    public Temp_Node_Map Node_Graph;
    // Use this for initialization
    void Start () {


        Node_Graph.BL_Nodes[3, 1].DN_NODE = null;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
