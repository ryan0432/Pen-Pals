using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Play : MonoBehaviour
{
    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//

    [SerializeField]
    public Pencil_Case pc;

    [SerializeField]
    public Node[,] BL_Nodes { get; set; }
    [SerializeField]
    public Node[,] LI_Nodes { get; set; }

    [SerializeField]
    public Edge[,] BL_U_Edges { get; set; }
    [SerializeField]
    public Edge[,] BL_V_Edges { get; set; }

    [SerializeField]
    public Edge[,] LI_U_Edges { get; set; }
    [SerializeField]
    public Edge[,] LI_V_Edges { get; set; }

    public int row;
    public int col;

    void Awake()
    {
        Initializa_Map_Data();
    }

    void Update()
    {
        Debug.Log(BL_Nodes[0, 0].Position);
    }

    //*!----------------------------!*//
    //*!    Private Functions
    //*!----------------------------!*//

    private void Initializa_Map_Data()
    {
        BL_Nodes = pc.BL_Nodes;
    }
}
