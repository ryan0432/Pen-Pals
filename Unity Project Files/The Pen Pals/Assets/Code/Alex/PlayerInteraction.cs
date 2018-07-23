//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;


public class PlayerInteraction : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    

    [SerializeField]
    private Blocky blocky = new Blocky();

    [SerializeField]
    private Lionel lionel = new Lionel();

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    public enum Player_Type
    {
        RED,
        BLUE
    }

    public enum Movement_Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    //*! Player Interaction Singleton
    public static PlayerInteraction Instance;

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

    }
 
    private void Update()
    {
 
    }



    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions


    #endregion



    //*! Private Access
    #region Private Functions
    

    //*! Move the player in the direction based on the input the player gave. Move by set distance
    private void Move_Player_Direction(Player_Type a_player_Type, Movement_Direction a_movement_Direction)
    {
        //*! Get a reference to the player object for 'blocky' and 'lionel'
        blocky.player = Player.Instance;
        lionel.player = Player.Instance;

        switch (a_player_Type)
        {
            case Player_Type.RED:

                //*! Apply the movement to the player - blocky
                switch (a_movement_Direction)
                {
                    case Movement_Direction.UP:
                        //+- Y Axis
                        blocky.player.current_position += new Vector2(0, blocky.player.movement_distance);
                        break;
                    case Movement_Direction.DOWN:
                        //-- Y Axis
                        blocky.player.current_position -= new Vector2(0, blocky.player.movement_distance);
                        break;
                    case Movement_Direction.LEFT:
                        //-- X Axis
                        blocky.player.current_position -= new Vector2(blocky.player.movement_distance, 0);
                        break;
                    case Movement_Direction.RIGHT:
                        //+- X Axis
                        blocky.player.current_position += new Vector2(blocky.player.movement_distance, 0);
                        break;
                    default:
                        break;
                }

                break;


            case Player_Type.BLUE:

                //*! Apply the movement to the player - lionel
                switch (a_movement_Direction)
                {
                    case Movement_Direction.UP:
                        //+- Y Axis
                        lionel.player.current_position += new Vector2(0, lionel.player.movement_distance);
                        break;
                    case Movement_Direction.DOWN:
                        //-- Y Axis
                        lionel.player.current_position -= new Vector2(0, lionel.player.movement_distance);
                        break;
                    case Movement_Direction.LEFT:
                        //-- X Axis
                        lionel.player.current_position -= new Vector2(lionel.player.movement_distance, 0);
                        break;
                    case Movement_Direction.RIGHT:
                        //+- X Axis
                        lionel.player.current_position += new Vector2(lionel.player.movement_distance, 0);
                        break;
                    default:
                        break;
                }

                break;

                
            default:
                break;
        }
    }

    //*! Check if Blocky has inputed something
    private bool Get_Blocky_Input()
    {
        //*! Check if the player is falling, if they are then there is no control in the air.
        if (blocky.is_falling)
            return false;


        //*! Blocky Input Checks
        if (Input.GetKeyDown(blocky.Controls.move_up_key))
        {
            Move_Player_Direction(Player_Type.RED, Movement_Direction.UP);
            return true;
        }
        else if (Input.GetKeyDown(blocky.Controls.move_down_key))
        {
            Move_Player_Direction(Player_Type.RED, Movement_Direction.DOWN);
            return true;
        }
        else if (Input.GetKeyDown(blocky.Controls.move_left_key))
        {
            Move_Player_Direction(Player_Type.RED, Movement_Direction.LEFT);
            return true;
        }
        else if (Input.GetKeyDown(blocky.Controls.move_right_key))
        {
            Move_Player_Direction(Player_Type.RED, Movement_Direction.RIGHT);
            return true;
        }

        return false;
    }

    //*! Check if Lionel has inputed something
    private bool Get_Lionel_Input()
    {
        //*! Lionel Input Checks
        if (Input.GetKeyDown(lionel.Controls.move_up_key))
        {
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.UP);
            return true;
        }
        else if (Input.GetKeyDown(lionel.Controls.move_down_key))
        {
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.DOWN);
            return true;
        }
        else if (Input.GetKeyDown(lionel.Controls.move_left_key))
        {
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.LEFT);
            return true;
        }
        else if (Input.GetKeyDown(lionel.Controls.move_right_key))
        {
            Move_Player_Direction(Player_Type.BLUE, Movement_Direction.RIGHT);
            return true;
        }

        return false;
    }

    #endregion



    //*! Inherited Access
    #region Protected Functions


    //*! Checking for player input
    public bool Check_For_Input(Player_Type a_player_type)
    {
        switch (a_player_type)
        {
            case Player_Type.RED:
                return Get_Blocky_Input();
                ///break;
            case Player_Type.BLUE:
                return Get_Lionel_Input();
                ///break;
            default:
                return false;
                ///break;
        }

        ///return false;
    }

    #endregion


}


/// <summary>
/// Blocky Class Object
/// </summary>
[System.Serializable]
public class Blocky
{
    [HideInInspector]
    //*! Player Class Reference
    public Player player = Player.Instance;

    [SerializeField]
    //*! Controls
    private Movement controls;

    public Movement Controls
    { get { return controls; } }


    //*! Is the player falling - used for disabling the input (No air control)
    public bool is_falling;



}

/// <summary>
/// Lionel Class Object
/// </summary>
[System.Serializable]
public class Lionel
{

    [HideInInspector]
    //*! Player Class Reference
    public Player player = Player.Instance;

    [SerializeField]
    //*! Controls
    private Movement controls;

    public Movement Controls
    { get { return controls; } }

}

/// <summary>
/// Universal Controls for both players
/// </summary>
[System.Serializable]
public struct Movement
{

    //*! Controls
    public KeyCode move_up_key;
    public KeyCode move_down_key;
    public KeyCode move_right_key;
    public KeyCode move_left_key;

    public static void Check(PlayerInteraction.Player_Type a_player_type)
    {
        //PlayerInteraction.Check_For_Input(a_player_type);
    }
 

}