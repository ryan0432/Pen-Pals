//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;


public class Line_Player_Controller : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    //*! How far the player can move.
    [SerializeField]
    [Range(1,2)]
    private int movement_distance;

    //*! How fast the player to move
    [SerializeField]
    [Range(1, 5)]
    private int movement_speed;

    [SerializeField]
    private Controls controls;

    //*! Line Segments
    private Line_Renderer_Container line_segments;

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions

    private void Start()
    {
        //*! As long as the script is also attached to the same game object
        line_segments = GetComponent<Line_Renderer_Container>();

        
        //*! Line Player Position Update
        transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, -0.5f);

        //*! Zero or less set it to the minimum value of one. 
        //*! Anything else is an override
        if (movement_distance < 1)
            movement_distance = 1;

        //*! Zero or less set it to the minimum value of one. 
        //*! Anything else is an override
        if (movement_speed < 1)
            movement_speed = 1;


        //*! Check to see if anything is set to none
        if (controls.move_up_key == KeyCode.None || controls.move_down_key == KeyCode.None || controls.move_left_key == KeyCode.None || controls.move_right_key == KeyCode.None)
        {
            Debug.LogError("Input keys were not set.");
        }

    }

    private void Update()
    {
        //*! When Player inputs a matching control
        if (Check_Player_Input())
        {
            //*! Move the player by the current input key code
            Move_Player_In_Direction(controls.current_input);
        }
        else
        {
            //*! When the line can move again - clear the inputs
            if (line_segments.Can_Move)
            {
                Clear_Current_Input();
                Clear_Next_Input();
            }
        }
              

    }

    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions
   
    public void Run_Next()
    {
        Move_Player_In_Direction(controls.next_input);
    }

    public KeyCode Get_Next_Input()
    {
        return controls.next_input;
    }

    #endregion



    //*! Private Access
    #region Private Functions

    private bool Check_Player_Input()
    {
        //*! Up Key
        if (Input.GetKeyDown(controls.move_up_key))
        {
            if (controls.current_input == KeyCode.None)
            {
                controls.current_input = controls.move_up_key;
            }
            else
            {
                controls.next_input = controls.move_up_key;
            }
            //*! Successful Input
            return true;
        }
        //*! Down Key
        else if (Input.GetKeyDown(controls.move_down_key))
        {

            if (controls.current_input == KeyCode.None)
            {
                controls.current_input = controls.move_down_key;
            }
            else
            {
                controls.next_input = controls.move_down_key;
            }
            //*! Successful Input
            return true;
        }
        //*! Left Key
        else if (Input.GetKeyDown(controls.move_left_key))
        {
            if (controls.current_input == KeyCode.None)
            {
                controls.current_input = controls.move_left_key;
            }
            else
            {
                controls.next_input = controls.move_left_key;
            }
            //*! Successful Input
            return true;
        }
        //*! Right Key
        else if (Input.GetKeyDown(controls.move_right_key))
        {
            if (controls.current_input == KeyCode.None)
            {
                controls.current_input = controls.move_right_key;
            }
            else
            {
                controls.next_input = controls.move_right_key;
            }
            //*! Successful Input
            return true;
        }

        //*! Default case when noting from above hits
        return false;
    }

    private void Clear_Current_Input()
    {
        controls.current_input = KeyCode.None;
    }
    private void Clear_Next_Input()
    {
        controls.next_input = KeyCode.None;
    }

    /// <summary>
    /// Set the new target position for the player to move towards.
    /// </summary>
    /// <param name="current_key">-KeyCode to move the player by-</param>
    private void Move_Player_In_Direction(KeyCode current_key)
    {
        //*! Up Key
        if (current_key == controls.move_up_key)
        {
            //*! Only when the player can move
            if (line_segments.Can_Move)
            {
                //*! Cache the old positions
                ///Vector3 old_head_position = line_segments.Point_Position[0].position;
                Vector3 old_mid_position = line_segments.Point_Position[1].position;
                ///Vector3 old_tail_position = line_segments.Point_Position[2].position;

                line_segments.Target_Position[0] += new Vector3(0, movement_distance, 0);
                line_segments.Target_Position[1] = line_segments.Target_Position[0];
                line_segments.Target_Position[2] = old_mid_position;
            }

            //*! Clear current input
            //controls.current_input = KeyCode.None;
        }
        //*! Down Key
        else if (current_key == controls.move_down_key)
        {
            //*! Only when the player can move
            if (line_segments.Can_Move)
            {
                //*! Cache the old positions
                ///Vector3 old_head_position = line_segments.Point_Position[0].position;
                Vector3 old_mid_position = line_segments.Point_Position[1].position;
                ///Vector3 old_tail_position = line_segments.Point_Position[2].position;

                line_segments.Target_Position[0] -= new Vector3(0, movement_distance, 0);
                line_segments.Target_Position[1] = line_segments.Target_Position[0];
                line_segments.Target_Position[2] = old_mid_position;
            }

            //*! Clear current input
            //controls.current_input = KeyCode.None;
        }
        //*! Left Key
        else if (current_key == controls.move_left_key)
        {
            //*! Only when the player can move
            if (line_segments.Can_Move)
            {
                //*! Cache the old positions
                ///Vector3 old_head_position = line_segments.Point_Position[0].position;
                Vector3 old_mid_position = line_segments.Point_Position[1].position;
                ///Vector3 old_tail_position = line_segments.Point_Position[2].position;

                line_segments.Target_Position[0] -= new Vector3(movement_distance, 0, 0);
                line_segments.Target_Position[1] = line_segments.Target_Position[0];
                line_segments.Target_Position[2] = old_mid_position;
            }


            //*! Clear current input
            //controls.current_input = KeyCode.None;
        }
        //*! Right Key
        else if (current_key == controls.move_right_key)
        {
            //*! Only when the player can move
            if (line_segments.Can_Move)
            {
                //*! Cache the old positions
                ///Vector3 old_head_position = line_segments.Point_Position[0].position;
                Vector3 old_mid_position = line_segments.Point_Position[1].position;
                ///Vector3 old_tail_position = line_segments.Point_Position[2].position;

                line_segments.Target_Position[0] += new Vector3(movement_distance, 0, 0);
                line_segments.Target_Position[1] = line_segments.Target_Position[0];
                line_segments.Target_Position[2] = old_mid_position;
            }
            
            ///*! Clear current input
            ///controls.current_input = KeyCode.None;
        }
    }

    #endregion



    //*! Protected Access
    #region Protected Functions

    #endregion

}

[System.Serializable]
public class Controls
{
    //*! Input codes
    public KeyCode move_up_key;
    public KeyCode move_down_key;
    public KeyCode move_left_key;
    public KeyCode move_right_key;
    //*! Current Input
    public KeyCode current_input;
    //*! Next Input
    public KeyCode next_input;
}



