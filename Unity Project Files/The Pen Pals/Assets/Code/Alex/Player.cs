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

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        type = PlayerInteraction.Player_Type.BLUE;

        interaction = PlayerInteraction.Instance;
    }

    private void Update()
    {
        //*! Check the input of the player based on the type
        interaction.Check_For_Input(type);

        transform.position = new Vector3(current_position.x, current_position.y, 0);

    }
        


    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions

 

    #endregion



    //*! Private Access
    #region Private Functions

 
    #endregion
}
