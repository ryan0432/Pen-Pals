//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: This is the [Block_Control] class.
//*!              This class in an experimental class to test using
//*!              FSM to control [Block Player]
//*!
//*! Last edit  : 05/10/2018
//*!--------------------------------------------------------------!*//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Control : MonoBehaviour
{
    //*!----------------------------!*//
    //*!      Public Variables
    //*!----------------------------!*//

    //*! Player Colour
    public Player_Type playerType;

    //*! Player Shape
    [HideInInspector]
    public Player_Shape playerShape = Player_Shape.BLOCK;

    [Range(0f, 5f)]
    public float movingSpeed;

    [Range(0f, 5f)]
    public float fallingSpeed;

    public Player_Save save_data;


    //*!----------------------------!*//
    //*!      Private Variables
    //*!----------------------------!*//

    private Game_Manager gm;
    private Node[,] BL_Nodes;
    private Block_State currState;
    private Block_State prevState;

    private Node currNode;
    private Node nextNode;

    private float snapDistance = 0.01f;

    private KeyCode UP_Key
    { get { return (playerType == Player_Type.BLUE) ? KeyCode.UpArrow : KeyCode.W; } }

    private KeyCode DN_Key
    { get { return (playerType == Player_Type.BLUE) ? KeyCode.DownArrow : KeyCode.S; } }

    private KeyCode LFT_Key
    { get { return (playerType == Player_Type.BLUE) ? KeyCode.LeftArrow : KeyCode.A; } }

    private KeyCode RGT_Key
    { get { return (playerType == Player_Type.BLUE) ? KeyCode.RightArrow : KeyCode.D; } }

    private KeyCode NONE
    { get { return KeyCode.None; } }

    private KeyCode currPressedKey;
    private KeyCode qeuePressedKey;

    private bool isArrived;


    //*!----------------------------!*//
    //*!      Unity Functions
    //*!----------------------------!*//

    [ContextMenu("Start")]
    void Start()
    {
        gm = FindObjectOfType<Game_Manager>();
        BL_Nodes = gm.BL_Nodes;

        if (playerType == Player_Type.BLUE)
        {
            currNode = gm.Block_Blue_Start_Node;
        }
        else
        {
            currNode = gm.Block_Red_Start_Node;
        }
        
        transform.position = currNode.Position;
        currState = Block_State.STATIC;
        save_data = gm.gameObject.GetComponent<XML_SaveLoad>().Get_Active_Save((int)playerType);
        Set_Sticker_Count();
    }

    [ContextMenu("Update")]
    void Update()
    {
        Runtime_Update();
    }


    //*!----------------------------!*//
    //*!      Custom Functions
    //*!----------------------------!*//

    #region FSM Updates
    [ContextMenu("Static_State_Update")]
    private void Static_State_Update()
    {
        Debug.Log("State: Static");

        currNode.Set_Traversability(false);

        qeuePressedKey = NONE;

        Ground_Check();

        if (Input.GetKeyDown(UP_Key))
        {
            isArrived = false;
            currPressedKey = UP_Key;
            prevState = currState;
            currState = Block_State.JUMPING;
        }

        if (Input.GetKeyDown(LFT_Key))
        {
            isArrived = false;
            currPressedKey = LFT_Key;
            prevState = currState;
            currState = Block_State.MOVING;
        }

        if (Input.GetKeyDown(RGT_Key))
        {
            isArrived = false;
            currPressedKey = RGT_Key;
            prevState = currState;
            currState = Block_State.MOVING;
        }
    }

    [ContextMenu("Jumping_State_Update")]
    private void Jumping_State_Update()
    {
        Debug.Log("State: Jumping");

        if (!isArrived)
        {
            if (Input.GetKeyDown(LFT_Key) && nextNode != null && nextNode.Can_LFT)
            {
                qeuePressedKey = LFT_Key;
            }

            if (Input.GetKeyDown(RGT_Key) && nextNode != null && nextNode.Can_RGT)
            {
                qeuePressedKey = RGT_Key;
            }

            Move_Block(Return_Input_Node(UP_Key), movingSpeed);
        } 
        else
        {
            if (qeuePressedKey != NONE)
            {
                isArrived = false;
                prevState = currState;
                currState = Block_State.JUMP_MOVING;
            }
            else
            {
                isArrived = false;
                prevState = currState;
                currState = Block_State.FALLING;
            }
        }
    }

    [ContextMenu("Moving_State_Update")]
    private void Moving_State_Update()
    {
        Debug.Log("State: Moving");

        Ground_Check();

        if (!isArrived)
        {
            if (Input.GetKeyDown(UP_Key) && nextNode != null && nextNode.Can_UP && !nextNode.Can_DN)
            {
                qeuePressedKey = UP_Key;
            }

            if (Input.GetKeyDown(LFT_Key) && nextNode != null && nextNode.Can_LFT && !nextNode.Can_DN)
            {
                qeuePressedKey = LFT_Key;
            }

            if (Input.GetKeyDown(RGT_Key) && nextNode != null && nextNode.Can_RGT && !nextNode.Can_DN)
            {
                qeuePressedKey = RGT_Key;
            }

            Move_Block(Return_Input_Node(currPressedKey), movingSpeed);
        }
        else
        {
            if (qeuePressedKey != NONE)
            {
                isArrived = false;
                prevState = currState;
                currState = Block_State.SECOND_MOVING;
            }
            else
            {
                prevState = currState;
                currState = Block_State.STATIC;
            }
        }
    }

    [ContextMenu("Falling_State_Update")]
    private void Falling_State_Update()
    {
        Debug.Log("State: Falling");

        if (!isArrived)
        {
            Move_Block(currNode.DN_NODE, fallingSpeed);
        }
        else
        {
            if (currNode.Can_DN)
            {
                isArrived = false;
            }
            else
            {
                qeuePressedKey = NONE;
                prevState = currState;
                currState = Block_State.STATIC;
            }
        }
    }

    [ContextMenu("Second_Moving_State_Update")]
    private void Second_Moving_State_Update()
    {
        Debug.Log("State: Second-Moving");

        Ground_Check();
        
        if (!isArrived)
        {
            Move_Block(Return_Input_Node(qeuePressedKey), movingSpeed);
        }
        else
        {
            if (qeuePressedKey == UP_Key)
            {
                qeuePressedKey = NONE;
                isArrived = false;
                prevState = currState;
                currState = Block_State.FALLING;
            }
            else
            {
                qeuePressedKey = NONE;
                prevState = currState;
                currState = Block_State.STATIC;
            }
        }
    }

    [ContextMenu("Jump_Moving_State_Update")]
    private void Jump_Moving_State_Update()
    {
        Debug.Log("State: Jump-Moving");

        if (!isArrived)
        {
            Move_Block(Return_Input_Node(qeuePressedKey), movingSpeed);
        }
        else
        {
            qeuePressedKey = NONE;
            isArrived = false;
            prevState = currState;
            currState = Block_State.FALLING;
        }
    }
    #endregion

    [ContextMenu("Runtime_Update")]
    private void Runtime_Update()
    {
        if (currState == Block_State.STATIC)
        {
            Static_State_Update();
        }

        if (currState == Block_State.JUMPING)
        {
            Jumping_State_Update();
        }

        if (currState == Block_State.MOVING)
        {
            Moving_State_Update();
        }

        if (currState == Block_State.FALLING)
        {
            Falling_State_Update();
        }

        if (currState == Block_State.SECOND_MOVING)
        {
            Second_Moving_State_Update();
        }

        if (currState == Block_State.JUMP_MOVING)
        {
            Jump_Moving_State_Update();
        }
    }

    [ContextMenu("Ground_Check")]
    private bool Ground_Check()
    {
        if (currNode.Can_DN)
        {
            isArrived = false;
            currState = Block_State.FALLING;
            return false;
        }
        else
        {
            return true;
        }
    }

    [ContextMenu("Return_Input_Node")]
    private Node Return_Input_Node(KeyCode direction)
    {
        if (direction == UP_Key && currNode.Can_UP) { return currNode.UP_NODE; }
        if (direction == LFT_Key && currNode.Can_LFT) { return currNode.LFT_NODE; }
        if (direction == RGT_Key && currNode.Can_RGT) { return currNode.RGT_NODE; }
        return null;
    }

    [ContextMenu("Move_Block")]
    private void Move_Block(Node destNode, float speed)
    {
        if (destNode == null)
        {
            isArrived = true;
            return;
        } 

        nextNode = destNode;

        transform.position = Vector3.MoveTowards(transform.position, nextNode.Position, speed * Time.deltaTime);

        float moveDistance = (transform.position - nextNode.Position).magnitude;

        if (moveDistance < snapDistance)
        {
            currNode.Set_Traversability(true);
            nextNode.Set_Traversability(false);
            currNode = nextNode;
            nextNode = null;
            transform.position = currNode.Position;
            Collect_Sticker();
            isArrived = true;
        }
    }

    [ContextMenu("Set_Sticker_Count")]
    private void Set_Sticker_Count()
    {
        if (save_data == null)
        {
            Debug.LogError("Block Player has no save data");
            return;
        }

        if (playerType == Player_Type.BLUE)
        {
            save_data.Level_Count[gm.lvDataIndex].sticker_count = new bool[gm.Blue_Sticker_Count];
        }
        else if (playerType == Player_Type.RED)
        {
            save_data.Level_Count[gm.lvDataIndex].sticker_count = new bool[gm.Red_Sticker_Count];
        }
    }

    //*! Collect sticker when arrived.
    [ContextMenu("Collect_Sticker")]
    private void Collect_Sticker()
    {
        if (playerType == Player_Type.BLUE)
        {
            if (currNode.Gizmos_GO != null && currNode.Node_Type == Node_Type.Block_Blue_Goal)
            {
                currNode.Node_Type = Node_Type.NONE;
                currNode.Gizmos_GO.SetActive(false);
                gm.Blue_Sticker_Count--;

                if (gm.Blue_Sticker_Count == 0 && gm.Red_Sticker_Count == 0)
                {
                    gm.Initialize_Level();
                }
            }
        }
        else
        {
            if (currNode.Gizmos_GO != null && currNode.Node_Type == Node_Type.Block_Red_Goal)
            {
                currNode.Node_Type = Node_Type.NONE;
                currNode.Gizmos_GO.SetActive(false);
                gm.Red_Sticker_Count--;

                if (gm.Blue_Sticker_Count == 0 && gm.Red_Sticker_Count == 0)
                {
                    gm.Initialize_Level();
                }
            }
        }
    }
}


//*!----------------------------!*//
//*!    Custom Enum Classes
//*!----------------------------!*//

public enum Block_State
{
    STATIC = 0,
    JUMPING = 1,
    MOVING = 2,
    FALLING = 3,
    SECOND_MOVING = 4,
    JUMP_MOVING = 5
}

public enum Player_Type
{
    RED = 1,
    BLUE = 2
}

public enum Player_Shape
{
    BLOCK = 1,
    LINE = 2
}