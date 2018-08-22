//*!----------------------------!*//
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
    private bool is_moving;
    private bool can_second;
    private bool is_grounded = false;

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
        
        //*! Ground Check on the current grid position
        if (type == Player_Type.BLOCK)
        {
            Controller.Current_node = Node_Graph.BL_Nodes[(int)grid_position.x, (int)grid_position.y];
            Ground_Check(grid_position);
        }
        else
        {
            Controller.Current_node = Node_Graph.LI_Nodes[(int)grid_position.x, (int)grid_position.y];
        }
    }


    /*-
     * 
     * Adam's notes
     * 
     * Always be proccessing next node. - update only
     * 
     * input only assigns to queued node.
     * 
     * input buffer allows fall node correct timing
     *      previous node is below - can side step
     *      else 
     *          queue node below current node         
     * 
     * -*/
 

    private void Update()
    {        
        //*! The player is moving
        if (is_moving)
        {
            //*! Move the player towards the next node
            Apply_New_Position();
        }
        else
        {
            //*! Block Player Ground Check
            if (type == Player_Type.BLOCK)
            {
                //*! Check if the Block player is grounded
                Ground_Check(grid_position);
            }
        }
        
        
        //*! Player Update, was something pressed.
        if (!is_moving && Check_Input(type) == true)
        {
            //*! Player is moving
            is_moving = true;
        }

        //*! Early Input check of input
        else if (is_moving && Check_Input(type) == true && can_second)
        {
            //*! Player is moving - continue
            is_moving = true;
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
    /// Move player towards the next node
    /// </summary>
    /// <param name="end_node"> Target position to move the player towards </param>
    private void Apply_New_Position()
    {

        /*-
         * Order of the processes
         * Always process the next node
         * 
         * Queued node is not null pass it to next node if next node is null
         * Clear the queued node
         * update the grid position of the next node
         * 
         * move towards the next node
         * 
         * when player is at the next node
         * store the previous node as the current node
         * store current node as the next node
         * clear next node
         * 
         * ...
         * Check if the previous node is 1 below, then allow side step
         * else set queued node to be 1 below, causing the player to fall
         * 
         * -*/



        //*! Queued node has data and next does not, assign queued node to next
        if (Controller.Queued_node != null && Controller.Next_node == null)
        {
            //*! Assign next the queued node
            Controller.Next_node = Controller.Queued_node;
            //*! Clear the queued node
            Controller.Queued_node = null;

            //*! Update the grid position - on next node
            grid_position.x = Controller.Next_node.Position.x;
            grid_position.y = Controller.Next_node.Position.y;
        }


        //*! Move the players position
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(Controller.Next_node.Position.x - 0.5f, Controller.Next_node.Position.y - 0.5f, 0), 4 * Time.deltaTime);

        //*! Get the distance between the two points
        float mag_distance = (transform.position - new Vector3(Controller.Next_node.Position.x - 0.5f, Controller.Next_node.Position.y - 0.5f, 0)).magnitude;

        //*! X% Distance remaining to end position
        if (mag_distance < 0.25f)
        {
            can_second = true;
        }


        //*! Reached the end node position - Adam says its risky... small distance check to allow for a slight inaccuacy 
        if (transform.position == new Vector3(Controller.Next_node.Position.x - 0.5f, Controller.Next_node.Position.y - 0.5f, 0) || mag_distance < 0.05f)
        {
            //*! Snap the player to the next nodes position minus the offset
            transform.position = new Vector3(Controller.Next_node.Position.x - 0.5f, Controller.Next_node.Position.y - 0.5f, 0);

            //*! Previous node is equal to the current
            Controller.Previous_node = Controller.Current_node;
            //*! Current node is equal to the next node
            Controller.Current_node = Controller.Next_node;

            //*! There is queued node, clear the next node if it is the same as the current node
            if (Controller.Queued_node != null)
            {
                //*! Reached the target / next node and there was queued node information
                Controller.Next_node = Controller.Queued_node;
                //*! Clear queued node
                Controller.Queued_node = null;

                //*! Block player ground check
                if (type == Player_Type.BLOCK)
                {
                    Ground_Check(grid_position);
                }
            }
            else
            {
                //*! The was no queued node informtion
                //*! Clear the next node.
                Controller.Next_node = null;
                //*! Finished moving. 

                //*! Player is not moving
                is_moving = false;
            }

            //*! Reset back to false
            can_second = false;

        }
    }
 

    /// <summary>
    /// Block player only to use.
    /// Adds a node to Queued if the current node position can down == true;
    /// </summary>
    /// <param name="grid_position"></param>
    private void Ground_Check(Vector2 grid_position)
    {
        //*! Set the Current node to be one below it    
        Controller.Queued_node = Block_Ground_Check(grid_position);

        //*! If the soon to be next node is equal to the current one. 
        //*! Same position, so that the player must be grounded
        if (Controller.Queued_node == null)
        {
            //*! Player is grounded
            is_grounded = true; 
        }
        else
        {
            //*! Player is not grounded
            is_grounded = false;
            //*! They are now moving
            is_moving = true;
        }
    }

#endregion

    //*!----------------------------!*//
    //*!    Protected Access
    //*!----------------------------!*//
    #region Protected Functions

    /// <summary>
    /// Has the player made an input
    /// </summary>
    /// <param name="player_Type"> What player is it checking for </param>
    /// <returns> If they have changed queued nodes </returns>
    protected override bool Check_Input(Player_Type player_Type)
    {
        switch (player_Type)
        {
            case Player_Type.BLOCK:
                {
                    //*! Default return of Block Input is the current nodes position
                    Controller.Queued_node = Block_Input(Controller, grid_position);
                    break;
                }
               
            case Player_Type.LINE:
                {
                    //*! Default return of Line Input is the current nodes position
                    Controller.Queued_node = Line_Input(Controller, grid_position);

                    break;
                }
        
            default:
                break;
        }
        
        //*! If the result is null they have not moved.
        if (Controller.Queued_node == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion //*! Protected Access


#endregion //*! Custom Functions

}