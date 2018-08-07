//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;



public class Block_Collision_Left : MonoBehaviour {

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables
    //*! Is the player touching an object that has the tag 'Ground'
    private bool touching_left;

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
    public static Block_Collision_Left Instance;





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
        attached_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Block>();
        
    }

    private void Update()
    {
 
    }


    //*! When the player hits something, or something hits the player.
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground")
        {
            touching_left = true;
        }
    }
    //*! When the player leaves the ground
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            //Debug.LogError("IsOUt");
            touching_left = false;
        }
    }



    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions

    //*! Is the player grounded
    public void Touching()
    {
        if (attached_player == null)
        {
            return;
        }

        if (touching_left)
        {
            interaction.PLAYER_BLOCK_DATA.Controls.can_move_left = false;
            return;
        }
        else
        {
            interaction.PLAYER_BLOCK_DATA.Controls.can_move_left = true;
            return;

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
