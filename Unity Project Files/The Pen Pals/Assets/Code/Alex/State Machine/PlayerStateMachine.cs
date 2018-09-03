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


    /// <summary>
    /// As these are worded with past tense. In the action of doing so you set it to true, otherwise you have not done it, false
    /// </summary>
    private bool up_key_pressed;
    /// <summary>
    /// As these are worded with past tense. In the action of doing so you set it to true, otherwise you have not done it, false
    /// </summary>
    private bool down_key_pressed;
    /// <summary>
    /// As these are worded with past tense. In the action of doing so you set it to true, otherwise you have not done it, false
    /// </summary>
    private bool left_key_pressed;
    /// <summary>
    /// As these are worded with past tense. In the action of doing so you set it to true, otherwise you have not done it, false
    /// </summary>
    private bool right_key_pressed;


    private bool is_grounded;
    private bool is_moving;
    private bool can_second;
    private bool is_falling;


    private Vector2 grid_position;
    
    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables
 

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

        //*! Assign the current grid position of the current world coodinates.
        grid_position.x = transform.position.x;
        grid_position.y = transform.position.y;

        //*! Current node is alligned to where it was placed
        Current_Node = Node_Graph.BL_Nodes[(int)grid_position.x, (int)grid_position.y];

        is_grounded = Ground_Check();

    }
        

    /// <summary>
    ///  Main Update loop for the state machine
    /// </summary>
    private void Update()
    {
        ///Check_Player_State();

        /////Remove soon
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Queued_Node = Node_Graph.BL_Nodes[(int)transform.position.x, (int)transform.position.y];
        //}

        Just_Move_Input();
    }

    #endregion


    void Just_Move_Input()
    {
        //*! Only when the player is not moving but are grounded
        if (is_moving == false && is_grounded == true && is_falling == false)
        {
            //*! Player Input checks - based on Current Node position.
            Queued_Node = Controller_Input();

            //*! Does it have a value, did the player input something?
            if (Queued_Node != null)
            {
                is_moving = true;
            }

        }



        /*-can second input to override the current Queued node-*/
        /*-when X% distance remaining of current to next -*/
        if (is_moving == true && can_second == true && is_falling == false)
        {
            //*! Player Input checks - based on Current Node position.
            Queued_Node = Controller_Input();

            //*! Does it have a value, did the player input something?
            if (Queued_Node != null)
            {
                //*! Reset the second input flag
                can_second = false;
            }
        }
            

        //*! Does Queued node have a value
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
            //*! Move towards with precision to have the player exactly reach the next node
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(Next_Node.Position.x - 0.5f, Next_Node.Position.y - 0.5f, 0), 4 * Time.deltaTime);


            //*! Get the distance from the player to the next node
            float mag_distance = (transform.position - new Vector3(Next_Node.Position.x - 0.5f, Next_Node.Position.y - 0.5f, 0)).magnitude;
           
            //*! If distance is less then the threshhold - allow player to override the Queued node
            if (mag_distance < 1.0f)
            {
                can_second = true;
            }


            //*! Reached the next node
            if (transform.position == new Vector3(Next_Node.Position.x - 0.5f, Next_Node.Position.y - 0.5f, 0))
            {
                //*! Finished moving, unless the below checks override that
                is_moving = false;

                //*! Reset the seond input permission
                can_second = false;

                //*! Shift the next node into the current node
                Current_Node = Next_Node;

                //*! Clear the next node
                Next_Node = null;
               
                //*! Update the grid position
                grid_position.x = Current_Node.Position.x;
                grid_position.y = Current_Node.Position.y;


                //*! Does Queued node have a value
                if (Queued_Node != null)
                {
                    ///Debug.Log("Q: Not null" + Queued_Node.Position);
                    //*! Shift nodes if next is empty
                    if (Next_Node == null && Queued_Node != null)
                    {
                        ///Debug.Log("N: null" + Next_Node);
                        //*! Shift Queued into the next node
                        Next_Node = Queued_Node;
                        ///Debug.Log("N: not null" + Next_Node.Position);
                        //*! Clear the Queued node
                        Queued_Node = null;
                        ///Debug.Log("Q: null" + Queued_Node);
                    }
                }
                else
                {
                    ///Debug.Log("Q: null - Ground Check!");
                    Ground_Check();
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

            //*! Player is now falling - lock out controlls until grounded
            is_falling = true;

            //*! Not grounded, so they must be moving / falling
            is_moving = true;
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

            //*! Player is not falling as it is grounded - re-enable the controlls
            is_falling = false;

            //*! Stopped moving
            is_moving = false;
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
        else if (Input.GetKeyDown(Down_Key) == true && down_key_pressed == false)
        {
            //*! Set the flag for later use
            down_key_pressed = true;

            //*! Return the node DOWN to the current grid position
            return (Next_Node != null) ? Next_Node.DN_NODE : Current_Node.DN_NODE;     //*! Down Node
        }
        else if (Input.GetKeyDown(Left_Key) == true && left_key_pressed == false)
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
                left_key_pressed = true;
            }


            //*! Return the node LEFT to the current grid position
            //*! If the next node doesnt have a value, current node's left
            //*! If the next node has a value then it hasn't being cleared being that it has not reached the next node, return the next nodes left
            return (Next_Node != null) ? Next_Node.LFT_NODE : Current_Node.LFT_NODE;    //*! Left Node
        }
        else if (Input.GetKeyDown(Right_Key) == true && right_key_pressed == false)
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
                right_key_pressed = true;
            }


            //*! Return the node RIGHT to the current grid position
            //*! If the next node doesnt have a value, current node's right
            //*! If the next node has a value then it hasn't being cleared being that it has not reached the next node, return the next nodes right
            return (Next_Node != null) ? Next_Node.RGT_NODE : Current_Node.RGT_NODE;    //*! Right Node
        }
        else
        {
            return Queued_Node;   
        }
        
    }

    #endregion //*! End of Private functions


    //*! Protected Access
    #region Protected Functions

    #endregion
    
}
 