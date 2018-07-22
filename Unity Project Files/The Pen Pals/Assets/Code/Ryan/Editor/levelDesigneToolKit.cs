using UnityEngine;
using UnityEditor;

public class levelDesigneToolKit : EditorWindow
{
    //public Texture icon_Highlighter;

    [MenuItem("Window/Level Design Tool Kit")]
    public static void showLevelDesignToolKitWindow()
    {
        GetWindow<levelDesigneToolKit>("Level Design Tool Kit");
    }

    public void OnGUI()
    {
        GUILayout.Label("Highlighter", EditorStyles.boldLabel);
        //if (guilayout.button(icon_highlighter))
        //{

        //}
    }
}
