//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for operating shape editor in edit mode
//*!
//*! Last edit  : 01/08/2018
//*!--------------------------------------------------------------!*//

//*! Using namespaces
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeCreator))]
public class ShapeEditor : Editor
{
    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    List<ShapeCreator> shapeGroups = new List<ShapeCreator>();
    SelectionInfo selectionInfo;
    ShapeCreator shapeCreator;
    bool needRepaint;


    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Method of custom handle input
    private void HandleInput(Event guiEvent)
    {
        //--- Gets mouse position in editor window ---//
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHight = 0;
        float dstToDrawPlane = (drawPlaneHight - mouseRay.origin.z) / mouseRay.direction.z;
        Vector3 mousePos = mouseRay.GetPoint(dstToDrawPlane);

        //--- Round [mousePos] to 0.5f --- //
        //Vector3 mousePosRounded = new Vector3(Math.Round(mousePos.x * 2) / 2,
        //                                      Math.Round(mousePos.y * 2) / 2,
        //                                      Math.Round(mousePos.z * 2) / 2);

        //--- Round [mousePos] to 1 ---//
        Vector3 mousePosRounded = RoundVec3(mousePos);

        // Security check for preventing mouse losing selected handle
        if(!selectionInfo.nodeSelected)
        {
            UpdateMouseSelection(mousePos);
        }

        //--- Boolean flags for checking mouse behaviors ---//
        bool drawLineInput = (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None && !selectionInfo.nodeHovered);
        bool erasLineInput = (guiEvent.type == EventType.MouseDrag && guiEvent.button == 1 && guiEvent.modifiers == EventModifiers.None);

        bool dragNodeStartInput = (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None && selectionInfo.nodeHovered);
        bool dragNodeDraggingInput = (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None && selectionInfo.nodeSelected);
        bool dragNodeEndInput = (guiEvent.type == EventType.MouseUp && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None && selectionInfo.nodeHovered);

        //--- Conditions for checking mouse behaviors ---//
        if (drawLineInput)
        {
            HandleDrawLine(mousePosRounded);
        }

        if (dragNodeStartInput)
        {
            DragNodeStart();
        }

        if (dragNodeDraggingInput)
        {
            DragNodeDragging(mousePos);
        }

        if (dragNodeEndInput)
        {
            DragNodeEnd();
        }

        if (erasLineInput)
        {
            for (int i = 0; i < shapeCreator.Nodes.Count; ++i)
            {

            }
        }
    }

    //*! Renders handles in editor window
    private void PrintHandles()
    {
        if (shapeCreator.Nodes.Count > 0)
        {
            for (int i = 0; i < shapeCreator.Nodes.Count; ++i)
            {
                if (i == selectionInfo.hoveredNodeIndex)
                {
                    Handles.color = (selectionInfo.nodeSelected) ? Color.yellow : Color.red;
                }
                else
                {
                    Handles.color = new Color(1, 0.5f, 0, 1);
                }

                Handles.DrawSolidDisc(shapeCreator.Nodes[i], Vector3.forward, shapeCreator.nodeRadius);
            }
        }

        if (shapeCreator.Edges.Count > 0)
        {
            for (int i = 1; i < shapeCreator.Nodes.Count; ++i)
            {
                Vector3 prevNode = shapeCreator.Nodes[i - 1];
                Vector3 currNode = shapeCreator.Nodes[i];
                Handles.color = Color.white;
                Handles.DrawDottedLine(prevNode, currNode, 0.2f);
            }
        }

        needRepaint = false;
    }

