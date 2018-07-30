//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*! Player Script
public class Player : MonoBehaviour
{
    //*! Player Singleton
    public static Player Instance;

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables


    [Header("Player Object")]
    [SerializeField]
    private PlayerInteraction.Player_Type type;

    [HideInInspector]
    //*! Player Interaction Reference for movement
    private PlayerInteraction interaction;

    //*! if the player is moving lock out the controls input
    private bool is_moving;

    //*! When can the player input a movement control
    private bool can_enter_second_input;

    //*! Player Ground Check Reference, only used when aligned to the grid
    private Player_Ground_Check ground_check;


    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    //*! Current grid postition
    public Vector2 current_position;

    //*! Movement distance for player to move
    [Range(1, 10)]
    public int movement_distance;

    //*! Movement speed for player to move
    [Range(1, 10)]
    public int movement_speed;

    [HideInInspector]
    public PlayerInteraction.Player_Type Type
    { get { return type; } }



    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions

    private void Awake()
    {
        //*! Creating the singleton
        Instance = this;        

        if (movement_distance < 1)
            movement_distance = 1;
    }

    private void Start()
    {
        //*! Get a singleton reference to the interaction component
        interaction = PlayerInteraction.Instance;

        //*! Get a singleton reference to the ground check component
        ground_check = Player_Ground_Check.Instance;

        //*! Can move is set to true allowing the player to move
        can_enter_second_input = false;

        //*! Check if the player is grounded
        ground_check.Touching();
    }
    
    /// <summary>
    /// Main Player logic loop
    /// </summary>
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

 
    #endregion


    //*! Private Access
    #region Private Functions

    //*! When is the player moving
    private void Player_Movement()
    {
        //*! Check if the player is NOT grounded
        if(!ground_check.Touching() && !is_moving)
        {
            is_moving = true;

            switch (type)
            {
                case PlayerInteraction.Player_Type.RED:
                    //*! Move the red player by its down key
                    interaction.Move_Player_By_Next_Input(type, interaction.Red.controls.move_down_key);
                    break;
                case PlayerInteraction.Player_Type.BLUE:
                    //*! Move the blue player by its down key
                    interaction.Move_Player_By_Next_Input(type, interaction.Blue.controls.move_down_key);
                    break;
                default:
                    break;
            }
        }
        
        //*! Check the input of the player based on the type
        if (!is_moving)
        {
            //*! What input was pressed from what player and set that input as the current Input
            if (interaction.Check_For_Input(type))
            {
                //*! When the player is moving
                is_moving = true;
            }
        }
        //*! When the player is moving, move it
        else if (is_moving)
        {
                        
            //*! When can the player queue the next input
            if (can_enter_second_input)
            {
                //*! Dis-allow Double Jump
                interaction.Double_Jumping(type);

                //*! Dis-allow Long Jumping
                interaction.Long_Jumping(type);

                //*! Set the next input 
                interaction.Queue_Next_Input(type);

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
                 *  
                 *  when grounded
                 *       right > disable nothing > gravity check
                 *       left > disable nothing > gravity check
                 *  
                !*/


                //*! Disable second  queing input
                can_enter_second_input = false;
            }

            //*! If the player is holding down a key
            if (interaction.Get_Current_Input(type) != KeyCode.None)
            {
                //*! Apply the new Position
                Apply_New_Position(transform.position, new Vector3(current_position.x, current_position.y, 0));
            }
            else
            {
                is_moving = false;
                //Debug.LogWarning("Player moving but current input is none");
            }
        }
 
    }

    //*! New Position Apply
    private void Apply_New_Position(Vector3 old_position, Vector3 new_position)
    {
        //*! Calculate its next position and move the player towards that location
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(current_position.x, current_position.y, 0), Time.deltaTime * movement_speed);
        
        //*! distance / mag check 90% ~ near allow for second input
        float distance_between = (transform.position - new Vector3(current_position.x, current_position.y, 0)).magnitude;

        
        //*! When the player is Magic Number (Percentage of distance between the two points)
        if (distance_between < interaction.Get_Distance_Remaining(type))
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
            interaction.Clear_Current_Input(type);

            //*! re enable input - should be false anyways
            ///can_enter_second_input = false;

            //*! Check if the player is grounded
            ground_check.Touching();

            //*! Not nothing - move the player in that direction based on the input
            if (interaction.Get_Next_Input(type) != KeyCode.None)
            {

                

                //*! Allow the player update so call this function and move the player.
                is_moving = true;

                //*! Jump out if the next input is None
                if (interaction.Get_Next_Input(type) == KeyCode.None)
                {
                    Debug.LogError(interaction.Get_Next_Input(type));

                    //*! Check if the player is grounded
                    ///ground_check.Touching();

                    return;
                }
                else
                {
                    //*! Automatically move the player towards its next location based on the next input
                    interaction.Move_Player_By_Next_Input(type, interaction.Get_Next_Input(type));
                }
            }
            //else
            //{
            //    Debug.Log("No next input.");

            //    //*! Check if the player is grounded
            //    ground_check.Touching();

            //}



        }
        
    }

  


    #endregion
}
