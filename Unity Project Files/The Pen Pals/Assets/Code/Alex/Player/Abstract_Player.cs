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

        //*! Check for Input - Nothing to implement here
        protected virtual bool Check_Input(Player_Type player_Type)
        {
            return false;
        }

        /// <summary>
        /// Based on the players controller input keys and the current grid position of the 
        /// Block Graph depends what node is returned as the target
        /// </summary>
        /// <param name="controller"> Player Controller Input Key Info of the current Player </param>
        /// <param name="grid_position"> Grid position used to step into the 2D Array of the graph </param>
        /// <returns> Returns the target node based on the direction of travel after stepping into the 2D node graph. </returns>
        protected Temp_Node_Map.Node Block_Input(Controller controller, Vector2 grid_position)
        {
            //*! Up key was pressed and it can move up
            if (Input.GetKeyDown(controller.Up_Key) && Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_UP == true)
            {
                //*! Return the target node - Vertically above the current node
                return Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y + 1];
            }
            else if (Input.GetKeyDown(controller.Down_Key) && Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_DN == true)
            {
                //*! Return the target node - Vertically below the current node
                return Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y - 1];
            }
            else if (Input.GetKeyDown(controller.Left_Key) && Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_LFT == true)
            {
                //*! Return the target node - Horizontally to the Left of the current node
                return Player_Grid.BL_Nodes[(int)grid_position.x - 1, (int)grid_position.y];
            }
            else if (Input.GetKeyDown(controller.Right_Key) && Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_RGT == true)
            {
                //*! Return the target node - Horizontally to the Right of the current node
                return Player_Grid.BL_Nodes[(int)grid_position.x + 1, (int)grid_position.y];
            }
            //*! Nothing was pressed, pass back the current node
            else
            {
                //*! Return the target node
                return Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y];
            }

        }

        /// <summary>
        /// Based on the players controller input keys and the current grid position of the 
        /// Line Graph depends what node is returned as the target
        /// </summary>
        /// <param name="controller"> Player Controller Input Key Info of the current Player </param>
        /// <param name="grid_position"> Grid position used to step into the 2D Array of the graph </param>
        /// <returns> Returns the target node based on the direction of travel after stepping into the 2D node graph. </returns>
        protected /*Temp_Node_Map.Node*/ void Line_Input(Controller controller, Vector2 grid_position)
        {
            //*!--------------------------------------------------------!*//
            //*!            Temporary - Not yet Implemented
            //*!--------------------------------------------------------!*//

            ////*! Up key was pressed and it can move up
            //if (Input.GetKeyDown(controller.Up_Key) && Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_UP == true)
            //{
            //    //*! Return the target node - Vertically above the current node
            //    return Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y + 1];
            //}
            //else if (Input.GetKeyDown(controller.Down_Key) && Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_DN == true)
            //{
            //    //*! Return the target node - Vertically below the current node
            //    return Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y - 1];
            //}
            //else if (Input.GetKeyDown(controller.Left_Key) && Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_LFT == true)
            //{
            //    //*! Return the target node - Horizontally to the Left of the current node
            //    return Player_Grid.BL_Nodes[(int)grid_position.x - 1, (int)grid_position.y];
            //}
            //else if (Input.GetKeyDown(controller.Right_Key) && Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y].Can_RGT == true)
            //{
            //    //*! Return the target node - Horizontally to the Right of the current node
            //    return Player_Grid.BL_Nodes[(int)grid_position.x + 1, (int)grid_position.y];
            //}
            ////*! Nothing was pressed, pass back the current node
            //else
            //{
            //    //*! Return the target node
            //    return Player_Grid.BL_Nodes[(int)grid_position.x, (int)grid_position.y];
            //}
        }

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

        //*! Current Input
        [SerializeField] private KeyCode current_input;
        //*! Next Input
        [SerializeField] private KeyCode next_input;

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

}