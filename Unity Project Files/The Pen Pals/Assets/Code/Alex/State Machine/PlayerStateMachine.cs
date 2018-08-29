//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//

//*! Using namespaces
using UnityEngine;

 

public class PlayerStateMachine : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    //*! Input codes
    [SerializeField] private KeyCode move_up_key;
    [SerializeField] private KeyCode move_down_key;
    [SerializeField] private KeyCode move_left_key;
    [SerializeField] private KeyCode move_right_key;


    //*! Bool Checks for what key was pressed - BUTTON STATES
    private bool up_key_pressed;
    private bool down_key_pressed;
    private bool left_key_pressed;
    private bool right_key_pressed;


    private bool is_grounded;
    private bool is_moving;

    private PlayerState player_state;

    private Vector2 grid_position;
    
    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables
    public enum PlayerState
    {
        NONE,
        MOVING,
        FALLING
    }


    //*! Current Input
    public Temp_Node_Map.Node Current_Node = null;
    //*! Next Input
    public Temp_Node_Map.Node Next_Node = null;
    //*! Queued Input
    public Temp_Node_Map.Node Queued_Node = null;


    //*! Graph Container
    public Temp_Node_Map Node_Graph;

    //*! Property Accessor(s)
    public KeyCode Up_Key
    { get; set; }

    public KeyCode Down_Key
    { get; set; }

    public KeyCode Left_Key
    { get; set; }

    public KeyCode Right_Key
    { get; set; }

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        //*! Set the base state
        player_state = PlayerState.NONE;

        //*! Assign the current grid position of the current world coodinates.
        grid_position.x = transform.position.x;
        grid_position.y = transform.position.y;

        //*! Current node is alligned to where it was placed
        Current_Node = Node_Graph.BL_Nodes[(int)grid_position.x, (int)grid_position.y];

        //*! Clear the next node
        Next_Node = null;

        //*! Check if the player is grounded
        Ground_Check();

        //*! if the player is not grounded
        if (is_grounded == false)
        {
            //*! Start Moving
            Move_Towards_Next_Node();
        }
        
    }

    /// <summary>
    ///  Main Update loop for the state machine
    /// </summary>
    private void Update()
    {
        Check_Player_State();
    }

    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions
    
    public void Check_Player_State()
    {
        switch (player_state)
        {
            case PlayerState.NONE:
                None_State();
                break;
            case PlayerState.MOVING:
                Moving_State();
                break;
            case PlayerState.FALLING:
                Falling_State();
                break;
            default:
                break;
        }
    }
 

    #endregion
    

    //*! Private Access
    #region Private Functions


    #region State Functions

    private void None_State()
    {
        //*! Check for input from the player
        if (Check_Input() == true)
        {
            //*! Change the state
            player_state = PlayerState.MOVING;
            //*! Set the flag
            is_moving = true;
            //*! Get out
            return;
        }
        

        #region Update State
        //*! When the player IS NOT grounded and not moving, they are FALLING
        if (Ground_Check() == false && is_moving == false)
        {
            //*! Change state
            player_state = PlayerState.FALLING;
            //*! Set the flag
            is_moving = true;
            //*! Get out
            return;
        }
        //*! When they are NOT grounded but they are moving, they are FALLING
        else if (Ground_Check() == false && is_moving == true)
        {
            //*! Change state
            player_state = PlayerState.FALLING;
            //*! Get out
            return;
        }
        //*! When the player IS grounded and not moving, they are MOVING
        else if ((Ground_Check() == true && is_moving == true))
        {
            //*! Change state
            player_state = PlayerState.MOVING;
            //*! Get out
            return;
        }
        //*! When the player IS grounded and not moving, they are MOVING
        else if ((Ground_Check() == true && is_moving == false))
        {
            //*! Change state
            player_state = PlayerState.NONE;
            //*! Get out
            return;
        }
        #endregion
    }

    private void Moving_State()
    {
        //*! Check for input from the player
        if (Check_Input() == true)
        {
            //*! Change the state
            player_state = PlayerState.MOVING;
            //*! Set the flag
            is_moving = true;
            //*! Get out
            return;
        }

        //*! Start Moving
        Move_Towards_Next_Node();

        #region Update State
        //*! When the player IS NOT grounded and not moving, they are FALLING
        if (Ground_Check() == false && is_moving == false)
        {
            //*! Change state
            player_state = PlayerState.FALLING;
            //*! Get out
            return;
        }
        //*! When they are NOT grounded but they are moving, they are MOVING
        else if (Ground_Check() == false && is_moving == true)
        {
            //*! Change state
            player_state = PlayerState.MOVING;
            //*! Get out
            return;
        }
        //*! When the player IS grounded and not moving, they are MOVING
        else if ((Ground_Check() == true && is_moving == true))
        {
            //*! Change state
            player_state = PlayerState.MOVING;
            //*! Get out
            return;
        }
        //*! When the player IS grounded and not moving, they are MOVING
        else if ((Ground_Check() == true && is_moving == false))
        {
            //*! Change state
            player_state = PlayerState.NONE;
            //*! Get out
            return;
        }
        #endregion
    }
    
    private void Falling_State()
    {

        //*! Player is moving / falling
        is_moving = true;

        //*! Start Moving
        Move_Towards_Next_Node();

        #region Update State
        //*! When the player is grounded
        if (Ground_Check() == true)
        {
            //*! Change state
            player_state = PlayerState.NONE;

            //*! Get out
            return;
        }
        //*! When the player is grounded
        if (Ground_Check() == false)
        {
            //*! Change state
            player_state = PlayerState.FALLING;

            //*! Get out
            return;
        }
        #endregion
    }

    #endregion //*! End of State Functions

    private bool Check_Input()
    {
        //*! Default State of Queued Node
        Queued_Node = null;

        //*! Assign the input to Queued Node, returns null if there was none
        Queued_Node = Controller_Input();

        //*! Was a value assigned to the Queued Node
        return (Queued_Node != null) ? true : false;
    }
    

    private bool Ground_Check()
    {
        //*! Default State for Queued Node
        Queued_Node = null;

        //*! Can go down from the current grid position
        if (Current_Node.Can_DN == true)
        {
            //*! Assign the current grid positions down node to the Queued Node
            Queued_Node = Current_Node.DN_NODE;

            //*! Update State
            player_state = PlayerState.FALLING;

            //*! Is NOT Grounded
            is_grounded = false;
        }
        else
        {
            //*! Can NOT go down from the current grid position
            Queued_Node = null;

            //*! Reset the key pressed flags
            up_key_pressed = false;
            down_key_pressed = false;
            left_key_pressed = false;
            right_key_pressed = false;


            //*! Update State
            player_state = PlayerState.NONE;

            //*! Is Grounded
            is_grounded = true;
        }

        //*! Was a value assigned to the Queued Node
        return (Queued_Node != null) ? false : true;
    }


    private void Move_Towards_Next_Node()
    {
 
        ///*! if Next Node has a value, the above checks won't see it and it will move the player towards the next node
        if (Next_Node == null)
        {
            Check_Node_Queue();
        }
        else
        {
            //*! Move towards the next node
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(Next_Node.Position.x - 0.5f, Next_Node.Position.y - 0.5f, 0), 4 * Time.deltaTime);
        }

            

        //*! Has the player arrived at the next node?
        if (transform.position == new Vector3(Next_Node.Position.x - 0.5f, Next_Node.Position.y - 0.5f, 0))
        {
            //*! Stays not moving unless it is falling or player input was made
            is_moving = false;

            //*! Shift the next node into the current node
            Current_Node = Next_Node;

            //*! Update the grid position
            grid_position.x = Current_Node.Position.x;
            grid_position.y = Current_Node.Position.y;
             
            //*! Clear the next node - stays null unless the player is falling
            Next_Node = null;
            //*! When the Next Node is null
            if (Next_Node == null)
            {
                Check_Node_Queue();
            }

            //*! Check for if the player is on the ground
            if (Ground_Check() == true)
            {
                //*! Player is grounded set to none state - waiting for input
                player_state = PlayerState.NONE;
            }
            else if (Ground_Check() == false)
            {
                //*! Player is not grounded set to falling state
                player_state = PlayerState.FALLING;
            }
        }
 




    }

    private bool Validated_Queued_Node()
    {
        //*! What key was pressed
        if (up_key_pressed == true)
        {
            //*! Does the corresponding node in the direction match 
            if (Next_Node.UP_NODE == Queued_Node.DN_NODE)
            {
                //*! The Queued Node can be traversed from the Next Node
                return true;
            }
            else
            {
                return false;
            }
        }
        //*! What key was pressed
        else if (down_key_pressed == true)
        {
            //*! Does the corresponding node in the direction match 
            if (Next_Node.DN_NODE == Queued_Node.UP_NODE)
            {
                //*! The Queued Node can be traversed from the Next Node
                return true;
            }
            else
            {
                return false;
            }
        }
        //*! What key was pressed
        else if (left_key_pressed == true)
        {
            //*! Does the corresponding node in the direction match 
            if (Next_Node.LFT_NODE == Queued_Node.RGT_NODE)
            {
                //*! The Queued Node can be traversed from the Next Node
                return true;
            }
            else
            {
                return false;
            }
        }
        //*! What key was pressed
        else if (right_key_pressed == true)
        {
            //*! Does the corresponding node in the direction match 
            if (Next_Node.RGT_NODE == Queued_Node.LFT_NODE)
            {
                //*! The Queued Node can be traversed from the Next Node
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }


    }


    private void Check_Node_Queue()
    {

        //*! Next and Queued Node are null
        if (Next_Node == null && Queued_Node == null)
        {
            //*! Do nothing
            //*! Change State
            player_state = PlayerState.NONE;
            //*! Get out
            return;
        }
        //*! Next Node is empty and Queued Node has a value
        else if (Next_Node == null && Queued_Node != null)
        {
            //*! Check to see if the Queued Node can be reached by the Next Node
            if (Validated_Queued_Node() == true && player_state == PlayerState.MOVING)
            {
                //*! Shift the node into the Next Node
                Next_Node = Queued_Node;
                //*! Clear
                Queued_Node = null;
                
            }
            else if (Validated_Queued_Node() == false && player_state == PlayerState.FALLING)
            {
                //*! Shift the node into the Next Node
                Next_Node = Queued_Node;
                //*! Clear
                Queued_Node = null;
                
            }
        }
  
    }

    /// <summary>
    /// Checks the current key pressed and sets the appropirate flag
    /// </summary>
    /// <returns>-The node to be assigned to the queue-</returns>
    private Temp_Node_Map.Node Controller_Input()
    {
        //*! Up key was pressed
        if (Input.GetKeyDown(Up_Key) == true /*&& up_key_pressed == false*/)
        {
            //*! Set the flag for later use
            up_key_pressed = true;

            //*! Return the node UP to the current grid position
            return Current_Node.UP_NODE;     //*! Up Node
        }
        else if (Input.GetKeyDown(Down_Key) == true /*&& down_key_pressed == false*/)
        {
            //*! Set the flag for later use
            down_key_pressed = true;

            //*! Return the node DOWN to the current grid position
            return Current_Node.DN_NODE;     //*! Down Node
        }
        else if (Input.GetKeyDown(Left_Key) == true /*&& left_key_pressed == false*/)
        {
            //*! Has the player jumped
            if (up_key_pressed == true)
            {
                //*! Set the flag for later use
                left_key_pressed = true;

                //*! Set the flag for later use
                right_key_pressed = true;
            }
            else
            {
                //*! Set the flag for later use
                left_key_pressed = true;
            }

            //*! Return the node LEFT to the current grid position
            return Current_Node.LFT_NODE;    //*! Left Node
        }
        else if (Input.GetKeyDown(Right_Key) == true /*&& right_key_pressed == false*/)
        {
            //*! Has the player jumped
            if (up_key_pressed == true)
            {
                //*! Set the flag for later use
                left_key_pressed = true;

                //*! Set the flag for later use
                right_key_pressed = true;
            }
            else
            {
                //*! Set the flag for later use
                right_key_pressed = true;
            }

            //*! Return the node RIGHT to the current grid position
            return Current_Node.RGT_NODE;    //*! Right Node
        }
        else
        {
            return null;    //*! Nothing
        }
    }

    #endregion //*! End of Private functions


    //*! Protected Access
    #region Protected Functions

    #endregion
    
}