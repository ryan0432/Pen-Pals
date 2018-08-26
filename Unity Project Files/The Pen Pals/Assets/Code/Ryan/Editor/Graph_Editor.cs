//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for operating Graph Editor in edit mode
//*!
//*! Last edit  : 25/08/2018
//*!--------------------------------------------------------------!*//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Graph_Creator))]
public class Graph_Editor : Editor
{
    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    private SelectionInfo selectionInfo;
    private Graph_Creator gc;
    private bool needRepaint;

    //*!----------------------------!*//
    //*!    Private Functions
    //*!----------------------------!*//

    //*! Setup [Block] and [Line] nodes by the number of row and col
    private void Initialize_Graph()
    {
        //gc.BL_Nodes = new Graph_Creator.Node[gc.row - 1, gc.col - 1];
        //bl_edges = new List<Edge>();
        //li_nodes = new Node[row, col];
        //li_edges = new List<Edge>();
    }



    //*!----------------------------!*//
    //*!    Custom Subclasses
    //*!----------------------------!*//

        //*! Subclass for storing temperary mouse behaviors
    private class SelectionInfo
    {
        // Stores node or edge hovered info
        public int hoveredNodeIndex = -1;
        public int hoveredEdgeIndex = -1;
        public bool nodeHovered;
        public bool edgeHovered;

        // Stores node or edge selected info
        public bool edgeSelected;
        public bool nodeSelected;

        // Stores mousedrag start position
        public Vector3 dragStartPos;
        // Stores mousedrag end position
        public Vector3 dragEndPos;
    }
}
