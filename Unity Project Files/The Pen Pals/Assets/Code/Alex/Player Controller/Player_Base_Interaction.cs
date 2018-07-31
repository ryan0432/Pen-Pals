//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;


public class Player_Base_Interaction : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables
    
    [SerializeField]
    //*! Red Player
    private Player_Data RED = new Player_Data();

    [SerializeField]
    //*! Blue Player
    private Player_Data BLUE = new Player_Data();

    //*! Directions of movement used to determin where the player is going
    private enum Movement_Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        OVERRIDE
    }

    //*! Air temp timer
    private float t_timer;


    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    //*! Singeton reference
    public static Player_Base_Interaction Instance;

    //*! Player types
    public enum P_Type
    {
        RED_BLOCK,
        BLUE_BLOCK,
        RED_LINE,
        BLUE_LINE,
        RED,
        BLUE
    }

    public Player_Data Player_RED
    { get { return RED; } }


    public Player_Data Player_BLUE
    { get { return BLUE; } }


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
        //*! RED Default Values when game starts
        RED.is_grounded = true;

        RED.Controls.can_move_up = true;
        RED.Controls.can_move_down = false;
        RED.Controls.can_move_left = true;
        RED.Controls.can_move_right = true;

        RED.player_red = Player_Base.Red_Instance;
        RED.player_blue = Player_Base.Blue_Instance;

        BLUE.Controls.can_move_up = true;
        BLUE.Controls.can_move_down = false;
        BLUE.Controls.can_move_left = true;
        BLUE.Controls.can_move_right = true;
    }

    private void Update()
    {

        ////*! Air timer start and finish for player, a delay between when the system knows the player is not touching the ground.
        ////*! Either player in the air?
        //if (RED.in_air || BLUE.in_air)
        //{
        //    //*! Counter
        //    t_timer += Time.deltaTime;
        //    //*! Red Player air timer
        //    if (t_timer >= RED.air_time)
        //    {
        //        //*! Not Grounded when jumping
        //        RED.in_air = false;
        //        //*! Reset
        //        t_timer = 0.0f;
        //    }
        //    //*! Blue Player air timer
        //    else if (t_timer >= BLUE.air_time)
        //    {
        //        //*! Not Grounded when jumping
        //        BLUE.in_air = false;
        //        //*! Reset
        //        t_timer = 0.0f;
        //    }
        //    //*! While not what??
        //    else
        //    {               
        //        //*! Not Grounded when jumping
        //        RED.is_grounded = false;
        //        //*! Not Grounded when jumping
        //        BLUE.is_grounded = false;
        //    }

        //}
    }

    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions

    /// <summary>
    /// When the player is moving and inputs a movement key code
    /// </summary>
    /// <param name="a_ptype">-Based on the player, compare against its controls-</param>
    public bool Check_For_Input(P_Type a_ptype)
    {
        //*! What Player Type
        switch (a_ptype)
        {
            case P_Type.RED_LINE:
                Set_First_Input_RED(false);
                return true;

            case P_Type.RED_BLOCK:
            case P_Type.RED:
                Set_First_Input_RED(true);
                return true;
                //break;


            case P_Type.BLUE_LINE:
                Set_First_Input_BLUE(false);
                return true;
            case P_Type.BLUE_BLOCK:
            case P_Type.BLUE:
                Set_First_Input_BLUE(true);
                return true;
                //break;
            default:
                return false;
                //break;
        }
        //return false;
    }

    /// <summary>
    /// When the player is moving and inputs a movement key code
    /// </summary>
    /// <param name="a_ptype">-Based on the player, compare against its controls-</param>
    public void Queue_Next_Input(P_Type a_ptype)
    {
        //*! What Player Type
        switch (a_ptype)
        {
            case P_Type.RED_LINE:
            case P_Type.RED_BLOCK:
            case P_Type.RED:
                Set_Second_Input_RED(true);
                break;
            case P_Type.BLUE_BLOCK:
            case P_Type.BLUE_LINE:
            case P_Type.BLUE:
                Set_Second_Input_BLUE(false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Return the distance remaining based on the player type, divided by 100 and casted to a float
    /// </summary>
    /// <param name="a_player_type"></param>
    /// <returns>-Of type float for the comparison check in player.-</returns>
    public float Get_Distance_Remaining(P_Type a_ptype)
    {
        //*! Based on the player type, return the appropirate value
        switch (a_ptype)
        {
            case P_Type.RED_LINE:
            case P_Type.RED_BLOCK:
            case P_Type.RED:
                return (float)RED.distance_remaining / 100;
            ///break;
            case P_Type.BLUE_BLOCK:
            case P_Type.BLUE_LINE:
            case P_Type.BLUE:
                return (float)BLUE.distance_remaining / 100;
            ///break;
            default:
                return 0.0f;
                ///break;
        }
    }

    /// <summary>
    /// Clears the current input based on the current player.
    /// </summary>
    /// <param name="a_player_type"></param>
    public void Clear_Current_Input(P_Type a_ptype)
    {
        switch (a_ptype)
        {
            case P_Type.RED_LINE:
            case P_Type.RED_BLOCK:
            case P_Type.RED:
                RED.Controls.current_input = KeyCode.None;
                break;

            case P_Type.BLUE_BLOCK:
            case P_Type.BLUE_LINE:
            case P_Type.BLUE:
                BLUE.Controls.current_input = KeyCode.None;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Clears the current input based on the current player.
    /// </summary>
    /// <param name="a_player_type"></param>
    public void Clear_Next_Input(P_Type a_ptype)
    {
        switch (a_ptype)
        {
            case P_Type.RED_LINE:
            case P_Type.RED_BLOCK:
            case P_Type.RED:
                RED.Controls.next_input = KeyCode.None;
                break;

            case P_Type.BLUE_BLOCK:
            case P_Type.BLUE_LINE:
            case P_Type.BLUE:
                BLUE.Controls.next_input = KeyCode.None;
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
    public void Move_Player_By_Next_Input(P_Type a_ptype, KeyCode a_next_input_key)
    {

        switch (a_ptype)
        {
            //*! When the player type is red, with the movement direction override, you can use the key code
            case P_Type.RED_LINE:
            case P_Type.RED_BLOCK:
            case P_Type.RED:        
                //*! Store the last key stroke
                RED.Controls.current_input = a_next_input_key;
                //*! Clear the next key code
                RED.Controls.next_input = KeyCode.None;

                Move_Player_In_Direction(a_ptype, Movement_Direction.OVERRIDE, a_next_input_key);
                break;

            //*! When the player type is blue, with the movement direction override, you can use the key code
            case P_Type.BLUE_LINE:
            case P_Type.BLUE_BLOCK:
            case P_Type.BLUE:
                //*! Store the last key stroke
                BLUE.Controls.current_input = a_next_input_key;
                //*! Clear the next key code
                BLUE.Controls.next_input = KeyCode.None;

                Move_Player_In_Direction(a_ptype, Movement_Direction.OVERRIDE, a_next_input_key);
                break;


            default:
                break;
        }
    }

    public KeyCode Get_Next_Input(P_Type a_ptype)
    {
        switch (a_ptype)
        {
            case P_Type.RED_LINE:
            case P_Type.RED_BLOCK:
            case P_Type.RED:
                return RED.Controls.next_input;
            //break;
            case P_Type.BLUE_LINE:
            case P_Type.BLUE_BLOCK:
            case P_Type.BLUE:
                return BLUE.Controls.next_input;
            //break;
            default:
                Debug.Log("No, this can't be right...");
                return KeyCode.None;
                //break;
        }
    }

    //*! After a slight re-work this is not needed, but here if it is needed for later
    public KeyCode Get_Current_Input(P_Type a_ptype)
    {
        switch (a_ptype)
        {
            case P_Type.RED_LINE:
            case P_Type.RED_BLOCK:
            case P_Type.RED:
                return RED.Controls.current_input;
            //break;
            case P_Type.BLUE_LINE:
            case P_Type.BLUE_BLOCK:
            case P_Type.BLUE:
                return RED.Controls.current_input;
            //break;
            default:
                return KeyCode.None;
                //break;
        }
    }


    public void Re_Enable_Input(P_Type a_ptype)
    {
        switch (a_ptype)
        {
            case P_Type.RED_LINE:
            case P_Type.RED_BLOCK:
            case P_Type.RED:
                RED.Controls.can_move_up = true;
                //RED.Controls.can_move_down = true;//?
                RED.Controls.can_move_left = true;
                RED.Controls.can_move_right = true;
                break;
                
            case P_Type.BLUE_LINE:
            case P_Type.BLUE_BLOCK:
            case P_Type.BLUE:
                BLUE.Controls.can_move_up = true;
                //BLUE.Controls.can_move_down = true;//?
                BLUE.Controls.can_move_left = true;
                BLUE.Controls.can_move_right = true;
                break;

            default:
                break;
                
        }
    }


    #endregion



    //*! Private Access
    #region Private Functions

    #region Player First Input

    //*! Red Controls First Input
    private void Set_First_Input_RED(bool affected_by_gravity)
    {
        //*! Gravity Check
        if (RED.is_grounded)
        {
            //*! RED Input Checks
            if (Input.GetKeyDown(RED.Controls.move_up_key) && RED.Controls.can_move_up)
            {
                //*! Disable Up and Down
                RED.Controls.can_move_up = false;
                RED.Controls.can_move_down = false;

                //*! Not Grounded when jumping
                RED.is_grounded = false;

                //*! Set the current input
                RED.Controls.current_input = RED.Controls.move_up_key;

                //*! Move the player up
                Move_Player_In_Direction(P_Type.RED, Movement_Direction.UP, RED.Controls.move_up_key);

            }
            else if (Input.GetKeyDown(RED.Controls.move_down_key) && RED.Controls.can_move_down)
            {

                //*! Set the current input
                RED.Controls.current_input = RED.Controls.move_down_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.RED, Movement_Direction.DOWN, RED.Controls.move_down_key);

            }
            else if (Input.GetKeyDown(RED.Controls.move_left_key))
            {
                //*! Set the current input
                RED.Controls.current_input = RED.Controls.move_left_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.RED, Movement_Direction.LEFT, RED.Controls.move_left_key);
            }
            else if (Input.GetKeyDown(RED.Controls.move_right_key))
            {
                //*! Set the current input
                RED.Controls.current_input = RED.Controls.move_right_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.RED, Movement_Direction.RIGHT, RED.Controls.move_right_key);
            }

        }
        //*! Player was not grounded AND not affected by gravity -> Red Line
        else if (!RED.is_grounded && !affected_by_gravity)
        {
            //*! RED Input Checks
            if (Input.GetKeyDown(RED.Controls.move_up_key))
            {
                //*! Set the current input
                RED.Controls.current_input = RED.Controls.move_up_key;

                //*! Move the player up
                Move_Player_In_Direction(P_Type.RED, Movement_Direction.UP, RED.Controls.move_up_key);
            }
            else if (Input.GetKeyDown(RED.Controls.move_down_key) && RED.Controls.can_move_down)
            {
                
                //*! Set the current input
                //RED.Controls.current_input = RED.Controls.move_down_key;

                //*! Move the player down
                //Move_Player_In_Direction(P_Type.RED, Movement_Direction.DOWN, RED.Controls.move_down_key);
            }
            else if (Input.GetKeyDown(RED.Controls.move_left_key))
            {
                //*! Set the current input
                RED.Controls.current_input = RED.Controls.move_left_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.RED, Movement_Direction.LEFT, RED.Controls.move_left_key);
            }
            else if (Input.GetKeyDown(RED.Controls.move_right_key))
            {
                //*! Set the current input
                RED.Controls.current_input = RED.Controls.move_right_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.RED, Movement_Direction.RIGHT, RED.Controls.move_right_key);
            }
        }
        //*! Player was not grounded and is affected by gravity -> Red Block
        else
        {
            //*! RED Input Checks
            if (Input.GetKeyDown(RED.Controls.move_up_key) && !RED.Controls.can_move_up)
            {
 
                //*! Set the current input
                RED.Controls.current_input = KeyCode.None;

                ///*! Move the player DOWN??
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.DOWN);

            }
            else if (Input.GetKeyDown(RED.Controls.move_down_key))
            {
                //*! Set the current input
                //RED.Controls.current_input = RED.Controls.move_down_key;

                //*! Move the player down
                //Move_Player_In_Direction(P_Type.RED, Movement_Direction.DOWN, RED.Controls.move_down_key);
            }
            else if (Input.GetKeyDown(RED.Controls.move_left_key) && RED.Controls.can_move_left)
            {
                //*! Disable Left and Right
                RED.Controls.can_move_left = false;
                RED.Controls.can_move_right = false;

                //*! Set the current input
                RED.Controls.current_input = RED.Controls.move_left_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.RED, Movement_Direction.LEFT, RED.Controls.move_left_key);
            }
            else if (Input.GetKeyDown(RED.Controls.move_right_key) && RED.Controls.can_move_right)
            {
                //*! Disable Left and Right
                RED.Controls.can_move_left = false;
                RED.Controls.can_move_right = false;

                //*! Set the current input
                RED.Controls.current_input = RED.Controls.move_right_key;
                
                //*! Move the player down
                Move_Player_In_Direction(P_Type.RED, Movement_Direction.RIGHT, RED.Controls.move_right_key);
            }
        }


    }
    

    //*! Blue Controls First Input
    private void Set_First_Input_BLUE(bool affected_by_gravity)
    {
        //*! Gravity Check
        if (BLUE.is_grounded)
        {
            //*! RED Input Checks
            if (Input.GetKeyDown(BLUE.Controls.move_up_key) && BLUE.Controls.can_move_up)
            {
                //*! Disable Up and Down
                BLUE.Controls.can_move_up = false;
                BLUE.Controls.can_move_down = false;

                //*! Not Grounded when jumping
                BLUE.is_grounded = false;

                //*! Set the current input
                BLUE.Controls.current_input = BLUE.Controls.move_up_key;

                //*! Move the player up
                Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.UP, RED.Controls.move_up_key);

            }
            else if (Input.GetKeyDown(BLUE.Controls.move_down_key))
            {

                //*! Set the current input
                BLUE.Controls.current_input = BLUE.Controls.move_down_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.DOWN, RED.Controls.move_down_key);

            }
            else if (Input.GetKeyDown(BLUE.Controls.move_left_key))
            {
                //*! Set the current input
                BLUE.Controls.current_input = BLUE.Controls.move_left_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.LEFT, RED.Controls.move_left_key);
            }
            else if (Input.GetKeyDown(BLUE.Controls.move_right_key))
            {
                //*! Set the current input
                BLUE.Controls.current_input = BLUE.Controls.move_right_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.RIGHT, RED.Controls.move_right_key);
            }

        }
        //*! Player was not grounded AND not affected by gravity -> BLUE Line
        else if (!BLUE.is_grounded && !affected_by_gravity)
        {
            //*! BLUE Input Checks
            if (Input.GetKeyDown(BLUE.Controls.move_up_key))
            {
                //*! Set the current input
                BLUE.Controls.current_input = BLUE.Controls.move_up_key;

                //*! Move the player up
                Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.UP, RED.Controls.move_up_key);
            }
            else if (Input.GetKeyDown(BLUE.Controls.move_down_key))
            {

                //*! Set the current input
                BLUE.Controls.current_input = BLUE.Controls.move_down_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.DOWN, RED.Controls.move_down_key);
            }
            else if (Input.GetKeyDown(BLUE.Controls.move_left_key))
            {
                //*! Set the current input
                BLUE.Controls.current_input = BLUE.Controls.move_left_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.LEFT, RED.Controls.move_left_key);
            }
            else if (Input.GetKeyDown(BLUE.Controls.move_right_key))
            {
                //*! Set the current input
                BLUE.Controls.current_input = BLUE.Controls.move_right_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.RIGHT, RED.Controls.move_right_key);
            }
        }
        //*! Player was not grounded and is affected by gravity -> BLUE Block
        else
        {
            //*! BLUE Input Checks
            if (Input.GetKeyDown(BLUE.Controls.move_up_key) && !BLUE.Controls.can_move_up)
            {

                //*! Set the current input
                BLUE.Controls.current_input = KeyCode.None;

                ///*! Move the player DOWN??
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.DOWN);

            }
            else if (Input.GetKeyDown(BLUE.Controls.move_down_key))
            {
                //*! Set the current input
                BLUE.Controls.current_input = BLUE.Controls.move_down_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.DOWN, RED.Controls.move_down_key);
            }
            else if (Input.GetKeyDown(BLUE.Controls.move_left_key) && BLUE.Controls.can_move_left)
            {
                //*! Disable Left and Right
                BLUE.Controls.can_move_left = false;
                BLUE.Controls.can_move_right = false;

                //*! Set the current input
                BLUE.Controls.current_input = BLUE.Controls.move_left_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.LEFT, RED.Controls.move_left_key);
            }
            else if (Input.GetKeyDown(BLUE.Controls.move_right_key) && BLUE.Controls.can_move_right)
            {
                //*! Disable Left and Right
                BLUE.Controls.can_move_left = false;
                BLUE.Controls.can_move_right = false;

                //*! Set the current input
                BLUE.Controls.current_input = BLUE.Controls.move_right_key;

                //*! Move the player down
                Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.RIGHT, RED.Controls.move_right_key);
            }
        }

    }


    #endregion

    #region Player Second Input

    //*! Red Controls Second Input - Not to move player but to set the next_input key code
    private void Set_Second_Input_RED(bool affected_by_gravity)
    {
        //*! Gravity Check
        if (RED.is_grounded)
        {
            //*! RED Input Checks
            if (Input.GetKeyDown(RED.Controls.move_up_key) && RED.Controls.can_move_up)
            {
                //*! Disable Up and Down
                RED.Controls.can_move_up = false;
                RED.Controls.can_move_down = false;

                //*! Player is in the air
                //RED.in_air = true;

                //*! Not Grounded when jumping
                RED.is_grounded = false;

                //*! Set the current input
                RED.Controls.next_input = RED.Controls.move_up_key;

                ///*! Move the player up
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.UP);

            }
            else if (Input.GetKeyDown(RED.Controls.move_down_key))
            {

                //*! Set the current input
                RED.Controls.next_input = RED.Controls.move_down_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.DOWN);

            }
            else if (Input.GetKeyDown(RED.Controls.move_left_key))
            {
                //*! Set the current input
                RED.Controls.next_input = RED.Controls.move_left_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.LEFT);
            }
            else if (Input.GetKeyDown(RED.Controls.move_right_key))
            {
                //*! Set the current input
                RED.Controls.next_input = RED.Controls.move_right_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.RIGHT);
            }

        }
        //*! Player was not grounded AND not affected by gravity -> Red Line
        else if (!RED.is_grounded && !affected_by_gravity)
        {
            //*! RED Input Checks
            if (Input.GetKeyDown(RED.Controls.move_up_key))
            {
                //*! Set the current input
                RED.Controls.next_input = RED.Controls.move_up_key;

                ///*! Move the player up
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.UP);
            }
            else if (Input.GetKeyDown(RED.Controls.move_down_key))
            {

                //*! Set the current input
                RED.Controls.next_input = RED.Controls.move_down_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.DOWN);
            }
            else if (Input.GetKeyDown(RED.Controls.move_left_key))
            {
                //*! Set the current input
                RED.Controls.next_input = RED.Controls.move_left_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.LEFT);
            }
            else if (Input.GetKeyDown(RED.Controls.move_right_key))
            {
                //*! Set the current input
                RED.Controls.next_input = RED.Controls.move_right_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.RIGHT);
            }
        }
        //*! Player was not grounded and is affected by gravity -> Red Block
        else
        {
            //*! RED Input Checks
            if (Input.GetKeyDown(RED.Controls.move_up_key) && !RED.Controls.can_move_up)
            {

                //*! Set the current input
                RED.Controls.next_input = KeyCode.None;

                ///*! Move the player DOWN??
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.DOWN);

            }
            else if (Input.GetKeyDown(RED.Controls.move_down_key))
            {
                //*! Set the current input
                RED.Controls.next_input = RED.Controls.move_down_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.DOWN);
            }
            else if (Input.GetKeyDown(RED.Controls.move_left_key) && RED.Controls.can_move_left)
            {
                //*! Disable Left and Right
                RED.Controls.can_move_left = false;
                RED.Controls.can_move_right = false;

                //*! Set the current input
                RED.Controls.next_input = RED.Controls.move_left_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.LEFT);
            }
            else if (Input.GetKeyDown(RED.Controls.move_right_key) && RED.Controls.can_move_right)
            {
                //*! Disable Left and Right
                RED.Controls.can_move_left = false;
                RED.Controls.can_move_right = false;

                //*! Set the current input
                RED.Controls.next_input = RED.Controls.move_right_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.RIGHT);
            }
        }


    }

    //*! Blue Controls Second Input
    private void Set_Second_Input_BLUE(bool affected_by_gravity)
    {
        //*! Gravity Check
        if (BLUE.is_grounded)
        {
            //*! BLUE Input Checks
            if (Input.GetKeyDown(BLUE.Controls.move_up_key) && BLUE.Controls.can_move_up)
            {
                //*! Disable Up and Down
                BLUE.Controls.can_move_up = false;
                BLUE.Controls.can_move_down = false;

                //*! Blue player in air
                //BLUE.in_air = true;

                //*! Not Grounded when jumping
                BLUE.is_grounded = false;

                //*! Set the current input
                BLUE.Controls.next_input = BLUE.Controls.move_up_key;

                ///*! Move the player up
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.UP);

            }
            else if (Input.GetKeyDown(BLUE.Controls.move_down_key))
            {

                //*! Set the current input
                BLUE.Controls.next_input = BLUE.Controls.move_down_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.DOWN);

            }
            else if (Input.GetKeyDown(BLUE.Controls.move_left_key))
            {
                //*! Set the current input
                BLUE.Controls.next_input = BLUE.Controls.move_left_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.LEFT);
            }
            else if (Input.GetKeyDown(BLUE.Controls.move_right_key))
            {
                //*! Set the current input
                BLUE.Controls.next_input = BLUE.Controls.move_right_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.RIGHT);
            }

        }
        //*! Player was not grounded AND not affected by gravity -> BLUE Line
        else if (!BLUE.is_grounded && !affected_by_gravity)
        {
            //*! BLUE Input Checks
            if (Input.GetKeyDown(BLUE.Controls.move_up_key))
            {
                //*! Set the current input
                BLUE.Controls.next_input = BLUE.Controls.move_up_key;

                ///*! Move the player up
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.UP);
            }
            else if (Input.GetKeyDown(BLUE.Controls.move_down_key))
            {

                //*! Set the current input
                BLUE.Controls.next_input = BLUE.Controls.move_down_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.DOWN);
            }
            else if (Input.GetKeyDown(BLUE.Controls.move_left_key))
            {
                //*! Set the current input
                BLUE.Controls.next_input = BLUE.Controls.move_left_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.LEFT);
            }
            else if (Input.GetKeyDown(BLUE.Controls.move_right_key))
            {
                //*! Set the current input
                BLUE.Controls.next_input = BLUE.Controls.move_right_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.RIGHT);
            }
        }
        //*! Player was not grounded and is affected by gravity -> BLUE Block
        else
        {
            //*! BLUE Input Checks
            if (Input.GetKeyDown(BLUE.Controls.move_up_key) && !BLUE.Controls.can_move_up)
            {

                //*! Set the current input
                BLUE.Controls.next_input = KeyCode.None;

                ///*! Move the player DOWN??
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.DOWN);

            }
            else if (Input.GetKeyDown(BLUE.Controls.move_down_key))
            {
                //*! Set the current input
                BLUE.Controls.next_input = BLUE.Controls.move_down_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.DOWN);
            }
            else if (Input.GetKeyDown(BLUE.Controls.move_left_key) && BLUE.Controls.can_move_left)
            {
                //*! Disable Left and Right
                BLUE.Controls.can_move_left = false;
                BLUE.Controls.can_move_right = false;

                //*! Set the current input
                BLUE.Controls.next_input = BLUE.Controls.move_left_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.LEFT);
            }
            else if (Input.GetKeyDown(BLUE.Controls.move_right_key) && BLUE.Controls.can_move_right)
            {
                //*! Disable Left and Right
                BLUE.Controls.can_move_left = false;
                BLUE.Controls.can_move_right = false;

                //*! Set the current input
                BLUE.Controls.next_input = BLUE.Controls.move_right_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.BLUE, Movement_Direction.RIGHT);
            }
        }
    }

    #endregion

    #region Move Player By Direction

    //*! What direction to move the player in
    private void Move_Player_In_Direction(P_Type a_ptype, Movement_Direction a_direction, KeyCode a_directional_key)
    {
        //*! Re-get the reference for the players
        RED.player_red = Player_Base.Red_Instance;
        BLUE.player_blue = Player_Base.Blue_Instance;

        //*! If the player is not grounded and the player wants to move, allow them to move but then set the next input to down
        if (!RED.is_grounded)
        {
            RED.Controls.next_input = RED.Controls.move_down_key;
        }
        else if (!BLUE.is_grounded)
        {
            BLUE.Controls.next_input = BLUE.Controls.move_down_key;
        }

        //*! Player Switch
        switch (a_ptype)
        {
            case P_Type.RED_LINE:
            case P_Type.RED_BLOCK:
            case P_Type.RED:
                //*! Direction switch
                switch (a_direction)
                {
                    case Movement_Direction.UP:
                        RED.player_red.current_position += new Vector2(0, RED.player_red.movement_distance);
                        break;
                    case Movement_Direction.DOWN:
                        RED.player_red.current_position -= new Vector2(0, RED.player_red.movement_distance);
                        break;                       
                    case Movement_Direction.LEFT:
                        RED.player_red.current_position -= new Vector2(RED.player_red.movement_distance, 0);
                        break;
                    case Movement_Direction.RIGHT:  
                        RED.player_red.current_position += new Vector2(RED.player_red.movement_distance, 0);
                        break;
                    case Movement_Direction.OVERRIDE:

                        //*! Directional Key Override to move the current position
                        if (RED.Controls.move_up_key == a_directional_key)
                        {
                            RED.player_red.current_position += new Vector2(0, RED.player_red.movement_distance);
                        }
                        else if (RED.Controls.move_down_key == a_directional_key)
                        {
                            if (!RED.is_grounded)
                            {
                                RED.player_red.current_position -= new Vector2(0, RED.player_red.movement_distance);
                            }
                        }
                        else if (RED.Controls.move_left_key == a_directional_key)
                        {
                            RED.player_red.current_position -= new Vector2(RED.player_red.movement_distance, 0);
                        }
                        else if (RED.Controls.move_right_key == a_directional_key)
                        {
                            RED.player_red.current_position += new Vector2(RED.player_red.movement_distance, 0);
                        }
                        break;
                            
                    default:
                        break;                       
                }
                break;

            case P_Type.BLUE_BLOCK:
            case P_Type.BLUE_LINE:
            case P_Type.BLUE:
                //*! Direction switch
                switch (a_direction)
                {
                    case Movement_Direction.UP:
                        BLUE.player_blue.current_position += new Vector2(0, BLUE.player_blue.movement_distance);
                        break;
                    case Movement_Direction.DOWN:
                        BLUE.player_blue.current_position -= new Vector2(0, BLUE.player_blue.movement_distance);
                        break;
                    case Movement_Direction.LEFT:
                        BLUE.player_blue.current_position -= new Vector2(BLUE.player_blue.movement_distance, 0);
                        break;
                    case Movement_Direction.RIGHT:
                        BLUE.player_blue.current_position += new Vector2(BLUE.player_blue.movement_distance, 0);
                        break;
                    case Movement_Direction.OVERRIDE:

                        //*! Directional Key Override to move the current position
                        if (BLUE.Controls.move_up_key == a_directional_key)
                        {
                            BLUE.player_blue.current_position += new Vector2(0, BLUE.player_blue.movement_distance);
                        }
                        else if (BLUE.Controls.move_down_key == a_directional_key)
                        {
                            if (!BLUE.is_grounded)
                            {
                                BLUE.player_blue.current_position -= new Vector2(0, BLUE.player_blue.movement_distance);
                            }

                        }
                        else if (BLUE.Controls.move_left_key == a_directional_key)
                        {
                            BLUE.player_blue.current_position -= new Vector2(BLUE.player_blue.movement_distance, 0);
                        }
                        else if (BLUE.Controls.move_right_key == a_directional_key)
                        {
                            BLUE.player_blue.current_position += new Vector2(BLUE.player_blue.movement_distance, 0);
                        }
                        break;

                    default:
                        break;
                }
                break;
            default:
                break;
        }


    }


    #endregion
    
    #endregion
    
    //*! Protected Access
    #region Protected Functions

    #endregion
}


/// <summary>
/// Player Data
/// </summary>
[System.Serializable]
public class Player_Data
{
    [HideInInspector]
    //*! Player Reference - Careful with this one.
    public Player_Base player_red = Player_Base.Red_Instance;
    [HideInInspector]
    public Player_Base player_blue = Player_Base.Blue_Instance;

 
    [Header("Percentage of distance remaining before the next point")]
    [Tooltip("How much is left to travel for the player between the two points")]
    [Range(1, 100)]
    //*! Default value of 1% remaining
    public int distance_remaining = 1;
    
  
    //*! Controls
    public Movement_Data Controls;

    //*! Player air-time - in seconds
    [Range(0.01f, 0.99f)]
    public float air_time;

    
    public bool in_air;

    //*! Is the player grounded - bool
    public bool is_grounded;

    //*! Is the player touching other player - bool
    public bool is_player_t_player;
}


/// <summary>
/// Universal Controls for both players
/// </summary>
[System.Serializable]
public struct Movement_Data
{
    //*! Can move in that direction
    public bool can_move_up;
    [HideInInspector]
    public bool can_move_down;
    public bool can_move_right;
    public bool can_move_left;

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

//!* Opps!