//*!--------------------------------------------------------------!*//
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

    //public Player_Save save_data;

    [HideInInspector]
    public bool isDead;


    //*!----------------------------!*//
    //*!      Private Variables
    //*!----------------------------!*//

    public GameObject head;
    public Sprite headTailCap;

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
            segments[i].transform.SetParent(transform, true);

            if (i == 0 || i == segments.GetUpperBound(0))
            {
                segments[i].AddComponent<SpriteRenderer>();
                segments[i].GetComponent<SpriteRenderer>().sprite = headTailCap;
            }
        }

        Instantiate(head, anchors[0].Position, Quaternion.identity, transform);
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

    }

    [ContextMenu("Moving_State_Update")]
    private void Moving_State_Update()
    {

    }

    [ContextMenu("Reversing_State_Update")]
    private void Reversing_State_Update()
    {

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
        if (direction == UP_Key && currNode.Can_UP) { return currNode.UP_NODE; }
        if (direction == DN_Key && currNode.Can_DN) { return currNode.DN_NODE; }
        if (direction == LFT_Key && currNode.Can_LFT) { return currNode.LFT_NODE; }
        if (direction == RGT_Key && currNode.Can_RGT) { return currNode.RGT_NODE; }
        return null;
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