//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//

//*! Using namespaces
using System.Collections;
using UnityEngine;

public class PlayerStateMachine_Line : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    [HideInInspector]
    public bool show_debugger = false;

    [ContextMenu("Debugger")]
    private void Show_Debugger()
    {
        show_debugger = !show_debugger;
    }

    #region Input codes
    //[SerializeField]
    //private KeyCode move_up_key;
    //[SerializeField]
    //private KeyCode move_down_key;
    //[SerializeField]
    //private KeyCode move_left_key;
    //[SerializeField]
    //private KeyCode move_right_key;


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
    #endregion

    #region Boolean State Controls
    private bool is_moving;
    private bool can_second;
    private bool head_at_tail;
    private bool head_traversing_body;
    #endregion


    //*! Graph Container
    private Game_Manager Node_Graph;


    //*! Current Head Position
    private Vector2 grid_position;
    //*! Line Renderer component - set in awake
    private LineRenderer line_renderer;

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    public enum Player_Type
    {
        BLUE = 0,
        RED = 1
    }
    public Player_Type player_type;


    [Tooltip("Speed that the player moves at.")]
    [Range(1, 6)]
    public int movement_speed = 1;



    [System.Serializable]
    public struct Line_Point
    {
        public Vector3 target;
        public GameObject segment;
    }

    public Line_Point[] Line_Points;

    //*! Previous Input
    public Node Pivot_Node = null;
    //*! Current Input
    public Node Current_Node = null;
    //*! Next Input
    public Node Next_Node = null;
    //*! Queued Input
    public Node Queued_Node = null;


    //*! Property Accessor(s)
    public KeyCode Up_Key
    { get { return (player_type == Player_Type.BLUE) ? KeyCode.UpArrow : KeyCode.W; } private set { } }

    public KeyCode Down_Key
    { get { return (player_type == Player_Type.BLUE) ? KeyCode.DownArrow : KeyCode.S; } private set { } }

    public KeyCode Left_Key
    { get { return (player_type == Player_Type.BLUE) ? KeyCode.LeftArrow : KeyCode.A; } private set { } }

    public KeyCode Right_Key
    { get { return (player_type == Player_Type.BLUE) ? KeyCode.RightArrow : KeyCode.D; } private set { } }

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions

    private void Awake()
    {
        Node_Graph = FindObjectOfType<Game_Manager>();
        //*! Line Render component
        line_renderer = GetComponent<LineRenderer>();

        //*! Set the size of the array
        Line_Points = new Line_Point[transform.childCount];

        //*! Fill the array with the children
        for (int index = 0; index < transform.childCount; index++)
        {
            //*! Points, alligned to the grid corners
            Line_Points[index].segment = transform.GetChild(index).gameObject;
            //*! Target Positions for the above points
            Line_Points[index].target = transform.GetChild(index).position;
        }


        //*! Set the count of positions to use in the line renderer based on the length of the array
        line_renderer.positionCount = Line_Points.Length - 1;



    }


    private void Start()
    {
        //*! Assign the current grid position of the current world coodinates.
        grid_position.x = transform.position.x;
        grid_position.y = transform.position.y;

        //*! Current node is alligned to where it was placed
        Current_Node = Node_Graph.LI_Nodes[(int)grid_position.x, (int)grid_position.y];


        Current_Node.Is_Occupied = true;
        Current_Node.LFT_EDGE.Set_Traversability(false);


        //*! Default condistion, game starts with the head where it should be.
        head_at_tail = false;

        //*! Update the pivot nodes position
        if (head_at_tail == true)
        {
            Pivot_Node = Node_Graph.LI_Nodes[(int)Line_Points[Line_Points.Length - 2].segment.transform.position.x,
                                             (int)Line_Points[Line_Points.Length - 2].segment.transform.position.y];
        }
        else
        {
            Pivot_Node = Node_Graph.LI_Nodes[(int)Line_Points[3].segment.transform.position.x,
                                             (int)Line_Points[3].segment.transform.position.y];
        }
        //*! Default value at the start of the game... not updating Do not use
        ///Pivot_Node = Current_Node.RGT_NODE;
    }

    /// <summary>
    ///  Main Update loop for the state machine
    /// </summary>
    private void Update()
    {
        //Node_Graph.Temp_Update_Graph();






        Line_Movement();

        //*! Update the positions of the lines
        for (int index = 1; index < Line_Points.Length; index++)
        {
            //*! Set the line segments to equal the linepoints segment positions
            line_renderer.SetPosition(index - 1, Line_Points[index].segment.transform.position);
        }

        //*! Update all the pivot positions to where they are in the game world
        for (int index = 0; index < transform.childCount; index++)
        {
            //*! *-Expected-* Only pivots elements [2] through [Length -2]
            if (Line_Points[index].segment.name == "PIVOT")
            {
                //*! Points, alligned to the grid corners
                Line_Points[index].segment = transform.GetChild(index).gameObject;
                //*! Target Positions for the above points
                Line_Points[index].target = transform.GetChild(index).position;
            }
        }


        //*! Tail update
        //Line_Points[Line_Points.Length - 1].segment.transform.position = Line_Points[Line_Points.Length - 1].segment.transform.position;

        //Node tail_node = Node_Graph.LI_Nodes[(int)Line_Points[Line_Points.Length - 1].segment.transform.position.x, (int)Line_Points[Line_Points.Length - 1].segment.transform.position.y];
        //Node pivot_before_tail = Node_Graph.LI_Nodes[(int)Line_Points[Line_Points.Length - 2].segment.transform.position.x, (int)Line_Points[Line_Points.Length - 2].segment.transform.position.y];

        //if (tail_node.UP_NODE == pivot_before_tail)
        //{
        //    if (tail_node.DN_EDGE != null) tail_node.DN_EDGE.Set_Traversability(true);
        //    if (tail_node.LFT_EDGE != null) tail_node.LFT_EDGE.Set_Traversability(true);
        //    if (tail_node.RGT_EDGE != null) tail_node.RGT_EDGE.Set_Traversability(true);
        //}
        //else if (tail_node.DN_NODE == pivot_before_tail)
        //{
        //    if (tail_node.UP_EDGE != null) tail_node.UP_EDGE.Set_Traversability(true);
        //    if (tail_node.LFT_EDGE != null) tail_node.LFT_EDGE.Set_Traversability(true);
        //    if (tail_node.RGT_EDGE != null) tail_node.RGT_EDGE.Set_Traversability(true);
        //}
        //else if (tail_node.LFT_NODE == pivot_before_tail)
        //{
        //    if (tail_node.UP_EDGE != null) tail_node.UP_EDGE.Set_Traversability(true);
        //    if (tail_node.DN_EDGE != null) tail_node.DN_EDGE.Set_Traversability(true);
        //    if (tail_node.RGT_EDGE != null) tail_node.RGT_EDGE.Set_Traversability(true);
        //}
        //else if (tail_node.RGT_NODE == pivot_before_tail)
        //{
        //    if (tail_node.UP_EDGE != null) tail_node.UP_EDGE.Set_Traversability(true);
        //    if (tail_node.DN_EDGE != null) tail_node.DN_EDGE.Set_Traversability(true);
        //    if (tail_node.LFT_EDGE != null) tail_node.LFT_EDGE.Set_Traversability(true);
        //}
        //else
        //{
        //    if (tail_node.UP_EDGE != null) tail_node.UP_EDGE.Set_Traversability(true);
        //    if (tail_node.DN_EDGE != null) tail_node.DN_EDGE.Set_Traversability(true);
        //    if (tail_node.LFT_EDGE != null) tail_node.LFT_EDGE.Set_Traversability(true);
        //    if (tail_node.RGT_EDGE != null) tail_node.RGT_EDGE.Set_Traversability(true);
        //}

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
    /// Main Movement controller for the line player
    /// </summary>
    private void Line_Movement()
    {
        ///LOCK THE PLAYER OUT WHILE TRAVERSING THROUGH THE LINE
        //*! Check for the players input
        if (head_traversing_body == false)
        {
            Check_Input();
        }



        //*! Does Queued node have a value
        if (Queued_Node != null)
        {
            //*! Do nothing if equal 
            if (Queued_Node == Current_Node)
            {
                Queued_Node = null;
                return;
            }
            //*! Was it the Pivot node
            else if (Queued_Node == Pivot_Node)
            {

                //*! Update the pivot nodes position
                Pivot_Node = Node_Graph.LI_Nodes[(int)Line_Points[3].segment.transform.position.x,
                                                 (int)Line_Points[3].segment.transform.position.y];

                ///*! Start the coroutine for moving the head along the tail
                StartCoroutine(Move_Head_To_Tail());

                //*! Clear the node
                Queued_Node = null;


            }
            //*! Shift nodes if next is empty
            else if (Next_Node == null && Queued_Node != null)
            {
                Shift_Nodes();
            }

        }

        //*! Foward and 'backwards' motion of the update of the nodes

        //*! Move player next node is not null
        if (Next_Node != null)
        {
            //*! Iterate over all the Line Points that are not the pivot points (HEAD, HEAD_CAP, TAIL_CAP)
            for (int index = 0; index < Line_Points.Length; index++)
            {
                if (Line_Points[index].segment.name != "PIVOT")
                {
                    Line_Points[index].segment.transform.position = Vector3.MoveTowards(Line_Points[index].segment.transform.position, Line_Points[index].target, movement_speed * Time.deltaTime);
                }
            }

            //*! Get the distance from the player to the next node
            float mag_distance = (Line_Points[0].segment.transform.position - Next_Node.Position).magnitude;

            //*! If distance is less then the threshhold - allow player to override the Queued node
            if (mag_distance < 0.5f)
            {
                can_second = true;
            }


            //*! Reached the next node within a slight tollerence
            if (Line_Points[0].segment.transform.position == Next_Node.Position || mag_distance < 0.01f)
            {
                //*! Snap the head and head cap
                Line_Points[0].segment.transform.position = Line_Points[0].target;
                Line_Points[1].segment.transform.position = Line_Points[1].target;

                //*! Snap all pivot points
                for (int index = Line_Points.Length - 1; index >= 0; index--)
                {
                    if (Line_Points[index].segment.name == "PIVOT")
                    {
                        Line_Points[index].segment.transform.position = Line_Points[index - 1].segment.transform.position;
                    }

                }

                //*! Tail update
                Line_Points[Line_Points.Length - 1].segment.transform.position = Line_Points[Line_Points.Length - 1].segment.transform.position;


                Node tail_node = Node_Graph.LI_Nodes[(int)Line_Points[Line_Points.Length - 1].segment.transform.position.x, (int)Line_Points[Line_Points.Length - 1].segment.transform.position.y];
                Node pivot_before_tail = Node_Graph.LI_Nodes[(int)Line_Points[Line_Points.Length - 2].segment.transform.position.x, (int)Line_Points[Line_Points.Length - 2].segment.transform.position.y];

                if (tail_node.UP_NODE == pivot_before_tail)
                {
                    if (tail_node.DN_EDGE != null) tail_node.DN_EDGE.Set_Traversability(true);
                    if (tail_node.LFT_EDGE != null) tail_node.LFT_EDGE.Set_Traversability(true);
                    if (tail_node.RGT_EDGE != null) tail_node.RGT_EDGE.Set_Traversability(true);
                }
                else if (tail_node.DN_NODE == pivot_before_tail)
                {
                    if (tail_node.UP_EDGE != null) tail_node.UP_EDGE.Set_Traversability(true);
                    if (tail_node.LFT_EDGE != null) tail_node.LFT_EDGE.Set_Traversability(true);
                    if (tail_node.RGT_EDGE != null) tail_node.RGT_EDGE.Set_Traversability(true);
                }
                else if (tail_node.LFT_NODE == pivot_before_tail)
                {
                    if (tail_node.UP_EDGE != null) tail_node.UP_EDGE.Set_Traversability(true);
                    if (tail_node.DN_EDGE != null) tail_node.DN_EDGE.Set_Traversability(true);
                    if (tail_node.RGT_EDGE != null) tail_node.RGT_EDGE.Set_Traversability(true);
                }
                else if (tail_node.RGT_NODE == pivot_before_tail)
                {
                    if (tail_node.UP_EDGE != null) tail_node.UP_EDGE.Set_Traversability(true);
                    if (tail_node.DN_EDGE != null) tail_node.DN_EDGE.Set_Traversability(true);
                    if (tail_node.LFT_EDGE != null) tail_node.LFT_EDGE.Set_Traversability(true);
                }
                else
                {
                    if (tail_node.UP_EDGE != null) tail_node.UP_EDGE.Set_Traversability(true);
                    if (tail_node.DN_EDGE != null) tail_node.DN_EDGE.Set_Traversability(true);
                    if (tail_node.LFT_EDGE != null) tail_node.LFT_EDGE.Set_Traversability(true);
                    if (tail_node.RGT_EDGE != null) tail_node.RGT_EDGE.Set_Traversability(true);
                }


                //*! Finished moving, unless the below checks override that
                is_moving = false;

                up_key_pressed = false;
                down_key_pressed = false;
                left_key_pressed = false;
                right_key_pressed = false;

                //*! Reset the seond input permission
                can_second = false;

                //*! Collect a sticker
                if (player_type == Player_Type.BLUE)
                {
                    if (Next_Node.Gizmos_GO != null && Next_Node.Node_Type == Node_Type.Line_Red_Goal)
                    {
                        Next_Node.Node_Type = Node_Type.NONE;
                        Next_Node.Gizmos_GO.SetActive(false);
                    }
                }
                else
                {
                    if (Next_Node.Gizmos_GO != null && Next_Node.Node_Type == Node_Type.Line_Red_Goal)
                    {
                        Next_Node.Node_Type = Node_Type.NONE;
                        Next_Node.Gizmos_GO.SetActive(false);
                    }
                }



                //*! Shift the next node into the current node
                Current_Node = Next_Node;

                //*! Clear the next node
                Next_Node = null;

                //*! Update the grid position
                grid_position.x = Current_Node.Position.x;
                grid_position.y = Current_Node.Position.y;



                //*! Does Queued node have a value - was a queued input
                if (Queued_Node != null)
                {
                    //*! Shift nodes if next is empty
                    if (Next_Node == null && Queued_Node != null)
                    {
                        Shift_Nodes();
                    }
                }
            }
            ///*! End of reached location
        }
        ///*! End of Next node not null
    }
    ///*! End of just move


    /// <summary>
    /// Lock out the player controller for the line player, until the head is at the tail.
    /// </summary>
    IEnumerator Move_Head_To_Tail()
    {
        //*! Initialise the current target to 3 - First key pivot when the head is at the original head position
        int current_target = 3;

        //*! Set the heads starting target to be of the pivot 
        Line_Points[0].target = Pivot_Node.Position;//Line_Points[current_target].segment.transform.position;

        //*! Head traversing body starting condition as it is now correct
        head_traversing_body = true;

        //*! Coroutine Loop - Keep looping until it results to false
        while (head_traversing_body == true)
        {
            //*! Move the head towards its target
            Line_Points[0].segment.transform.position = Vector3.MoveTowards(Line_Points[0].segment.transform.position, Line_Points[0].target, movement_speed * Time.deltaTime);

            //*! Distance calculation
            float mag_distance = (Line_Points[0].target - Line_Points[0].segment.transform.position).magnitude;

            //*! Reached the target position
            if (Line_Points[0].segment.transform.position == Line_Points[0].target || mag_distance < 0.01f)
            {
                //*! Snap the head to its target
                Line_Points[0].segment.transform.position = Line_Points[0].target;

                //*! While it is not at the end of the array
                //if (Line_Points[0].segment.transform.position != Line_Points[Line_Points.Length - 1].segment.transform.position || mag_distance < 0.01f)
                //{
                //*! Increment the current target to index into the line points[]
                if (current_target < Line_Points.Length - 1)
                {

                    current_target++;
                    //*! Assign the new target based on the next segments position not target
                    Line_Points[0].target = Line_Points[current_target].segment.transform.position;
                }
                else
                {
                    //*! At the tail position
                    head_traversing_body = false;

                    //*! Finished moving
                    is_moving = false;


                    up_key_pressed = false;
                    down_key_pressed = false;
                    left_key_pressed = false;
                    right_key_pressed = false;

                    //    //*! Never should happen, but just in case haha.
                    //    Debug.LogError("NOPE! *-\\_(>.<)_//-* : " + current_target);
                    //    //*! Decreament it?
                    //    current_target--;
                }
                //}

            }

            //*! Keep returning null until the head is at the tail positon
            yield return null;
        }

        //*! Current node to move from is now at the tails position converted into node
        Current_Node = Node_Graph.LI_Nodes[(int)Line_Points[Line_Points.Length - 1].segment.transform.position.x,
                                           (int)Line_Points[Line_Points.Length - 1].segment.transform.position.y];

        //*! Update the grid position
        grid_position.x = Current_Node.Position.x;
        grid_position.y = Current_Node.Position.y;

        //*! Swap the line points around in reverse order
        Line_Point_Swap();
    }


    /// <summary>
    /// Lock out the player controller for the line player, until the head is at the orignal head position.
    /// </summary>
    /// <returns></returns>
    //IEnumerator Move_Head_From_Tail()
    //{
    //    //*! Initialise the current target to Line_Points.Length - 2 - First key pivot when the head is at the original head position
    //    int current_target = Line_Points.Length - 1;

    //    //*! Set the heads starting target to be of the pivot 
    //    Line_Points[0].target = Pivot_Node.Position;//Line_Points[current_target].segment.transform.position;

    //    //*! Head traversing body starting condition as it is now correct
    //    head_traversing_body = true;

    //    //*! Coroutine Loop - Keep looping until it results to false
    //    while (head_traversing_body == true)
    //    {

    //        //*! Move the head towards its target
    //        Line_Points[0].segment.transform.position = Vector3.MoveTowards(Line_Points[0].segment.transform.position, Line_Points[0].target, movement_speed * Time.deltaTime);

    //        //*! Distance calculation
    //        float mag_distance = (Line_Points[0].target - Line_Points[0].segment.transform.position).magnitude;

    //        //*! Reached the target position
    //        if (Line_Points[0].segment.transform.position == Line_Points[0].target || mag_distance < 0.01f)
    //        {
    //            //*! Snap the head to its target
    //            Line_Points[0].segment.transform.position = Line_Points[0].target;


    //            //*! While it is not at the start of the array [1] = head cap
    //            if (Line_Points[0].segment.transform.position != Line_Points[1].segment.transform.position)
    //            {
    //                //*! Decreament the current target to index into the line points[]
    //                if (current_target > 0)
    //                {
    //                    current_target--;
    //                    //*! Assign the new target
    //                    Line_Points[0].target = Line_Points[current_target].target;
    //                }
    //                //else
    //                //{
    //                //    //*! Never should happen, but just in case haha.
    //                //    Debug.LogError("NOPE! *-\\_(>.<)_//-* : " + current_target);
    //                //    //*! Increment it?
    //                //    current_target++;
    //                //}
    //            }
    //            else
    //            {
    //                //*! Used to excape the while loop
    //                //*! Above Corotine finished
    //                head_at_tail = false;
    //                //*! At the tail position
    //                head_traversing_body = false;
    //            }
    //        }

    //        //*! Keep returning null until the head is at the tail positon
    //        yield return null;
    //    }

    //    //*! Current node to move from is now at the heads position converted into node
    //    Current_Node = Node_Graph.LI_Nodes[(int)Line_Points[0].segment.transform.position.x,
    //                                       (int)Line_Points[0].segment.transform.position.y];

    //    Line_Point_Swap();

    //}


    /// <summary>
    /// Set the target positions of the HEAD, HEAD_CAP, TAIL_CAP
    /// </summary>
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

            if (Line_Points[index].segment.name == "HEAD_CAP")
            {
                Line_Points[index].target = Next_Node.Position;
            }
            else
            {
                Line_Points[index].target = Next_Node.Position;
            }

            //*! Stays where it is until the player has reached the next node
            ///if (Line_Points[index].segment.name == "PIVOT")
            ///{
            ///    Line_Points[index].target = Line_Points[index - 1].segment.transform.position;
            ///}

            if (Line_Points[index].segment.name == "TAIL_CAP")
            {
                Line_Points[index].target = Line_Points[index - 1].segment.transform.position;
            }


        }
    }


    /// <summary>
    /// Check for players input, sets the Queued node to a value of the direction choosen
    /// </summary>
    private void Check_Input()
    {
        //Debug.Log("Controller Input");
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
    /// If the next node doesnt have a value, current node's right
    ///  If the next node has a value then it hasn't being cleared being that it has not reached the next node, return the next nodes right
    /// </summary>
    /// <returns>-The node to be assigned to the queue-</returns>
    private Node Controller_Input()
    {
        //*! Temp Line point, is the point at the position of the Current nodes 'direction' or Next nodes 'direction'
        //*! Stays false if there is no point at that position then return the appropirate node
        bool t_line_point = false;

        //*! Up key was pressed
        if (Input.GetKeyDown(Up_Key) == true)
        {
            up_key_pressed = true;
            //*! Does next node already have a value, it is currently moving, but another input was made
            if (Next_Node != null)
            {
                ///*! Default Value for the current position
                Vector3Int t_next_position = Vector3Int.zero;

                if (Next_Node.Can_UP == true)
                {
                    Next_Node.UP_EDGE.Set_Traversability(false);

                    t_next_position = new Vector3Int(Mathf.RoundToInt(Next_Node.UP_NODE.Position.x), Mathf.RoundToInt(Next_Node.UP_NODE.Position.y), Mathf.RoundToInt(Next_Node.UP_NODE.Position.z));
                }
                else
                {
                    //*! The Current node can not go down from where it is,  the Current_Node.'dir' is null
                    return Current_Node;
                }

                //*! For all line points check for any potential overlap
                for (int index = 0; index < Line_Points.Length; index++)
                {

                    //*! To ensure the following comparison does not fall victim of the floating point error
                    Vector3Int t_line_point_position = new Vector3Int(Mathf.RoundToInt(Line_Points[index].segment.transform.position.x), Mathf.RoundToInt(Line_Points[index].segment.transform.position.y), Mathf.RoundToInt(Line_Points[index].segment.transform.position.z));

                    //*! Compares the next nodes position against the current line point in the array's position
                    if (t_next_position != t_line_point_position)
                    {
                        //*! No line point here
                        t_line_point = false;
                    }
                    else
                    {
                        //*! There is a line point here
                        t_line_point = true;

                        //*! If the index is at [3]
                        if (index == 3)
                        {
                            //*! The Pivot node behind the head
                            return Pivot_Node;
                        }
                        else
                        {
                            //*! No movement
                            return Current_Node;
                        }
                    }
                }

                //*! Was there a line segment point there? Return the current nodes 'direction', else next nodes 'direction'
                return (t_line_point == true) ? Current_Node : Next_Node.UP_NODE;
            }
            else
            {
                ///*! Default Value for the current position
                Vector3Int t_current_position = Vector3Int.zero;

                if (Current_Node.Can_UP == true)
                {
                    Current_Node.UP_EDGE.Set_Traversability(false);

                    t_current_position = new Vector3Int(Mathf.RoundToInt(Current_Node.UP_NODE.Position.x), Mathf.RoundToInt(Current_Node.UP_NODE.Position.y), Mathf.RoundToInt(Current_Node.UP_NODE.Position.z));
                }
                else
                {
                    //*! The Current node can not go down from where it is, the Current_Node.'dir' is null
                    return Current_Node;
                }
                //*! For all line points check for any potential overlap
                for (int index = 0; index < Line_Points.Length; index++)
                {

                    //*! To ensure the following comparison does not fall victim of the floating point error

                    Vector3Int t_line_point_position = new Vector3Int(Mathf.RoundToInt(Line_Points[index].segment.transform.position.x), Mathf.RoundToInt(Line_Points[index].segment.transform.position.y), Mathf.RoundToInt(Line_Points[index].segment.transform.position.z));

                    //*! Compares the next nodes position against the current line point in the array's position
                    if (t_current_position != t_line_point_position)
                    {
                        //*! No line point here
                        t_line_point = false;
                    }
                    else
                    {
                        //*! There is a line point here
                        t_line_point = true;


                        //*! If the index is at [3]
                        if (index == 3)
                        {
                            //*! The Pivot node behind the head
                            return Pivot_Node;
                        }
                        else
                        {
                            //*! No movement
                            return Current_Node;
                        }
                    }
                }

                //*! Was there a line segment point there? Return the current node position (no move) 'direction' else current nodes 'direction
                return (t_line_point == true) ? Current_Node : Current_Node.UP_NODE;
            }
            //*! Up Node
        }
        else if (Input.GetKeyDown(Down_Key) == true /*&& down_key_pressed == false*/)
        {
            down_key_pressed = true;
            //*! Does next node already have a value, it is currently moving, but another input was made
            if (Next_Node != null)
            {
                ///*! Default Value for the current position
                Vector3Int t_next_position = Vector3Int.zero;

                if (Next_Node.Can_DN == true)
                {
                    Next_Node.DN_EDGE.Set_Traversability(false);

                    t_next_position = new Vector3Int(Mathf.RoundToInt(Next_Node.DN_NODE.Position.x), Mathf.RoundToInt(Next_Node.DN_NODE.Position.y), Mathf.RoundToInt(Next_Node.DN_NODE.Position.z));
                }
                else
                {
                    //*! The Current node can not go down from where it is, the Current_Node.'dir' is null
                    return Current_Node;
                }

                //*! For all line points check for any potential overlap
                for (int index = 0; index < Line_Points.Length; index++)
                {

                    //*! To ensure the following comparison does not fall victim of the floating point error

                    Vector3Int t_line_point_position = new Vector3Int(Mathf.RoundToInt(Line_Points[index].segment.transform.position.x), Mathf.RoundToInt(Line_Points[index].segment.transform.position.y), Mathf.RoundToInt(Line_Points[index].segment.transform.position.z));

                    //*! Compares the next nodes position against the current line point in the array's position
                    if (t_next_position != t_line_point_position)
                    {
                        //*! No line point here
                        t_line_point = false;
                    }
                    else
                    {
                        //*! There is a line point here
                        t_line_point = true;


                        //*! If the index is at [3]
                        if (index == 3)
                        {
                            //*! The Pivot node behind the head
                            return Pivot_Node;
                        }
                        else
                        {
                            //*! No movement
                            return Current_Node;
                        }
                    }
                }

                //*! Was there a line segment point there? Return the current nodes 'direction', else next nodes 'direction'
                return (t_line_point == true) ? Current_Node : Next_Node.DN_NODE;
            }
            else
            {
                ///*! Default Value for the current position
                Vector3Int t_current_position = Vector3Int.zero;

                if (Current_Node.Can_DN == true)
                {
                    Current_Node.DN_EDGE.Set_Traversability(false);

                    t_current_position = new Vector3Int(Mathf.RoundToInt(Current_Node.DN_NODE.Position.x), Mathf.RoundToInt(Current_Node.DN_NODE.Position.y), Mathf.RoundToInt(Current_Node.DN_NODE.Position.z));
                }
                else
                {
                    //*! The Current node can not go down from where it is, the Current_Node.'dir' is null
                    return Current_Node;
                }
                //*! For all line points check for any potential overlap
                for (int index = 0; index < Line_Points.Length; index++)
                {

                    //*! To ensure the following comparison does not fall victim of the floating point error

                    Vector3Int t_line_point_position = new Vector3Int(Mathf.RoundToInt(Line_Points[index].segment.transform.position.x), Mathf.RoundToInt(Line_Points[index].segment.transform.position.y), Mathf.RoundToInt(Line_Points[index].segment.transform.position.z));

                    //*! Compares the next nodes position against the current line point in the array's position
                    if (t_current_position != t_line_point_position)
                    {
                        //*! No line point here
                        t_line_point = false;
                    }
                    else
                    {
                        //*! There is a line point here
                        t_line_point = true;


                        //*! If the index is at [3] 
                        if (index == 3)
                        {
                            //*! The Pivot node behind the head
                            return Pivot_Node;
                        }
                        else
                        {
                            //*! No movement
                            return Current_Node;
                        }
                    }
                }

                //*! Was there a line segment point there? Return the current node position (no move) 'direction' else current nodes 'direction
                return (t_line_point == true) ? Current_Node : Current_Node.DN_NODE;
            }
            //*! Down Node
        }
        else if (Input.GetKeyDown(Left_Key) == true /*&& left_key_pressed == false*/)
        {
            left_key_pressed = true;
            //*! Does next node already have a value, it is currently moving, but another input was made
            if (Next_Node != null)
            {
                ///*! Default Value for the current position
                Vector3Int t_next_position = Vector3Int.zero;

                if (Next_Node.Can_LFT == true)
                {
                    Next_Node.LFT_EDGE.Set_Traversability(false);

                    t_next_position = new Vector3Int(Mathf.RoundToInt(Next_Node.LFT_NODE.Position.x), Mathf.RoundToInt(Next_Node.LFT_NODE.Position.y), Mathf.RoundToInt(Next_Node.LFT_NODE.Position.z));
                }
                else
                {
                    //*! The Current node can not go down from where it is, the Current_Node.'dir' is null
                    return Current_Node;
                }
                //*! For all line points check for any potential overlap
                for (int index = 0; index < Line_Points.Length; index++)
                {

                    //*! To ensure the following comparison does not fall victim of the floating point error
                    Vector3Int t_line_point_position = new Vector3Int(Mathf.RoundToInt(Line_Points[index].segment.transform.position.x), Mathf.RoundToInt(Line_Points[index].segment.transform.position.y), Mathf.RoundToInt(Line_Points[index].segment.transform.position.z));

                    //*! Compares the next nodes position against the current line point in the array's position
                    if (t_next_position != t_line_point_position)
                    {
                        //*! No line point here
                        t_line_point = false;
                    }
                    else
                    {
                        //*! There is a line point here
                        t_line_point = true;


                        //*! If the index is at [3] 
                        if (index == 3)
                        {
                            //*! The Pivot node behind the head
                            return Pivot_Node;
                        }
                        else
                        {
                            //*! No movement
                            return Current_Node;
                        }
                    }
                }

                //*! Was there a line segment point there? Return the current nodes 'direction', else next nodes 'direction'
                return (t_line_point == true) ? Current_Node : Next_Node.LFT_NODE;
            }
            else
            {
                ///*! Default Value for the current position
                Vector3Int t_current_position = Vector3Int.zero;

                if (Current_Node.Can_LFT == true)
                {
                    Current_Node.LFT_EDGE.Set_Traversability(false);

                    t_current_position = new Vector3Int(Mathf.RoundToInt(Current_Node.LFT_NODE.Position.x), Mathf.RoundToInt(Current_Node.LFT_NODE.Position.y), Mathf.RoundToInt(Current_Node.LFT_NODE.Position.z));
                }
                else
                {
                    //*! The Current node can not go down from where it is, the Current_Node.'dir' is null
                    return Current_Node;
                }
                //*! For all line points check for any potential overlap
                for (int index = 0; index < Line_Points.Length; index++)
                {

                    //*! To ensure the following comparison does not fall victim of the floating point error
                    Vector3Int t_line_point_position = new Vector3Int(Mathf.RoundToInt(Line_Points[index].segment.transform.position.x), Mathf.RoundToInt(Line_Points[index].segment.transform.position.y), Mathf.RoundToInt(Line_Points[index].segment.transform.position.z));

                    //*! Compares the next nodes position against the current line point in the array's position
                    if (t_current_position != t_line_point_position)
                    {
                        //*! No line point here
                        t_line_point = false;
                    }
                    else
                    {
                        //*! There is a line point here
                        t_line_point = true;

                        //*! If the index is at [3]
                        if (index == 3)
                        {
                            //*! The Pivot node behind the head
                            return Pivot_Node;
                        }
                        else
                        {
                            //*! No movement
                            return Current_Node;
                        }
                    }
                }

                //*! Was there a line segment point there? Return the current node position (no move) 'direction' else current nodes 'direction
                return (t_line_point == true) ? Current_Node : Current_Node.LFT_NODE;
            }
            //*! Left Node
        }
        else if (Input.GetKeyDown(Right_Key) == true /*&& right_key_pressed == false*/)
        {
            right_key_pressed = true;
            //*! Does next node already have a value, it is currently moving, but another input was made
            if (Next_Node != null)
            {
                ///*! Default Value for the current position
                Vector3Int t_next_position = Vector3Int.zero;

                if (Next_Node.Can_RGT == true)
                {
                    Next_Node.RGT_EDGE.Set_Traversability(false);

                    t_next_position = new Vector3Int(Mathf.RoundToInt(Next_Node.RGT_NODE.Position.x), Mathf.RoundToInt(Next_Node.RGT_NODE.Position.y), Mathf.RoundToInt(Next_Node.RGT_NODE.Position.z));
                }
                else
                {
                    //*! The Current node can not go down from where it is, the Current_Node.'dir' is null
                    return Current_Node;
                }
                //*! For all line points check for any potential overlap
                for (int index = 0; index < Line_Points.Length; index++)
                {

                    //*! To ensure the following comparison does not fall victim of the floating point error
                    Vector3Int t_line_point_position = new Vector3Int(Mathf.RoundToInt(Line_Points[index].segment.transform.position.x), Mathf.RoundToInt(Line_Points[index].segment.transform.position.y), Mathf.RoundToInt(Line_Points[index].segment.transform.position.z));

                    //*! Compares the next nodes position against the current line point in the array's position
                    if (t_next_position != t_line_point_position)
                    {
                        //*! No line point here
                        t_line_point = false;
                    }
                    else
                    {
                        //*! There is a line point here
                        t_line_point = true;

                        //*! If the index is at [3] 
                        if (index == 3)
                        {
                            //*! The Pivot node behind the head
                            return Pivot_Node;
                        }
                        else
                        {
                            //*! No movement
                            return Current_Node;
                        }
                    }
                }

                //*! Was there a line segment point there? Return the current nodes 'direction', else next nodes 'direction'
                return (t_line_point == true) ? Current_Node : Next_Node.RGT_NODE;
            }
            else
            {
                ///*! Default Value for the current position
                Vector3Int t_current_position = Vector3Int.zero;

                if (Current_Node.Can_RGT == true)
                {
                    Current_Node.RGT_EDGE.Set_Traversability(false);

                    t_current_position = new Vector3Int(Mathf.RoundToInt(Current_Node.RGT_NODE.Position.x), Mathf.RoundToInt(Current_Node.RGT_NODE.Position.y), Mathf.RoundToInt(Current_Node.RGT_NODE.Position.z));
                }
                else
                {
                    //*! The Current node can not go down from where it is, the Current_Node.'dir' is null
                    return Current_Node;
                }
                //*! For all line points check for any potential overlap
                for (int index = 0; index < Line_Points.Length; index++)
                {

                    //*! To ensure the following comparison does not fall victim of the floating point error
                    Vector3Int t_line_point_position = new Vector3Int(Mathf.RoundToInt(Line_Points[index].segment.transform.position.x), Mathf.RoundToInt(Line_Points[index].segment.transform.position.y), Mathf.RoundToInt(Line_Points[index].segment.transform.position.z));

                    //*! Compares the next nodes position against the current line point in the array's position
                    if (t_current_position != t_line_point_position)
                    {
                        //*! No line point here
                        t_line_point = false;
                    }
                    else
                    {
                        //*! There is a line point here
                        t_line_point = true;

                        //*! If the index is at [3]
                        if (index == 3)
                        {
                            //*! The Pivot node behind the head
                            return Pivot_Node;
                        }
                        else
                        {
                            //*! No movement
                            return Current_Node;
                        }
                    }
                }

                //*! Was there a line segment point there? Return the current node position (no move) 'direction' else current nodes 'direction'
                return (t_line_point == true) ? Current_Node : Current_Node.RGT_NODE;
            }
            //*! Right Node
        }
        ///*! Nothing was pressed, return what ever value is sitting in Queued Node
        else
        {
            return Queued_Node;
        }

    }


    /// <summary>
    /// Swap the data in all the line points
    /// </summary>
    private void Line_Point_Swap()
    {
        int current_end = Line_Points.Length - 1;

        Vector3 x;
        x = Line_Points[1].segment.transform.position;
        Line_Points[1].segment.transform.position = Line_Points[Line_Points.Length - 1].segment.transform.position;
        Line_Points[2].segment.transform.position = Line_Points[Line_Points.Length - 1].segment.transform.position;
        Line_Points[Line_Points.Length - 1].segment.transform.position = x;

        current_end--;

        for (int index = 3; index < Line_Points.Length - 1; index++)
        {
            //*! Reset the target position
            //Line_Points[index].target = Vector3.zero;

            //*! Move the head cap with the first pivot

            //*! Keep swaping the values while they have not crossed
            if (current_end > index)
            {
                //*! Create a container for the temp line point
                Vector3 t_lp;
                //*! Hold onto the current Line_Point
                t_lp = Line_Points[index].segment.transform.position;
                //*! Override it with the data from the current_end index
                Line_Points[index].segment.transform.position = Line_Points[current_end].segment.transform.position;
                //*! The Current_End index is now equal to the current Line_Point
                Line_Points[current_end].segment.transform.position = t_lp;
                //*! Decrement the current end with each pass
                current_end--;
            }
        }



    }



    #endregion //*! End of Private functions


    //*! Protected Access
    #region Protected Functions

    #endregion

}
