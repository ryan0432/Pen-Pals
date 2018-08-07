//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;


public class Line_Renderer_Container : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    //*! Line component
    [SerializeField]
    private LineRenderer line_segment;


    /// <summary>
    /// Positions, first is always the head and the last is the tail.
    /// [0] Head, [1] Mid Point, [points.Length] tail
    /// </summary>
    //*! Array of points
    [SerializeField]
    private Transform[] points;
    
    //*! Target grid postition
    [SerializeField]
    private Vector3[] target_position;

    private bool can_move_mid_point;
    private bool line_moving;
    private bool can_move;
    private bool can_enter_second;
    
    //*! All points finished moving
    private bool finished_moving = false;

    private Line_Player_Controller line_player;

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables


    //*! Line Segment point accessor
    public Transform[] Point_Position
    { get { return points; } }

    public Vector3[] Target_Position
    {
        get { return target_position; }
        set { target_position = value; }
    }

    public bool Can_Move
    { get { return can_move; } }

    public bool Is_Moving
    { get { return line_moving; } }

    public bool Can_Enter_Second
    {
        get { return can_enter_second; }
        set { can_enter_second = value; }
    }


    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions
    private void Start()
    {
        line_player = GetComponent<Line_Player_Controller>();

        line_segment = GetComponent<LineRenderer>();
        
        target_position = new Vector3[points.Length];

        target_position[0] = points[0].position;
        target_position[1] = points[1].position;
        target_position[2] = points[2].position;

        //*! Update the line segment count to match the size of the array of points
        line_segment.positionCount = points.Length;

        can_move = false;
    }

    private void Update()
    {

        //*! If either of the points are not at the target position - set the flags to start moving
        if (!line_moving && points[0].position != Target_Position[0] || points[1].position != Target_Position[1])
        {
            line_moving = true;
            can_move = false;
        }

        if (!line_moving && points[points.Length-1].position != Target_Position[points.Length-1])
        {
            Debug.Log("::" + points[points.Length-1].position);
            ///points[points.Length - 1].position = Target_Position[points.Length -1];//snap
            line_moving = true;
            can_move = false;
        }

        //*! If the line is moving and the player can not enter input - move points towards target
        if (line_moving && !can_move)
        {
            points[0].position = Vector3.MoveTowards(points[0].position, Target_Position[0], 4 * Time.deltaTime);
            points[points.Length -1].position = Vector3.MoveTowards(points[points.Length - 1].position, Target_Position[points.Length - 1], 4 * Time.deltaTime);
        }

        //*! If either point reaches it's destination - target position allow the mid point to move
        if (points[0].position == Target_Position[0] || points[1].position == Target_Position[1])
        {
            can_move_mid_point = true;
        }        
        else
        {
            can_move_mid_point = false;
        }

        //*! When the mid point can move
        if (can_move_mid_point)
        {
            ///points[1].position = Vector3.MoveTowards(points[1].position, Target_Position[1], 4 * Time.deltaTime);

            //*! Snap!
            points[1].position = Target_Position[1];

            //*! When the mid point equals its target destination
            if (points[1].position == Target_Position[1])
            {
                can_move_mid_point = false;
                line_moving = false;
                can_move = true;
            }



            //*! All points have to be stopped
            for (int index = 0; index < points.Length -1; index++)
            {
                if (points[index].position == Target_Position[index])
                {
                    finished_moving = true;
                }
                else
                {
                    finished_moving = false;
                    break;
                }
            }

            //*! When the line is not moving
            if (!line_moving)
            {
                //*! When the next input is not nothing
                if (line_player.Get_Next_Input() != KeyCode.None && finished_moving)
                {
                    //*! RUN NEXT Input
                    line_player.Run_Next();
                }
          

            }
        }
        

        //*! Update the positions of the points
        Update_Line_Segments();
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
    /// Go through the array of points and update the line segments positions from the points transform.position.
    /// </summary>
    private void Update_Line_Segments()
    {
        //*! Iterate over all points and update the line segment points
        for (int index = 0; index < points.Length; index++)
        {
            line_segment.SetPosition(index, points[index].position);
        }
            
    }

    #endregion



    //*! Protected Access
    #region Protected Functions

    #endregion



}