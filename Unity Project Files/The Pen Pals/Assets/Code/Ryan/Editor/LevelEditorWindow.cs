using UnityEngine;
using UnityEditor;

enum CurrTool
{
    HIGHLIGHTER,
    TERRAIN,
    PENCIL,
    COLOUR_PENCIL,
    MOVING_TERRAIN
}
public class LevelEditorWindow : EditorWindow
{
    [SerializeField]
    private Texture icon_Highlighter;
    [SerializeField]
    private Texture icon_Terrain;
    [SerializeField]
    private Texture icon_Pencil;
    [SerializeField]
    private Texture icon_ColourPencil;
    [SerializeField]
    private Texture icon_MovingTerrain;

    private CurrTool currTool;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Level Editor")]
    static void Init()
    {
        LevelEditorWindow window = (LevelEditorWindow)GetWindow(typeof(LevelEditorWindow), false, "Level Editor");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Highlighter", EditorStyles.boldLabel);
        if (GUILayout.Button(icon_Highlighter, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64)))
        {
            currTool = CurrTool.HIGHLIGHTER;
        }

        GUILayout.Label("Terrain", EditorStyles.boldLabel);
        if (GUILayout.Button(icon_Terrain, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64)))
        {
            currTool = CurrTool.TERRAIN;
        }

        GUILayout.Label("Pencil", EditorStyles.boldLabel);
        if (GUILayout.Button(icon_Pencil, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64)))
        {
            currTool = CurrTool.PENCIL;
        }

        GUILayout.Label("Colour Pencil", EditorStyles.boldLabel);
        if (GUILayout.Button(icon_ColourPencil, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64)))
        {
            currTool = CurrTool.COLOUR_PENCIL;
        }

        GUILayout.Label("Moving Terrain", EditorStyles.boldLabel);
        if (GUILayout.Button(icon_MovingTerrain, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64)))
        {
            currTool = CurrTool.MOVING_TERRAIN;
        }
    }

    CurrTool GetCurrTool() { return currTool; }
}