    //*! Update mouse selection info
    private void UpdateMouseSelection(Vector3 mousePos)
    {
        // Set [mouseOverNodeIndex] to -1 so it can be used to check if a node
        // is hovered or not. If a node is hovered, set this var into hovered
        // node index.
        int mouseOverNodeIndex = -1;

        // Loop over all nodes in node list
        for (int i = 0; i < shapeCreator.Nodes.Count; ++i)
        {
            // If mouse position is within node radius
            if (Vector3.Distance(mousePos, shapeCreator.Nodes[i]) <= shapeCreator.nodeRadius)
            {
                // Set [mouseOverNodeIndex] to current hovered node index
                mouseOverNodeIndex = i;
                // Break the loop
                break; 
            }
        }

        // If [hoveredNodeIndex] in [selectionInfo] is not the same as current mouse
        // hovered index
        if (mouseOverNodeIndex != selectionInfo.hoveredNodeIndex)
        {
            // Set [hoveredNodeIndex] to current mouse hovered node index
            selectionInfo.hoveredNodeIndex = mouseOverNodeIndex;

            // Set node is hovered in [selectionInfo] True/False by checking
            // if hovered index is -1 or not
            selectionInfo.nodeHovered = (selectionInfo.hoveredNodeIndex != -1);

            // Request a repaint event
            needRepaint = true;
        }
    }

    //*! Behavior of MLB down in editor window to draw a line
    private void HandleDrawLine(Vector3 mousePosRounded)
    {
        Undo.RecordObject(shapeCreator, "Add Node");

        if (shapeCreator.Nodes.Contains(mousePosRounded))
        {
            return;
        }

        shapeCreator.Nodes.Add(mousePosRounded);

        if (shapeCreator.Nodes.Count > 1)
        {
            Vector3 prevNode = shapeCreator.Nodes[shapeCreator.Nodes.Count - 2];
            Vector3 currNode = shapeCreator.Nodes[shapeCreator.Nodes.Count - 1];
            Vector3 edgePos = (currNode - prevNode) / 2;
            Vector3 edgeNormal = Vector3.Cross(Vector3.Normalize((currNode - prevNode)), Vector3.forward);
            shapeCreator.Edges.Add(edgePos);
            shapeCreator.EdgeNormals.Add(edgeNormal);

            Debug.Log("Edge Added: " + edgePos);
            Debug.Log("Edge Nomal: " + edgeNormal);
            Debug.Log("Edge Count: " + shapeCreator.Edges.Count);
        }

        Debug.Log("Nodes Added: " + mousePosRounded);
        Debug.Log("Node Count: " + shapeCreator.Nodes.Count);

        selectionInfo.nodeSelected = false;
        needRepaint = true;
    }

    //*! Behavior of mouse selecting a node to drag with MLB in editor window
    private void DragNodeStart()
    {
        selectionInfo.nodeSelected = true;
        needRepaint = true;
    }

    //*! Behavior of dragging an existing node with MLB in editor window
    private void DragNodeDragging(Vector3 mousePos)
    {
        shapeCreator.Nodes[selectionInfo.hoveredNodeIndex] = mousePos;
        needRepaint = true;
    }
    //*! Behavior of ending dragging a node with MLB up
    private void DragNodeEnd()
    {
        selectionInfo.nodeSelected = false;
        shapeCreator.Nodes[selectionInfo.hoveredNodeIndex] =
            RoundVec3(shapeCreator.Nodes[selectionInfo.hoveredNodeIndex]);
        needRepaint = true;
    }

    //*! Tool method that round inputted Vector3
    private Vector3 RoundVec3(Vector3 inVec3)
    {
        Vector3 outVec3 = new Vector3 (Mathf.Round(inVec3.x),
                                       Mathf.Round(inVec3.y),
                                       Mathf.Round(inVec3.z));
        return outVec3;
    }

    //!* Subclass to store selection/hovering behavior infos
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


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//

    //*! Operations on OnSceneGUI, get GUI event instance
    private void OnSceneGUI()
    {
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
            HandleInput(guiEvent);

            if (needRepaint == true)
            {
                HandleUtility.Repaint();
            }
        }
    }

    //!* Enable when launch
    void OnEnable()
    {
        // Inspected target as shapeCreator instance
        shapeCreator = target as ShapeCreator;
        // New selection info instance when launch
        selectionInfo = new SelectionInfo();
    }
}
 