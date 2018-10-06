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
    private Block_State state;
    
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
        currNode = BL_Nodes[0, 7];
        transform.position = currNode.Position;
        state = Block_State.STATIC;
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
            pressedKey = UP_Key;
            state = Block_State.JUMPING;
        }

        if (Input.GetKeyDown(LFT_Key))
        {
            pressedKey = LFT_Key;
            state = Block_State.MOVING;
        }

        if (Input.GetKeyDown(RGT_Key))
        {
            pressedKey = RGT_Key;
            state = Block_State.MOVING;
        }

        Ground_Check();
    }

    [ContextMenu("Jumping_State_Update")]
    private void Jumping_State_Update()
    {
        isArrived = false;
        if(!isArrived) Move_Block(Return_Input_Node(UP_Key), jumpingSpeed);
        if (isArrived) { state = Block_State.STATIC; }
    }

    [ContextMenu("Moving_State_Update")]
    private void Moving_State_Update()
    {
        isArrived = false;

        if (pressedKey == LFT_Key && !isArrived)
        {
            Move_Block(Return_Input_Node(LFT_Key), movingSpeed);
        }

        if (pressedKey == RGT_Key && !isArrived)
        {
            Move_Block(Return_Input_Node(RGT_Key), movingSpeed);
        }

        if (isArrived) { state = Block_State.STATIC; }
    }

    [ContextMenu("Falling_State_Update")]
    private void Falling_State_Update()
    {
        #region "All the way down" falling method (Experimental)
        //Node floorNode = currNode.DN_NODE;

        //while (!Ground_Check())
        //{
        //    floorNode = floorNode.DN_NODE;
        //}

        //transform.position = Vector3.MoveTowards(transform.position, floorNode.Position, fallingSpeed * Time.deltaTime);

        //float fallingDistance = (transform.position - floorNode.Position).magnitude;

        //if (fallingDistance < snapDistance)
        //{
        //    currNode = floorNode;
        //    transform.position = currNode.Position;
        //}
        #endregion

        #region "One after another" falling method (Experimental)
        //nextNode = currNode.DN_NODE;

        //transform.position = Vector3.MoveTowards(transform.position, nextNode.Position, fallingSpeed * Time.deltaTime);

        //float downDistance = (transform.position - nextNode.Position).magnitude;

        //if (downDistance < snapDistance)
        //{
        //    currNode.Set_Traversability(true);
        //    nextNode.Set_Traversability(false);
        //    currNode = nextNode;
        //    nextNode = null;
        //    transform.position = currNode.Position;
        //}
        #endregion

        isArrived = false;

        Move_Block(currNode.DN_NODE, fallingSpeed);

        if (isArrived) { state = Block_State.STATIC; }
    }

    [ContextMenu("Runtime_Update")]
    private void Runtime_Update()
    {
        if (state == Block_State.STATIC)
        {
            Static_State_Update();
            Debug.Log("State: Static");
        }

        if (state == Block_State.JUMPING)
        {
            Jumping_State_Update();
            Debug.Log("State: Jumping");
        }

        if (state == Block_State.MOVING)
        {
            Moving_State_Update();
            Debug.Log("State: Moving");
        }

        if (state == Block_State.FALLING)
        {
            Falling_State_Update();
            Debug.Log("State: Falling");
        }
    }

    [ContextMenu("Ground_Check")]
    private bool Ground_Check()
    {
        if (currNode.Can_DN)
        {
            state = Block_State.FALLING;
            return false;
        }
        else
        {
            return true;
        }
    }

    [ContextMenu("Controller_Input")]
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
            qeueNode = null;
            transform.position = currNode.Position;
            isArrived = true;
        }
    }
}



public enum Block_State
{
    STATIC = 0,
    JUMPING = 1,
    MOVING = 2,
    FALLING = 3
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