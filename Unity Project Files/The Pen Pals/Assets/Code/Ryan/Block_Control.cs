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

    public Player_Type playerType;

    [HideInInspector]
    public Player_Shape playerShape = Player_Shape.BLOCK;

    [Range(0f, 5f)]
    public float movingSpeed;
    [Range(0f, 5f)]
    public float jumpingSpeed;
    [Range(0f, 5f)]
    public float fallingSpeed;


    //*!----------------------------!*//
    //*!      Private Variables
    //*!----------------------------!*//

    private Game_Manager gm;
    private Node[,] BL_Nodes;
    private Block_State currState;
    private Block_State prevState;

    private Node currNode;
    private Node nextNode;
    private Node qeueNode;

    private float snapDistance = 0.01f;

    private KeyCode UP_Key
    { get { return (playerType == Player_Type.BLUE) ? KeyCode.UpArrow : KeyCode.W; } }

    private KeyCode DN_Key
    { get { return (playerType == Player_Type.BLUE) ? KeyCode.DownArrow : KeyCode.S; } }

    private KeyCode LFT_Key
    { get { return (playerType == Player_Type.BLUE) ? KeyCode.LeftArrow : KeyCode.A; } }

    private KeyCode RGT_Key
    { get { return (playerType == Player_Type.BLUE) ? KeyCode.RightArrow : KeyCode.D; } }

    private KeyCode pressedKey;

    private bool isArrived;


    //*!----------------------------!*//
    //*!      Unity Functions
    //*!----------------------------!*//

    [ContextMenu("Start")]
    void Start()
    {
        gm = FindObjectOfType<Game_Manager>();
        BL_Nodes = gm.BL_Nodes;
        currNode = BL_Nodes[1, 7];
        transform.position = currNode.Position;
        currState = Block_State.STATIC;
    }

    [ContextMenu("Update")]
    void Update()
    {
        Runtime_Update();
        Debug.Log("Curr Node Pos:" + currNode.Position);
    }


    //*!----------------------------!*//
    //*!      Custom Functions
    //*!----------------------------!*//

    [ContextMenu("Static_State_Update")]
    private void Static_State_Update()
    {
        currNode.Set_Traversability(false);

        if (Input.GetKeyDown(UP_Key))
        {
            isArrived = false;
            pressedKey = UP_Key;
            currState = Block_State.JUMPING;
        }

        if (Input.GetKeyDown(LFT_Key))
        {
            isArrived = false;
            pressedKey = LFT_Key;
            currState = Block_State.MOVING;
        }

        if (Input.GetKeyDown(RGT_Key))
        {
            isArrived = false;
            pressedKey = RGT_Key;
            currState = Block_State.MOVING;
        }

        Ground_Check();
    }

    [ContextMenu("Jumping_State_Update")]
    private void Jumping_State_Update()
    {
        if (!isArrived)
        {
            if (Input.GetKeyDown(LFT_Key) && nextNode != null && nextNode.Can_LFT)
            {
                qeueNode = nextNode.LFT_NODE;
                pressedKey = LFT_Key;
            }

            if (Input.GetKeyDown(RGT_Key) && nextNode != null && nextNode.Can_RGT)
            {
                qeueNode = nextNode.RGT_NODE;
                pressedKey = RGT_Key;
            }

            Move_Block(Return_Input_Node(UP_Key), jumpingSpeed);
        } 
        else
        {
            if (qeueNode != null)
            {
                isArrived = false;
                currState = Block_State.JUMP_MOVING;
            }
            else
            {
                currState = Block_State.STATIC;
            }
        }
    }

    [ContextMenu("Moving_State_Update")]
    private void Moving_State_Update()
    {
        if (!isArrived)
        {
            Move_Block(Return_Input_Node(pressedKey), movingSpeed);

            if (Input.GetKeyDown(UP_Key) && nextNode != null && nextNode.Can_UP && !nextNode.Can_DN)
            {
                qeueNode = nextNode.UP_NODE;
            }
        }
        else
        {
            if (qeueNode != null)
            {
                isArrived = false;
                currState = Block_State.MOVE_JUMPING;
            }
            else
            {
                currState = Block_State.STATIC;
            }
        }
    }

    [ContextMenu("Move_Jumping_State_Update")]
    private void Move_Jumping_State_Update()
    {
        Ground_Check();
        qeueNode = null;

        if (!isArrived)
        {
            Move_Block(Return_Input_Node(UP_Key), jumpingSpeed);
        }
        else
        {
            
            currState = Block_State.STATIC;
        }
    }

    [ContextMenu("Jump_Moving_State_Update")]
    private void Jump_Moving_State_Update()
    {
        qeueNode = null;

        if (!isArrived)
        {
            Move_Block(Return_Input_Node(pressedKey), movingSpeed);
        }
        else
        {
            currState = Block_State.STATIC;
        }
    }

    [ContextMenu("Falling_State_Update")]
    private void Falling_State_Update()
    {
        Move_Block(currNode.DN_NODE, fallingSpeed);

        if (isArrived) { currState = Block_State.STATIC; }
    }

    [ContextMenu("Runtime_Update")]
    private void Runtime_Update()
    {
        if (currState == Block_State.STATIC)
        {
            Static_State_Update();
            Debug.Log("State: Static");
        }

        if (currState == Block_State.JUMPING)
        {
            Jumping_State_Update();
            Debug.Log("State: Jumping");
        }

        if (currState == Block_State.MOVING)
        {
            Moving_State_Update();
            Debug.Log("State: Moving");
        }

        if (currState == Block_State.FALLING)
        {
            Falling_State_Update();
            Debug.Log("State: Falling");
        }

        if (currState == Block_State.MOVE_JUMPING)
        {
            Move_Jumping_State_Update();
            Debug.Log("State: Move-Jumping");
        }

        if (currState == Block_State.JUMP_MOVING)
        {
            Jump_Moving_State_Update();
            Debug.Log("State: Jump-Moving");
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
        prevState = currState;

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
            isArrived = true;
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
    MOVE_JUMPING = 4,
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