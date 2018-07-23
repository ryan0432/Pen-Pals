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
        Vector3 nodePos = new Vector3 ( (int)mousePos.x, (int)mousePos.y , (int)mousePos.z );

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            Undo.RecordObject(shapeCreator, "Add Node");
            Debug.Log("Nodes Added: " + nodePos);
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
