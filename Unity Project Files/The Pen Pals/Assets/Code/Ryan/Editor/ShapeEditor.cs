using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeCreator))]
public class ShapeEditor : Editor
{
    ShapeCreator shapeCreator;

    private void OnSceneGUI()
    {
        Event guiEvent = Event.current;
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHight = 0;
        float dstToDrawPlane = (drawPlaneHight - mouseRay.origin.z) / mouseRay.direction.z;
        Vector3 mousePos = mouseRay.GetPoint(dstToDrawPlane);
        Vector3 nodePos = new Vector3 (Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), Mathf.Round(mousePos.z));

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            Undo.RecordObject(shapeCreator, "Add Node");
            shapeCreator.Nodes.Add(nodePos);
            Debug.Log("Nodes Added: " + nodePos);
            Debug.Log(shapeCreator.Nodes.Count);
        }

        for (int i = 0; i < shapeCreator.Nodes.Count; ++i)
        {
            Handles.DrawSolidDisc(shapeCreator.Nodes[i], Vector3.forward, 0.2f);
        }

        if (guiEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

    }

    private void OnEnable()
    {
        shapeCreator = target as ShapeCreator;
    }
}
