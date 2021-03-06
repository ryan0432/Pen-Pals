﻿//*!--------------------------------------------------------------!*//
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

    public float floatingTime;

    [HideInInspector]
    public float timer;

    public Player_Save save_data;

    public Node currNode;
    public Node nextNode;


    //*!----------------------------!*//
    //*!      Private Variables
    //*!----------------------------!*//

    private Game_Manager gm;
    private Block_State currState;
    private Block_State prevState;

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

    private Animator ani;
    private SpriteRenderer spr;
    private Sound_Manager snd;

    private bool isArrived;
    private bool isLanded;
    private bool isSoundPlayed;


    //*!----------------------------!*//
    //*!      Unity Functions
    //*!----------------------------!*//

    [ContextMenu("Start")]
    void Start()
    {
        gm = FindObjectOfType<Game_Manager>();
        snd = FindObjectOfType<Sound_Manager>();
        ani = transform.GetChild(0).GetComponent<Animator>();
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        timer = floatingTime;

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
        //Debug.Log("Gnd Pos:" + Ground_Node().Position);
    }


    //*!----------------------------!*//
    //*!      Custom Functions
    //*!----------------------------!*//

    #region FSM Updates
    [ContextMenu("Static_State_Update")]
    private void Static_State_Update()
    {
        //Debug.Log("State: Static");

        if (prevState == Block_State.FALLING && !isLanded) { ani.Play("landing", 0); snd.Block_PlaySound(DN_Key, false); }

        isLanded = true;

        currNode.Set_Traversability(false);

        currPressedKey = NONE;
        qeuePressedKey = NONE;

        Ground_Check();

        if (Input.GetKeyDown(UP_Key) && currNode.UP_NODE != null && currNode.Can_UP && !currNode.UP_NODE.Is_Occupied /*&& !currNode.Can_DN*/)
        {
            Set_Line_Traversability(UP_Key, true);
            isSoundPlayed = false;
            isArrived = false;
            currPressedKey = UP_Key;
            prevState = currState;
            currState = Block_State.JUMPING;
        }

        if (Input.GetKeyDown(LFT_Key) && currNode.LFT_NODE != null && currNode.Can_LFT && !currNode.LFT_NODE.Is_Occupied /*&& !currNode.Can_DN*/)
        {
            Set_Line_Traversability(LFT_Key, true);
            isSoundPlayed = false;
            isArrived = false;
            currPressedKey = LFT_Key;
            prevState = currState;
            currState = Block_State.MOVING;
        }

        if (Input.GetKeyDown(RGT_Key) && currNode.RGT_NODE != null && currNode.Can_RGT && !currNode.RGT_NODE.Is_Occupied /*&& !currNode.Can_DN*/)
        {
            Set_Line_Traversability(RGT_Key, true);
            isSoundPlayed = false;
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

        ani.Play("jumping", 0);

        if (!isSoundPlayed) { snd.Block_PlaySound(UP_Key, true); isSoundPlayed = true; }

        if (!isArrived)
        {
            Move_Towards(Return_Input_Node(UP_Key), movingSpeed);

            if (Input.GetKeyDown(LFT_Key) && nextNode != null && nextNode.LFT_NODE != null && nextNode.Can_LFT && !nextNode.LFT_NODE.Is_Occupied)
            {
                qeuePressedKey = LFT_Key;
            }

            if (Input.GetKeyDown(RGT_Key) && nextNode != null && nextNode.RGT_NODE != null && nextNode.Can_RGT && !nextNode.RGT_NODE.Is_Occupied)
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
                isSoundPlayed = false;
                isArrived = false;
                currPressedKey = qeuePressedKey;
                qeuePressedKey = NONE;
                prevState = currState;
                currState = Block_State.MOVING;
            }
            else
            {
                isSoundPlayed = false;
                qeuePressedKey = NONE;
                prevState = currState;
                currState = Block_State.FLOATING;
            }
        }
    }

    [ContextMenu("Moving_State_Update")]
    private void Moving_State_Update()
    {
        //Debug.Log("State: Moving");

        if (currPressedKey == RGT_Key) { spr.flipX = false; ani.Play("sidestep", 0); }
        if (currPressedKey == LFT_Key) { spr.flipX = true; ani.Play("sidestep", 0); }

        if ((prevState == Block_State.FLOATING || prevState == Block_State.JUMPING) && !isSoundPlayed) { snd.Block_PlaySound(currPressedKey, true); isSoundPlayed = true; }
        else if (!isSoundPlayed) { snd.Block_PlaySound(currPressedKey, false); isSoundPlayed = true; }

        if (!isArrived)
        {
            Move_Towards(Return_Input_Node(currPressedKey), movingSpeed);

            if (Input.GetKeyDown(UP_Key) && nextNode != null && nextNode.UP_NODE != null && nextNode.Can_UP && !nextNode.UP_NODE.Is_Occupied/* && !nextNode.Can_DN && nextNode.DN_NODE != null*/)
            {
                qeuePressedKey = UP_Key;
            }

            if (Input.GetKeyDown(LFT_Key) && nextNode != null && nextNode.LFT_NODE != null && nextNode.Can_LFT && !nextNode.LFT_NODE.Is_Occupied/* && !nextNode.Can_DN && nextNode.DN_NODE != null*/)
            {
                qeuePressedKey = LFT_Key;
            }

            if (Input.GetKeyDown(RGT_Key) && nextNode != null && nextNode.RGT_NODE != null && nextNode.Can_RGT && !nextNode.RGT_NODE.Is_Occupied/* && !nextNode.Can_DN && nextNode.DN_NODE != null*/)
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
                isSoundPlayed = false;
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
                isSoundPlayed = false;
                isArrived = false;
                currPressedKey = qeuePressedKey;
                qeuePressedKey = NONE;
                prevState = currState;
                currState = Block_State.MOVING;
            }
            else
            {
                if (Ground_Check())
                {
                    isSoundPlayed = false;
                    prevState = currState;
                    currState = Block_State.STATIC;
                }
            }
        }
    }

    [ContextMenu("Floating_State_Update")]
    private void Floating_State_Update()
    {
        //Debug.Log("State: Floating");

        currNode.Set_Traversability(false);

        ani.Play("stopping", 0);

        timer -= Time.deltaTime;

        if (timer > 0f)
        {
            if (Input.GetKeyDown(LFT_Key) && currNode.LFT_NODE != null && currNode.Can_LFT && !currNode.LFT_NODE.Is_Occupied)
            {
                timer = floatingTime;
                qeuePressedKey = LFT_Key;
                Set_Line_Traversability(LFT_Key, true);
                isSoundPlayed = false;
                isArrived = false;
                currPressedKey = qeuePressedKey;
                qeuePressedKey = NONE;
                prevState = currState;
                currState = Block_State.MOVING;
            }

            if (Input.GetKeyDown(RGT_Key) && currNode.RGT_NODE != null && currNode.Can_RGT && !currNode.RGT_NODE.Is_Occupied)
            {
                timer = floatingTime;
                qeuePressedKey = RGT_Key;
                Set_Line_Traversability(RGT_Key, true);
                isSoundPlayed = false;
                isArrived = false;
                currPressedKey = qeuePressedKey;
                qeuePressedKey = NONE;
                prevState = currState;
                currState = Block_State.MOVING;
            }
        }
        else
        {
            timer = floatingTime;

            if (Ground_Check())
            {
                isSoundPlayed = false;
                prevState = currState;
                currState = Block_State.STATIC;
            }
        }
    }

    [ContextMenu("Falling_State_Update")]
    private void Falling_State_Update()
    {
        //Debug.Log("State: Falling");

        isLanded = false;

        ani.Play("falling", 0);

        if (!isSoundPlayed) { snd.Block_PlaySound(DN_Key, true); isSoundPlayed = true; }

        if (!isArrived)
        {
            Set_Line_Traversability(DN_Key, true);
            Move_Towards(currNode.DN_NODE, fallingSpeed);

            if (Input.GetKeyDown(UP_Key))
            {
                qeuePressedKey = UP_Key;
            }

            if (Input.GetKeyDown(LFT_Key))
            {
                qeuePressedKey = LFT_Key;
            }

            if (Input.GetKeyDown(RGT_Key))
            {
                qeuePressedKey = RGT_Key;
            }
        }
        else
        {
            Set_Line_Traversability(UP_Key, false);
            Set_Line_Traversability(DN_Key, false);

            if (currNode.DN_NODE != null && currNode.Can_DN && !currNode.DN_NODE.Is_Occupied)
            {
                Set_Line_Traversability(DN_Key, true);
                isArrived = false;
                prevState = currState;
            }
            else
            {
                if (qeuePressedKey == UP_Key && currNode.UP_NODE != null && currNode.Can_UP && !currNode.UP_NODE.Is_Occupied)
                {
                    Set_Line_Traversability(UP_Key, true);
                    isSoundPlayed = false;
                    isArrived = false;
                    currPressedKey = qeuePressedKey;
                    qeuePressedKey = NONE;
                    prevState = currState;
                    currState = Block_State.JUMPING;
                }
                else if ((qeuePressedKey == LFT_Key && currNode.LFT_NODE != null && currNode.Can_LFT && !currNode.LFT_NODE.Is_Occupied) ||
                    (qeuePressedKey == RGT_Key && currNode.RGT_NODE != null && currNode.Can_RGT && !currNode.RGT_NODE.Is_Occupied))
                {
                    if (qeuePressedKey == LFT_Key) { Set_Line_Traversability(LFT_Key, true); }
                    if (qeuePressedKey == RGT_Key) { Set_Line_Traversability(RGT_Key, true); }
                    isSoundPlayed = false;
                    isArrived = false;
                    currPressedKey = qeuePressedKey;
                    qeuePressedKey = NONE;
                    prevState = currState;
                    currState = Block_State.MOVING;
                }
                else
                {
                    if (Ground_Check())
                    {
                        isSoundPlayed = false;
                        prevState = currState;
                        currState = Block_State.STATIC;
                    }
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

            case Block_State.FLOATING:
                {
                    Floating_State_Update();
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
        if (currNode.DN_NODE != null && currNode.Can_DN && !currNode.DN_NODE.Is_Occupied)
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

        if (moveDistance < distBetweenNodes)
        {
            nextNode.Set_Traversability(false);
        }

        if (moveDistance < snapDistance)
        {
            currNode.Set_Traversability(true);
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
                snd.Game_PlaySound(Game_Sound.STICKER_COLLECT);
                currNode.Node_Type = Node_Type.NONE;
                currNode.Gizmos_GO.SetActive(false);
                gm.Blue_Sticker_Count--;

                if (gm.Blue_Sticker_Count == 0 && gm.Red_Sticker_Count == 0)
                {
                    snd.Game_PlaySound(Game_Sound.LEVEL_COMPLETE);
                    gm.Save_Players_Data();
                    gm.Initialize_Level(gm.lvDataIndex + 1);
                }
            }
        }
        else
        {
            if (currNode.Gizmos_GO != null && currNode.Node_Type == Node_Type.Block_Red_Goal)
            {
                snd.Game_PlaySound(Game_Sound.STICKER_COLLECT);
                currNode.Node_Type = Node_Type.NONE;
                currNode.Gizmos_GO.SetActive(false);
                gm.Red_Sticker_Count--;

                if (gm.Blue_Sticker_Count == 0 && gm.Red_Sticker_Count == 0)
                {
                    snd.Game_PlaySound(Game_Sound.LEVEL_COMPLETE);
                    gm.Save_Players_Data();
                    gm.Initialize_Level(gm.lvDataIndex + 1);
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
    FLOATING = 2,
    MOVING = 3,
    FALLING = 4
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