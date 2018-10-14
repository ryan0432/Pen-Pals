//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: This is the [Lv_Data], creates scriptable objects
//*!              that store essential information of [Node], [Edge]
//*!              This class in an experimental class to test using
//*!              editor with MonoBehavior.
//*!
//*! Last edit  : 17/09/2018
//*!--------------------------------------------------------------!*//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Lv_Data : ScriptableObject
{
    public int row;
    public int col;
    public Inter_Node[] BL_Nodes;
    public Inter_Node[] LI_Nodes;
    public Inter_Edge[] BL_U_Edges;
    public Inter_Edge[] BL_V_Edges;
    public Inter_Edge[] LI_U_Edges;
    public Inter_Edge[] LI_V_Edges;
    public Inter_Cam Cam;
}

[System.Serializable]
public class Inter_Node
{
    public Vector3 Position;
    public Node_Type Node_Type;
}

[System.Serializable]
public class Inter_Edge
{
    public Vector3 Position;
    public Edge_Type Edge_Type;
    public Edge_Direction Edge_Direction;
}

[System.Serializable]
public class Inter_Cam
{
    public Vector3 Position;
    public float Size;
}