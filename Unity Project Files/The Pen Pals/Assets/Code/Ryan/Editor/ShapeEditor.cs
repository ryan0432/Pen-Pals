using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeCreator))]
public class ShapeEditor : Editor
{
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

    void HandleInput(Event guiEvent)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHight = 0;
        float dstToDrawPlane = (drawPlaneHight - mouseRay.origin.z) / mouseRay.direction.z;
        Vector3 mousePos = mouseRay.GetPoint(dstToDrawPlane);

        //--- Round to 0.5f --- //
        //Vector3 nodePos = new Vector3((float)Math.Round(mousePos.x * 2) / 2,
        //                              (float)Math.Round(mousePos.y * 2) / 2,
        //                              (float)Math.Round(mousePos.z * 2) / 2);
        
        //--- Round to 1 ---//
        Vector3 mousePosRounded = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), Mathf.Round(mousePos.z));

        bool drawLineKeyInput = (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None && !selectionInfo.nodeSelected);
        bool erasLineKeyInput = (guiEvent.type == EventType.MouseDrag && guiEvent.button == 1 && guiEvent.modifiers == EventModifiers.None && selectionInfo.nodeHovered);
        bool dragNodeKeyInput = (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None && selectionInfo.nodeSelected);

        if (drawLineKeyInput)
        {
            HandleLeftMouseDown(mousePosRounded);
        }

        if (erasLineKeyInput)
        {
            for (int i = 0; i < shapeCreator.Nodes.Count; ++i)
            {

            }
        }

        if (dragNodeKeyInput)
        {
            HandleLeftMouseDown(mousePosRounded);
        }

        UpdateMouseSelection(mousePos);



        //if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        //{
        //    Undo.RecordObject(shapeCreator, "Add Node");
        //    shapeCreator.Nodes.Add(nodePos);

        //    if (shapeCreator.Nodes.Count > 1)
        //    {
        //        Vector3 prevNode = shapeCreator.Nodes[shapeCreator.Nodes.Count - 2];
        //        Vector3 currNode = shapeCreator.Nodes[shapeCreator.Nodes.Count - 1];
        //        Vector3 edgePos = (currNode - prevNode) / 2;
        //        Vector3 edgeNormal = Vector3.Cross(Vector3.Normalize((currNode - prevNode)), Vector3.forward);
        //        shapeCreator.Edges.Add(edgePos);
        //        shapeCreator.EdgeNormals.Add(edgeNormal);

        //        Debug.Log("Edge Added: " + edgePos);
        //        Debug.Log("Edge Nomal: " + edgeNormal);
        //        Debug.Log("Edge Count: " + shapeCreator.Edges.Count);
        //    }

        //    Debug.Log("Nodes Added: " + nodePos);
        //    Debug.Log("Node Count: " + shapeCreator.Nodes.Count);
        //    needRepaint = true;
        //}
    }

    void PrintHandles()
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

    void UpdateMouseSelection(Vector3 mousePos)
    {
        int mouseOverNodeIndex = -1;
        for (int i = 0; i < shapeCreator.Nodes.Count; ++i)
        {
            if (Vector3.Distance(mousePos, shapeCreator.Nodes[i]) <= shapeCreator.nodeRadius)
            {
                mouseOverNodeIndex = i;
                break;
            }
        }

        if (mouseOverNodeIndex != selectionInfo.hoveredNodeIndex)
        {
            selectionInfo.hoveredNodeIndex = mouseOverNodeIndex;
            selectionInfo.nodeHovered = (selectionInfo.hoveredNodeIndex != -1);
            needRepaint = true;
        }

    }

    void HandleLeftMouseDown(Vector3 mousePos)
    {
        if (!selectionInfo.nodeHovered)
        {
            Undo.RecordObject(shapeCreator, "Add Node");

            if (shapeCreator.Nodes.Contains(mousePos))
            {
                return;
            }

            shapeCreator.Nodes.Add(mousePos);

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

            Debug.Log("Nodes Added: " + mousePos);
            Debug.Log("Node Count: " + shapeCreator.Nodes.Count);
        }

        selectionInfo.nodeSelected = true;
        needRepaint = true;
    }

    void HandleLeftMouseUp(Vector3 mousePos)
    {
        if (selectionInfo.nodeSelected)
        {
            selectionInfo.nodeSelected = false;
            selectionInfo.hoveredNodeIndex = -1;
            needRepaint = true;
        }
    }

    void DragSelectedNode(Vector3 mousePos)
    {
        if (selectionInfo.nodeSelected)
        {
            shapeCreator.Nodes[selectionInfo.hoveredNodeIndex] = mousePos;
            needRepaint = true;
        }
    }

    //Enable when launch
    void OnEnable()
    {
        //Inspected target as shapeCreator instance
        shapeCreator = target as ShapeCreator;
        //New selection info instance when launch
        selectionInfo = new SelectionInfo();
    }

    //Subclass to store selection/hovering behavior infos
    private class SelectionInfo
    {
        //Store node or edge hovered info
        public int hoveredNodeIndex = -1;
        public int hoveredEdgeIndex = -1;
        public bool nodeHovered;
        public bool edgeHovered;

        //Store node or edge selected info
        public bool edgeSelected;
        public bool nodeSelected;

        //Store mousedrag start position
        public Vector3 dragStartPos;
        //Store mousedrag end position
        public Vector3 dragEndPos;
    }
}
 