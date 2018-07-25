using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeCreator : MonoBehaviour
{
    [HideInInspector]
    private List<Vector3> nodes = new List<Vector3>();
    [HideInInspector]
    private List<Vector3> edges = new List<Vector3>();
    [HideInInspector]
    private List<Vector3> edgeNormals = new List<Vector3>();

    [Range(0.05f, 0.8f)]
    public float nodeRadius = 0.1f;

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
