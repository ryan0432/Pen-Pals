//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//

//*! Using namespaces
using UnityEngine;

//*! Player Interaction
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

    //*! Player References
    public RED Red
    { get { return red; } }

    public BLUE Blue
    { get { return blue; } }


    //*! Player Interaction Singleton
    public static PlayerInteraction Instance;

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions

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

    #endregion


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
                Debug.Log("No, this can't be right...");
                return KeyCode.None;
                //break;
        }
    }

    //*! After a slight re-work this is not needed, but here if it is needed for later
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
    public void Queue_Next_Input(Player_Type a_player_type)
    {
        switch (a_player_type)
        {
            case Player_Type.RED:
                
                //*! RED Input Checks
                if (Input.GetKey(red.controls.move_up_key))
                {
                    red.controls.next_input = red.controls.move_up_key;
                }
                else if (Input.GetKey(red.controls.move_down_key))
                {
                    red.controls.next_input = red.controls.move_down_key;
                }
                else if (Input.GetKey(red.controls.move_left_key))
                {
                    red.controls.next_input = red.controls.move_left_key;
                }
                else if (Input.GetKey(red.controls.move_right_key))
                {
                    red.controls.next_input = red.controls.move_right_key;
                }
                else
                {
                    //*! All above checks have to result to false before getting here
                    ///Debug.Log("no red next input made : " + red.controls.next_input);
                    red.controls.next_input = KeyCode.None;
                }

                //*! Double Jumping Check
                if(Double_Jumping(Player_Type.RED))
                {
                    //red.controls.next_input = KeyCode.None;
                    return;
                }

                if (Long_Jumping(Player_Type.RED))
                {
                    //red.controls.next_input = KeyCode.None;
                    return;
                }

                break;

            case Player_Type.BLUE:

                //*! BLUE Input Checks
                if (Input.GetKey(blue.controls.move_up_key))
                {
                    blue.controls.next_input = blue.controls.move_up_key;
                }
                else if (Input.GetKey(blue.controls.move_down_key))
                {
                    blue.controls.next_input = blue.controls.move_down_key;
                }
                else if (Input.GetKey(blue.controls.move_left_key))
                {
                    blue.controls.next_input = blue.controls.move_left_key;
                }
                else if (Input.GetKey(blue.controls.move_right_key))
                {
                    blue.controls.next_input = blue.controls.move_right_key;
                }
                else
                {
                    //*! All above checks have to result to false before getting here
                    ///Debug.Log("no blue next input made : " + red.controls.next_input);
                    blue.controls.next_input = KeyCode.None;
                }

                //*! Double Jumping Check
                if(Double_Jumping(Player_Type.BLUE))
                {
                    //blue.controls.next_input = KeyCode.None;
                    return;
                }

                if (Long_Jumping(Player_Type.BLUE))
                {
                    //blue.controls.next_input = KeyCode.None;
                    return;
                }

                break;

            default:
                Debug.Log("no player type found");
                break;
        }
                 
    }


    /// <summary>
    /// Clears the current input based on the current player.
    /// </summary>
    /// <param name="a_player_type"></param>
    public void Clear_Current_Input(Player_Type a_player_type)
    {
         switch (a_player_type)
        {
            case Player_Type.RED:
                red.controls.current_input = KeyCode.None;
                break;

            case Player_Type.BLUE:
                blue.controls.current_input = KeyCode.None;
                break;

            default:
                break;
        }
                
    }

    /// <summary>
    /// Move the player by the current input, this is only called when the player is less than 90% of the way between the start and end location.
    /// Based on what player calls for the function and what they press determines where the player goes
    /// </summary>
    /// <param name="a_player_type">-What player called this function-</param>
    /// <param name="a_next_input_key">-What keycode was pressed-</param>
    public void Move_Player_By_Next_Input(Player_Type a_player_type, KeyCode a_next_input_key)
    {

        switch (a_player_type)
        {
            case Player_Type.RED:
                //red.controls.current_input = a_key_code;
                RED_Move_By_Key(a_next_input_key);
                break;

            case Player_Type.BLUE:
                //blue.controls.current_input = a_key_code;
                BLUE_Move_By_Key(a_next_input_key);
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

    /// <summary>
    /// Return the distance remaining based on the player type, divided by 100 and casted to a float
    /// </summary>
    /// <param name="a_player_type"></param>
    /// <returns>-Of type float for the comparison check in player.-</returns>
    public float Get_Distance_Remaining(Player_Type a_player_type)
    {
        //*! Based on the player type, return the appropirate value
        switch (a_player_type)
        {
            case Player_Type.RED:
                return (float)red.distance_remaining / 100;
                ///break;
            case Player_Type.BLUE:
                return (float)blue.distance_remaining / 100;
                ///break;
            default:
                return 0.0f;
                ///break;
        }
    }

    /// <summary>
    /// Based on the player type it will disable the double jumping for that player.
    /// </summary>
    /// <param name="a_player_type"></param>
    /// <returns>Is the player Double Jumping</returns>
    public bool Double_Jumping(Player_Type a_player_type)
    {
        switch (a_player_type)
        {
            case Player_Type.RED:
                if (red.controls.current_input == red.controls.move_up_key &&
                    red.controls.next_input == red.controls.move_up_key)
                {
                    //*! Clear the next input
                    red.controls.next_input = KeyCode.None;
                }
                return true;
                //break;
            case Player_Type.BLUE:
                if (blue.controls.current_input == blue.controls.move_up_key &&
                   blue.controls.next_input == blue.controls.move_up_key)
                {
                    //*! Clear the next input
                    blue.controls.next_input = KeyCode.None;
                }
                return true;
                //break;
            default:
                break;
        }
        return false;
    }

    /// <summary>
    /// If the player is in the air and try to long jump, clear the next input;
    /// </summary>
    /// <param name="a_player_type"></param>
    /// <returns>Is the player long jumping</returns>
    public bool Long_Jumping(Player_Type a_player_type)
    {
        switch (a_player_type)
        {
            case Player_Type.RED:
                //*! Not Grounded (in air) Then disable the long jump
                if (!red.is_grounded)
                {
                    //*! Double Left
                    if (red.controls.current_input == red.controls.move_left_key &&
                        red.controls.next_input == red.controls.move_left_key)
                    {
                        //*! Clear the next input
                        red.controls.next_input = KeyCode.None;
                    }
                    //*! Double Right
                    if (red.controls.current_input == red.controls.move_right_key &&
                             red.controls.next_input == red.controls.move_right_key)
                    {
                        //*! Clear the next input
                        red.controls.next_input = KeyCode.None;
                    }
                    //*! Not Grounded
                    return true;
                }
                else
                {
                    //*! Is Grounded
                    return false;
                }
            //break;
            case Player_Type.BLUE:
                //*! Not Grounded (in air) Then disable the long jump
                if (!blue.is_grounded)
                {
                    //*! Double Left
                    if (blue.controls.current_input == blue.controls.move_left_key &&
                       blue.controls.next_input == blue.controls.move_left_key)
                    {
                        //*! Clear the next input
                        blue.controls.next_input = KeyCode.None;
                    }
                    //*! Double Right
                    if (blue.controls.current_input == blue.controls.move_right_key &&
                            blue.controls.next_input == blue.controls.move_right_key)
                    {
                        //*! Clear the next input
                        blue.controls.next_input = KeyCode.None;
                    }
                    //*! Not Grounded
                    return true;
                }
                else
                {
                    //*! Is Grounded
                    return false;
                }
        //break;
        default:
                break;
        }
        return false;
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
        ////*! Check if the player is falling, if they are then there is no control in the air.
        //if (red.sub.type == Sub_Type.SubType.BLOCK && red.is_grounded == false)
        //{
        //    return true;
        //}
        
        //*! RED Input Checks
        if (Input.GetKeyDown(red.controls.move_up_key))
        {
            //*! Store the last key stroke
            red.controls.current_input = red.controls.move_up_key;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.UP);
            return true;
        }
        else if (Input.GetKeyDown(red.controls.move_down_key))
        {
            //*! Store the last key stroke
            red.controls.current_input = red.controls.move_down_key;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.DOWN);
            return true;
        }
        else if (Input.GetKeyDown(red.controls.move_left_key))
        {
            //*! Store the last key stroke
            red.controls.current_input = red.controls.move_left_key;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.LEFT);
            return true;
        }
        else if (Input.GetKeyDown(red.controls.move_right_key))
        {
            //*! Store the last key stroke
            red.controls.current_input = red.controls.move_right_key;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.RIGHT);
            return true;
        }

        return false;
    }

    //*! Check if BLUE has inputed something
    private bool Get_BLUE_Input()
    {
        ////*! Check if the player is falling, if they are then there is no control in the air.
        //if (blue.sub.type == Sub_Type.SubType.BLOCK  &&  blue.is_grounded == false)
        //{
        //    return true;
        //}



        //*! BLUE Input Checks
        if (Input.GetKeyDown(blue.controls.move_up_key))
        {
            //*! Store the last key stroke
            blue.controls.current_input = blue.controls.move_up_key;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.UP);
            return true;
        }
        else if (Input.GetKeyDown(blue.controls.move_down_key))
        {
            //*! Store the last key stroke
            blue.controls.current_input = blue.controls.move_down_key;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.DOWN);
            return true;
        }
        else if (Input.GetKeyDown(blue.controls.move_left_key))
        {
            //*! Store the last key stroke
            blue.controls.current_input = blue.controls.move_left_key;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.LEFT);
            return true;
        }
        else if (Input.GetKeyDown(blue.controls.move_right_key))
        {
            //*! Store the last key stroke
            blue.controls.current_input = blue.controls.move_right_key;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.RIGHT);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Move the player in the direction based on the players input
    /// </summary>
    /// <param name="a_next_input_code">-This is the keycode that is passed in to check what was entered-</param>
    private void RED_Move_By_Key(KeyCode a_next_input_code)
    {


        //*! RED Input Checks
        if (red.controls.move_up_key == a_next_input_code)
        {
            //*! Store the last key stroke
            red.controls.current_input = a_next_input_code;
            //*! Clear the next key code
            red.controls.next_input = KeyCode.None;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.UP);
        }
        else if (red.controls.move_down_key == a_next_input_code)
        {
            //*! Store the last key stroke
            red.controls.current_input = a_next_input_code;
            //*! Clear the next key code
            red.controls.next_input = KeyCode.None;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.DOWN);
        }
        else if (red.controls.move_left_key == a_next_input_code)
        {
            //*! Store the last key stroke
            red.controls.current_input = a_next_input_code;
            //*! Clear the next key code
            red.controls.next_input = KeyCode.None;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.LEFT);
        }
        else if (red.controls.move_right_key == a_next_input_code)
        {
            //*! Store the last key stroke
            red.controls.current_input = a_next_input_code;
            //*! Clear the next key code
            red.controls.next_input = KeyCode.None;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.RED, Movement_Direction.RIGHT);
        }
        else
        {
            Debug.LogError("No next key match!");
        }

        //*! Clear the current input when done with above
        //red.controls.next_input = KeyCode.None;

    }

    /// <summary>
    /// Move the player in the direction based on the players input
    /// </summary>
    /// <param name="a_next_input_code">-This is the keycode that is parsesd in to check what was entered-</param>
    private void BLUE_Move_By_Key(KeyCode a_next_input_code)
    {

        //*! BLUE Input Checks
        if (blue.controls.move_up_key == a_next_input_code)
        {
            //*! Store the last key stroke
            blue.controls.current_input = a_next_input_code;
            //*! Clear the next key code
            blue.controls.next_input = KeyCode.None;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.UP);
        }
        else if (blue.controls.move_down_key == a_next_input_code)
        {
            //*! Store the last key stroke
            blue.controls.current_input = a_next_input_code;
            //*! Clear the next key code
            blue.controls.next_input = KeyCode.None;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.DOWN);
        }
        else if (blue.controls.move_left_key == a_next_input_code)
        {
            //*! Store the last key stroke
            blue.controls.current_input = a_next_input_code;
            //*! Clear the next key code
            blue.controls.next_input = KeyCode.None;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.LEFT);
        }
        else if (blue.controls.move_right_key == a_next_input_code)
        {
            //*! Store the last key stroke
            blue.controls.current_input = a_next_input_code;
            //*! Clear the next key code
            blue.controls.next_input = KeyCode.None;
            //*! Move Player in the direction
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.RIGHT);
        }
        else
        {
            Debug.LogError("No next key match!");
        }


        //*! Clear the current input
        //blue.controls.next_input = KeyCode.None;

    }




    #endregion



    //*! Inherited Access
    #region Protected Functions



    #endregion


}

