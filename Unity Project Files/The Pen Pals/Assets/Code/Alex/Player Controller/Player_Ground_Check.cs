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
    private PlayerInteraction interaction;

    private Player attached_player;

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
        interaction = PlayerInteraction.Instance;

        //*! Short hand for grabbing the player
        attached_player = transform.parent.GetComponent<Player>();

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
        if (touching_ground)
        {
            //Debug.Log("Player is grounded!");
            switch (attached_player.Type)
            {
                case PlayerInteraction.Player_Type.RED:
                    interaction.Red.is_grounded = true;
                    break;
                case PlayerInteraction.Player_Type.BLUE:
                    interaction.Blue.is_grounded = true;
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
                case PlayerInteraction.Player_Type.RED:
                    interaction.Red.is_grounded = false;
                    break;
                case PlayerInteraction.Player_Type.BLUE:
                    interaction.Blue.is_grounded = false;
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