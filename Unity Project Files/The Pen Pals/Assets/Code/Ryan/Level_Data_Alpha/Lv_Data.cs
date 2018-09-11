using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Lv_Data : ScriptableObject
{
    public int row;
    public int col;
    public Node[] BL_Nodes;
    public Node[] LI_Nodes;
    public Edge[] BL_U_Edges;
    public Edge[] BL_V_Edges;
    public Edge[] LI_U_Edges;
    public Edge[] LI_V_Edges;
}
