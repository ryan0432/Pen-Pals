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
    public Node[] BL_Nodes;
    [SerializeField]
    public Node[] LI_Nodes;
    [SerializeField]
    public Edge[] BL_U_Edges;
    [SerializeField]
    public Edge[] BL_V_Edges;
    [SerializeField]
    public Edge[] LI_U_Edges;
    [SerializeField]
    public Edge[] LI_V_Edges;
}
