//*!----------------------------!*//
//*! Programmer: Alex Scicluna  
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;
using System.Collections.Generic;

//*! Initial Namespace
namespace APS
{
 
    /// <summary>
    /// Base Player Controller Class
    /// </summary>
    public abstract class Abstract_Player : MonoBehaviour
    {
        //*!----------------------------!*//
        //*!    Private Variables
        //*!----------------------------!*//
        #region Private Variables

        
        
        #endregion


        //*!----------------------------!*//
        //*!    Public Variables
        //*!----------------------------!*//
        #region Public Variables


        #endregion

        //*!----------------------------!*//
        //*!    Protected Variables
        //*!----------------------------!*//
        #region Protected Variables

        //*! Player Type
        protected enum Player_Type
        {
            BLOCK,
            LINE
        }

        protected Player_Type Type;

        protected Temp_Node_Map Player_Grid;
        
        #endregion


        //*!----------------------------!*//
        //*!    Unity Functions
        //*!----------------------------!*//
        #region Unity Functions
        private void Start()
        {

        }

        private void Update()
        {

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

 

        #endregion



        //*! Protected Access
        #region Protected Functions

 

        /// <summary>
        /// This checks if the block is grounded and returns the current node or the node below it
        /// </summary>
        /// <param name="grid_position"> Current grid position of the player </param>
        /// <returns></returns>
        protected Directional_Node Block_Ground_Check(Vector2 grid_position, ref Directional_Node button_state)
        {
             //*! Can the player go down from the current position
            if (Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_DN == true)
            {
                button_state.node = Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].DN_NODE;
                //*! Return the node below the current
                return button_state;
            }
            else
            {
                //*! Reset the button state
                button_state.up_key_pressed = false;
                button_state.down_key_pressed = false;
                button_state.left_key_pressed = false;
                button_state.right_key_pressed = false;

                //*! Clear the node
                button_state.node = null;
                //*! Return the current node - or null?
                return button_state;
            }

        }


        /// <summary>
        /// Based on the players controller input keys and the current grid position of the 
        /// Block Graph depends what node is returned as the target
        /// </summary>
        /// <param name="controller"> Player Controller Input Key Info of the current Player </param>
        /// <param name="grid_position"> Grid position used to step into the 2D Array of the graph </param>
        /// <returns> Returns the target node based on the direction of travel after stepping into the 2D node graph. </returns>
        protected Directional_Node Block_Input(Controller controller, Vector2 grid_position, ref Directional_Node button_state)
        {
 

            //*! Up key was pressed and it can move up
            if (Input.GetKeyDown(controller.Up_Key) && /*button_state.up_key_pressed == false &&*/ Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_UP == true)
            {
                button_state.node = Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].UP_NODE;
                button_state.up_key_pressed = true;

                return button_state;
            }
            else if (Input.GetKeyDown(controller.Down_Key) && /*button_state.down_key_pressed == false &&*/ Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_DN == true)
            {
                button_state.node = Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].DN_NODE;
                button_state.down_key_pressed = true;

                return button_state;
            }
            else if (Input.GetKeyDown(controller.Left_Key) && /*button_state.left_key_pressed == false &&*/ Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_LFT == true)
            {
                //*! Left was pressed, but up was also pressed set the left to true
                //if (button_state.up_key_pressed == true)
                //{
                //    button_state.right_key_pressed = true;
                //}
                 button_state.left_key_pressed = true;
                //*! else, keep remaining false unless the above check sets it to true

                button_state.node = Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].LFT_NODE;

