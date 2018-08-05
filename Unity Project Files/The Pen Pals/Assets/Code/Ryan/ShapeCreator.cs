//*!-------------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class storing graph infos such as [node positions],
//*!              [edge positions], and [edge normals].
//*!
//*! Last edit  : 01/08/2018
//*!-------------------------------------------------------------------!*//

//*! Using namespaces
using System.Collections.Generic;
using UnityEngine;

public class ShapeCreator : MonoBehaviour
{
    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    [HideInInspector]
    private List<Vector3> nodes = new List<Vector3>();
    [HideInInspector]
    private List<Vector3> edges = new List<Vector3>();
    [HideInInspector]
    private List<Vector3> edgeNormals = new List<Vector3>();

    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    [Range(0.05f, 0.8f)]
    public float nodeRadius = 0.1f;
    public float edgewidth = 0.1f;

    //*! Public Access
    public List<Vector3> Nodes
    {
        get { return nodes; }
        set { nodes = value; }
    }

    public List<Vector3> Edges
    {
        get { return edges; }
        set { edges = value; }
    }

    public List<Vector3> EdgeNormals
    {
        get { return edgeNormals; }
        set { edgeNormals = value; }
    }
}
