using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeCreator))]
public class ShapeEditor : Editor
{
    ShapeCreator shapeCreator;
    bool needRepaint;

    private void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.Repaint)
        {
            printHandles();
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
        Vector3 nodePos = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), Mathf.Round(mousePos.z));

        //HandleUtility.CalcLineTranslation
        

        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            Undo.RecordObject(shapeCreator, "Add Node");

            if (shapeCreator.Nodes.Contains(nodePos))
            {
                return;
            }

            shapeCreator.Nodes.Add(nodePos);

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

            Debug.Log("Nodes Added: " + nodePos);
            Debug.Log("Node Count: " + shapeCreator.Nodes.Count);
            needRepaint = true;
        }

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

    private void printHandles()
    {
        if (shapeCreator.Nodes.Count > 0)
        {
            for (int i = 0; i < shapeCreator.Nodes.Count; ++i)
            {
                Handles.color = new Color(1, 0.5f, 0, 1 );
                Handles.DrawSolidDisc(shapeCreator.Nodes[i], Vector3.forward, 0.2f);
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

    private void OnEnable()
    {
        shapeCreator = target as ShapeCreator;
    }
}
