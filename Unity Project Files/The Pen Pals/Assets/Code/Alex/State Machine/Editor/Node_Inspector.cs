using UnityEditor;

//*! Short common used words
using EGL = UnityEditor.EditorGUILayout;
using GL = UnityEngine.GUILayout;


[CustomEditor(typeof(PlayerStateMachine_Line))]
public class Node_Inspector : Editor
{

    bool show_pivot_node = false;
    bool show_current_node = false;
    bool show_next_node = false;
    bool show_queued_node = false;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerStateMachine_Line line_player = (PlayerStateMachine_Line)target;

        if (line_player.show_debugger == true)
        {
            GL.Space(25);
            GL.Label("Node Debugger");
            GL.Space(25);


            GL.Label("Pivot Node");
            show_pivot_node = EGL.Foldout(show_pivot_node, "Show Pivot Node");
            if (show_pivot_node == true)
            {
                if (line_player.Pivot_Node != null)
                {

                    EGL.Vector3Field("Node Position", line_player.Pivot_Node.Position);

                    EGL.Toggle("Can UP", line_player.Pivot_Node.Can_UP);
                    EGL.Toggle("Can DN", line_player.Pivot_Node.Can_DN);
                    EGL.Toggle("Can LFT", line_player.Pivot_Node.Can_LFT);
                    EGL.Toggle("Can RGT", line_player.Pivot_Node.Can_RGT);
                }
                else
                {
                    GL.Label("Pivot Node == NULL");
                }

            }


            GL.Space(25);
            GL.Label("Current Node");
            show_current_node = EGL.Foldout(show_current_node, "Show Current Node");
            if (show_current_node == true)
            {
                if (line_player.Current_Node != null)
                {
                    EGL.Vector3Field("Node Position", line_player.Current_Node.Position);

                    EGL.Toggle("Can UP", line_player.Current_Node.Can_UP);
                    EGL.Toggle("Can DN", line_player.Current_Node.Can_DN);
                    EGL.Toggle("Can LFT", line_player.Current_Node.Can_LFT);
                    EGL.Toggle("Can RGT", line_player.Current_Node.Can_RGT);
                }
                else
                {
                    GL.Label("Current Node == NULL");
                }

            }


            GL.Space(25);
            GL.Label("Next Node");
            show_next_node = EGL.Foldout(show_next_node, "Show Next Node");
            if (show_next_node == true)
            {
                if (line_player.Next_Node != null)
                {
                    EGL.Vector3Field("Node Position", line_player.Next_Node.Position);

                    EGL.Toggle("Can UP", line_player.Next_Node.Can_UP);
                    EGL.Toggle("Can DN", line_player.Next_Node.Can_DN);
                    EGL.Toggle("Can LFT", line_player.Next_Node.Can_LFT);
                    EGL.Toggle("Can RGT", line_player.Next_Node.Can_RGT);
                }
                else
                {
                    GL.Label("Next Node == NULL");
                }

            }


            GL.Space(25);
            GL.Label("Queued Node");
            show_queued_node = EGL.Foldout(show_queued_node, "Show Queued Node");
            if (show_queued_node == true)
            {
                if (line_player.Queued_Node != null)
                {

                    EGL.Vector3Field("Node Position", line_player.Queued_Node.Position);

                    EGL.Toggle("Can UP", line_player.Queued_Node.Can_UP);
                    EGL.Toggle("Can DN", line_player.Queued_Node.Can_DN);
                    EGL.Toggle("Can LFT", line_player.Queued_Node.Can_LFT);
                    EGL.Toggle("Can RGT", line_player.Queued_Node.Can_RGT);

                }
                else
                {
                    GL.Label("Queued Node == NULL");
                }
            }


            GL.Space(50);
        }
    }
}


