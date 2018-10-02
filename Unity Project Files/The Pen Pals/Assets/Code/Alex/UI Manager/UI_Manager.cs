//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    //*! Main menu panel
    [SerializeField] private GameObject menu_panel;

    //*! Level selection panel
    [SerializeField] private GameObject level_select_panel;

    //*! Current menu state
    private Menu_State current_state;

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables


    //*! Menu States
    public enum Menu_State
    {
        MENU_PANEL = 0,
        LEVEL_SELECT_PANEL = 1
    }


    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions
    private void Start()
    {

    }

    private void Update()
    {


    }

    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions

    //*! Changing states of the menu
    public void Change_Panel(int a_menu_state)
    {
        //*! Casting the interger value to an enum
        switch ((Menu_State)a_menu_state)
        {
            case Menu_State.MENU_PANEL:
                menu_panel.SetActive(true);
                level_select_panel.SetActive(false);
                break;

            case Menu_State.LEVEL_SELECT_PANEL:
                menu_panel.SetActive(false);
                level_select_panel.SetActive(true);
                break;

            default:
                Debug.LogError("Not a valid panel number.");
                break;
        }
    }

    //*! Closes the game in editor and build
    public void End_Game()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    //*! Changes scene by the index value of the build orders
    public void Change_Scene(int a_scene_index)
    {
        SceneManager.LoadScene(a_scene_index, LoadSceneMode.Single);
    }


    #endregion



    //*! Private Access
    #region Private Functions


    #endregion



    //*! Protected Access
    #region Protected Functions

    #endregion



}