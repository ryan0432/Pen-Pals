//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//

//*! Using namespaces
using UnityEngine;


public class Block_Controller : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Player Save Data
    //*!----------------------------!*//
    public Player_Save save_data;

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    //*! Other player with the game object tag of "Player"
    private GameObject Other_Player;

    //*! Graph Container
    private Game_Manager GameManager;


    //*! Storing the current grid position on the map
    private Vector2 grid_position;

    #region Movement Flags
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


    private bool is_grounded;
    private bool is_moving;
    private bool can_second;
    private bool is_falling;

    #endregion

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    #region Node Storage

    //*! Previous Input
    public Node Previous_Node = null;
    //*! Current Input
    public Node Current_Node = null;
    //*! Next Input
    public Node Next_Node = null;
    //*! Queued Input
    public Node Queued_Node = null;

    #endregion


    #region Player Type Controller Define

    //*! DO NOT CHANGE THE NUMBERS
    public enum Player_Type
    {
        RED = 1,
        BLUE = 2
    }
    public Player_Type player_type;


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



    [Tooltip("Speed that the player moves at.")]
    [Range(1, 6)]
    public int movement_speed = 1;
    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions

    /// <summary>
    /// Get the other Player and get a reference of the Game Map
    /// </summary>
    private void Start()
    {
        Get_Other_Player();

        Player_Initialise();

    }

    /// <summary>
    ///  Main Update loop for the state machine
    /// </summary>
    private void Update()
    {
        Block_Movement();
    }

    #endregion



    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions

    public void Player_Initialise()
    {
        //Node_Graph = GameObject.FindGameObjectWithTag("Game_Manager").GetComponent<Game_Manager>();

        GameManager = FindObjectOfType<Game_Manager>();

        //*! Assign the current grid position of the current world coodinates.
        grid_position.x = transform.position.x;
        grid_position.y = transform.position.y;

        //*! Current node is alligned to where it was placed
        Current_Node = GameManager.BL_Nodes[(int)(grid_position.x), (int)(grid_position.y)];

        is_grounded = Ground_Check();//false

        //*! Get the approiate player save based on the players type
        save_data = GameManager.gameObject.GetComponent<XML_SaveLoad>().Get_Active_Save((int)player_type);

        //*! Set the sticker count based on what level index
        Set_Sticker_Count();
    }

    #endregion


    //*! Private Access
    #region Private Functions

    #region Other Player Update
    private void Get_Other_Player()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int index = 0; index < players.Length; index++)
        {
            if (players[index].GetInstanceID() != this.gameObject.GetInstanceID())
            {
                Other_Player = players[index];
                ///Debug.Log("Blocks other player" + Other_Player.GetInstanceID());
            }
        }
    }
    private void Other_Player_Get_Map_Data()
    {
        if (Other_Player.GetComponent<Line_Controller>() != null)
        {
            Other_Player.GetComponent<Line_Controller>().Player_Initialise();
        }

        if (Other_Player.GetComponent<Block_Controller>() != null)
        {
            Other_Player.GetComponent<Block_Controller>().Player_Initialise();
        }
    }
    #endregion

    #region Sticker Collection

    private void Set_Sticker_Count()
    {

        if (save_data == null)
        {
            Debug.LogError("Block Player has no save data");
            return;
        }

        if (player_type == Player_Type.BLUE)
        {
            save_data.Level_Count[GameManager.lvDataIndex].sticker_count = new bool[GameManager.Blue_Sticker_Count];
        }
        else if (player_type == Player_Type.RED)
        {
            save_data.Level_Count[GameManager.lvDataIndex].sticker_count = new bool[GameManager.Red_Sticker_Count];
        }
    }

    private void Sticker_Collect()
    {
        if (player_type == Player_Type.BLUE)
        {
            if (Next_Node.Gizmos_GO != null && Next_Node.Node_Type == Node_Type.Block_Blue_Goal)
            {
                Next_Node.Node_Type = Node_Type.NONE;
                Next_Node.Gizmos_GO.SetActive(false);

                //*! Minus one from the current sticker count
                GameManager.Blue_Sticker_Count--;
                if (GameManager.Blue_Sticker_Count == 0 && GameManager.Red_Sticker_Count == 0)
                {
                    //GameManager.Initialize_Level();
                    Player_Initialise();
                    Other_Player_Get_Map_Data();
                }
            }
        }
        else
        {
            if (Next_Node.Gizmos_GO != null && Next_Node.Node_Type == Node_Type.Block_Red_Goal)
            {
                Next_Node.Node_Type = Node_Type.NONE;
                Next_Node.Gizmos_GO.SetActive(false);

                //*! Minus one from the current sticker count
                GameManager.Red_Sticker_Count--;
                if (GameManager.Blue_Sticker_Count == 0 && GameManager.Red_Sticker_Count == 0)
                {
                    //GameManager.Initialize_Level();
                    Player_Initialise();
                    Other_Player_Get_Map_Data();
                }
            }
        }

    }

    #endregion

    #region Queued Node Value Check and Shift the nodes accordinly
    private void Queued_Node_Check()
    {
        if (Queued_Node != null)
        {
            Queued_Node.Set_Traversability(false);
            //Queued_Node.Is_Occupied = true;
            ///Debug.Log("Q: Not null" + Queued_Node.Position);
            //*! Shift nodes if next is empty
            if (Next_Node == null && Queued_Node != null)
            {
                Queued_Node.Set_Traversability(false);
                //Queued_Node.Is_Occupied = true;
                ///Debug.Log("N: null" + Next_Node);
                //*! Shift Queued into the next node
                Next_Node = Queued_Node;
                ///Debug.Log("N: not null" + Next_Node.Position);
                //*! Clear the Queued node
                Queued_Node = null;
                ///Debug.Log("Q: null" + Queued_Node);
            }
        }
        else
        {
            ///Debug.Log("Q: null - Ground Check!");
            //StartCoroutine(Ground_Check());
            Ground_Check();
        }
    }
    private void Shift_Nodes()
    {
        if (Queued_Node != null)
        {
            Queued_Node.Set_Traversability(false);
            //Queued_Node.Is_Occupied = true;
            //*! Shift nodes if next is empty
            if (Next_Node == null && Queued_Node != null)
            {
                //*! Shift Queued into the next node
                Next_Node = Queued_Node;

                //*! Clear the Queued node
                Queued_Node = null;
            }
        }
    }

    #endregion

    #region Move and Check if at Next Node
    private void Move_And_Distance_Check()
    {
        //*! Current Position before the move
        Next_Node.Set_Traversability(false);

        //*! Move towards with precision to have the player exactly reach the next node
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(Next_Node.Position.x, Next_Node.Position.y, 0), movement_speed * Time.deltaTime);


        //*! Get the distance from the player to the next node
        float mag_distance = (new Vector3(Next_Node.Position.x, Next_Node.Position.y, 0) - transform.position).magnitude;

        //*! If distance is less then the threshhold - allow player to override the Queued node
        if (mag_distance < 0.5f)
        {
            can_second = true;
        }
    }
    private void Reached_Next_Node()
    {
        if (transform.position == new Vector3(Next_Node.Position.x, Next_Node.Position.y, 0))
        {
            //*! Finished moving, unless the below checks override that
            is_moving = false;

            //*! Reset the seond input permission
            can_second = false;

            //*! Collect a sticker
            Sticker_Collect();

            Reset_Traversability();

            //*! Does Queued node have a value
            Queued_Node_Check();

            //*! Update the other players map information
            //Other_Player_Get_Map_Data();
        }
    }


    #endregion

    #region Input Functions

    private void Block_Movement()
    {
        //*! Only when the player is not moving but are grounded
        Check_Input();

        /*-can second input to override the current Queued node-*/
        /*-when X% distance remaining of current to next -*/
        Second_Input();

        //*! Does Queued node have a value
        Shift_Nodes();

        //*! Move player next node is not null
        if (Next_Node != null)
        {
            Move_And_Distance_Check();


            //*! Reached the next node
            Reached_Next_Node();
        }
    }

    private void Second_Input()
    {
        if (is_moving == true && can_second == true && is_falling == false)
        {
            //*! Player Input checks - based on Current Node position.
            Queued_Node = Controller_Input();

            //*! Does it have a value, did the player input something?
            if (Queued_Node != null)
            {
                Queued_Node.Set_Traversability(false);
                //Queued_Node.Is_Occupied = true;
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

    private void Check_Input()
    {
        if (is_moving == false && is_grounded == true && is_falling == false)
        {
            //*! Current Position before the move
            //Current_Node.Is_Occupied = true;
            Current_Node.Set_Traversability(false);

            //*! Player Input checks - based on Current Node position.
            Queued_Node = Controller_Input();

            //*! Does it have a value, did the player input something?
            if (Queued_Node != null)
            {
                is_moving = true;

                Queued_Node.Set_Traversability(false);
                //Queued_Node.Is_Occupied = true;
            }

        }
    }

    #endregion

    #region Traverability Override - Controlling the Player
    private void Reset_Traversability()
    {
        Previous_Node = Current_Node;
        //*! When it is at the next node is not at the current node
        Previous_Node.Set_Traversability(true);
        //Previous_Node.Is_Occupied = false;

        //*! Shift the next node into the current node
        Current_Node = Next_Node;

        //*! Clear the next node
        Next_Node = null;

        //*! Update the grid position
        grid_position.x = Current_Node.Position.x;
        grid_position.y = Current_Node.Position.y;
    }

    /// <summary>
    /// Returns false if the player is not grounded
    /// </summary>
    /// <returns></returns>
    private bool Ground_Check()
    {
        //*! Can go down from the current grid position
        if (Current_Node.Can_DN == true)
        {
            //*! Assign the current grid positions down node to the Queued Node
            Queued_Node = Current_Node.DN_NODE;

            Queued_Node.Set_Traversability(false);


            //*! Is NOT Grounded
            is_grounded = false;

            //*! Player is now falling - lock out controlls until grounded
            is_falling = true;

            //*! Not grounded, so they must be moving / falling
            is_moving = true;


            //Other_Player_Get_Map_Data();

        }
        else
        {
            //*! Can not go down from current node

            Next_Node = null;
            Queued_Node = null;

            //*! Reset the key pressed flags
            up_key_pressed = false;
            down_key_pressed = false;
            left_key_pressed = false;
            right_key_pressed = false;

            //*! Is Grounded
            is_grounded = true;

            //*! Player is not falling as it is grounded - re-enable the controlls
            is_falling = false;

            //*! Stopped moving
            is_moving = false;
        }

        //*! Was a value assigned to the Queued Node
        return (Queued_Node != null) ? false : true;
    }

    #endregion


    /// <summary>
    /// Checks the current key pressed and sets the appropirate flag
    /// </summary>
    /// <returns>-The node to be assigned to the queue-</returns>
    private Node Controller_Input()
    {
        //*! Up key was pressed
        if (Input.GetKeyDown(Up_Key) == true && up_key_pressed == false && Current_Node.Can_UP == true)
        {
            //*! Set the flag for later use
            up_key_pressed = true;

            //*! Not grounded
            is_grounded = false;

            //*! Current Position before the move
            //Current_Node.Is_Occupied = true;

            //*! Return the node UP to the current grid position BUT if the next node is not null assign the node adacent to next
            return (Next_Node != null) ? Next_Node.UP_NODE : Current_Node.UP_NODE;     //*! Up Node
        }
        else if (Input.GetKeyDown(Down_Key) == true && down_key_pressed == false && Current_Node.Can_DN == true)
        {
            //*! Set the flag for later use
            down_key_pressed = true;

            //*! Current Position before the move
            //Current_Node.Is_Occupied = true;

            //*! Return the node DOWN to the current grid position
            return (Next_Node != null) ? Next_Node.DN_NODE : Current_Node.DN_NODE;     //*! Down Node
        }
        else if (Input.GetKeyDown(Left_Key) == true && left_key_pressed == false)
        {
            //*! Has the player jumped
            if (up_key_pressed == true && is_grounded == false)
            {
                //*! Set the flag for later use
                left_key_pressed = true;

                //*! Set the flag for later use
                right_key_pressed = true;
            }
            //*! Move along ground
            else if (up_key_pressed == false && is_grounded == true)
            {
                //*! Set the flag for later use
                left_key_pressed = false;
            }

            //*! Current Position before the move
            //Current_Node.Is_Occupied = true;

            //*! Return the node LEFT to the current grid position
            //*! If the next node doesnt have a value, current node's left
            //*! If the next node has a value then it hasn't being cleared being that it has not reached the next node, return the next nodes left
            return (Next_Node != null) ? Next_Node.LFT_NODE : Current_Node.LFT_NODE;    //*! Left Node
        }
        else if (Input.GetKeyDown(Right_Key) == true && right_key_pressed == false)
        {
            //*! Has the player jumped
            if (up_key_pressed == true && is_grounded == false)
            {
                //*! Set the flag for later use
                left_key_pressed = true;

                //*! Set the flag for later use
                right_key_pressed = true;
            }
            //*! Move along ground
            else if (up_key_pressed == false && is_grounded == true)
            {
                //*! Set the flag for later use
                right_key_pressed = false;
            }

            //*! Current Position before the move
            //Current_Node.Is_Occupied = true;

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


    #endregion


    //*! Protected Access
    #region Protected Functions

    #endregion

}



