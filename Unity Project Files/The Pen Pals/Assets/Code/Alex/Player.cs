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

    private bool is_moving;
    private float timer;

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

    public AnimationCurve movement_curve;

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
        //type = PlayerInteraction.Player_Type.BLUE;

        interaction = PlayerInteraction.Instance;
    }

    private void Update()
    {
        //*! Check the input of the player based on the type
        if (interaction.Check_For_Input(type))
        {
            is_moving = true;
        }
        


        //*! Apply the new Position
        Apply_New_Position(transform.position, new Vector3(current_position.x, current_position.y, 0));



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
        //if (timer <= movement_curve.length && is_moving)
        //{
        //    timer += Time.deltaTime;
        //    Debug.Log(movement_curve.Evaluate(timer));
        //}
        //else
        //{
        //    Debug.LogWarning("Loop");
        //    timer = 0.0f;
        //}


        transform.position = new Vector3(current_position.x, current_position.y, 0);
    }

    #endregion
}
