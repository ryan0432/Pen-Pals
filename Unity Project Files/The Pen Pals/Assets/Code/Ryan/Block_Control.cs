//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: This is the [Block_Control] class.
//*!              This class in an experimental class to test using
//*!              FSM to control [Block Player]
//*!
//*! Last edit  : 20/10/2018
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

    [Range(0f, 10f)]
    public float movingSpeed;

    [Range(0f, 10f)]
    public float fallingSpeed;

    public Player_Save save_data;

    [HideInInspector]
    public bool isDead;


    //*!----------------------------!*//
    //*!      Private Variables
    //*!----------------------------!*//

    private Game_Manager gm;
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
        save_data = gm.GetComponent<XML_SaveLoad>().Get_Active_Save((int)playerType);
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
        //Debug.Log("State: Static");

        currNode.Is_Occupied = true;
        currNode.Set_Traversability(false);
        Set_Line_Traversability(UP_Key, false);
        Set_Line_Traversability(DN_Key, false);

        currPressedKey = NONE;
        qeuePressedKey = NONE;

        Ground_Check();

        if (Input.GetKeyDown(UP_Key) && currNode.UP_NODE != null && !currNode.UP_NODE.Is_Occupied && !currNode.Can_DN)
        {
            Set_Line_Traversability(UP_Key, true);
            isArrived = false;
            currPressedKey = UP_Key;
            prevState = currState;
            currState = Block_State.JUMPING;
        }

        if (Input.GetKeyDown(LFT_Key) && currNode.LFT_NODE != null && !currNode.LFT_NODE.Is_Occupied && !currNode.Can_DN)
        {
            Set_Line_Traversability(LFT_Key, true);
            isArrived = false;
            currPressedKey = LFT_Key;
            prevState = currState;
            currState = Block_State.MOVING;
        }

        if (Input.GetKeyDown(RGT_Key) && currNode.RGT_NODE != null && !currNode.RGT_NODE.Is_Occupied && !currNode.Can_DN)
        {
            Set_Line_Traversability(RGT_Key, true);
            isArrived = false;
            currPressedKey = RGT_Key;
            prevState = currState;
            currState = Block_State.MOVING;
        }
    }

    [ContextMenu("Jumping_State_Update")]
    private void Jumping_State_Update()
    {
        //Debug.Log("State: Jumping");

        if (!isArrived)
        {
            Move_Towards(Return_Input_Node(UP_Key), movingSpeed);

            if (Input.GetKeyDown(LFT_Key) && nextNode != null && nextNode.Can_LFT && !nextNode.LFT_NODE.Is_Occupied)
            {
                qeuePressedKey = LFT_Key;
            }

            if (Input.GetKeyDown(RGT_Key) && nextNode != null && nextNode.Can_RGT && !nextNode.RGT_NODE.Is_Occupied)
            {
                qeuePressedKey = RGT_Key;
            }           
        } 
        else
        {
            Set_Line_Traversability(UP_Key, false);
            Set_Line_Traversability(DN_Key, false);

            if (qeuePressedKey != NONE)
            {
                if (qeuePressedKey == LFT_Key) { Set_Line_Traversability(LFT_Key, true); }
                if (qeuePressedKey == RGT_Key) { Set_Line_Traversability(RGT_Key, true); }
                isArrived = false;
                currPressedKey = qeuePressedKey;
                qeuePressedKey = NONE;
                prevState = currState;
                currState = Block_State.MOVING;
            }
            else
            {
                isArrived = false;
                qeuePressedKey = NONE;
                prevState = currState;
                currState = Block_State.FALLING;
            }
        }
    }

    [ContextMenu("Moving_State_Update")]
    private void Moving_State_Update()
    {
        //Debug.Log("State: Moving");

        if (!isArrived)
        {
            Move_Towards(Return_Input_Node(currPressedKey), movingSpeed);

            if (Input.GetKeyDown(UP_Key) && nextNode != null && nextNode.Can_UP && !nextNode.UP_NODE.Is_Occupied && !nextNode.Can_DN)
            {
                qeuePressedKey = UP_Key;
            }

            if (Input.GetKeyDown(LFT_Key) && nextNode != null && nextNode.Can_LFT && !nextNode.LFT_NODE.Is_Occupied && !nextNode.Can_DN)
            {
                qeuePressedKey = LFT_Key;
            }

            if (Input.GetKeyDown(RGT_Key) && nextNode != null && nextNode.Can_RGT && !nextNode.RGT_NODE.Is_Occupied && !nextNode.Can_DN)
            {
                qeuePressedKey = RGT_Key;
            }
        }
        else
        {
            Set_Line_Traversability(UP_Key, false);
            Set_Line_Traversability(DN_Key, false);

            Ground_Check();

            if (qeuePressedKey == UP_Key)
            {
                Set_Line_Traversability(UP_Key, true);
                isArrived = false;
                currPressedKey = qeuePressedKey;
                qeuePressedKey = NONE;
                prevState = currState;
                currState = Block_State.JUMPING;
            }
            else if (qeuePressedKey == LFT_Key || qeuePressedKey == RGT_Key)
            {
                if (qeuePressedKey == LFT_Key) { Set_Line_Traversability(LFT_Key, true); }
                if (qeuePressedKey == RGT_Key) { Set_Line_Traversability(RGT_Key, true); }
                isArrived = false;
                currPressedKey = qeuePressedKey;
                qeuePressedKey = NONE;
                prevState = currState;
                currState = Block_State.MOVING;
            }
            else
            {
                Ground_Check();
                prevState = currState;
                currState = Block_State.STATIC;
            }
        }
    }

    [ContextMenu("Falling_State_Update")]
    private void Falling_State_Update()
    {
        //Debug.Log("State: Falling");

        if (!isArrived)
        {
            Set_Line_Traversability(DN_Key, true);
            Move_Towards(currNode.DN_NODE, fallingSpeed);

            if (Input.GetKeyDown(UP_Key) && Ground_Node().Can_UP && !Ground_Node().UP_NODE.Is_Occupied)
            {
                qeuePressedKey = UP_Key;
            }

            if (Input.GetKeyDown(LFT_Key) && Ground_Node().Can_LFT && !Ground_Node().LFT_NODE.Is_Occupied)
            {
                qeuePressedKey = LFT_Key;
            }

            if (Input.GetKeyDown(RGT_Key) && Ground_Node().Can_RGT && !Ground_Node().RGT_NODE.Is_Occupied)
            {
                qeuePressedKey = RGT_Key;
            }
        }
        else
        {
            Set_Line_Traversability(UP_Key, false);
            Set_Line_Traversability(DN_Key, false);

            if (currNode.Can_DN && !currNode.DN_NODE.Is_Occupied)
            {
                Set_Line_Traversability(DN_Key, true);
                isArrived = false;
                prevState = currState;
            }
            else
            {
                if (qeuePressedKey == UP_Key)
                {
                    Set_Line_Traversability(UP_Key, true);
                    isArrived = false;
                    currPressedKey = qeuePressedKey;
                    qeuePressedKey = NONE;
                    prevState = currState;
                    currState = Block_State.JUMPING;
                }
                else if (qeuePressedKey == LFT_Key || qeuePressedKey == RGT_Key)
                {
                    if (qeuePressedKey == LFT_Key) { Set_Line_Traversability(LFT_Key, true); }
                    if (qeuePressedKey == RGT_Key) { Set_Line_Traversability(RGT_Key, true); }
                    isArrived = false;
                    currPressedKey = qeuePressedKey;
                    qeuePressedKey = NONE;
                    prevState = currState;
                    currState = Block_State.MOVING;
                }
                else
                {
                    Ground_Check();
                    prevState = currState;
                    currState = Block_State.STATIC;
                }
            }
        }
    }
    #endregion

    [ContextMenu("Runtime_Update")]
    private void Runtime_Update()
    {
        switch (currState)
        {
            case Block_State.STATIC:
                {
                    Static_State_Update();
                    break;
                }

            case Block_State.JUMPING:
                {
                    Jumping_State_Update();
                    break;
                }

            case Block_State.MOVING:
                {
                    Moving_State_Update();
                    break;
                }

            case Block_State.FALLING:
                {
                    Falling_State_Update();
                    break;
                }
        }
    }

    [ContextMenu("Ground_Check")]
    private bool Ground_Check()
    {
        if (currNode.Can_DN && !currNode.DN_NODE.Is_Occupied)
        {
            isArrived = false;
            qeuePressedKey = NONE;
            prevState = currState;
            currState = Block_State.FALLING;
            return false;
        }
        else
        {
            return true;
        }
    }

    [ContextMenu("Ground_Node")]
    private Node Ground_Node()
    {
        Node groundNode = currNode;

        while (groundNode.Can_DN && !groundNode.DN_NODE.Is_Occupied)
        {
            groundNode = groundNode.DN_NODE;
        }

        return groundNode;
    }

    [ContextMenu("Return_Input_Node")]
    private Node Return_Input_Node(KeyCode direction)
    {
        if (direction == UP_Key) { return currNode.UP_NODE; }
        if (direction == LFT_Key) { return currNode.LFT_NODE; }
        if (direction == RGT_Key) { return currNode.RGT_NODE; }
        return null;
    }

    [ContextMenu("Set_Line_Traversability")]
    private void Set_Line_Traversability(KeyCode direction, bool isOccupied)
    {
        if (direction == UP_Key)
        {
            currNode.UP_LFT_NODE.Is_Occupied = isOccupied;
            currNode.UP_RGT_NODE.Is_Occupied = isOccupied;
        }

        if (direction == DN_Key)
        {
            currNode.DN_LFT_NODE.Is_Occupied = isOccupied;
            currNode.DN_RGT_NODE.Is_Occupied = isOccupied;
        }

        if (direction == LFT_Key)
        {
            currNode.UP_LFT_NODE.Is_Occupied = isOccupied;
            currNode.DN_LFT_NODE.Is_Occupied = isOccupied;
        }

        if (direction == RGT_Key)
        {
            currNode.UP_RGT_NODE.Is_Occupied = isOccupied;
            currNode.DN_RGT_NODE.Is_Occupied = isOccupied;
        }
    }

    [ContextMenu("Move_Towards")]
    private void Move_Towards(Node destNode, float speed)
    {
        if (destNode == null)
        {
            isArrived = true;
            return;
        } 

        nextNode = destNode;

        transform.position = Vector3.MoveTowards(transform.position, nextNode.Position, speed * Time.deltaTime);

        float moveDistance = (transform.position - nextNode.Position).magnitude;
        float distBetweenNodes = (currNode.Position - nextNode.Position).magnitude;

        if (moveDistance < distBetweenNodes * 0.9f)
        {
            nextNode.Is_Occupied = true;
            currNode.Is_Occupied = false;
            currNode.Set_Traversability(true);
        }

        if (moveDistance < snapDistance)
        {
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
            Debug.LogError("[Block] Player has no save data");
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