                return button_state;
            }
            else if (Input.GetKeyDown(controller.Right_Key) && /*button_state.right_key_pressed == false &&*/ Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_RGT == true)
            {
                //*! Right was pressed, but up was also pressed set the right to true
                //if (button_state.up_key_pressed == true)
                //{
                //    button_state.left_key_pressed = true;
                //}
                    button_state.right_key_pressed = true;
                //*! else, keep remaining false unless the above check sets it to true
                button_state.node = Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].RGT_NODE;
                           
                return button_state;
            }
            //*! Nothing was pressed, or they can not go in the direction they want
            else
            {
                button_state.node = null;
                //*! Return the current node - or null?
                return /*Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y]*/button_state;
            }

        }
 

        /// <summary>
        /// Based on the players controller input keys and the current grid position of the 
        /// Line Graph depends what node is returned as the target
        /// </summary>
        /// <param name="controller"> Player Controller Input Key Info of the current Player </param>
        /// <param name="grid_position"> Grid position used to step into the 2D Array of the graph </param>
        /// <returns> Returns the target node based on the direction of travel after stepping into the 2D node graph. </returns>
        //protected Temp_Node_Map.Node Line_Input(Controller controller, Vector2 grid_position)
        //{

        //    //*! Up key was pressed and it can move up
        //    if (Input.GetKeyDown(controller.Up_Key) && Player_Grid.LI_Nodes[(int)grid_position.x, (int)grid_position.y].Can_UP == true)
        //    {
        //        //*! Return the target node - Vertically above the current node
        //        return Player_Grid.LI_Nodes[(int)grid_position.x, (int)grid_position.y + 1];
        //    }
        //    else if (Input.GetKeyDown(controller.Down_Key) && Player_Grid.LI_Nodes[(int)grid_position.x, (int)grid_position.y].Can_DN == true)
        //    {
        //        //*! Return the target node - Vertically below the current node
        //        return Player_Grid.LI_Nodes[(int)grid_position.x, (int)grid_position.y - 1];
        //    }
        //    else if (Input.GetKeyDown(controller.Left_Key) && Player_Grid.LI_Nodes[(int)grid_position.x, (int)grid_position.y].Can_LFT == true)
        //    {
        //        //*! Return the target node - Horizontally to the Left of the current node
        //        return Player_Grid.LI_Nodes[(int)grid_position.x - 1, (int)grid_position.y];
        //    }
        //    else if (Input.GetKeyDown(controller.Right_Key) && Player_Grid.LI_Nodes[(int)grid_position.x, (int)grid_position.y].Can_RGT == true)
        //    {
        //        //*! Return the target node - Horizontally to the Right of the current node
        //        return Player_Grid.LI_Nodes[(int)grid_position.x + 1, (int)grid_position.y];
        //    }
        //    //*! Nothing was pressed, pass back the current node
        //    else
        //    {
        //        //*! Return the current node
        //        return /*Player_Grid.LI_Nodes[(int)grid_position.x, (int)grid_position.y]*/null;
        //    }
        //}

        #endregion


    }

    [System.Serializable]
    public class Controller
    {
        //*! Input codes
        [SerializeField] private KeyCode move_up_key;
        [SerializeField] private KeyCode move_down_key;
        [SerializeField] private KeyCode move_left_key;
        [SerializeField] private KeyCode move_right_key;

        ////*! Current Input
        //public Temp_Node_Map.Node Previous_node = new Temp_Node_Map.Node();
        //*! Current Input
        public Temp_Node_Map.Node Current_node = new Temp_Node_Map.Node();
        ////*! Next Input
        //public Temp_Node_Map.Node Next_node = new Temp_Node_Map.Node();
        ////*! Queued Input
        //public Temp_Node_Map.Node Queued_node = new Temp_Node_Map.Node();

        //*! Actual queue of nodes 
        //public Queue<Temp_Node_Map.Node> node_queue = new Queue<Temp_Node_Map.Node>();


        public Stack<Temp_Node_Map.Node> node_stack = new Stack<Temp_Node_Map.Node>();


        //*! Property Accessor(s)
        public KeyCode Up_Key
        {
            get { return move_up_key; }
            set { move_up_key = value; }
        }
        public KeyCode Down_Key
        {
            get { return move_down_key; }
            set { move_down_key = value; }
        }
        public KeyCode Left_Key
        {
            get { return move_left_key; }
            set { move_left_key = value; }
        }
        public KeyCode Right_Key
        {
            get { return move_right_key; }
            set { move_right_key = value; }
        }
    }

 
    public struct Directional_Node
    {
        //*! Node in direction of travel
        public Temp_Node_Map.Node node;

        //*! Bool Checks for what key was pressed
        public bool up_key_pressed;
        public bool down_key_pressed;
        public bool left_key_pressed;
        public bool right_key_pressed;
    }







}