//*!----------------------------!*//
//*!    Player Information
//*!----------------------------!*//
    #region Player Information Data

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
        [Header("Percentage of distance remaining before the next point")]
        [Tooltip("How much is left to travel for the player between the two points")]
        [Range(1, 100)]
        //*! Default value of 1% remaining
        public int distance_remaining = 1;

        [SerializeField]
        //*! Controls
        public Movement controls;

        [SerializeField]
        //*! Sub player type
        public Sub_Type sub;

        //*! Is the player falling - used for disabling the input (No air control)
        public bool is_grounded;
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
        [Header("Percentage of distance remaining before the next point")]
        [Tooltip("How much is left to travel for the player between the two points")]
        [Range(1, 100)]
        //*! Default value of 1% remaining
        public int distance_remaining = 1;

        [SerializeField]
        //*! Controls
        public Movement controls;

        [SerializeField]
        //*! Sub player type
        public Sub_Type sub;

        //*! Is the player falling - used for disabling the input (No air control)
        public bool is_grounded;
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

    /// <summary>
    /// What Sub type is the player
    /// </summary>
    [System.Serializable]
    public struct Sub_Type
    {
        public enum SubType
        {
            BLOCK,
            LINE
        }                                        
        //*! What sub type is the player
        public SubType type;
    }

    #endregion