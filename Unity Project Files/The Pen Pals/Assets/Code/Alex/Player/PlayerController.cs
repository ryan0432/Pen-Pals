﻿//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//


//*! Using namespaces
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
        //*! Handball
        Player_Grid = Node_Graph;

        //*! Assign the current grid position of the current world coodinates.
        grid_position.x = transform.position.x;
        grid_position.y = transform.position.y;

        //*! Set the queued node
        Temp_Node_Map.Node t_node = Node_Graph.BL_Nodes[(int)grid_position.x, (int)grid_position.y];

        if (t_node != null)
        {
            Controller.node_queue.Enqueue(t_node);
            Debug.Log("Node has been added to the node queue - via the start method, to initialise the position on the grid");
        }


        ///Controller.node_queue.Dequeue();
        //*! Ground Check on the current grid position
        Ground_Check(grid_position);
    }


 
 

    private void Update()
    {        
        if (Check_Input() == true)
        {
            is_moving = true;
        }

        if (is_moving == true)
        {
            Apply_New_Position();
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
        Temp_Node_Map.Node t_node = Block_Input(Controller, grid_position);

        if (t_node != null)
        {
            //*! Default return of Block Input is the current nodes position
            Controller.node_queue.Enqueue(t_node);

            Debug.Log("Node has been added to the node queue");
            
            //*! Update the grid position - on the front node in the queue
            grid_position.x = Controller.node_queue.Peek().Position.x;
            grid_position.y = Controller.node_queue.Peek().Position.y;
        }

        //*! If the result is null they have not moved.
        if (Controller.node_queue.Count != 0)
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
        if (transform.position == new Vector3(Controller.node_queue.Peek().Position.x - 0.5f, Controller.node_queue.Peek().Position.y - 0.5f, 0) || mag_distance < 0.05f)
        {
            //*! Update the grid position - on the front node in the queue
            grid_position.x = Controller.node_queue.Peek().Position.x;
            grid_position.y = Controller.node_queue.Peek().Position.y;

            //*! Pop the front node off the queue
            Controller.node_queue.Dequeue();

            if (Controller.node_queue.Count == 0)
            {
                //*! Finished moving
                is_moving = false;
            }

            Controller.node_queue[X].Position;

        }

    }



    /// <summary>
    /// Move player towards the next node
    /// </summary>
    /// <param name="end_node"> Target position to move the player towards </param>
    private void Apply_New_Position()
    {
 

        //*! IF the count of nodes is more than 0, there is something there
        if (Controller.node_queue.Count != 0)
        {
            //*! Move towards the front node in the node queue
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(Controller.node_queue.Peek().Position.x - 0.5f, Controller.node_queue.Peek().Position.y - 0.5f, 0), 4 * Time.deltaTime);

            //*! Get the distance between the two points
            float mag_distance = (transform.position - new Vector3(Controller.node_queue.Peek().Position.x - 0.5f, Controller.node_queue.Peek().Position.y - 0.5f, 0)).magnitude;

            //*! Have I reached the next node
            Reached_Next_Node(mag_distance);
        }
        else
        {
           //*! Snap the player to the next nodes position minus the offset
           ///transform.position = new Vector3(Controller.Current_node.Position.x - 0.5f, Controller.Current_node.Position.y - 0.5f, 0);
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
        Temp_Node_Map.Node t_node = Block_Ground_Check(grid_position);

        


        if (t_node != null && !is_moving)
        {
            //*! Add a node if it can fall
            Controller.node_queue.Enqueue(t_node);
            Debug.Log("Node has been added to the node queue - via the ground check");

            //*! Update the grid position - on the front node in the queue
            grid_position.x = Controller.node_queue.Peek().Position.x;
            grid_position.y = Controller.node_queue.Peek().Position.y;

            //*! Player is not grounded
            is_grounded = false;
            //*! They are now moving
            is_moving = true;
        }
        else
        {
            //*! Player is grounded
            is_grounded = true; 
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