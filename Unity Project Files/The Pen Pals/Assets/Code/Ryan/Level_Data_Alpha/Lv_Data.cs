using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class Lv_Data : ScriptableObject
{
    [SerializeField]
    public int row;
    [SerializeField]
    public int col;
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
}
