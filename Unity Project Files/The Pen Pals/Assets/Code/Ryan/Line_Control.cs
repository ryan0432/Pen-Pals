﻿//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: This is the [Line_Control] class.
//*!              This class in an experimental class to test using
//*!              FSM to control [Line Player]
//*!
//*! Last edit  : 09/10/2018
//*!--------------------------------------------------------------!*//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_Control : MonoBehaviour
{
    //*!----------------------------!*//
    //*!      Public Variables
    //*!----------------------------!*//
    
    //*! Player Colour
    public Player_Type playerType;

    //*! Player Shape
    [HideInInspector]
    public Player_Shape playerShape = Player_Shape.LINE;

    [Range(0f, 5f)]
    public float movingSpeed;

    [Range(0f, 10f)]
    public float reversingSpeed;

    public GameObject head;

    public Sprite headTailCap;

    //public Player_Save save_data;

    [HideInInspector]
    public bool isDead;


    //*!----------------------------!*//
    //*!      Private Variables
    //*!----------------------------!*//

    private GameObject headGO;

    private Game_Manager gm;
    private Node[,] LI_Nodes;

    private Line_State currState;
    private Line_State prevState;

    private Node currNode;
    private Node nextNode;
    private Node[] anchors;
    private GameObject[] segments;

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
    private bool isArrayMovedForward;
    private bool isReachedTail;


    //*!----------------------------!*//
    //*!      Unity Functions
    //*!----------------------------!*//

    [ContextMenu("Start")]
    void Start()
    {
        gm = FindObjectOfType<Game_Manager>();
        LI_Nodes = gm.LI_Nodes;
        currState = Line_State.STATIC;
        Init_Line();
    }

    [ContextMenu("Init_Line")]
    private void Init_Line()
    {
        switch (playerType)
        {
            case Player_Type.BLUE:
                {
                    anchors = gm.Line_Blue_Start_Nodes;
                    break;
                }
            case Player_Type.RED:
                {
                    anchors = gm.Line_Red_Start_Nodes;
                    break;
                }
        }

        segments = new GameObject[anchors.Length];

        for (int i = 0; i < segments.Length; ++i)
        {
            segments[i] = new GameObject("segment" + i);
            segments[i].transform.position = anchors[i].Position;
            segments[i].transform.SetParent(transform);

            if (i == 0 || i == segments.GetUpperBound(0))
            {
                segments[i].AddComponent<SpriteRenderer>();
                segments[i].GetComponent<SpriteRenderer>().sprite = headTailCap;
            }
        }

        headGO = Instantiate(head, anchors[0].Position, Quaternion.identity, transform);
        currNode = anchors[0];
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
        currPressedKey = NONE;
        qeuePressedKey = NONE;

        if (Input.GetKeyDown(UP_Key))
        {
            currPressedKey = UP_Key;
            isArrived = false;
            prevState = currState;

            if (Return_Input_Node(UP_Key) == anchors[1])
            {
                currState = Line_State.REVERSING;
            }
            else
            {
                isArrayMovedForward = false;
                currState = Line_State.MOVING;
            }
        }

        if (Input.GetKeyDown(DN_Key))
        {
            currPressedKey = DN_Key;
            isArrived = false;
            prevState = currState;

            if (Return_Input_Node(DN_Key) == anchors[1])
            {
                currState = Line_State.REVERSING;
            }
            else
            {
                isArrayMovedForward = false;
                currState = Line_State.MOVING;
            }
        }

        if (Input.GetKeyDown(LFT_Key))
        {
            currPressedKey = LFT_Key;
            isArrived = false;
            prevState = currState;

            if (Return_Input_Node(LFT_Key) == anchors[1])
            {
                currState = Line_State.REVERSING;
            }
            else
            {
                isArrayMovedForward = false;
                currState = Line_State.MOVING;
            }
        }

        if (Input.GetKeyDown(RGT_Key))
        {
            currPressedKey = RGT_Key;
            isArrived = false;
            prevState = currState;

            if (Return_Input_Node(RGT_Key) == anchors[1])
            {
                currState = Line_State.REVERSING;
            }
            else
            {
                isArrayMovedForward = false;
                currState = Line_State.MOVING;
            }
        }
    }

    [ContextMenu("Moving_State_Update")]
    private void Moving_State_Update()
    {
        //Debug.Log("State: Moving");
        if (Return_Input_Node(currPressedKey) == null)
        {
            isArrayMovedForward = true;
            prevState = currState;
            currState = Line_State.STATIC;
        }

        if (!isArrayMovedForward)
        {
            anchors[anchors.GetUpperBound(0)].Set_Traversability(true);

            Set_Input_Edge_Traversability(currPressedKey);

            for (int i = anchors.GetUpperBound(0); i > 0; --i)
            {
                anchors[i] = anchors[i - 1];
            }
            anchors[0] = Return_Input_Node(currPressedKey);

            isArrayMovedForward = true;
        }

        if (!isArrived)
        {
            Move_Towards(Return_Input_Node(currPressedKey), movingSpeed);
        }
        else
        {
            Reset_Tail_Edge_Traversability();
            prevState = currState;
            currState = Line_State.STATIC;
        }
    }

    [ContextMenu("Reversing_State_Update")]
    private void Reversing_State_Update()
    {
        //Debug.Log("State: Reversing");
    }
    #endregion

    [ContextMenu("Runtime_Update")]
    private void Runtime_Update()
    {
        switch(currState)
        {
            case Line_State.STATIC:
                {
                    Static_State_Update();
                    break;
                }

            case Line_State.MOVING:
                {
                    Moving_State_Update();
                    break;
                }

            case Line_State.REVERSING:
                {
                    Reversing_State_Update();
                    break;
                }
        }
    }

    [ContextMenu("Return_Input_Node")]
    private Node Return_Input_Node(KeyCode direction)
    {
        if (direction == UP_Key) { return currNode.UP_NODE; }
        if (direction == DN_Key) { return currNode.DN_NODE; }
        if (direction == LFT_Key) { return currNode.LFT_NODE; }
        if (direction == RGT_Key) { return currNode.RGT_NODE; }
        return null;
    }

    [ContextMenu("Set_Input_Edge_Traversability")]
    private void Set_Input_Edge_Traversability(KeyCode direction)
    {
        if (direction == UP_Key) { currNode.UP_EDGE.Set_Traversability(false); }
        if (direction == DN_Key) { currNode.DN_EDGE.Set_Traversability(false); }
        if (direction == LFT_Key) { currNode.LFT_EDGE.Set_Traversability(false); }
        if (direction == RGT_Key) { currNode.RGT_EDGE.Set_Traversability(false); }
    }

    [ContextMenu("Reset_Tail_Edge_Traversability")]
    private void Reset_Tail_Edge_Traversability()
    {
        Node tail_node = anchors[anchors.GetUpperBound(0)];
        Node before_tail = anchors[anchors.GetUpperBound(0) - 1];

        if (tail_node.UP_NODE == before_tail)
        {
            if (tail_node.DN_EDGE != null) tail_node.DN_EDGE.Set_Traversability(true);
            if (tail_node.LFT_EDGE != null) tail_node.LFT_EDGE.Set_Traversability(true);
            if (tail_node.RGT_EDGE != null) tail_node.RGT_EDGE.Set_Traversability(true);
        }

        if (tail_node.DN_NODE == before_tail)
        {
            if (tail_node.UP_EDGE != null) tail_node.UP_EDGE.Set_Traversability(true);
            if (tail_node.LFT_EDGE != null) tail_node.LFT_EDGE.Set_Traversability(true);
            if (tail_node.RGT_EDGE != null) tail_node.RGT_EDGE.Set_Traversability(true);
        }
        if (tail_node.LFT_NODE == before_tail)
        {
            if (tail_node.UP_EDGE != null) tail_node.UP_EDGE.Set_Traversability(true);
            if (tail_node.DN_EDGE != null) tail_node.DN_EDGE.Set_Traversability(true);
            if (tail_node.RGT_EDGE != null) tail_node.RGT_EDGE.Set_Traversability(true);
        }
        if (tail_node.RGT_NODE == before_tail)
        {
            if (tail_node.UP_EDGE != null) tail_node.UP_EDGE.Set_Traversability(true);
            if (tail_node.DN_EDGE != null) tail_node.DN_EDGE.Set_Traversability(true);
            if (tail_node.LFT_EDGE != null) tail_node.LFT_EDGE.Set_Traversability(true);
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

        headGO.transform.position = Vector3.MoveTowards(headGO.transform.position, nextNode.Position, movingSpeed * Time.deltaTime);

        for (int i = 0; i < segments.Length; ++i)
        {
            segments[i].transform.position = Vector3.MoveTowards(segments[i].transform.position, anchors[i].Position, movingSpeed * Time.deltaTime);
        }

        float moveDistance = (headGO.transform.position - destNode.Position).magnitude;

        if (moveDistance < snapDistance)
        {
            currNode = nextNode;
            headGO.transform.position = currNode.Position;
            for (int i = 0; i < segments.Length; ++i)
            {
                segments[i].transform.position = anchors[i].Position;
                anchors[i].Set_Traversability(false);
            }
            nextNode = null;
            Collect_Sticker();
            isArrived = true;
        }
    }

    //*! Collect sticker when arrived.
    [ContextMenu("Collect_Sticker")]
    private void Collect_Sticker()
    {
        if (playerType == Player_Type.BLUE)
        {
            if (currNode.Gizmos_GO != null && currNode.Node_Type == Node_Type.Line_Blue_Goal)
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
            if (currNode.Gizmos_GO != null && currNode.Node_Type == Node_Type.Line_Red_Goal)
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

public enum Line_State
{
    STATIC = 0,
    MOVING = 1,
    REVERSING = 2
}