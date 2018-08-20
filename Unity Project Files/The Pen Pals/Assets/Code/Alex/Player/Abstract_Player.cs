//*!----------------------------!*//
//*! Programmer: Alex Scicluna  
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;

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

        //*! Private Controls
        private Controller controls;

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

        //*! Player Controls Access
        [SerializeField]
        protected Controller Controller
        { get { return controls; } }

        protected enum Player_Type
        {
            BLOCK,
            LINE
        }


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

        //*! Check for Input
        protected bool Check_Input(Player_Type player_Type)
        {
            return false;
        }

        #endregion


    }

    [System.Serializable]
    public class Controller
    {
        //*! Input codes
        KeyCode move_up_key;
        KeyCode move_down_key;
        KeyCode move_left_key;
        KeyCode move_right_key;
        //*! Current Input
        KeyCode current_input;
        //*! Next Input
        KeyCode next_input;

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