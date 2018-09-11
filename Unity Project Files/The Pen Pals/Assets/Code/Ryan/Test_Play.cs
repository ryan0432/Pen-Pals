using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Play : MonoBehaviour
{
    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//

    [SerializeField]
    public Lv_Data lvData;

    public Node[,] BL_Nodes { get; set; }
    public Node[,] LI_Nodes { get; set; }

    public Edge[,] BL_U_Edges { get; set; }
    public Edge[,] BL_V_Edges { get; set; }

    public Edge[,] LI_U_Edges { get; set; }
    public Edge[,] LI_V_Edges { get; set; }

    public int row;
    public int col;

    void Awake()
    {
        Initializa_Level_Data();
    }

    void Update()
    {
        Debug.Log(LI_Nodes[1,1].Position);
    }

    //*!----------------------------!*//
    //*!    Private Functions
    //*!----------------------------!*//

    private void Initializa_Level_Data()
    {

    }
}
