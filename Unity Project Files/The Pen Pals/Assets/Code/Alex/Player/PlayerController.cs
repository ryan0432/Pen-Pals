//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//


//*! Using namespaces
using APS;
using UnityEngine;

/// <summary>
/// Main Player Controller
/// </summary>
public class PlayerController : Abstract_Player
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables
        
    //*! What type the player is. BLOCK OR LINE
    [SerializeField]
    private Player_Type type;

    //*! Used to pass the current players controller key information
    [SerializeField]
    private Controller Controller;

    
    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables
    
    //*! Handball the graph to the parent class
    public Temp_Node_Map Node_Graph;

    //*! Current grid Position
    public Vector2 grid_position;


    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions
    private void Start()
    {
        //*! Handball
        Player_Grid = Node_Graph;
    }


    private void Update()
    {
        //*! Player Update, was something pressed.
        if (Check_Input(type) == true)
        {
            
        }
        
    }

    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions


    #endregion



    //*! Private Access
    #region Private Functions
    
    private void Apply_New_Position(Temp_Node_Map.Node end_position)
    {
        // do it
    }
   
    #endregion



    //*! Protected Access
    #region Protected Functions


    protected override bool Check_Input(Player_Type player_Type)
    {
        switch (player_Type)
        {
            case Player_Type.BLOCK:
                {
                    Apply_New_Position(Block_Input(Controller, grid_position));
                }
                break;
            case Player_Type.LINE:
                {
                    Line_Input(Controller);
                }
                break;
            default:

                break;
        }

        return base.Check_Input(player_Type);

    }

    #endregion


}