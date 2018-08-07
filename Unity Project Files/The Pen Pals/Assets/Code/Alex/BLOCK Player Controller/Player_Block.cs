//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;


public class Player_Block : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    [Header("Player Object")]
    //*! if the player is moving lock out the controls input
    private bool is_moving;
    
    //*! When can the player queue an input to be executed
    private bool can_enter_second_input;
 

    [HideInInspector]
    //*! Player Interaction Reference for movement
    private Player_Block_Interaction interaction_base;

    //*! Player Ground Check Reference, only used when aligned to the grid
    private Block_Collision_Ground ground_check;

    private Block_Collision_Left left_check;

    private Block_Collision_Right right_check;

    private Block_Collision_Up up_check;

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    //*! Block Instance of the player
    public static Player_Block player_block;
 

    //*! Current grid postition
    public Vector2 current_position;

    //*! Movement distance for player to move
    [Range(1, 2)]
    public int movement_distance;

    //*! Movement speed for player to move
    [Range(1, 5)]
    public int movement_speed;

 

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions
    private void Awake()
    {
        //*! Singleton Reference(s)
        player_block = this;

        //*! Assign the current Position to not 0,0
        current_position.x = transform.position.x;
        current_position.y = transform.position.y;

        //*! Default value unless overriden
        if (movement_distance < 1)
            movement_distance = 1;
    }
    private void Start()
    {
        //*! Get a singleton reference to the interaction component
        interaction_base = Player_Block_Interaction.Instance;

        //*! Get a singleton reference to the ground check component
        ground_check = Block_Collision_Ground.Instance;
        left_check = Block_Collision_Left.Instance;
        right_check = Block_Collision_Right.Instance;
        up_check = Block_Collision_Up.Instance;


        //*! Can not queue the input
        can_enter_second_input = false;

        //*! Check if the player is grounded
        ground_check.Touching();
    }

    private void Update()
    {
        //*! Is the player moving
        Player_Movement();
    }
        

    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions
    
    public void Stop_Player()
    {
        is_moving = false;
        transform.position = transform.position;
    }

    #endregion


    //*! New Player Input system 
    /*! 
     *  
     *  when grounded
     *  up > disable up && down, enable left && right
     *  not grounded 
     *       right > disable right && left  > gravity check
     *       left > disable right && left > gravity check
     *       up > nothing as it is disabled
     *       
     *  Gravity kicks in, no input allowed until grounded
     *  - Just after you jump, 0 to 1 seconds of air time before falling
     *  
     *  when grounded
     *       right > disable nothing > gravity check
     *       left > disable nothing > gravity check
     *  
    !*/


    //*! Private Access
    #region Private Functions

    //*! Input Logic
    private void Player_Movement()
    {
        //*! When the player is not touching the ground and not moving
        if (!ground_check.Touching() && !is_moving)
        {
            //*! Auto hit the down key, until grounded
            Player_Falling();
        }
        else if (!ground_check.Touching() && is_moving)
        {
            //*! Affects the can_move_ L/R bool
            left_check.Touching();
            right_check.Touching();
            up_check.Touching();
        }



        //*! The player is not moving, BUT is touching the ground
        if (ground_check.Touching() && !is_moving)
        {

            //*! Affects the can_move_ L/R bool
            left_check.Touching();
            right_check.Touching();
            up_check.Touching();

            //*! Assign the first input key code
            if (interaction_base.Check_For_Input())
            {
                //*! Only when there is input from the user
                is_moving = true; 
                //*! Affects the can_move_ L/R bool
                left_check.Touching();
                right_check.Touching();
                up_check.Touching();
            }
 
        }
        //*! The player is currently moving
        else if (is_moving)
        {
            //*! Early Input opportunity
            if (can_enter_second_input)
            {
                //*! Assign the second / next input key code
                interaction_base.Queue_Next_Input();

                //*! Flag it back to false as this only needs to happen once
                can_enter_second_input = false;
            }

            //Debug.Log("A");
            //*! Apply the new Position     //-? Helps it be more consitant
            Move_Towards_Target_Location(transform.position, new Vector3(current_position.x, current_position.y, 0));

            //*! Move the player in the direction of the current input
            if (interaction_base.Get_Current_Input() != KeyCode.None)
            {
                //Debug.Log("B");
                //*! Apply the new Position
                Move_Towards_Target_Location(transform.position, new Vector3(current_position.x, current_position.y, 0));
            }
            else
            {
                is_moving = false;
                //Debug.LogWarning("Player moving but current input is none");
            }
        }


        if (!interaction_base.PLAYER_BLOCK_DATA.Controls.can_move_left && !interaction_base.PLAYER_BLOCK_DATA.Controls.can_move_right && !interaction_base.PLAYER_BLOCK_DATA.Controls.can_move_up)
        {
            //Debug.LogError(interaction_base.Get_Current_Input());
        }

    }

    //*! When the player is moving, update its position
    private void Move_Towards_Target_Location(Vector3 old_position, Vector3 new_position)
    {
        //*! Calculate its next position and move the player towards that location
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(current_position.x, current_position.y, 0), Time.deltaTime * movement_speed);
        
        //*! distance / mag check 90% ~ near allow for second input
        float distance_between = (transform.position - new Vector3(current_position.x, current_position.y, 0)).magnitude;


        //*! When the player is Magic Number (Percentage of distance between the two points)
        if (distance_between < interaction_base.Get_Distance_Remaining())
        {
            //Debug.Log("mag : " + m.ToString("0.00"));
            can_enter_second_input = true;
        }

        //*! When the player is exactly at the end location
        if (transform.position == new Vector3(current_position.x, current_position.y, 0))
        {
            //*! Player has finished moving
            is_moving = false;

            //*! Clear the current input
            interaction_base.Clear_Current_Input();

            //*! Check if the player is grounded
            //*! Not Grounded, and there is no next input
            if(!ground_check.Touching() && interaction_base.Get_Next_Input() == KeyCode.None)
            {
                Player_Falling();
            }
            //*! If the player has queued some input and not none
            else if (interaction_base.Get_Next_Input() != KeyCode.None)
            {
                //*! Allow the player update so call this function and move the player.
                is_moving = true;


                //*! Affects the can_move_ L/R bool
                left_check.Touching();
                right_check.Touching();
                up_check.Touching();



                //*! Automatically move the player towards its next location based on the next input
                interaction_base.Move_Player_By_Next_Input(interaction_base.Get_Next_Input());
                
                
                //*! Clear the current input
                interaction_base.Clear_Next_Input();
            }
            else
            {
                //*! When the player has reached the end point re enable the controls
                interaction_base.Re_Enable_Input();
                //*! Affects the can_move_ L/R bool
                left_check.Touching();
                right_check.Touching();
                up_check.Touching();
            }



        }

    }

    /// <summary>
    /// Custom Gravity Input Override
    /// </summary>
    private void Player_Falling()
    {
        //*! The player is now moving until it is grounded
        is_moving = true;
        //*! Set player input to DOWN
        interaction_base.Move_Player_By_Next_Input(interaction_base.PLAYER_BLOCK_DATA.Controls.move_down_key);
    }



    #endregion



    //*! Protected Access
    #region Protected Functions

    #endregion



}