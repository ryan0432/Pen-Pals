//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for operating Graph Editor in edit mode.
//*!              This class in an experimental class to test using
//*!              editor with Editor class combined with Node systme.
//*!
//*! Last edit  : 27/08/2018
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
    //private Selection_Info selectionInfo;
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
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                    {
                        Start_Node = gC.BL_Nodes[i, j],
                        End_Node = gC.BL_Nodes[i, j + 1]
                    };
                    gC.BL_Edges.Add(new_edge);
                }

                if (gC.BL_Nodes[i, j].Can_RGT)
                {
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                    {
                        Start_Node = gC.BL_Nodes[i, j],
                        End_Node = gC.BL_Nodes[i + 1, j]
                    };
                    gC.BL_Edges.Add(new_edge);
                }
            }
        }

        for (int i = 0; i < gC.BL_Nodes.GetUpperBound(0); ++i)
        {
            if (gC.BL_Nodes[i, gC.BL_Nodes.GetUpperBound(1)].Can_RGT)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                {
                    Start_Node = gC.BL_Nodes[i, gC.BL_Nodes.GetUpperBound(1)],
                    End_Node = gC.BL_Nodes[i + 1, gC.BL_Nodes.GetUpperBound(1)]
                };
                gC.BL_Edges.Add(new_edge);
            }
        }

        for (int i = 0; i < gC.BL_Nodes.GetUpperBound(1); ++i)
        {
            if (gC.BL_Nodes[gC.BL_Nodes.GetUpperBound(0), i].Can_UP)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                {
                    Start_Node = gC.BL_Nodes[gC.BL_Nodes.GetUpperBound(0), i],
                    End_Node = gC.BL_Nodes[gC.BL_Nodes.GetUpperBound(0), i + 1]
                };
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
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                    {
                        Start_Node = gC.BL_Nodes[i, j],
                        End_Node = gC.BL_Nodes[i, j - 1]
                    };
                    gC.BL_Edges.Add(new_edge);
                }

                if (gC.BL_Nodes[i, j].Can_LFT)
                {
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                    {
                        Start_Node = gC.BL_Nodes[i, j],
                        End_Node = gC.BL_Nodes[i - 1, j]
                    };
                    gC.BL_Edges.Add(new_edge);
                }
            }
        }

        for (int i = gC.BL_Nodes.GetUpperBound(0); i > 0; --i)
        {
            if (gC.BL_Nodes[i, gC.BL_Nodes.GetLowerBound(1)].Can_LFT)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                {
                    Start_Node = gC.BL_Nodes[i, gC.BL_Nodes.GetLowerBound(1)],
                    End_Node = gC.BL_Nodes[i - 1, gC.BL_Nodes.GetLowerBound(1)]
                };
                gC.BL_Edges.Add(new_edge);
            }
        }

        for (int i = gC.BL_Nodes.GetUpperBound(1); i > 0; --i)
        {
            if (gC.BL_Nodes[gC.BL_Nodes.GetLowerBound(0), i].Can_DN)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                {
                    Start_Node = gC.BL_Nodes[gC.BL_Nodes.GetLowerBound(0), i],
                    End_Node = gC.BL_Nodes[gC.BL_Nodes.GetLowerBound(0), i - 1]
                };
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
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                    {
                        Start_Node = gC.LI_Nodes[i, j],
                        End_Node = gC.LI_Nodes[i, j + 1]
                    };
                    gC.LI_Edges.Add(new_edge);
                }

                if (gC.LI_Nodes[i, j].Can_RGT)
                {
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                    {
                        Start_Node = gC.LI_Nodes[i, j],
                        End_Node = gC.LI_Nodes[i + 1, j]
                    };
                    gC.LI_Edges.Add(new_edge);
                }
            }
        }

        for (int i = 0; i < gC.LI_Nodes.GetUpperBound(0); ++i)
        {
            if (gC.LI_Nodes[i, gC.LI_Nodes.GetUpperBound(1)].Can_RGT)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                {
                    Start_Node = gC.LI_Nodes[i, gC.LI_Nodes.GetUpperBound(1)],
                    End_Node = gC.LI_Nodes[i + 1, gC.LI_Nodes.GetUpperBound(1)]
                };
                gC.LI_Edges.Add(new_edge);
            }
        }

        for (int i = 0; i < gC.LI_Nodes.GetUpperBound(1); ++i)
        {
            if (gC.LI_Nodes[gC.LI_Nodes.GetUpperBound(0), i].Can_UP)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                {
                    Start_Node = gC.LI_Nodes[gC.LI_Nodes.GetUpperBound(0), i],
                    End_Node = gC.LI_Nodes[gC.LI_Nodes.GetUpperBound(0), i + 1]
                };
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
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                    {
                        Start_Node = gC.LI_Nodes[i, j],
                        End_Node = gC.LI_Nodes[i, j - 1]
                    };
                    gC.LI_Edges.Add(new_edge);
                }

                if (gC.LI_Nodes[i, j].Can_LFT)
                {
                    Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                    {
                        Start_Node = gC.LI_Nodes[i, j],
                        End_Node = gC.LI_Nodes[i - 1, j]
                    };
                    gC.LI_Edges.Add(new_edge);
                }
            }
        }

        for (int i = gC.LI_Nodes.GetUpperBound(0); i > 0; --i)
        {
            if (gC.LI_Nodes[i, gC.LI_Nodes.GetLowerBound(1)].Can_LFT)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                {
                    Start_Node = gC.LI_Nodes[i, gC.LI_Nodes.GetLowerBound(1)],
                    End_Node = gC.LI_Nodes[i - 1, gC.LI_Nodes.GetLowerBound(1)]
                };
                gC.LI_Edges.Add(new_edge);
            }
        }

        for (int i = gC.LI_Nodes.GetUpperBound(1); i > 0; --i)
        {
            if (gC.LI_Nodes[gC.LI_Nodes.GetLowerBound(0), i].Can_DN)
            {
                Graph_Creator.Edge new_edge = new Graph_Creator.Edge
                {
                    Start_Node = gC.LI_Nodes[gC.LI_Nodes.GetLowerBound(0), i],
                    End_Node = gC.LI_Nodes[gC.LI_Nodes.GetLowerBound(0), i - 1]
                };
                gC.LI_Edges.Add(new_edge);
            }
        }
        #endregion

        //*! Request a repaint
        needRepaint = true;
    }

    private void Edit_Graph(Event guiEvent)
    {
        needRepaint = true;
    }

    //*! Renders handles in editor window
    #region Render all nodes and edges in the graph
    private void PrintHandles()
    {
        //*! Setup space between indicators and their parent node
        float gizmos_spacing = 0.15f + gC.handle_size * 0.5f;

        #region Setup defult colours for [Block] & [Line] handles
        Color bl_colour = new Color(1.0f, 0.8f, 0.1f, 1.0f);
        Color li_colour = new Color(0.4f, 0.75f, 1.0f, 1.0f);
        #endregion

        #region Render [Block] nodes
        for (int i = 0; i < gC.BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < gC.BL_Nodes.GetLength(1); ++j)
            {

                //Handles.color = bl_colour;
                //Handles.DrawSolidDisc(gC.BL_Nodes[i,j].Position, Vector3.forward, gC.handle_size);

                Matrix4x4 handleMatx = Matrix4x4.Translate(gC.BL_Nodes[i, j].Position) *
                                       Matrix4x4.Scale(new Vector3(gC.handle_size, gC.handle_size, gC.handle_size) * 2);

                Graphics.DrawMesh(gC.node_giz, handleMatx, gC.bl_giz_mat, 0);
            }
        }
        #endregion

        #region Render [Block] direction indications
        for (int i = 0; i < gC.BL_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < gC.BL_Nodes.GetLength(1); ++j)
            {
                //Handles.color = bl_colour;
                if (gC.BL_Nodes[i, j].Can_UP)
                {
                    //Vector3 arrowPos = gC.BL_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0);
                    //Handles.DrawSolidDisc(arrowPos, Vector3.forward, gC.handle_size * 0.35f);

                    Matrix4x4 handleMatx = Matrix4x4.Translate(gC.BL_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0)) *
                                           Matrix4x4.Scale(new Vector3(gC.handle_size, gC.handle_size, gC.handle_size));

                    Graphics.DrawMesh(gC.arrw_giz, handleMatx, gC.bl_giz_mat, 0);
                }
                
                if (gC.BL_Nodes[i, j].Can_DN)
                {
                    //Vector3 arrowPos = gC.BL_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0);
                    //Handles.DrawSolidDisc(arrowPos, Vector3.forward, gC.handle_size * 0.35f);

                    Matrix4x4 handleMatx = Matrix4x4.Translate(gC.BL_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0)) *
                                           Matrix4x4.Scale(new Vector3(gC.handle_size, gC.handle_size, gC.handle_size)) *
                                           Matrix4x4.Rotate(Quaternion.AngleAxis(180f, Vector3.forward));

                    Graphics.DrawMesh(gC.arrw_giz, handleMatx, gC.bl_giz_mat, 0);
                }
                 
                if (gC.BL_Nodes[i, j].Can_LFT)
                {
                    //Vector3 arrowPos = gC.BL_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0f, 0);
                    //Handles.DrawSolidDisc(arrowPos, Vector3.forward, gC.handle_size * 0.35f);

                    Matrix4x4 handleMatx = Matrix4x4.Translate(gC.BL_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0)) *
                                           Matrix4x4.Scale(new Vector3(gC.handle_size, gC.handle_size, gC.handle_size)) *
                                           Matrix4x4.Rotate(Quaternion.AngleAxis(90f, Vector3.forward));

                    Graphics.DrawMesh(gC.arrw_giz, handleMatx, gC.bl_giz_mat, 0);
                }
                
                if (gC.BL_Nodes[i, j].Can_RGT)
                {
                    //Vector3 arrowPos = gC.BL_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0f, 0);
                    //Handles.DrawSolidDisc(arrowPos, Vector3.forward, gC.handle_size * 0.35f);

                    Matrix4x4 handleMatx = Matrix4x4.Translate(gC.BL_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0)) *
                                           Matrix4x4.Scale(new Vector3(gC.handle_size, gC.handle_size, gC.handle_size))*
                                           Matrix4x4.Rotate(Quaternion.AngleAxis(270f, Vector3.forward));

                    Graphics.DrawMesh(gC.arrw_giz, handleMatx, gC.bl_giz_mat, 0);
                }
            }
        }
        #endregion

        #region Render [Line] nodes
        for (int i = 0; i < gC.LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < gC.LI_Nodes.GetLength(1); ++j)
            {
                //Handles.color = li_colour;
                //Handles.DrawSolidDisc(gC.LI_Nodes[i, j].Position, Vector3.forward, gC.handle_size);

                Matrix4x4 handleMatx = Matrix4x4.Translate(gC.LI_Nodes[i, j].Position) *
                                       Matrix4x4.Scale(new Vector3(gC.handle_size, gC.handle_size, gC.handle_size) * 2);

                Graphics.DrawMesh(gC.node_giz, handleMatx, gC.li_giz_mat, 0);
            }
        }
        #endregion

        #region Render [Line] direction indications
        for (int i = 0; i < gC.LI_Nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < gC.LI_Nodes.GetLength(1); ++j)
            {
                //Handles.color = li_colour;
                if (gC.LI_Nodes[i, j].Can_UP)
                {
                    //Vector3 arrowPos = gC.LI_Nodes[i, j].Position + new Vector3(0f, gizmos_spacing, 0);
                    //Handles.DrawSolidDisc(arrowPos, Vector3.forward, gC.handle_size * 0.35f);

                    Matrix4x4 handleMatx = Matrix4x4.Translate(gC.LI_Nodes[i, j].Position + new Vector3(0, gizmos_spacing, 0)) *
                                           Matrix4x4.Scale(new Vector3(gC.handle_size, gC.handle_size, gC.handle_size));

                    Graphics.DrawMesh(gC.arrw_giz, handleMatx, gC.li_giz_mat, 0);
                }

                if (gC.LI_Nodes[i, j].Can_DN)
                {
                    //Vector3 arrowPos = gC.LI_Nodes[i, j].Position + new Vector3(0, -gizmos_spacing, 0);
                    //Handles.DrawSolidDisc(arrowPos, Vector3.forward, gC.handle_size * 0.35f);

                    Matrix4x4 handleMatx = Matrix4x4.Translate(gC.LI_Nodes[i, j].Position + new Vector3(0, - gizmos_spacing, 0)) *
                                           Matrix4x4.Scale(new Vector3(gC.handle_size, gC.handle_size, gC.handle_size)) *
                                           Matrix4x4.Rotate(Quaternion.AngleAxis(180f, Vector3.forward));

                    Graphics.DrawMesh(gC.arrw_giz, handleMatx, gC.li_giz_mat, 0);
                }

                if (gC.LI_Nodes[i, j].Can_LFT)
                {
                    //Vector3 arrowPos = gC.LI_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0);
                    //Handles.DrawSolidDisc(arrowPos, Vector3.forward, gC.handle_size * 0.35f);

                    Matrix4x4 handleMatx = Matrix4x4.Translate(gC.LI_Nodes[i, j].Position + new Vector3(-gizmos_spacing, 0, 0)) *
                                           Matrix4x4.Scale(new Vector3(gC.handle_size, gC.handle_size, gC.handle_size)) *
                                           Matrix4x4.Rotate(Quaternion.AngleAxis(90f, Vector3.forward));

                    Graphics.DrawMesh(gC.arrw_giz, handleMatx, gC.li_giz_mat, 0);
                }

                if (gC.LI_Nodes[i, j].Can_RGT)
                {
                    //Vector3 arrowPos = gC.LI_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0);
                    //Handles.DrawSolidDisc(arrowPos, Vector3.forward, gC.handle_size * 0.35f);

                    Matrix4x4 handleMatx = Matrix4x4.Translate(gC.LI_Nodes[i, j].Position + new Vector3(gizmos_spacing, 0, 0)) *
                                           Matrix4x4.Scale(new Vector3(gC.handle_size, gC.handle_size, gC.handle_size)) *
                                           Matrix4x4.Rotate(Quaternion.AngleAxis(270f, Vector3.forward));

                    Graphics.DrawMesh(gC.arrw_giz, handleMatx, gC.li_giz_mat, 0);
                }
            }
        }
        #endregion

        //#region Render [Block] edges
        //for (int i = 0; i < gC.BL_Edges.Count; ++i)
        //{
        //    Vector3 start_pos = gC.BL_Edges[i].Start_Node.Position;
        //    Vector3 end_pos = gC.BL_Edges[i].End_Node.Position;

        //    Handles.color = bl_colour;
        //    Handles.DrawDottedLine(start_pos, end_pos, 0.2f);
        //}
        //#endregion

        //#region Render [Line] edges
        //for (int i = 0; i < gC.LI_Edges.Count; ++i)
        //{
        //    Vector3 start_pos = gC.LI_Edges[i].Start_Node.Position;
        //    Vector3 end_pos = gC.LI_Edges[i].End_Node.Position;

        //    Handles.color = li_colour;
        //    Handles.DrawDottedLine(start_pos, end_pos, 0.2f);
        //}
        //#endregion

        //*! Reset the repaint request
        needRepaint = false;

    }
    #endregion

    //*!----------------------------!*//
    //*!    Custom Subclasses
    //*!----------------------------!*//

    #region Selection_Info subclass for storing temperary mouse behaviors
    //*! Subclass for storing temperary mouse behaviors
    //private class Selection_Info
    //{
    //    // Stores node or edge hovered info
    //    public int hoveredNodeIndex = -1;
    //    public int hoveredEdgeIndex = -1;
    //    public bool nodeHovered;
    //    public bool edgeHovered;

    //    // Stores node or edge selected info
    //    public bool edgeSelected;
    //    public bool nodeSelected;

    //    // Stores mousedrag start position
    //    public Vector3 dragStartPos;
    //    // Stores mousedrag end position
    //    public Vector3 dragEndPos;
    //}
    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//

    #region Unity Function Section
    //*! Operations on OnSceneGUI, get GUI event instance
    private void OnSceneGUI()
    {
        //*! Get current event instance
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.Repaint)
        {
            PrintHandles();
        }
        else if (guiEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            Edit_Graph(guiEvent);

            if (needRepaint == true)
            {
                HandleUtility.Repaint();
            }
        }
    }

    //!* Enable when launch
    void OnEnable()
    {
        //*! Inspected target as shapeCreator instance
        gC = target as Graph_Creator;
        //*! New selection info instance when launch
        //selectionInfo = new Selection_Info();
        //*! Initialize the graph once launch
        Initialize_Graph();
    }
    #endregion
}
