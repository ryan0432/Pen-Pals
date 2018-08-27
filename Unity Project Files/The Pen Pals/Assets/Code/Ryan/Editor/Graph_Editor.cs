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
    private Graph_Creator gC;
    private bool needRepaint;

    //*!----------------------------!*//
    //*!    Private Functions
    //*!----------------------------!*//

    //*! Setup [Block] and [Line] nodes by the number of row and col
    private void Initialize_Graph()
    {
        //*! Populate the graph with [Node] default construction
        #region Setup instances for [Block] and [Line] Nodes and Edges
        gC.BL_Nodes = new Graph_Creator.Node[gC.row - 1, gC.col - 1];
        gC.BL_Edges = new List<Graph_Creator.Edge>();
        gC.LI_Nodes = new Graph_Creator.Node[gC.row, gC.col];
        gC.LI_Edges = new List<Graph_Creator.Edge>();
        #endregion

        #region Setup [Block] Nodes and Preset Neighbor Nodes to Null
        for (int i = 0; i < gC.BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < gC.BL_Nodes.GetLength(1); ++j)
            {
                Graph_Creator.Node new_node = new Graph_Creator.Node();
                gC.BL_Nodes[i, j] = new_node;
                gC.BL_Nodes[i, j].Position = new Vector3(i + 0.5f, j + 0.5f);
                gC.BL_Nodes[i, j].Node_Size = gC.handle_size;
                gC.BL_Nodes[i, j].UP_NODE = null;
                gC.BL_Nodes[i, j].DN_NODE = null;
                gC.BL_Nodes[i, j].LFT_NODE = null;
                gC.BL_Nodes[i, j].RGT_NODE = null;
            }
        }
        #endregion

        #region Setup [Block] Nodes and Neighbor Nodes' linkage in UP & RGT Direction
        for (int i = 0; i < gC.BL_Nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < gC.BL_Nodes.GetUpperBound(1); ++j)
            {
                gC.BL_Nodes[i, j].UP_NODE = gC.BL_Nodes[i, j + 1];
                gC.BL_Nodes[i, j].RGT_NODE = gC.BL_Nodes[i + 1, j];
            }
        }

        for (int i = 0; i < gC.BL_Nodes.GetUpperBound(0); ++i)
        {
            gC.BL_Nodes[i, gC.BL_Nodes.GetUpperBound(1)].RGT_NODE = gC.BL_Nodes[i + 1, gC.BL_Nodes.GetUpperBound(1)];
        }

        for (int i = 0; i < gC.BL_Nodes.GetUpperBound(1); ++i)
        {
            gC.BL_Nodes[gC.BL_Nodes.GetUpperBound(0), i].UP_NODE = gC.BL_Nodes[gC.BL_Nodes.GetUpperBound(0), i + 1];
        }
        #endregion

        #region Setup [Block] Nodes and Neighbor Nodes' linkage in DN & LFT Direction
        for (int i = gC.BL_Nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = gC.BL_Nodes.GetUpperBound(1); j > 0; --j)
            {
                gC.BL_Nodes[i, j].DN_NODE = gC.BL_Nodes[i, j - 1];
                gC.BL_Nodes[i, j].LFT_NODE = gC.BL_Nodes[i - 1, j];
            }
        }

        for (int i = gC.BL_Nodes.GetUpperBound(0); i > 0; --i)
        {
            gC.BL_Nodes[i, gC.BL_Nodes.GetLowerBound(1)].LFT_NODE = gC.BL_Nodes[i - 1, gC.BL_Nodes.GetLowerBound(1)];
        }

        for (int i = gC.BL_Nodes.GetUpperBound(1); i > 0; --i)
        {
            gC.BL_Nodes[gC.BL_Nodes.GetLowerBound(0), i].DN_NODE = gC.BL_Nodes[gC.BL_Nodes.GetLowerBound(0), i - 1];
        }
        #endregion

        #region Setup [Line] Nodes and Preset Neighbor Nodes to Null
        for (int i = 0; i < gC.LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < gC.LI_Nodes.GetLength(1); ++j)
            {
                Graph_Creator.Node new_node = new Graph_Creator.Node();
                gC.LI_Nodes[i, j] = new_node;
                gC.LI_Nodes[i, j].Position = new Vector3(i, j);
                gC.LI_Nodes[i, j].Node_Size = gC.handle_size;
                gC.LI_Nodes[i, j].UP_NODE = null;
                gC.LI_Nodes[i, j].DN_NODE = null;
                gC.LI_Nodes[i, j].LFT_NODE = null;
                gC.LI_Nodes[i, j].RGT_NODE = null;
            }
        }
        #endregion

        #region Setup [Line] Nodes and Neighbor Nodes' linkage in UP & RGT Direction
        for (int i = 0; i < gC.LI_Nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < gC.LI_Nodes.GetUpperBound(1); ++j)
            {
                gC.LI_Nodes[i, j].UP_NODE = gC.LI_Nodes[i, j + 1];
                gC.LI_Nodes[i, j].RGT_NODE = gC.LI_Nodes[i + 1, j];
            }
        }

        for (int i = 0; i < gC.LI_Nodes.GetUpperBound(0); ++i)
        {
            gC.LI_Nodes[i, gC.LI_Nodes.GetUpperBound(1)].RGT_NODE = gC.LI_Nodes[i + 1, gC.LI_Nodes.GetUpperBound(1)];
        }

        for (int i = 0; i < gC.LI_Nodes.GetUpperBound(1); ++i)
        {
            gC.LI_Nodes[gC.LI_Nodes.GetUpperBound(0), i].UP_NODE = gC.LI_Nodes[gC.LI_Nodes.GetUpperBound(0), i + 1];
        }
        #endregion

        #region Setup [Line] Nodes and Neighbor Nodes' linkage in DN & LFT Direction
        for (int i = gC.LI_Nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = gC.LI_Nodes.GetUpperBound(1); j > 0; --j)
            {
                gC.LI_Nodes[i, j].DN_NODE = gC.LI_Nodes[i, j - 1];
                gC.LI_Nodes[i, j].LFT_NODE = gC.LI_Nodes[i - 1, j];
            }
        }

        for (int i = gC.LI_Nodes.GetUpperBound(0); i > 0; --i)
        {
            gC.LI_Nodes[i, gC.LI_Nodes.GetLowerBound(1)].LFT_NODE = gC.LI_Nodes[i - 1, gC.LI_Nodes.GetLowerBound(1)];
        }

        for (int i = gC.LI_Nodes.GetUpperBound(1); i > 0; --i)
        {
            gC.LI_Nodes[gC.LI_Nodes.GetLowerBound(0), i].DN_NODE = gC.LI_Nodes[gC.LI_Nodes.GetLowerBound(0), i - 1];
        }
        #endregion

        //*! Iterate from the buttom left node of [Block] 2D array to check [UP] & [RGT] direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Block] Edges in UP & RGT Direction
        for (int i = 0; i < gC.BL_Nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < gC.BL_Nodes.GetUpperBound(1); ++j)
            {
                if (gC.BL_Nodes[i, j].Can_UP)
                {
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                    new_edge.Start_Node = gC.BL_Nodes[i, j];
                    new_edge.End_Node = gC.BL_Nodes[i, j + 1];
                    gC.BL_Edges.Add(new_edge);
                }

                if (gC.BL_Nodes[i, j].Can_RGT)
                {
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                    new_edge.Start_Node = gC.BL_Nodes[i, j];
                    new_edge.End_Node = gC.BL_Nodes[i + 1, j];
                    gC.BL_Edges.Add(new_edge);
                }
            }
        }

        for (int i = 0; i < gC.BL_Nodes.GetUpperBound(0); ++i)
        {
            if (gC.BL_Nodes[i, gC.BL_Nodes.GetUpperBound(1)].Can_RGT)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                new_edge.Start_Node = gC.BL_Nodes[i, gC.BL_Nodes.GetUpperBound(1)];
                new_edge.End_Node = gC.BL_Nodes[i + 1, gC.BL_Nodes.GetUpperBound(1)];
                gC.BL_Edges.Add(new_edge);
            }
        }

        for (int i = 0; i < gC.BL_Nodes.GetUpperBound(1); ++i)
        {
            if (gC.BL_Nodes[gC.BL_Nodes.GetUpperBound(0), i].Can_UP)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                new_edge.Start_Node = gC.BL_Nodes[gC.BL_Nodes.GetUpperBound(0), i];
                new_edge.End_Node = gC.BL_Nodes[gC.BL_Nodes.GetUpperBound(0), i + 1];
                gC.BL_Edges.Add(new_edge);
            }
        }
        #endregion

        //*! Iterate from the upper right node of [Block] 2D array to check [DN] & [LFT] direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Block] Edges in DN & LFT Direction
        for (int i = gC.BL_Nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = gC.BL_Nodes.GetUpperBound(1); j > 0; --j)
            {
                if (gC.BL_Nodes[i, j].Can_DN)
                {
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                    new_edge.Start_Node = gC.BL_Nodes[i, j];
                    new_edge.End_Node = gC.BL_Nodes[i, j - 1];
                    gC.BL_Edges.Add(new_edge);
                }

                if (gC.BL_Nodes[i, j].Can_LFT)
                {
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                    new_edge.Start_Node = gC.BL_Nodes[i, j];
                    new_edge.End_Node = gC.BL_Nodes[i - 1, j];
                    gC.BL_Edges.Add(new_edge);
                }
            }
        }

        for (int i = gC.BL_Nodes.GetUpperBound(0); i > 0; --i)
        {
            if (gC.BL_Nodes[i, gC.BL_Nodes.GetLowerBound(1)].Can_LFT)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                new_edge.Start_Node = gC.BL_Nodes[i, gC.BL_Nodes.GetLowerBound(1)];
                new_edge.End_Node = gC.BL_Nodes[i - 1, gC.BL_Nodes.GetLowerBound(1)];
                gC.BL_Edges.Add(new_edge);
            }
        }

        for (int i = gC.BL_Nodes.GetUpperBound(1); i > 0; --i)
        {
            if (gC.BL_Nodes[gC.BL_Nodes.GetLowerBound(0), i].Can_DN)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                new_edge.Start_Node = gC.BL_Nodes[gC.BL_Nodes.GetLowerBound(0), i];
                new_edge.End_Node = gC.BL_Nodes[gC.BL_Nodes.GetLowerBound(0), i - 1];
                gC.BL_Edges.Add(new_edge);
            }
        }
        #endregion

        //*! Iterate from the upper right node of [Line] 2D array to check [UP] & [RGT] direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Line] Edges in UP & Right Direction
        for (int i = 0; i < gC.LI_Nodes.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < gC.LI_Nodes.GetUpperBound(1); ++j)
            {
                if (gC.LI_Nodes[i, j].Can_UP)
                {
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                    new_edge.Start_Node = gC.LI_Nodes[i, j];
                    new_edge.End_Node = gC.LI_Nodes[i, j + 1];
                    gC.LI_Edges.Add(new_edge);
                }

                if (gC.LI_Nodes[i, j].Can_RGT)
                {
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                    new_edge.Start_Node = gC.LI_Nodes[i, j];
                    new_edge.End_Node = gC.LI_Nodes[i + 1, j];
                    gC.LI_Edges.Add(new_edge);
                }
            }
        }

        for (int i = 0; i < gC.LI_Nodes.GetUpperBound(0); ++i)
        {
            if (gC.LI_Nodes[i, gC.LI_Nodes.GetUpperBound(1)].Can_RGT)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                new_edge.Start_Node = gC.LI_Nodes[i, gC.LI_Nodes.GetUpperBound(1)];
                new_edge.End_Node = gC.LI_Nodes[i + 1, gC.LI_Nodes.GetUpperBound(1)];
                gC.LI_Edges.Add(new_edge);
            }
        }

        for (int i = 0; i < gC.LI_Nodes.GetUpperBound(1); ++i)
        {
            if (gC.LI_Nodes[gC.LI_Nodes.GetUpperBound(0), i].Can_UP)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                new_edge.Start_Node = gC.LI_Nodes[gC.LI_Nodes.GetUpperBound(0), i];
                new_edge.End_Node = gC.LI_Nodes[gC.LI_Nodes.GetUpperBound(0), i + 1];
                gC.LI_Edges.Add(new_edge);
            }
        }
        #endregion

        //*! Iterate from the upper right node of [Line] 2D array to check [DN] & [LFT] direction's traversability.
        //*! If a direction's traversability is true, create an edge between them. Otherwise don't.
        #region Setup [Line] Edges in DN & LFT Direction
        for (int i = gC.LI_Nodes.GetUpperBound(0); i > 0; --i)
        {
            for (int j = gC.LI_Nodes.GetUpperBound(1); j > 0; --j)
            {
                if (gC.LI_Nodes[i, j].Can_DN)
                {
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                    new_edge.Start_Node = gC.LI_Nodes[i, j];
                    new_edge.End_Node = gC.LI_Nodes[i, j - 1];
                    gC.LI_Edges.Add(new_edge);
                }

                if (gC.LI_Nodes[i, j].Can_LFT)
                {
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                    new_edge.Start_Node = gC.LI_Nodes[i, j];
                    new_edge.End_Node = gC.LI_Nodes[i - 1, j];
                    gC.LI_Edges.Add(new_edge);
                }
            }
        }

        for (int i = gC.LI_Nodes.GetUpperBound(0); i > 0; --i)
        {
            if (gC.LI_Nodes[i, gC.LI_Nodes.GetLowerBound(1)].Can_LFT)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                new_edge.Start_Node = gC.LI_Nodes[i, gC.LI_Nodes.GetLowerBound(1)];
                new_edge.End_Node = gC.LI_Nodes[i - 1, gC.LI_Nodes.GetLowerBound(1)];
                gC.LI_Edges.Add(new_edge);
            }
        }

        for (int i = gC.LI_Nodes.GetUpperBound(1); i > 0; --i)
        {
            if (gC.LI_Nodes[gC.LI_Nodes.GetLowerBound(0), i].Can_DN)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge();
                new_edge.Start_Node = gC.LI_Nodes[gC.LI_Nodes.GetLowerBound(0), i];
                new_edge.End_Node = gC.LI_Nodes[gC.LI_Nodes.GetLowerBound(0), i - 1];
                gC.LI_Edges.Add(new_edge);
            }
        }
        #endregion
    }

    private void PrintHandles()
    {

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
