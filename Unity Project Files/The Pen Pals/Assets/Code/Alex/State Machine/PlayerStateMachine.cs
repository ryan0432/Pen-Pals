//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//

//*! Using namespaces
using UnityEngine;
using System.Collections;


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
    private bool can_second;

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


    //*! Previous Input
    public Temp_Node_Map.Node Previous_Node = null;
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
    { get { return move_up_key; } private set { } }

    public KeyCode Down_Key
    { get { return move_down_key; } private set { } }

    public KeyCode Left_Key
    { get { return move_left_key; } private set { } }

    public KeyCode Right_Key
    { get { return move_right_key; } private set { } }

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
        player_state = PlayerState.MOVING;

        //*! Assign the current grid position of the current world coodinates.
        grid_position.x = transform.position.x;
        grid_position.y = transform.position.y;

        //*! Current node is alligned to where it was placed
        Current_Node = Node_Graph.BL_Nodes[(int)grid_position.x, (int)grid_position.y];

        is_grounded = false;

        Ground_Check();
    }

    /// <summary>
    ///  Main Update loop for the state machine
    /// </summary>
    private void Update()
    {
        ///Check_Player_State();

        ///Remove soon
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //*! Reset the key pressed flags
            up_key_pressed = false;
            down_key_pressed = false;
            left_key_pressed = false;
            right_key_pressed = false;
        }

        Just_Move_Input();
    }

    #endregion

    /// <summary>
    /// Temp Code to just get it to move...
    /// </summary>
    void Just_Move_Input()
    {
        //*! Only when the player is not moving but are grounded
        if (is_moving == false && is_grounded == true)
        {
            if (Queued_Node == null)
            {
                //*! Player Input checks - based on Current Node position.
                Queued_Node = Controller_Input();
            }

        }


        /*-can second input to override the current Queued node-*/
        /*-when 90% distance remaining of current to next -*/
        if (can_second == true)
        {
            if (Queued_Node == null)
            {
                //*! Player Input checks - based on Current Node position.
                Queued_Node = Controller_Input();
            }

            //*! Reset the second input flag
            can_second = false;
        }


        if (Queued_Node != null)
        {
            //*! Shift nodes if next is empty
            if (Next_Node == null && Queued_Node != null)
            {
                //*! Shift Queued into the next node
                Next_Node = Queued_Node;

                //*! Clear the Queued node
                Queued_Node = null;
            }
        }


        //*! Move player next node is not null
        if (Next_Node != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(Next_Node.Position.x - 0.5f, Next_Node.Position.y - 0.5f, 0), 4 * Time.deltaTime);


            //*! Get the distance from the player to the next node
            float mag_distance = (Next_Node.Position - transform.position).magnitude;
            ///Debug.Log(mag_distance);
            //*! If distance is less then the threshhold - allow player to override the Queued node
            if (mag_distance < 0.9f)
            {
                can_second = true;
            }


            //*! Reached the next node
            if (transform.position == new Vector3(Next_Node.Position.x - 0.5f, Next_Node.Position.y - 0.5f, 0))
            {
                //*! Reset the seond input permission
                can_second = false;

                //*! Shift the next node into the current node
                Current_Node = Next_Node;

                //*! Clear the next node
                Next_Node = null;
               
                //*! Update the grid position
                grid_position.x = Current_Node.Position.x;
                grid_position.y = Current_Node.Position.y;
 

                if (Queued_Node == null)
                {
                    Ground_Check();
                }
                //*! Queued node is not null
                else
                {
                    //*! If the Current Node can not reach the Queued node
                    //*! Check if the player is grounded
                    if (Validated_Queued_Node() == false)
                    {
                        Ground_Check();
                    }
                    //*! Else the current node can move towards the queued node.
                }

            }
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


  
    private bool Check_Input()
    {
        //*! Queued node not null but position is zero
        if (Queued_Node != null && Queued_Node.Position == Vector3.zero)
        {
            //*! Default State of Queued Node
            Queued_Node = null;

            //*! Assign the input to Queued Node, returns null if there was none
            Queued_Node = Controller_Input();

            //*! Was a value assigned to the Queued Node
            return (Queued_Node != null) ? true : false;
        }
        else
        {
            //*! Default returm
            return false;
        }
    }

    /// <summary>
    /// Returns false if the player is not grounded
    /// </summary>
    /// <returns></returns>
    private bool Ground_Check()
    {
        //*! Can go down from the current grid position
        if (Current_Node.Can_DN == true)
        {               
            //*! Assign the current grid positions down node to the Queued Node
            Queued_Node = Current_Node.DN_NODE;

            //*! Is NOT Grounded
            is_grounded = false;            
        }
        else
        {
            //*! Can not go down from current node

            Next_Node = null;
            Queued_Node = null;

            //*! Reset the key pressed flags
            up_key_pressed = false;
            down_key_pressed = false;
            left_key_pressed = false;
            right_key_pressed = false;

            //*! Is Grounded
            is_grounded = true;
        }

        //*! Was a value assigned to the Queued Node
        return (Queued_Node != null) ? false : true;
    }

 
    /// <summary>
    /// Can the Queued node be reached by the Current node
    /// </summary>
    /// <returns> if the current can reach the Queued node </returns>
    private bool Validated_Queued_Node()
    {
        //*! What key was pressed
        if (up_key_pressed == true)
        {
            //*! Does the corresponding node in the direction match 
            if (Current_Node.UP_NODE == Queued_Node.DN_NODE)
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
            if (Current_Node.DN_NODE == Queued_Node.UP_NODE)
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
            if (Current_Node.LFT_NODE == Queued_Node.RGT_NODE)
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
            if (Current_Node.RGT_NODE == Queued_Node.LFT_NODE)
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



 
    /// <summary>
    /// Checks the current key pressed and sets the appropirate flag
    /// </summary>
    /// <returns>-The node to be assigned to the queue-</returns>
    private Temp_Node_Map.Node Controller_Input()
    {
        //*! Up key was pressed
        if (Input.GetKeyDown(Up_Key) == true && up_key_pressed == false)
        {
            //*! Set the flag for later use
            up_key_pressed = true;

            //*! Not grounded
            is_grounded = false;

            //*! Return the node UP to the current grid position BUT if the next node is not null assign the node adacent to next
            return (Next_Node != null) ? Next_Node.UP_NODE : Current_Node.UP_NODE;     //*! Up Node
        }
        else if (Input.GetKeyDown(Down_Key) == true /*&& down_key_pressed == false*/)
        {
            //*! Set the flag for later use
            down_key_pressed = true;

            //*! Return the node DOWN to the current grid position
            return (Next_Node != null) ? Next_Node.DN_NODE : Current_Node.DN_NODE;     //*! Down Node
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
            //*! Move along ground
            else if (is_grounded == true)
            {
                //*! Set the flag for later use
                left_key_pressed = false;
            }
            else
            {
                //*! Set the flag for later use
                left_key_pressed = true;
            }

            //*! Return the node LEFT to the current grid position
            return (Next_Node != null) ? Next_Node.LFT_NODE : Current_Node.LFT_NODE;    //*! Left Node
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
            //*! Move along ground
            else if (is_grounded == true)
            {
                //*! Set the flag for later use
                right_key_pressed = false;
            }
            else
            {
                //*! Set the flag for later use
                right_key_pressed = true;
            }

            //*! Return the node RIGHT to the current grid position
            return (Next_Node != null) ? Next_Node.RGT_NODE : Current_Node.RGT_NODE;    //*! Right Node
        }
        else
        {
            return Next_Node;   
        }
        
    }

    #endregion //*! End of Private functions


    //*! Protected Access
    #region Protected Functions

    #endregion
    
}
 