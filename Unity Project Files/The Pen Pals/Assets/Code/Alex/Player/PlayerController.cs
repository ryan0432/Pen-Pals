//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//


//*! Using namespace(s)
using APS;
using UnityEngine;

/// <summary>
/// Main Player Controller
/// </summary>
public class PlayerController : Abstract_Player
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables
        
    //*! What type the player is. BLOCK OR LINE
    [SerializeField]
    private Player_Type type;

    //*! Used to pass the current players controller key information
    [SerializeField]
    private Controller Controller;
    private Directional_Node button_state;

    //*! if the player is moving lock out the controls input
    [SerializeField]
    private bool is_moving;
    private bool is_grounded;

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    //*! Handball the graph to the parent class
    public Temp_Node_Map Node_Graph;

    //*! Current grid Position
    public Vector2 grid_position;


    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions

    private void Start()
    {
        ///*! Useless code to quieten the unity warning log
        if (is_grounded == false)
        {
            is_grounded = false;
        }


        //*! Handball - Giving the Abstract Player class access to the grid
        Player_Grid = Node_Graph;

        //*! Assign the current grid position of the current world coodinates.
        grid_position.x = transform.position.x;
        grid_position.y = transform.position.y;

        Controller.node_stack.Push(Node_Graph.BL_Nodes[(int)grid_position.x, (int)grid_position.y]);
        Controller.Next_Node = Controller.node_stack.Peek();
        is_moving = true;
        Debug.Log("Node has been added to the node queue - via the start method, to initialise the position on the grid");


        button_state = new Directional_Node();

        ///Controller.node_queue.Dequeue();
        //*! Ground Check on the current grid position
        ///Ground_Check(grid_position);
    }




    /// <summary>
    /// Main game loop
    /// </summary>
    private void FixedUpdate()
    {       
        
        //*! When the player is moving apply the new position
        if (is_moving == true)
        {
            Apply_New_Position();
        } 

        //*! Player input check
        if (Check_Input() == true)
        {
            is_moving = true;
        }
        
        //*! Block Player Ground Check
        if (type == Player_Type.BLOCK)
        {
            //*! Check if the Block player is grounded
            Ground_Check(grid_position);
        }

    }

    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//
    #region Custom Functions


    //*!----------------------------!*//
    //*!    Public Access
    //*!----------------------------!*//
    #region Public Functions
    #endregion

    //*!----------------------------!*//
    //*!    Private Access
    //*!----------------------------!*//
    #region Private Functions


    /// <summary>
    /// Has the player made an input
    /// </summary>
    /// <param name="player_Type"> What player is it checking for </param>
    /// <returns> If they have changed queued nodes </returns>
    private bool Check_Input()
    {
        //*! Store the node that it returns, if null then no input as been made
        button_state = Block_Input(Controller, grid_position, ref button_state);

        if (button_state.node != null)
        {
            #region old_code
            //*! Default return of Block Input is the current nodes position
            ///Controller.node_queue.Enqueue(button_state.node);
            #endregion
            
            Controller.node_stack.Push(button_state.node);
            Debug.Log("Node has been added to the node queue");

            #region old_code

           
            //*! Update the grid position - on the front node in the queue
            ///grid_position.x = Controller.node_stack.Peek().Position.x;
            ///grid_position.y = Controller.node_stack.Peek().Position.y;

            //*! Update the grid position - on the front node in the queue
            ///grid_position.x = Controller.Current_node.Position.x;
            ///grid_position.y = Controller.Current_node.Position.y;
            #endregion

            //*! Update the grid position - on the front node in the queue
            grid_position.x = button_state.node.Position.x;
            grid_position.y = button_state.node.Position.y;
        }

        //*! If the result is null they have not moved.
        if (Controller.node_stack.Count > 0)
        {
            ///Debug.Log("Node added via user input");
            return true;
        }
        else
        {
            return false;
        }
    }
    

    //*! Has the player reached the node that is at the front of the queue - with in a distance away from it
    private void Reached_Next_Node(float mag_distance)
    {
        //*! Reached the end node position
        ///if (transform.position == new Vector3(Controller.node_stack.Peek().Position.x - 0.5f, Controller.node_stack.Peek().Position.y - 0.5f, 0) || mag_distance < 0.05f)
        if (transform.position == new Vector3(Controller.Next_Node.Position.x - 0.5f, Controller.Next_Node.Position.y - 0.5f, 0))
        {
            //*! The final node you move towards is the next node
            //*! If the stack is empty, get out
            if (Controller.node_stack.Count == 0)
            {
                //*! Snap the player to the next nodes position minus the offset
                transform.position = new Vector3(Controller.Next_Node.Position.x - 0.5f, Controller.Next_Node.Position.y - 0.5f, 0);
                
                //*! Finished moving
                is_moving = false;
                
                //*! Get out of this and do nothing else
                //return;
            }
            //*! There was something on the stack
            else
            {

                Validate_Next_Node();
            }
        }

    }


    /// <summary>
    /// 
    /// </summary>
    private void Validate_Next_Node()
    {
        #region old_code
        //*! Update the grid position - on the front node in the queue
        ///grid_position.x = Controller.node_queue.Peek().Position.x;
        ///grid_position.y = Controller.node_queue.Peek().Position.y;

        //*! Update the grid position - on the front node in the queue
        ///grid_position.x = Controller.node_stack.Peek().Position.x;
        ///grid_position.y = Controller.node_stack.Peek().Position.y;

        //*! Assign the current node to the top node in the stack
        ///Controller.Current_node = Controller.node_stack.Pop();
        //*! Pop the front node off the queue
        ///Controller.node_queue.Dequeue();
        #endregion

        //*! Get a copy of the node on top of the stack
        button_state.node = Controller.node_stack.Peek();

        //*! The top of the stack is not the same as the current next node that we just arrived at.
        if (button_state.node != Controller.Next_Node && button_state.node != null)
        {
            //*! Validate that the current next node can move towards the top of the stack, if so assign it to
            //*! the next node to move towards

            //*! Validate current node if it can move in that direction 
            //*! Result to the down node if none of the checks work   
            if (button_state.up_key_pressed == true)
            {
                if (Controller.Next_Node.UP_NODE == button_state.node)
                {
                    //*! Current nodes up is the node on top of the stack
                    Controller.Next_Node = button_state.node;
                }
                else
                {
                    Controller.Next_Node = Controller.Next_Node.DN_NODE;
                }

            }
            else if (button_state.down_key_pressed == true)
            {
                if (Controller.Next_Node.DN_NODE == button_state.node)
                {
                    //*! Current nodes down is the node on top of the stack
                    Controller.Next_Node = button_state.node;
                }
                else
                {
                    Controller.Next_Node = Controller.Next_Node.DN_NODE;
                }

            }
            else if (button_state.left_key_pressed == true)
            {
                if (Controller.Next_Node.LFT_NODE == button_state.node)
                {
                    //*! Current nodes left is the node on top of the stack
                    Controller.Next_Node = button_state.node;
                }
                else
                {
                    Controller.Next_Node = Controller.Next_Node.DN_NODE;
                }

            }
            else if (button_state.right_key_pressed == true)
            {
                if (Controller.Next_Node.RGT_NODE == button_state.node)
                {
                    //*! Current nodes right is the node on top of the stack
                    Controller.Next_Node = button_state.node;
                }
                else
                {
                    Controller.Next_Node = Controller.Next_Node.DN_NODE;
                }

            }
        }

        ///*! What condition do I need to clear the stack correctly

        else if (button_state.node == Controller.Next_Node)
        {

        }
        else //*! Button state is null
        {

        }

        //*! Update the grid position - on the front node in the stack
        grid_position.x = button_state.node.Position.x;
        grid_position.y = button_state.node.Position.y;

        //*! Clear the stack after getting the top node
        Controller.node_stack.Clear();
        Debug.Log("Cleared node stack!");
    }

    /// <summary>
    /// Move player towards the next node
    /// </summary>
    /// <param name="end_node"> Target position to move the player towards </param>
    private void Apply_New_Position()
    {

        //*! IF the count of nodes is more than 0, there is something there
        ///if (Controller.node_queue.Count != 0)
        if (Controller.node_stack.Count > 0)
        {
            #region old_code


            //*! Move towards the front node in the node queue
            ///transform.position = Vector3.MoveTowards(transform.position, new Vector3(Controller.node_queue.Peek().Position.x - 0.5f, Controller.node_queue.Peek().Position.y - 0.5f, 0), 4 * Time.deltaTime);

            //*! Move towards the front node in the node queue
            ///transform.position = Vector3.MoveTowards(transform.position, new Vector3(Controller.node_stack.Peek().Position.x - 0.5f, Controller.node_stack.Peek().Position.y - 0.5f, 0), 4 * Time.deltaTime);

            #endregion

            if (Controller.Next_Node != null)
            {
                //*! Move towards the current node
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(Controller.Next_Node.Position.x - 0.5f, Controller.Next_Node.Position.y - 0.5f, 0), 4 * Time.deltaTime);

                #region old_code

                //*! Get the distance between the two points
                ///float mag_distance = (transform.position - new Vector3(Controller.node_queue.Peek().Position.x - 0.5f, Controller.node_queue.Peek().Position.y - 0.5f, 0)).magnitude;

                //*! Get the distance between the two points
                ///float mag_distance = (transform.position - new Vector3(Controller.node_stack.Peek().Position.x - 0.5f, Controller.node_stack.Peek().Position.y - 0.5f, 0)).magnitude;
                #endregion

                //*! Get the distance between the two points
                float mag_distance = (transform.position - new Vector3(Controller.Next_Node.Position.x - 0.5f, Controller.Next_Node.Position.y - 0.5f, 0)).magnitude;

                //*! Have I reached the next node
                Reached_Next_Node(mag_distance);

            }
        }
        else
        {
            //?
        }
    }

    /// <summary>
    /// Block player only to use.
    /// Adds a node to Queued if the current node position can down == true;
    /// </summary>
    /// <param name="grid_position"></param>
    private void Ground_Check(Vector2 grid_position)
    {
        //*! Store the node that it returns, if null the player is grounded
        button_state = Block_Ground_Check(grid_position, ref button_state);
        
        //*! Button states node is not null meaning that the current grid position can go down
        //*! And the player is not moving, make them fall
        if (button_state.node != null && !is_moving)
        {
            //*! Push the node on top of the stack if they can move in that direction
            Controller.node_stack.Push(button_state.node);

            Debug.Log("Node has been added to the node queue - via the ground check");

            #region old_code

            //*! Update the grid position - on the front node in the queue
            ///grid_position.x = Controller.node_queue.Peek().Position.x;
            ///grid_position.y = Controller.node_queue.Peek().Position.y;

            //*! Update the grid position - on the front node in the queue
            ///grid_position.x = Controller.node_stack.Peek().Position.x;
            ///grid_position.y = Controller.node_stack.Peek().Position.y;

            //*! Update the grid position - on the front node in the queue
            ///grid_position.x = Controller.Current_node.Position.x;
            ///grid_position.y = Controller.Current_node.Position.y;
            #endregion

            //*! Update the grid position - on the front node in the queue
            grid_position.x = button_state.node.Position.x;
            grid_position.y = button_state.node.Position.y;


            //*! Player is not grounded
            is_grounded = false;
            //*! They are now moving
            is_moving = true;
        }
        else
        {
            //*! Player is grounded
            is_grounded = true;
            //*! They are now moving
            is_moving = false;
        }
    }

#endregion

    //*!----------------------------!*//
    //*!    Protected Access
    //*!----------------------------!*//
    #region Protected Functions
    #endregion //*! Protected Access




#endregion //*! Custom Functions

}