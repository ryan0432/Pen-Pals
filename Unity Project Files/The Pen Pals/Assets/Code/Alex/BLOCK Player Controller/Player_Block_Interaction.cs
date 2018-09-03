//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;


public class Player_Block_Interaction : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    [SerializeField]
    //*! Block Player
    private Player_Data BLOCK = new Player_Data();


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
    public static Player_Block_Interaction Instance;



    public Player_Data PLAYER_BLOCK_DATA
    { get { return BLOCK; } }



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
        //*! BLOCK Default Values when game starts
        //BLOCK.is_grounded = true;
        //BLOCK.Controls.can_move_up = true;
        //BLOCK.Controls.can_move_down = false;
        //BLOCK.Controls.can_move_left = true;
        //BLOCK.Controls.can_move_right = true;
        BLOCK.player_block_interaction = Player_Block.player_block;




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
    /// <returns>-True if the player inputed something.-</returns>
    public bool Check_For_Input()
    {
        return Set_First_Input_BLOCK();
    }

    /// <summary>
    /// When the player is moving and inputs a movement key code
    /// </summary>
    /// <param name="a_ptype">-Based on the player, compare against its controls-</param>
    public void Queue_Next_Input()
    {
        Set_Second_Input_BLOCK();
    }

    /// <summary>
    /// Return the distance remaining based on the player type, divided by 100 and casted to a float
    /// </summary>
    /// <returns>-Of type float for the comparison check in player.-</returns>
    public float Get_Distance_Remaining()
    {
        return (float)BLOCK.distance_remaining / 100;
    }

    /// <summary>
    /// Clears the current input based on the current player.
    /// </summary>
    public void Clear_Current_Input()
    {
        BLOCK.Controls.current_input = KeyCode.None;
    }

    /// <summary>
    /// Clears the current input based on the current player.
    /// </summary>
    public void Clear_Next_Input()
    {
        BLOCK.Controls.next_input = KeyCode.None;
    }

    /// <summary>
    /// Move the player by the current input, this is only called when the player is less than 90% of the way between the start and end location.
    /// Based on what player calls for the function and what they press determines where the player goes
    /// </summary>
    /// <param name="a_next_input_key">-What keycode was pressed-</param>
    public void Move_Player_By_Next_Input(KeyCode a_next_input_key)
    {
        //*! Store the last key stroke
        BLOCK.Controls.current_input = a_next_input_key;
        //*! Clear the next key code
        BLOCK.Controls.next_input = KeyCode.None;

        Move_Player_In_Direction(Movement_Direction.OVERRIDE, a_next_input_key);



    }

    public KeyCode Get_Next_Input()
    {
        return BLOCK.Controls.next_input;
    }

    //*! After a slight re-work this is not needed, but here if it is needed for later
    public KeyCode Get_Current_Input()
    {
        return BLOCK.Controls.current_input;
    }


    public void Re_Enable_Input()
    {
        BLOCK.Controls.can_move_up = true;
        //BLOCK.Controls.can_move_down = true;//?
        //BLOCK.Controls.can_move_left = true;
        //BLOCK.Controls.can_move_right = true;
    }


    #endregion



    //*! Private Access
    #region Private Functions

    #region Player First Input

    //*! BLOCK Controls First Input
    private bool Set_First_Input_BLOCK()
    {

        //*! Previous frame was jumping? but now grounded
        if (BLOCK.is_grounded && BLOCK.Controls.is_jumping)
        {
            //*! Jumping when grounded
            BLOCK.Controls.is_jumping = false;
        }


        //*! When is the player in the air
        if (!BLOCK.is_grounded)
        {
            BLOCK.in_air = true;
        }
        else
        {
            BLOCK.in_air = false;
        }


        //*! Gravity Check
        if (BLOCK.is_grounded)
        {
            //*! RED Input Checks
            if (Input.GetKeyDown(BLOCK.Controls.move_up_key) && BLOCK.Controls.can_move_up)
            {
                //*! Disable Up and Down
                BLOCK.Controls.can_move_up = false;
                BLOCK.Controls.can_move_down = false;

                //*! Jumping when grounded
                BLOCK.Controls.is_jumping = true;


                //*! Set the current input
                BLOCK.Controls.current_input = BLOCK.Controls.move_up_key;

                //*! Move the player up
                Move_Player_In_Direction(Movement_Direction.UP, BLOCK.Controls.move_up_key);

                return true;
            }
            else if (Input.GetKeyDown(BLOCK.Controls.move_down_key) && BLOCK.Controls.can_move_down)
            {


                //*! Set the current input
                BLOCK.Controls.current_input = BLOCK.Controls.move_down_key;

                //*! Move the player down
                Move_Player_In_Direction(Movement_Direction.DOWN, BLOCK.Controls.move_down_key);
                return true;

            }
            else if (Input.GetKeyDown(BLOCK.Controls.move_left_key) && BLOCK.Controls.can_move_left)
            {
                //*! Disable left and right, it will enable when grounded
                BLOCK.Controls.can_move_left = false;
                BLOCK.Controls.can_move_right = false;

                //*! Set the current input
                BLOCK.Controls.current_input = BLOCK.Controls.move_left_key;

                //*! Move the player left
                Move_Player_In_Direction(Movement_Direction.LEFT, BLOCK.Controls.move_left_key);
                return true;
            }
            else if (Input.GetKeyDown(BLOCK.Controls.move_right_key) && BLOCK.Controls.can_move_right)
            {

                //*! Disable left and right, it will enable when grounded
                BLOCK.Controls.can_move_left = false;
                BLOCK.Controls.can_move_right = false;

                //*! Set the current input
                BLOCK.Controls.current_input = BLOCK.Controls.move_right_key;

                //*! Move the player right
                Move_Player_In_Direction(Movement_Direction.RIGHT, BLOCK.Controls.move_right_key);
                return true;
            }

        }
        //*! Player was not grounded and is affected by gravity -> BLOCK
        else
        {
            //*! RED Input Checks
            if (Input.GetKeyDown(BLOCK.Controls.move_up_key) && !BLOCK.Controls.can_move_up)
            {
                //*! Set the current input
                BLOCK.Controls.current_input = KeyCode.None;
                return true;
            }
            else if (Input.GetKeyDown(BLOCK.Controls.move_down_key))
            {
                //*! Set the current input
                //RED.Controls.current_input = RED.Controls.move_down_key;

                //*! Move the player down
                //Move_Player_In_Direction(P_Type.RED, Movement_Direction.DOWN, RED.Controls.move_down_key);
                return true;
            }
            else if (Input.GetKeyDown(BLOCK.Controls.move_left_key) && BLOCK.Controls.can_move_left)
            {

                //*! Disable Left and Right
                BLOCK.Controls.can_move_left = false;
                BLOCK.Controls.can_move_right = false;


                //*! Set the current input
                BLOCK.Controls.current_input = BLOCK.Controls.move_left_key;

                //*! Move the player down
                Move_Player_In_Direction(Movement_Direction.LEFT, BLOCK.Controls.move_left_key);
                return true;
            }
            else if (Input.GetKeyDown(BLOCK.Controls.move_right_key) && BLOCK.Controls.can_move_right)
            {

                //*! Disable Left and Right
                BLOCK.Controls.can_move_left = false;
                BLOCK.Controls.can_move_right = false;


                //*! Set the current input
                BLOCK.Controls.current_input = BLOCK.Controls.move_right_key;

                //*! Move the player down
                Move_Player_In_Direction(Movement_Direction.RIGHT, BLOCK.Controls.move_right_key);
                return true;
            }
        }

        return false;


    }

    #endregion

    #region Player Second Input

    //*! BLOCK Controls Second Input - Not to move player but to set the next_input key code
    private void Set_Second_Input_BLOCK()
    {
        //*! Previous frame was jumping? but now grounded
        if (BLOCK.is_grounded && BLOCK.Controls.is_jumping)
        {
            //*! Jumping when grounded
            BLOCK.Controls.is_jumping = false;
        }

        //*! When is the player in the air
        if (!BLOCK.is_grounded)
        {
            BLOCK.in_air = true;
        }
        else
        {
            BLOCK.in_air = false;
        }
        //*! Gravity Check
        if (BLOCK.is_grounded)
        {
            //*! RED Input Checks
            if (Input.GetKeyDown(BLOCK.Controls.move_up_key) && BLOCK.Controls.can_move_up)
            {
                //*! Disable Up and Down
                BLOCK.Controls.can_move_up = false;
                BLOCK.Controls.can_move_down = false;

                //*! Jumping when grounded
                BLOCK.Controls.is_jumping = true;

                //*! Player is in the air
                //RED.in_air = true;

                //*! Not Grounded when jumping
                BLOCK.is_grounded = false;

                //*! Set the current input
                BLOCK.Controls.next_input = BLOCK.Controls.move_up_key;

                ///*! Move the player up
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.UP);

            }
            else if (Input.GetKeyDown(BLOCK.Controls.move_down_key))
            {


                //*! Set the current input
                BLOCK.Controls.next_input = BLOCK.Controls.move_down_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.DOWN);

            }
            else if (Input.GetKeyDown(BLOCK.Controls.move_left_key) && BLOCK.Controls.can_move_left)
            {

                //*! Disable left and right, it will enable when grounded
                BLOCK.Controls.can_move_left = false;
                BLOCK.Controls.can_move_right = false;


                //*! Set the current input
                BLOCK.Controls.next_input = BLOCK.Controls.move_left_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.LEFT);
            }
            else if (Input.GetKeyDown(BLOCK.Controls.move_right_key) && BLOCK.Controls.can_move_right)
            {
                //*! Disable left and right, it will enable when grounded
                BLOCK.Controls.can_move_left = false;
                BLOCK.Controls.can_move_right = false;


                //*! Set the current input
                BLOCK.Controls.next_input = BLOCK.Controls.move_right_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.RIGHT);
            }

        }
        //*! Player was not grounded and is affected by gravity -> Red Block
        else
        {
            //*! RED Input Checks
            if (Input.GetKeyDown(BLOCK.Controls.move_up_key) && !BLOCK.Controls.can_move_up)
            {

                //*! Set the current input
                BLOCK.Controls.next_input = KeyCode.None;

                ///*! Move the player DOWN??
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.DOWN);

            }
            else if (Input.GetKeyDown(BLOCK.Controls.move_down_key))
            {
                //*! Set the current input
                BLOCK.Controls.next_input = BLOCK.Controls.move_down_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.DOWN);
            }
            else if (Input.GetKeyDown(BLOCK.Controls.move_left_key) && BLOCK.Controls.can_move_left)
            {
                //*! Disable Left and Right
                BLOCK.Controls.can_move_left = false;
                BLOCK.Controls.can_move_right = false;

                //*! Set the current input
                BLOCK.Controls.next_input = BLOCK.Controls.move_left_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.LEFT);
            }
            else if (Input.GetKeyDown(BLOCK.Controls.move_right_key) && BLOCK.Controls.can_move_right)
            {
                //*! Disable Left and Right
                BLOCK.Controls.can_move_left = false;
                BLOCK.Controls.can_move_right = false;

                //*! Set the current input
                BLOCK.Controls.next_input = BLOCK.Controls.move_right_key;

                ///*! Move the player down
                ///Move_Player_In_Direction(P_Type.RED, Movement_Direction.RIGHT);
            }
        }


    }


    #endregion

    #region Move Player By Direction

    //*! What direction to move the player in
    private void Move_Player_In_Direction(Movement_Direction a_direction, KeyCode a_directional_key)
    {
        //*! Re-get the reference for the players
        BLOCK.player_block_interaction = Player_Block.player_block;


        //*! If the player is not grounded and the player wants to move, allow them to move but then set the next input to down
        if (!BLOCK.is_grounded)
        {
            BLOCK.Controls.next_input = BLOCK.Controls.move_down_key;
        }
        //*! What direction to affect
        switch (a_direction)
        {
            case Movement_Direction.UP:
                BLOCK.player_block_interaction.current_position += new Vector2(0, BLOCK.player_block_interaction.movement_distance);
                break;
            case Movement_Direction.DOWN:
                BLOCK.player_block_interaction.current_position -= new Vector2(0, BLOCK.player_block_interaction.movement_distance);
                break;
            case Movement_Direction.LEFT:
                BLOCK.player_block_interaction.current_position -= new Vector2(BLOCK.player_block_interaction.movement_distance, 0);
                break;
            case Movement_Direction.RIGHT:
                BLOCK.player_block_interaction.current_position += new Vector2(BLOCK.player_block_interaction.movement_distance, 0);
                break;
            case Movement_Direction.OVERRIDE:

                //*! Directional Key Override to move the current position
                if (BLOCK.Controls.move_up_key == a_directional_key)
                {
                    if (BLOCK.Controls.can_move_up && BLOCK.is_grounded)
                    {
                        BLOCK.player_block_interaction.current_position += new Vector2(0, BLOCK.player_block_interaction.movement_distance);
                    }
                }
                else if (BLOCK.Controls.move_down_key == a_directional_key)
                {
                    if (!BLOCK.is_grounded)
                    {
                        BLOCK.player_block_interaction.current_position -= new Vector2(0, BLOCK.player_block_interaction.movement_distance);
                    }
                }
                else if (BLOCK.Controls.move_left_key == a_directional_key)
                {
                    if (BLOCK.Controls.can_move_left)
                    {
                        BLOCK.player_block_interaction.current_position -= new Vector2(BLOCK.player_block_interaction.movement_distance, 0);
                    }
                }
                else if (BLOCK.Controls.move_right_key == a_directional_key)
                {
                    if (BLOCK.Controls.can_move_right)
                    {
                        BLOCK.player_block_interaction.current_position += new Vector2(BLOCK.player_block_interaction.movement_distance, 0);
                    }
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
    public Player_Block player_block_interaction = Player_Block.player_block;


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
    //*! When is the player jumping
    public bool is_jumping;
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