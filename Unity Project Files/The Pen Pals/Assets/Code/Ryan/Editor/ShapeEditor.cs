//*!----------------------------!*//
//*! Programmer: Ryan Chung
//*!----------------------------!*//

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


    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Method of custom handle input
    private void HandleInput(Event guiEvent)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHight = 0;
        float dstToDrawPlane = (drawPlaneHight - mouseRay.origin.z) / mouseRay.direction.z;
        Vector3 mousePos = mouseRay.GetPoint(dstToDrawPlane);

        //--- Round to 0.5f --- //
        //Vector3 nodePos = new Vector3(Math.Round(mousePos.x * 2) / 2,
        //                              Math.Round(mousePos.y * 2) / 2,
        //                              Math.Round(mousePos.z * 2) / 2);

        //--- Round to 1 ---//
        Vector3 mousePosRounded = RoundVec3(mousePos);

        // Security check for preventing mouse losing selected handle
        if(!selectionInfo.nodeSelected)
        {
            UpdateMouseSelection(mousePos);
        }

        // Boolean flags for checking mouse behaviors
        bool drawLineInput = (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None && !selectionInfo.nodeHovered);
        bool erasLineInput = (guiEvent.type == EventType.MouseDrag && guiEvent.button == 1 && guiEvent.modifiers == EventModifiers.None);

        bool dragNodeStartInput = (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None && selectionInfo.nodeHovered);
        bool dragNodeDraggingInput = (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None && selectionInfo.nodeSelected);
        bool dragNodeEndInput = (guiEvent.type == EventType.MouseUp && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None && selectionInfo.nodeHovered);

        // Conditions for checking mouse behaviors
        if (drawLineInput)
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

        if (erasLineInput)
        {
            for (int i = 0; i < shapeCreator.Nodes.Count; ++i)
            {

            }
        }

        if (dragNodeStartInput)
        {
            selectionInfo.nodeSelected = true;
            needRepaint = true;
        }

        if (dragNodeDraggingInput)
        {
            shapeCreator.Nodes[selectionInfo.hoveredNodeIndex] = mousePos;
            needRepaint = true;
        }

        if (dragNodeEndInput)
        {
            selectionInfo.nodeSelected = false;
            shapeCreator.Nodes[selectionInfo.hoveredNodeIndex] = RoundVec3(shapeCreator.Nodes[selectionInfo.hoveredNodeIndex]);
            needRepaint = true;
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

    //*! Behavior of mouse left button down in editor window
    private void HandleLeftMouseDown(Vector3 mousePos)
    {
        
    }

    //*! Behavior of mouse left button up in editor window
    private void HandleLeftMouseUp(Vector3 mousePos)
    {

    }

    //*! Behavior of dragging a existing node with mouse in editor window
    private void DragSelectedNode(Vector3 mousePos)
    {

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

    //!* Enable when launch
    void OnEnable()
    {
        // Inspected target as shapeCreator instance
        shapeCreator = target as ShapeCreator;
        // New selection info instance when launch
        selectionInfo = new SelectionInfo();
    }
}
 