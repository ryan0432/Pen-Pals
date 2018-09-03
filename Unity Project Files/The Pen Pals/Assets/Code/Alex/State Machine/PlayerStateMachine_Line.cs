//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//

//*! Using namespaces
using UnityEngine;
using System.Collections.Generic;


public class PlayerStateMachine_Line : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    //*! Input codes
    [SerializeField] private KeyCode move_up_key;
    [SerializeField] private KeyCode move_down_key;
    [SerializeField] private KeyCode move_left_key;
    [SerializeField] private KeyCode move_right_key;


    /// <summary>
    /// As these are worded with past tense. In the action of doing so you set it to true, otherwise you have not done it, false
    /// </summary>
    private bool up_key_pressed;
    /// <summary>
    /// As these are worded with past tense. In the action of doing so you set it to true, otherwise you have not done it, false
    /// </summary>
    private bool down_key_pressed;
    /// <summary>
    /// As these are worded with past tense. In the action of doing so you set it to true, otherwise you have not done it, false
    /// </summary>
    private bool left_key_pressed;
    /// <summary>
    /// As these are worded with past tense. In the action of doing so you set it to true, otherwise you have not done it, false
    /// </summary>
    private bool right_key_pressed;


    private bool is_moving;
    private bool can_second;
    private bool head_at_tail;

    //*! Current Head Position
    private Vector2 grid_position;


    private LineRenderer line_renderer;

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables
    //*! Graph Container
    public Temp_Node_Map Node_Graph;


    [System.Serializable]
    public struct Line_Point
    {
        public Vector3 target;
        public GameObject segment;
    }

    public Line_Point[] Line_Points;


    //*! Previous Input
    public Temp_Node_Map.Node Previous_Node = null;
    //*! Current Input
    public Temp_Node_Map.Node Current_Node = null;
    //*! Next Input
    public Temp_Node_Map.Node Next_Node = null;
    //*! Queued Input
    public Temp_Node_Map.Node Queued_Node = null;



    //*! Property Accessor(s)
    public KeyCode Up_Key
    { get { return move_up_key; } private set { } }

    public KeyCode Down_Key
    { get { return move_down_key; } private set { } }

    public KeyCode Left_Key
    { get { return move_left_key; } private set { } }

    public KeyCode Right_Key
    { get { return move_right_key; } private set { } }

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        line_renderer = GetComponent<LineRenderer>();



        //*! Set the size of the array
        List<Line_Point> point_list = new List<Line_Point>();
        //Line_Points = new Line_Point[transform.childCount];

        //*! Fill the array with the children
        for (int index = 0; index < transform.childCount; index++)
        {
            //if (transform.GetChild(index).gameObject.name == "PIVOT")
            //{
                Line_Point temp;
                temp.segment = transform.GetChild(index).gameObject;
                temp.target = transform.GetChild(index).position;
                point_list.Add(temp);
            //}
        }
        Line_Points = point_list.ToArray();

        line_renderer.positionCount = Line_Points.Length;

        for (int index = 0; index < Line_Points.Length; index++)
        {
            line_renderer.SetPosition(index, Line_Points[index].segment.transform.position);
        }

       

        head_at_tail = false;


        //*! Assign the current grid position of the current world coodinates.
        grid_position.x = transform.position.x;
        grid_position.y = transform.position.y;

        //*! Current node is alligned to where it was placed
        Current_Node = Node_Graph.LI_Nodes[(int)grid_position.x, (int)grid_position.y];
 

    }

         

        

    /// <summary>
    ///  Main Update loop for the state machine
    /// </summary>
    private void Update()
    {
        ///Check_Player_State();

        /////Remove soon
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Queued_Node = Node_Graph.BL_Nodes[(int)transform.position.x, (int)transform.position.y];
        //}

        Just_Move_Input();

        //*! Update the positions of the lines
        for (int index = 0; index < Line_Points.Length; index++)
        {
            line_renderer.SetPosition(index, Line_Points[index].segment.transform.position);
        }
    }

    #endregion




    void Just_Move_Input()
    {
        //*! Check for the players input
        Check_Input();

        //*! Does Queued node have a value
        if (Queued_Node != null)
        {
            //*! Shift nodes if next is empty
            if (Next_Node == null && Queued_Node != null)
            {
                Shift_Nodes();
            }
      
        }
 


        //*! Move player next node is not null
        if (Next_Node != null)
        {

            #region head to tail
            //if (Line_Points[2].segment.transform.position == Next_Node.Position)
            //{
            //    Debug.Log("Moved back on it self");
            //    //*! Assign the next node the one before it in the array until at the tail - when at tail return null
            //    //Next_Node = Move_Head_To_Tail();
            //
            //
            //    //if (Next_Node != null)
            //    //{
            //    //    //*! Move towards with precision to have the player exactly reach the next node
            //    //    Line_Segments[0].transform.position = Vector3.MoveTowards(transform.position, Next_Node.Position, 4 * Time.deltaTime);
            //    //}
            //
            //    ////*! Reached the next node
            //    //if (Line_Segments[0].transform.position == Next_Node.Position)
            //    //{
            //    //    //*! Assign the next node the one before it in the array until at the tail - when at tail return null
            //    //    ///Next_Node = Move_Head_To_Tail();
            //    //
            //    //    return;
            //    //}
            //
            //}
            #endregion



            //*! Iterate over all the Line Points
            for (int index = 0; index < Line_Points.Length; index++)
            {
                if (Line_Points[index].segment.name != "PIVOT")
                {
                    Line_Points[index].segment.transform.position = Vector3.MoveTowards(Line_Points[index].segment.transform.position, Line_Points[index].target, 4 * Time.deltaTime);
                }
            }
 

            //*! Get the distance from the player to the next node
            float mag_distance = (Line_Points[0].segment.transform.position - Next_Node.Position).magnitude;
           
            //*! If distance is less then the threshhold - allow player to override the Queued node
            if (mag_distance < 0.75f)
            {
                can_second = true;
            }


            //*! Reached the next node
            if (Line_Points[0].segment.transform.position == Next_Node.Position || mag_distance < 0.01f)
            {
                Line_Points[0].segment.transform.position = Line_Points[0].target;
                Line_Points[1].segment.transform.position = Line_Points[1].target;
                

                //*! Snap all
                for (int index = Line_Points.Length -1; index >= 0; index--)
                {

                    if (Line_Points[index].segment.name == "PIVOT")
                    {
                        Line_Points[index].segment.transform.position = Line_Points[index -1].segment.transform.position;
                    }

                }
                //*! Tail update
                Line_Points[Line_Points.Length - 1].segment.transform.position = Line_Points[Line_Points.Length - 1].segment.transform.position;


                //*! Finished moving, unless the below checks override that
                is_moving = false;

                //*! Reset the seond input permission
                can_second = false;

                //*! Shift the next node into the current node
                Current_Node = Next_Node;

                //*! Clear the next node
                Next_Node = null;
               
                //*! Update the grid position
                grid_position.x = Current_Node.Position.x;
                grid_position.y = Current_Node.Position.y;



                //*! Does Queued node have a value
                if (Queued_Node != null)
                {
                    ///Debug.Log("Q: Not null" + Queued_Node.Position);
                    //*! Shift nodes if next is empty
                    if (Next_Node == null && Queued_Node != null)
                    {
                        Shift_Nodes();
                    }
                }
 

            }
        }
    }


    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions

 
 

    #endregion
    

    //*! Private Access
    #region Private Functions

    /// <summary>
    /// Lock controlls until head is at tail
    /// </summary>
    private Temp_Node_Map.Node Move_Head_To_Tail()
    {
     

        return null;
    }
 
    private void Shift_Nodes()
    {
        //*! Shift Queued into the next node
        Next_Node = Queued_Node;

        //*! Clear the Queued node
        Queued_Node = null;

        //*! Set the target positions
        for (int index = 0; index < Line_Points.Length; index++)
        {
            //*! Heads target position is equal to the next node
            if (Line_Points[index].segment.name == "HEAD")
            {
                Line_Points[index].target = Next_Node.Position;
            }

            if (Line_Points[index].segment.name == "HEAD_CAP" && head_at_tail == false)
            {
                Line_Points[index].target = Next_Node.Position;
            }

            //*! Stays where it is until the player has reached the next node
            //if (Line_Points[index].segment.name == "PIVOT")
            //{
            //    Line_Points[index].target = Line_Points[index - 1].segment.transform.position;
            //}

            if (Line_Points[index].segment.name == "TAIL_CAP")
            {
                Line_Points[index].target = Line_Points[index - 1].segment.transform.position;
            }

        }
    }

    private void Check_Input()
    {
        //*! Only when the player is not moving
        if (is_moving == false)
        {
            //*! Player Input checks - based on Current Node position.
            Queued_Node = Controller_Input();

            //*! Does it have a value, did the player input something?
            if (Queued_Node != null)
            {
                is_moving = true;
            }
        }


        //*! Is moving and can enter for queued input
        if (is_moving == true && can_second == true)
        {
            //*! Player Input checks - based on Current Node position.
            Queued_Node = Controller_Input();

            //*! Does it have a value, did the player input something?
            if (Queued_Node != null)
            {
                //*! Reset the second input flag
                can_second = false;
            }
        }
        else if (is_moving == true && can_second == false)
        {
            //*! Player Input checks - based on Current Node position.
            Queued_Node = Controller_Input();
        }
    }


    /// <summary>
    /// Checks the current key pressed and sets the appropirate flag
    /// </summary>
    /// <returns>-The node to be assigned to the queue-</returns>
    private Temp_Node_Map.Node Controller_Input()
    {
        //*! Up key was pressed
        if (Input.GetKeyDown(Up_Key) == true /*&& up_key_pressed == false*/)
        {
            //*! Return the node UP to the current grid position BUT if the next node is not null assign the node adacent to next
            return (Next_Node != null) ? Next_Node.UP_NODE : Current_Node.UP_NODE;     //*! Up Node
        }
        else if (Input.GetKeyDown(Down_Key) == true /*&& down_key_pressed == false*/)
        {
             //*! Return the node DOWN to the current grid position
            return (Next_Node != null) ? Next_Node.DN_NODE : Current_Node.DN_NODE;     //*! Down Node
        }
        else if (Input.GetKeyDown(Left_Key) == true /*&& left_key_pressed == false*/)
        {

            //*! Return the node LEFT to the current grid position
            //*! If the next node doesnt have a value, current node's left
            //*! If the next node has a value then it hasn't being cleared being that it has not reached the next node, return the next nodes left
            return (Next_Node != null) ? Next_Node.LFT_NODE : Current_Node.LFT_NODE;    //*! Left Node
        }
        else if (Input.GetKeyDown(Right_Key) == true /*&& right_key_pressed == false*/)
        {
            //*! Return the node RIGHT to the current grid position
            //*! If the next node doesnt have a value, current node's right
            //*! If the next node has a value then it hasn't being cleared being that it has not reached the next node, return the next nodes right
            return (Next_Node != null) ? Next_Node.RGT_NODE : Current_Node.RGT_NODE;    //*! Right Node
        }
        else
        {
            return Queued_Node;   
        }
        
    }

    #endregion //*! End of Private functions


    //*! Protected Access
    #region Protected Functions

    #endregion
    
}
 