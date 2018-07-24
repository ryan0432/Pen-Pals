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

    public KeyCode Get_Next_Input(Player_Type a_player_Type)
    {
        switch (a_player_Type)
        {
            case Player_Type.RED:
                return red.controls.next_input;
                //break;
            case Player_Type.BLUE:
                return blue.controls.next_input;
                //break;
            default:
                return KeyCode.None;
                //break;
        }
    }

    public KeyCode Get_Current_Input(Player_Type a_player_Type)
    {
        switch (a_player_Type)
        {
            case Player_Type.RED:
                return red.controls.current_input;
            //break;
            case Player_Type.BLUE:
                return blue.controls.current_input;
            //break;
            default:
                return KeyCode.None;
                //break;
        }
    }

    /// <summary>
    /// Grab the current input being pressed by the player based on the type
    /// </summary>
    /// <param name="a_player_type"></param>
    /// <returns></returns>
    public KeyCode Get_Now_Input(Player_Type a_player_type)
    {
        switch (a_player_type)
        {
            case Player_Type.RED:
                //*! RED Input Checks
                if (Input.GetKey(red.controls.move_up_key))
                {
                    return red.controls.move_up_key;
                }
                else if (Input.GetKey(red.controls.move_down_key))
                {
                    return red.controls.move_down_key;
                }
                else if (Input.GetKey(red.controls.move_left_key))
                {
                    return red.controls.move_left_key;
                }
                else if (Input.GetKey(red.controls.move_right_key))
                {
                    return red.controls.move_right_key;
                }
                else
                {
                    Debug.Log("no red controls found");
                }
                break;

            case Player_Type.BLUE:
                //*! BLUE Input Checks
                if (Input.GetKey(blue.controls.move_up_key))
                {
                    return blue.controls.move_up_key;
                }
                else if (Input.GetKey(blue.controls.move_down_key))
                {
                    return blue.controls.move_down_key;
                }
                else if (Input.GetKey(blue.controls.move_left_key))
                {
                    return blue.controls.move_left_key;
                }
                else if (Input.GetKey(blue.controls.move_right_key))
                {
                    return blue.controls.move_right_key;
                }
                else
                {
                    Debug.Log("no blue controls found");
                }
                break;

            default:
                Debug.Log("no player type found");
                break;
        }

        Debug.Log("no key press");
        return KeyCode.None;
                
    }


    /// <summary>
    /// Queues the next input
    /// </summary>
    /// <param name="a_player_type"></param>
    /// <param name="a_key_code"></param>
    public void Set_Player_Next_Input(Player_Type a_player_type, KeyCode a_key_code)
    {
        //Debug.Log("Setting the next input to be : " + a_key_code);

        switch (a_player_type)
        {
            case Player_Type.RED:
                red.controls.next_input = a_key_code;
                //Set_RED_Input(a_key_code);
                break;

            case Player_Type.BLUE:
                blue.controls.next_input = a_key_code;
                //Set_BLUE_Input(a_key_code);
                break;

            default:
                
                break;
        }
    }

    /// <summary>
    /// Move the player by the current input, this is only called when the player is less than 50% of the way between the start and end location.
    /// Based on what player calls for the function and what they press determines where the player goes
    /// </summary>
    /// <param name="a_player_type">-What player called this function-</param>
    /// <param name="a_key_code">-What keycode was pressed-</param>
    public void Move_Player_By_Next_Input(Player_Type a_player_type, KeyCode a_key_code)
    {

        switch (a_player_type)
        {
            case Player_Type.RED:
                //red.controls.current_input = a_key_code;
                RED_Move_By_Key(a_key_code);
                break;

            case Player_Type.BLUE:
                //blue.controls.current_input = a_key_code;
                BLUE_Move_By_Key(a_key_code);
                break;

            default:

                break;
        }
    }

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


    //*! Private Access
    #region Private Functions


    //*! Move the player in the direction based on the input the player gave. Move by set distance
    private void Move_Player_Direction(Player_Type a_player_type, Movement_Direction a_movement_Direction)
    {
        //*! Get a reference to the player object for 'red' and 'blue'
        red.player = Player.Instance;
        blue.player = Player.Instance;

        switch (a_player_type)
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
        {
            return false;
        }



        //*! RED Input Checks
        if (Input.GetKeyDown(red.controls.move_up_key))
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.UP);
            //*! Store the last key stroke
            red.controls.current_input = red.controls.move_up_key;
            return true;
        }
        else if (Input.GetKeyDown(red.controls.move_down_key))
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.DOWN);
            //*! Store the last key stroke
            red.controls.current_input = red.controls.move_down_key;
            return true;
        }
        else if (Input.GetKeyDown(red.controls.move_left_key))
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.LEFT);
            //*! Store the last key stroke
            red.controls.current_input = red.controls.move_left_key;
            return true;
        }
        else if (Input.GetKeyDown(red.controls.move_right_key))
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.RIGHT);
            //*! Store the last key stroke
            red.controls.current_input = red.controls.move_right_key;
            return true;
        }

        return false;
    }

    //*! Check if BLUE has inputed something
    private bool Get_BLUE_Input()
    {
        //*! Check if the player is falling, if they are then there is no control in the air.
        if (blue.is_falling)
        {
            return false;
        }



        //*! BLUE Input Checks
        if (Input.GetKeyDown(blue.controls.move_up_key))
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.UP);
            //*! Store the last key stroke
            blue.controls.current_input = blue.controls.move_up_key;
            return true;
        }
        else if (Input.GetKeyDown(blue.controls.move_down_key))
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.DOWN);
            //*! Store the last key stroke
            blue.controls.current_input = blue.controls.move_down_key;
            return true;
        }
        else if (Input.GetKeyDown(blue.controls.move_left_key))
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.LEFT);
            //*! Store the last key stroke
            blue.controls.current_input = blue.controls.move_left_key;
            return true;
        }
        else if (Input.GetKeyDown(blue.controls.move_right_key))
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.RIGHT);
            //*! Store the last key stroke
            blue.controls.current_input = blue.controls.move_right_key;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Move the player in the direction based on the players input
    /// </summary>
    /// <param name="a_key_code">-This is the keycode that is parsed in to check what was entered-</param>
    private void RED_Move_By_Key(KeyCode a_key_code)
    {
        //*! Clear the current input
        red.controls.next_input = KeyCode.None;

        //*! RED Input Checks
        if (red.controls.move_up_key == a_key_code)
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.UP);
            //*! Store the last key stroke
            red.controls.current_input = red.controls.move_up_key;
        }
        else if (red.controls.move_down_key == a_key_code)
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.DOWN);
            //*! Store the last key stroke
            red.controls.current_input = red.controls.move_down_key;
        }
        else if (red.controls.move_left_key == a_key_code)
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.LEFT);
            //*! Store the last key stroke
            red.controls.current_input = red.controls.move_left_key;
        }
        else if (red.controls.move_right_key == a_key_code)
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.RIGHT);
            //*! Store the last key stroke
            red.controls.current_input = red.controls.move_right_key;
        }
        
    }

    /// <summary>
    /// Move the player in the direction based on the players input
    /// </summary>
    /// <param name="a_key_code">-This is the keycode that is parsesd in to check what was entered-</param>
    private void BLUE_Move_By_Key(KeyCode a_key_code)
    {
        //*! Clear the current input
        blue.controls.next_input = KeyCode.None;

        //*! BLUE Input Checks
        if (blue.controls.move_up_key == a_key_code)
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.UP);
            //*! Store the last key stroke
            blue.controls.current_input = blue.controls.move_up_key;
        }
        else if (blue.controls.move_down_key == a_key_code)
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.DOWN);
            //*! Store the last key stroke
            blue.controls.current_input = blue.controls.move_down_key;
        }
        else if (blue.controls.move_left_key == a_key_code)
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.LEFT);
            //*! Store the last key stroke
            blue.controls.current_input = blue.controls.move_left_key;
        }
        else if (blue.controls.move_right_key == a_key_code)
        {
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.RIGHT);
            //*! Store the last key stroke
            blue.controls.current_input = blue.controls.move_right_key;
        }

    }




    #endregion



    //*! Inherited Access
    #region Protected Functions



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
    public Movement controls;
 


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
    public Movement controls;


    //*! Is the player falling - used for disabling the input (No air control)
    public bool is_falling;

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

    //*! Next key stroke
    public KeyCode next_input;

    //*! Current key stroke
    public KeyCode current_input;

}
 