//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//


//*! Using namespaces
using APS;
using UnityEngine;

public class PlayerController : Abstract_Player
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables
        
    [SerializeField]
    private Player_Type type;

    [SerializeField]
    private Controller Controller;
    
    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    public Temp_Node_Map Node_Graph;

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions
    private void Start()
    {
        //Controller.Up_Key = KeyCode.N;

        Player_Grid = Node_Graph;

    }

    private void Update()
    {
        //*! Block Update
        Check_Input(Player_Type.BLOCK);
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


    #endregion



    //*! Protected Access
    #region Protected Functions


    protected override bool Check_Input(Player_Type player_Type)
    {
        switch (player_Type)
        {
            case Player_Type.BLOCK:
                {
                    Block_Input(Controller);
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