//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;


public class PlayerInteraction : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    

    [SerializeField]
    private RED red = new RED();

    [SerializeField]
    private BLUE blue = new BLUE();

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    public enum Player_Type
    {
        RED,
        BLUE
    }

    public enum Movement_Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    //*! Player Interaction Singleton
    public static PlayerInteraction Instance;

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

    }
 
    private void Update()
    {
 
    }



    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions


    #endregion



    //*! Private Access
    #region Private Functions
    

    //*! Move the player in the direction based on the input the player gave. Move by set distance
    private void Move_Player_Direction(Player_Type a_player_Type, Movement_Direction a_movement_Direction)
    {
        //*! Get a reference to the player object for 'red' and 'blue'
        red.player = Player.Instance;
        blue.player = Player.Instance;

        switch (a_player_Type)
        {
            case Player_Type.RED:

                //*! Apply the movement to the player - red
                switch (a_movement_Direction)
                {
                    case Movement_Direction.UP:
                        //+- Y Axis
                        red.player.current_position += new Vector2(0, red.player.movement_distance);
                        break;
                    case Movement_Direction.DOWN:
                        //-- Y Axis
                        red.player.current_position -= new Vector2(0, red.player.movement_distance);
                        break;
                    case Movement_Direction.LEFT:
                        //-- X Axis
                        red.player.current_position -= new Vector2(red.player.movement_distance, 0);
                        break;
                    case Movement_Direction.RIGHT:
                        //+- X Axis
                        red.player.current_position += new Vector2(red.player.movement_distance, 0);
                        break;
                    default:
                        break;
                }

                break;


            case Player_Type.BLUE:

                //*! Apply the movement to the player - blue
                switch (a_movement_Direction)
                {
                    case Movement_Direction.UP:
                        //+- Y Axis
                        blue.player.current_position += new Vector2(0, blue.player.movement_distance);
                        break;
                    case Movement_Direction.DOWN:
                        //-- Y Axis
                        blue.player.current_position -= new Vector2(0, blue.player.movement_distance);
                        break;
                    case Movement_Direction.LEFT:
                        //-- X Axis
                        blue.player.current_position -= new Vector2(blue.player.movement_distance, 0);
                        break;
                    case Movement_Direction.RIGHT:
                        //+- X Axis
                        blue.player.current_position += new Vector2(blue.player.movement_distance, 0);
                        break;
                    default:
                        break;
                }

                break;

                
            default:
                break;
        }
    }

    //*! Check if RED has inputed something
    private bool Get_RED_Input()
    {
        //*! Check if the player is falling, if they are then there is no control in the air.
        if (red.is_falling)
            return false;


        //*! RED Input Checks
        if (Input.GetKeyDown(red.Controls.move_up_key))
        {
            Move_Player_Direction(Player_Type.RED, Movement_Direction.UP);
            return true;
        }
        else if (Input.GetKeyDown(red.Controls.move_down_key))
        {
            Move_Player_Direction(Player_Type.RED, Movement_Direction.DOWN);
            return true;
        }
        else if (Input.GetKeyDown(red.Controls.move_left_key))
        {
            Move_Player_Direction(Player_Type.RED, Movement_Direction.LEFT);
            return true;
        }
        else if (Input.GetKeyDown(red.Controls.move_right_key))
        {
            Move_Player_Direction(Player_Type.RED, Movement_Direction.RIGHT);
            return true;
        }

        return false;
    }

    //*! Check if BLUE has inputed something
    private bool Get_BLUE_Input()
    {
        //*! BLUE Input Checks
        if (Input.GetKeyDown(blue.Controls.move_up_key))
        {
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.UP);
            return true;
        }
        else if (Input.GetKeyDown(blue.Controls.move_down_key))
        {
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.DOWN);
            return true;
        }
        else if (Input.GetKeyDown(blue.Controls.move_left_key))
        {
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.LEFT);
            return true;
        }
        else if (Input.GetKeyDown(blue.Controls.move_right_key))
        {
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.RIGHT);
            return true;
        }

        return false;
    }

    #endregion



    //*! Inherited Access
    #region Protected Functions


    //*! Checking for player input
    public bool Check_For_Input(Player_Type a_player_type)
    {
        switch (a_player_type)
        {
            case Player_Type.RED:
                return Get_RED_Input();
                ///break;
            case Player_Type.BLUE:
                return Get_BLUE_Input();
                ///break;
            default:
                return false;
                ///break;
        }

        ///return false;
    }

    #endregion


}


/// <summary>
/// RED Class Object
/// </summary>
[System.Serializable]
public class RED
{
    [HideInInspector]
    //*! Player Class Reference
    public Player player = Player.Instance;

    [SerializeField]
    //*! Controls
    private Movement controls;

    public Movement Controls
    { get { return controls; } }


    //*! Is the player falling - used for disabling the input (No air control)
    public bool is_falling;



}

/// <summary>
/// BLUE Class Object
/// </summary>
[System.Serializable]
public class BLUE
{

    [HideInInspector]
    //*! Player Class Reference
    public Player player = Player.Instance;

    [SerializeField]
    //*! Controls
    private Movement controls;

    public Movement Controls
    { get { return controls; } }

}

/// <summary>
/// Universal Controls for both players
/// </summary>
[System.Serializable]
public struct Movement
{

    //*! Controls
    public KeyCode move_up_key;
    public KeyCode move_down_key;
    public KeyCode move_right_key;
    public KeyCode move_left_key;
 
 

}