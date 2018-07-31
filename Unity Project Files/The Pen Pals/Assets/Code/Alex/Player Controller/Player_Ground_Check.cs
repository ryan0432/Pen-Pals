//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player_Ground_Check : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables
    //*! Is the player touching an object that has the tag 'Ground'
    private bool touching_ground;

    //*! Player interaction Grounded
    private Player_Base_Interaction interaction;

    private Player_Base attached_player;

    private Player_Base player_RED;
    private Player_Base player_BLUE;

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    //*! Singleton for the Ground Checker
    public static Player_Ground_Check Instance;

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
        interaction = Player_Base_Interaction.Instance;

        player_RED = interaction.Player_RED.player_red;
        player_BLUE = interaction.Player_RED.player_blue;

        attached_player = gameObject.transform.parent.GetComponent<Player_Base>();
    }

    private void Update()
    {
        //*! Is the player touching the ground
        //Ground_Check();   //-! Player calles the Ground Check when its aligned with the grid.
    }



    //*! When the player hits something, or something hits the player.
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground")
        {
            touching_ground = true;
        }
    }
    //*! When the player leaves the ground
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            touching_ground = false;
        }
    }



    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions

    //*! Is the player grounded
    public bool Touching()
    {
        if (attached_player == null)
        {
            return false;
        }

        if (touching_ground)
        {
            //Debug.Log("Player is grounded!");
            switch (attached_player.Type)
            {
                case Player_Base_Interaction.P_Type.RED_BLOCK:
                case Player_Base_Interaction.P_Type.RED_LINE:
                case Player_Base_Interaction.P_Type.RED:
                    interaction.Player_RED.is_grounded = true;
                    //*! When the player is grounded enable all controls
                    //interaction.Player_RED.Controls.can_move_up = true;
                    //interaction.Player_RED.Controls.can_move_down = true;
                    //interaction.Player_RED.Controls.can_move_left = true;
                    //interaction.Player_RED.Controls.can_move_right = true;
                    break;
                case Player_Base_Interaction.P_Type.BLUE:
                    interaction.Player_BLUE.is_grounded = true;
                    //*! When the player is grounded enable all controls
                    //interaction.Player_BLUE.Controls.can_move_up = true;
                    //interaction.Player_BLUE.Controls.can_move_down = true;
                    //interaction.Player_BLUE.Controls.can_move_left = true;
                    //interaction.Player_BLUE.Controls.can_move_right = true;
                    break;
                default:
                    break;
            }
            return true;
        }
        else
        {
            //Debug.Log("Player is NOT grounded!");
            switch (attached_player.Type)
            {
                case Player_Base_Interaction.P_Type.RED_BLOCK:
                case Player_Base_Interaction.P_Type.RED_LINE:
                case Player_Base_Interaction.P_Type.RED:
                    interaction.Player_RED.is_grounded = false;
                    interaction.Player_RED.Controls.can_move_up = false;
                    ///player_RED.Stop_Player();
                    break;
                case Player_Base_Interaction.P_Type.BLUE:
                    interaction.Player_BLUE.is_grounded = false;
                    interaction.Player_BLUE.Controls.can_move_up = false;
                    ///player_BLUE.Stop_Player();
                    break;
                default:
                    break;
            }
            return false;
            
        }
    }

    #endregion



    //*! Private Access
    #region Private Functions


    #endregion



    //*! Protected Access
    #region Protected Functions

    #endregion



}