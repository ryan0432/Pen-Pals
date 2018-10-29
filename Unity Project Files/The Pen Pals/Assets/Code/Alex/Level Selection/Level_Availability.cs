//*!----------------------------!*//
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



    //*! Blue
    private Player_Save player_one;
    private int p1_pos = 1;

    private KeyCode P1_LEFT_Key
    { get { return KeyCode.A; } }
    private KeyCode P1_RIGHT_Key
    { get { return KeyCode.D; } }

    //*! Red
    private Player_Save player_two;
    private int p2_pos = 0;

    private KeyCode P2_LEFT_Key
    { get { return KeyCode.LeftArrow; } }
    private KeyCode P2_RIGHT_Key
    { get { return KeyCode.RightArrow; } }


    #endregion

    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables


    public GameObject Player_Blue_Selection = null;
    public GameObject Player_Red_Selection = null;

    public Level_Container[] Level_Selection;


    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions
    private void Start()
    {
        player_one = FindObjectOfType<XML_SaveLoad>().Get_Active_Save(1);
        player_two = FindObjectOfType<XML_SaveLoad>().Get_Active_Save(2);

        Player_Red_Selection.transform.position = Level_Selection[p1_pos].UI_Level.transform.position;
        Player_Blue_Selection.transform.position = Level_Selection[p2_pos].UI_Level.transform.position;


        game_manager = FindObjectOfType<Game_Manager>();

        if (game_manager.lvDataIndex == 0)
        {
            Load_Available_Levels();
        }


    }

    private void Update()
    {
        Menu_Check();
    }



    private void Menu_Check()
    {
        if (game_manager.lvDataIndex == 1)
        {
            Input_Check();

            if (GameObject.FindGameObjectWithTag("UI_Manager").transform.GetChild(0).gameObject.activeSelf == false)
            {
                GameObject.FindGameObjectWithTag("UI_Manager").transform.GetChild(0).gameObject.SetActive(true);
            }


            if (Player_Blue_Selection.activeSelf == false)
            {
                Player_Blue_Selection.SetActive(true);
            }

            if (Player_Red_Selection.activeSelf == false)
            {
                Player_Red_Selection.SetActive(true);
            }



            //*! Made a selection - Pass it to game manager
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {


                //Debug.Log("Player One RED : Current Selection index: " + Level_Selection[p1_pos].Game_Manager_Index);
                //Debug.Log("Player One BLUE : Current Selection index: " + Level_Selection[p2_pos].Game_Manager_Index);
                //*! They are on the same level selection 
                if (Level_Selection[p1_pos].Game_Manager_Index == Level_Selection[p2_pos].Game_Manager_Index)
                {
                    //*! Either player can initialize the level based on the level index that it is on.
                    game_manager.Initialize_Level(Level_Selection[p1_pos].Game_Manager_Index);

                    GameObject.FindGameObjectWithTag("UI_Manager").transform.GetChild(0).gameObject.SetActive(false);

                }
            }
        }
        else
        {
            GameObject.FindGameObjectWithTag("UI_Manager").transform.GetChild(0).gameObject.SetActive(false);

            if (Player_Blue_Selection.activeSelf == true)
            {
                Player_Blue_Selection.SetActive(false);
            }

            if (Player_Red_Selection.activeSelf == true)
            {
                Player_Red_Selection.SetActive(false);
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
        if (Player_ONE_Input() || Player_TWO_Input())
        {

        }
    }





    //*! Red
    private bool Player_ONE_Input()
    {

        if (Input.GetKeyDown(P1_LEFT_Key) && p1_pos > 0 && Level_Selection[p1_pos - 1].UI_Level.activeSelf == true)
        {
            if (p1_pos == 6 && Level_Selection[2].UI_Level.activeSelf == true)
            {
                p1_pos = 2;
                Player_Red_Selection.transform.position = Level_Selection[p1_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            }
            else if (p1_pos == 9 && Level_Selection[5].UI_Level.activeSelf == true)
            {
                p1_pos = 5;
                Player_Red_Selection.transform.position = Level_Selection[p1_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            }
            else
            {
                Player_Red_Selection.transform.position = Level_Selection[--p1_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            }
            return true;
        }
        else if (Input.GetKeyDown(P1_RIGHT_Key) && p1_pos < Level_Selection.Length - 1 && Level_Selection[p1_pos + 1].UI_Level.activeSelf == true)
        {
            if (p1_pos == 2 && Level_Selection[6].UI_Level.activeSelf == true)
            {
                p1_pos = 6;
                Player_Red_Selection.transform.position = Level_Selection[p1_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            }
            else if (p1_pos == 5 && Level_Selection[9].UI_Level.activeSelf == true)
            {
                p1_pos = 9;
                Player_Red_Selection.transform.position = Level_Selection[p1_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            }
            else
            {
                Player_Red_Selection.transform.position = Level_Selection[++p1_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            }

            return true;
        }
        else if (Input.GetKeyDown(KeyCode.S) && Level_Selection[Level_Selection[p1_pos].down].UI_Level.activeSelf == true)
        {
            p1_pos = Level_Selection[p1_pos].down;
            Player_Red_Selection.transform.position = Level_Selection[p1_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.W) && Level_Selection[Level_Selection[p1_pos].up].UI_Level.activeSelf == true)
        {
            p1_pos = Level_Selection[p1_pos].up;
            Player_Red_Selection.transform.position = Level_Selection[p1_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            return true;
        }
        else
        {
            return false;
        }
    }

    //*! Blue
    private bool Player_TWO_Input()
    {
        if (Input.GetKeyDown(P2_LEFT_Key) && p2_pos > 0 && Level_Selection[p2_pos - 1].UI_Level.activeSelf == true)
        {
            if (p2_pos == 6 && Level_Selection[2].UI_Level.activeSelf == true)
            {
                p2_pos = 2;
                Player_Blue_Selection.transform.position = Level_Selection[p2_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            }
            else if (p2_pos == 9 && Level_Selection[5].UI_Level.activeSelf == true)
            {
                p2_pos = 5;
                Player_Blue_Selection.transform.position = Level_Selection[p2_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            }
            else
            {
                Player_Blue_Selection.transform.position = Level_Selection[--p2_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            }

            return true;
        }
        else if (Input.GetKeyDown(P2_RIGHT_Key) && p2_pos < Level_Selection.Length - 1 && Level_Selection[p2_pos + 1].UI_Level.activeSelf == true)
        {
            if (p2_pos == 2 && Level_Selection[6].UI_Level.activeSelf == true)
            {
                p2_pos = 6;
                Player_Blue_Selection.transform.position = Level_Selection[p2_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            }
            else if (p2_pos == 5 && Level_Selection[5].UI_Level.activeSelf == true)
            {
                p2_pos = 9;
                Player_Blue_Selection.transform.position = Level_Selection[p2_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            }
            else
            {
                Player_Blue_Selection.transform.position = Level_Selection[++p2_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            }

            return true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && Level_Selection[Level_Selection[p2_pos].down].UI_Level.activeSelf == true)
        {
            p2_pos = Level_Selection[p2_pos].down;
            Player_Blue_Selection.transform.position = Level_Selection[p2_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && Level_Selection[Level_Selection[p2_pos].up].UI_Level.activeSelf == true)
        {
            p2_pos = Level_Selection[p2_pos].up;
            Player_Blue_Selection.transform.position = Level_Selection[p2_pos].UI_Level.transform.position;// new Vector3(1, 0, 0);
            return true;
        }
        else
        {
            return false;
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




}


[System.Serializable]
public class Level_Container
{
    [Range(0, 50)]
    public int Game_Manager_Index;
    public GameObject UI_Level;
    public int up;
    public int down;
}