//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//


//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    private PlayerInteraction interaction;

    //*! if the player is moving lock out the controls input
    private bool is_moving;

    //*! When can the player input a movement control
    private bool can_move;
    
 
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

   

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//

    private void Awake()
    {
        Instance = this;        

        if (movement_distance < 1)
            movement_distance = 1;
    }

    private void Start()
    {
        //*! Get a singleton reference to the interaction component
        interaction = PlayerInteraction.Instance;

        //*! Can move is set to true allowing the player to move
        can_move = true;
    }

    private void Update()
    {
        //*! Check the input of the player based on the type
        if (!is_moving && can_move && interaction.Check_For_Input(type))
        {
            is_moving = true;
            //disable input
            can_move = false;
        }

    
        //*! When the player is moving, move it
        else if (is_moving)
        {
            // Set the next input 
            interaction.Set_Player_Next_Input(type, interaction.Get_Now_Input(type));

            //*! Apply the new Position
            Apply_New_Position(transform.position, new Vector3(current_position.x, current_position.y, 0));
        }


    }

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions



    #endregion



    //*! Private Access
    #region Private Functions

    //*! New Position Apply
    private void Apply_New_Position(Vector3 old_position, Vector3 new_position)
    {
        
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(current_position.x, current_position.y, 0), Time.deltaTime * movement_speed);
        //mag check 50% ~ near allow for second input
        float mag = (transform.position - new Vector3(current_position.x, current_position.y, 0)).magnitude;

        

        if (mag < 0.75f)
        {
            //Debug.Log("mag : " + m.ToString("0.00"));

        }

        if (transform.position == new Vector3(current_position.x, current_position.y, 0))
        {
            
            is_moving = false;
            // re enable input
            can_move = true;

            //*! Not nothing - move the player in that direction based on the input
            if (interaction.Get_Current_Input(type) != KeyCode.None)
            {
                interaction.Move_Player_By_Next_Input(type, interaction.Get_Next_Input(type));
            }
        }
        else
        {
            is_moving = true;
            // disable input
            can_move = false;
        }
    }

    #endregion
}
