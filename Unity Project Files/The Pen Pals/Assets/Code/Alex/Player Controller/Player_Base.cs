//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;


public class Player_Base : MonoBehaviour
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

    [SerializeField]
    //*! What player type is the player
    private Player_Base_Interaction.P_Type type;

    [HideInInspector]
    //*! Player Interaction Reference for movement
    private Player_Base_Interaction interaction_base;

    //*! Player Ground Check Reference, only used when aligned to the grid
    private Player_Ground_Check ground_check;

 


    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    //*! Red Instance of the player
    public static Player_Base Red_Instance;

    //*! Blue Instance of the player
    public static Player_Base Blue_Instance;

    //*! Current grid postition
    public Vector2 current_position;

    //*! Movement distance for player to move
    [Range(1, 4)]
    public int movement_distance;

    //*! Movement speed for player to move
    [Range(1, 10)]
    public int movement_speed;

    [HideInInspector]
    //*! Getting the current player type
    public Player_Base_Interaction.P_Type Type
    { get { return type; } }


    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions
    private void Awake()
    {
        //*! Singleton Reference(s)
        Red_Instance = this;
        Blue_Instance = this;

        //*! Default value unless overriden
        if (movement_distance < 1)
            movement_distance = 1;
    }
    private void Start()
    {
        //*! Get a singleton reference to the interaction component
        interaction_base = Player_Base_Interaction.Instance;

        //*! Get a singleton reference to the ground check component
        ground_check = Player_Ground_Check.Instance;

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
        
        //*! The player is not moving, BUT is touching the ground
        if (ground_check.Touching() && !is_moving)
        {
            //*! Assign the first input key code
            if(interaction_base.Check_For_Input(Type))
            {
                //*! Only when there is input from the user
                is_moving = true;
            }
        }
        //*! The player is currently moving
        else if (is_moving)
        {
            //*! Early Input opportunity
            if (can_enter_second_input)
            {
                //*! Assign the second / next input key code
                interaction_base.Queue_Next_Input(Type);

                //*! Flag it back to false as this only needs to happen once
                can_enter_second_input = false;
            }

            //Debug.Log("A");
            //*! Apply the new Position     //-? Helps it be more consitant
            Move_Towards_Target_Location(transform.position, new Vector3(current_position.x, current_position.y, 0));

            //*! Move the player in the direction of the current input
            if (interaction_base.Get_Current_Input(Type) != KeyCode.None)
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
    }

    //*! When the player is moving, update its position
    private void Move_Towards_Target_Location(Vector3 old_position, Vector3 new_position)
    {
        //*! Calculate its next position and move the player towards that location
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(current_position.x, current_position.y, 0), Time.deltaTime * movement_speed);
        
        //*! distance / mag check 90% ~ near allow for second input
        float distance_between = (transform.position - new Vector3(current_position.x, current_position.y, 0)).magnitude;


        //*! When the player is Magic Number (Percentage of distance between the two points)
        if (distance_between < interaction_base.Get_Distance_Remaining(Type))
        {
            //Debug.Log("mag : " + m.ToString("0.00"));
            can_enter_second_input = true;
        }

        //*! When the player is exactly at the end location
        if (transform.position == new Vector3(current_position.x, current_position.y, 0))
        {
            //*! Player has finished moving
            is_moving = false;

            #region testing
            //if (interaction_base.Get_Current_Input(Type) == interaction_base.Player_RED.Controls.move_up_key)
            //{
            //    interaction_base.Player_RED.in_air = true;
            //}
            //else if(interaction_base.Get_Current_Input(Type) == interaction_base.Player_BLUE.Controls.move_up_key)
            //{
            //    interaction_base.Player_BLUE.in_air = true;
            //}

            switch (Type)
            {
                case Player_Base_Interaction.P_Type.RED_BLOCK:
                ///case Player_Base_Interaction.P_Type.RED_LINE:
                case Player_Base_Interaction.P_Type.RED:

                    if (interaction_base.Player_RED.in_air)
                    {
                        is_moving = true;
                        //*! Clear the current input
                        interaction_base.Clear_Current_Input(Type);
                        return;
                    }
                    else
                    {
                        break;
                    }


                case Player_Base_Interaction.P_Type.BLUE_BLOCK:
                ///case Player_Base_Interaction.P_Type.BLUE_LINE:
                case Player_Base_Interaction.P_Type.BLUE:

                    if (interaction_base.Player_BLUE.in_air)
                    {
                        is_moving = true;
                        //*! Clear the current input
                        interaction_base.Clear_Current_Input(Type);
                        return;
                    }
                    else
                    {
                        break;
                    }

                default:
                    break;
            }

            #endregion


            //*! Clear the current input
            interaction_base.Clear_Current_Input(Type);

            //*! Check if the player is grounded
            //*! Not Grounded, and there is no next input
            if(!ground_check.Touching() && interaction_base.Get_Next_Input(Type) == KeyCode.None)
            {
                Player_Falling();
            }
            //*! If the player has queued some input and not none
            else if (interaction_base.Get_Next_Input(Type) != KeyCode.None)
            {
                //*! Allow the player update so call this function and move the player.
                is_moving = true;

                //*! Automatically move the player towards its next location based on the next input
                interaction_base.Move_Player_By_Next_Input(Type, interaction_base.Get_Next_Input(Type));
                
                //*! Clear the current input
                interaction_base.Clear_Next_Input(Type);
            }
            else
            {
                //*! When the player has reached the end point re enable the controls
                interaction_base.Re_Enable_Input(Type);
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

        //*! What Player Type
        switch (Type)
        {
            case Player_Base_Interaction.P_Type.RED_BLOCK:
            case Player_Base_Interaction.P_Type.RED_LINE://?
            case Player_Base_Interaction.P_Type.RED:
                interaction_base.Move_Player_By_Next_Input(Type, interaction_base.Player_RED.Controls.move_down_key);
                break;
            case Player_Base_Interaction.P_Type.BLUE_BLOCK:
            case Player_Base_Interaction.P_Type.BLUE_LINE://?
            case Player_Base_Interaction.P_Type.BLUE:
                interaction_base.Move_Player_By_Next_Input(Type, interaction_base.Player_BLUE.Controls.move_down_key);
                break;
            default:
                break;
        }
    }

       







    #endregion



    //*! Protected Access
    #region Protected Functions

    #endregion



}