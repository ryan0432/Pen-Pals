//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//*! Player Block Collision Check
public class Block_Collision_Ground : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables
    //*! Is the player touching an object that has the tag 'Ground'
    private bool touching_ground;

    //*! Player interaction Grounded
    private Player_Block_Interaction interaction;

    private Player_Block attached_player;

    private Player_Block player_ground_block;
 

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    //*! Singleton for the Ground Checker
    public static Block_Collision_Ground Instance;





    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions

    private void Awake()
    {
        //*! Singleton of this
        Instance = this;
    }


    private void Start()
    {
        //*! Interaction Base Instacne
        interaction = Player_Block_Interaction.Instance;

        //*! Player Red / Blue Instances of Player_Base
        if (player_ground_block == null)
        {
            player_ground_block = interaction.PLAYER_BLOCK_DATA.player_block_interaction;
        }


        //*! Parent Player_Base Reference
        attached_player = gameObject.transform.parent.GetComponent<Player_Block>();
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

        //*! When the player is Grounded
        if (touching_ground)
        {
            interaction.PLAYER_BLOCK_DATA.is_grounded = true;
            return true;
        }
        else
        {
            interaction.PLAYER_BLOCK_DATA.is_grounded = false;
            interaction.PLAYER_BLOCK_DATA.Controls.can_move_up = false;
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