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


    private void Update()
    {
        //*! The player is moving
        if (is_moving)
        {
            //*! Fail safe to make sure that the next node is not null
            if (Controller.Next_node != null)
            {
                //*! Move the player towards the next node
                Apply_New_Position();
            }
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
        if (can_second && Check_Input(type) == true)
        {
            //*! Player is moving
            is_moving = true;
        }


    }

    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions


    #endregion



    //*! Private Access
    #region Private Functions
    
    /// <summary>
    /// Move player towards the end_position
    /// </summary>
    /// <param name="end_node"> Target position to move the player towards </param>
    private void Apply_New_Position()
    {
        //*! Update the grid position
        grid_position.x = Controller.Next_node.Position.x;
        grid_position.y = Controller.Next_node.Position.y;

        //transform.position = new Vector3(end_node.Position.x - 0.5f, end_node.Position.y - 0.5f, 0);

        //*! Move the players position
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(Controller.Next_node.Position.x - 0.5f, Controller.Next_node.Position.y - 0.5f, 0), 4 * Time.deltaTime);

        //*! Get the distance between the two points
        float mag_distance = (new Vector3(Controller.Next_node.Position.x - 0.5f, Controller.Next_node.Position.y - 0.5f, 0) - transform.position).magnitude;

        //*! X% Distance remaining to end position
        if (mag_distance < 0.7f)
        {
            can_second = true;
        }


        //*! Reached the end node position
        if (transform.position == new Vector3(Controller.Next_node.Position.x - 0.5f, Controller.Next_node.Position.y - 0.5f, 0))
        {

            //*! Previous node is equal to the current
            Controller.Previous_node = Controller.Current_node;
            //*! Current node is equal to the next node
            Controller.Current_node = Controller.Next_node;

            //*! Current node is the same as the next node, clear the next node
            if (Controller.Current_node == Controller.Next_node)
            {
                Controller.Next_node = null;
            }

            //*! Player is not moving
            is_moving = false;
   
            //*! Reset back to false
            can_second = false;

        }
    }


    /// <summary>
    /// Block player only to use
    /// </summary>
    /// <param name="grid_position"></param>
    private void Ground_Check(Vector2 grid_position)
    {
        //*! Set the Current node to be one below it    
        Controller.Next_node = Block_Ground_Check(grid_position);
 
        //*! They are now moving
        is_moving = true;
    }


   
    #endregion



    //*! Protected Access
    #region Protected Functions



    protected override bool Check_Input(Player_Type player_Type)
    {
        switch (player_Type)
        {
            case Player_Type.BLOCK:
                {
                    //*! Default return of Block Input is the current nodes position
                    Controller.Next_node = Block_Input(Controller, grid_position);

                    //*! The next node is the same as the current one
                    if (Controller.Current_node == Controller.Next_node)
                    {
                        return false;
                    }
                    
                    break;
                }
               
            case Player_Type.LINE:
                {
                    //*! Default return of Line Input is the current nodes position
                    Controller.Next_node = Line_Input(Controller, grid_position);

                    //*! The next node is the same as the current one
                    if (Controller.Current_node == Controller.Next_node)
                    {
                        return false;
                    }
          
                    break;
                }
                           
 
            default:
                break;

        }

        //*! If the result is the same they have not moved.
        if (Controller.Previous_node == Controller.Next_node)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion


}