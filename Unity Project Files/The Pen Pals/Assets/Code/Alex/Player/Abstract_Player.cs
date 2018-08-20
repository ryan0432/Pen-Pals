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


        protected void Block_Input(Controller controller)
        {
            if (Input.GetKeyDown(controller.Up_Key))
            {

            }
        }


        protected void Line_Input(Controller controller)
        {

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