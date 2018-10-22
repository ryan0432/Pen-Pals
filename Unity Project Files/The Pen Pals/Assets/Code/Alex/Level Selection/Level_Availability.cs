﻿//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;
using UnityEngine.UI;

public class Level_Availability : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    //*! Game Manager
    private Game_Manager game_manager = null;

    private float current_timer = 0f;

    //*! Red
    private Player_Save player_one;
    private int p1_pos = 0;
    private KeyCode P1_UP_Key
    { get { return KeyCode.UpArrow; } }
    private KeyCode P1_DOWN_Key
    { get { return KeyCode.DownArrow; } }
    private KeyCode P1_LEFT_Key
    { get { return KeyCode.LeftArrow; } }
    private KeyCode P1_RIGHT_Key
    { get { return KeyCode.RightArrow; } }

    //*! Blue
    private Player_Save player_two;
    private int p2_pos = 1;
    private KeyCode P2_UP_Key
    { get { return KeyCode.W; } }
    private KeyCode P2_DOWN_Key
    { get { return KeyCode.S; } }
    private KeyCode P2_LEFT_Key
    { get { return KeyCode.A; } }
    private KeyCode P2_RIGHT_Key
    { get { return KeyCode.D; } }


    #endregion

    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    public GameObject XML_Manager;

    public GameObject Player_RED_Selection = null;
    public GameObject Player_BLUE_Selection = null;

    public Level_Container[] Level_Selection;


    [Range(0.1f, 2.5f)]
    public float selection_timer = 0f;
    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions
    private void Start()
    {
        player_one = XML_Manager.GetComponent<XML_SaveLoad>().Get_Active_Save(1);
        player_two = XML_Manager.GetComponent<XML_SaveLoad>().Get_Active_Save(2);

        Player_RED_Selection.transform.position = Level_Selection[p1_pos].UI_Level.transform.position;
        Player_BLUE_Selection.transform.position = Level_Selection[p2_pos].UI_Level.transform.position;

        Load_Available_Levels();

        game_manager = FindObjectOfType<Game_Manager>();

        game_manager.player_one = player_one;
        game_manager.player_two = player_two;


    }

    private void Update()
    {

        Input_Check();

        //*! Sudo for when this executes.
        //*! 0 is the Menu Level
        if (game_manager.lvDataIndex == 0)
        {
            current_timer += Time.deltaTime;
            //*! Made a selection - Pass it to game manager
            if (Input.GetKeyDown(KeyCode.Y) || current_timer >= selection_timer)
            {
                current_timer = 0f;

                //Debug.Log("Player One RED : Current Selection index: " + Level_Selection[p1_pos].Game_Manager_Index);
                //Debug.Log("Player One BLUE : Current Selection index: " + Level_Selection[p2_pos].Game_Manager_Index);
                //*! They are on the same level selection 
                if (Level_Selection[p1_pos].Game_Manager_Index == Level_Selection[p2_pos].Game_Manager_Index)
                {
                    //Debug.Log("LOADING THE NEXT LEVEL");
                    //*! Either player can initialize the level based on the level index that it is on.
                    game_manager.Initialize_Level(Level_Selection[p1_pos].Game_Manager_Index);
                }
            }
        }
    }
    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Private Access
    #region Private Functions

    private void Input_Check()
    {
        Player_One_Input();
        Player_Two_Input();
    }

    //*! Red
    private void Player_One_Input()
    {
        /* if (Input.GetKeyDown(P1_UP_Key))
         {
             Player_RED_Selection.transform.position = Level_Selection[p1_pos -= 7].UI_Level.transform.position;// new Vector3(0, 1, 0);
         }
         else if (Input.GetKeyDown(P1_DOWN_Key))
         {
             Player_RED_Selection.transform.position = Level_Selection[p1_pos += 7].UI_Level.transform.position; //new Vector3(0, 1, 0);
         }
         else */
        if (Input.GetKeyDown(P1_LEFT_Key))
        {
            Player_RED_Selection.transform.position = Level_Selection[--p1_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
        }
        else if (Input.GetKeyDown(P1_RIGHT_Key))
        {
            Player_RED_Selection.transform.position = Level_Selection[++p1_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
        }
    }

    //*! Blue
    private void Player_Two_Input()
    {
        /*if (Input.GetKeyDown(P2_UP_Key))
        {
            Player_BLUE_Selection.transform.position = Level_Selection[p2_pos -= 7].UI_Level.transform.position;// new Vector3(0, 1, 0);
        }
        else if (Input.GetKeyDown(P2_DOWN_Key))
        {
            Player_BLUE_Selection.transform.position = Level_Selection[p2_pos += 7].UI_Level.transform.position; //new Vector3(0, 1, 0);
        }
        else */
        if (Input.GetKeyDown(P2_LEFT_Key))
        {
            Player_BLUE_Selection.transform.position = Level_Selection[--p2_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
        }
        else if (Input.GetKeyDown(P2_RIGHT_Key))
        {
            Player_BLUE_Selection.transform.position = Level_Selection[++p2_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
        }
    }


    private void Load_Available_Levels()
    {
        for (int index = 0; index < Level_Selection.Length; index++)
        {
            if (player_one.Level_Count[index].Unlocked)
            {
                Level_Selection[index].UI_Level.SetActive(true);

                if (player_one.Level_Count[index].Completed)
                {
                    Level_Selection[index].UI_Level.transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            if (player_two.Level_Count[index].Unlocked)
            {
                Level_Selection[index].UI_Level.SetActive(true);

                if (player_two.Level_Count[index].Completed)
                {
                    Level_Selection[index].UI_Level.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
        }

    }

    #endregion


    /// <summary>
    /// Helper Function
    /// </summary>
    #region Public Functions
    [ContextMenu("Populate_Level_List")]
    public void Populate_Level_List()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            Level_Selection[i - 1].UI_Level = transform.GetChild(i).gameObject;
        }
    }
    #endregion

}


[System.Serializable]
public class Level_Container
{
    [Range(0, 50)]
    public int Game_Manager_Index;
    public GameObject UI_Level;
}