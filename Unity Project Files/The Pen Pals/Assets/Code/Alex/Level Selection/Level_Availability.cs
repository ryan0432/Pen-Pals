//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;


public class Level_Availability : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    private XML_SaveLoad xml_file_data;

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    public GameObject XML_Manager;

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions
    private void Start()
    {
        xml_file_data = XML_Manager.GetComponent<XML_SaveLoad>();

        Unlock_Levels();

        Set_Completed_Levels();
    }




    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions


    #endregion



    //*! Private Access
    #region Private Functions


    private void Set_Completed_Levels()
    {
        for (int player_index = 0; player_index < xml_file_data.Player_Saves.Count; player_index++)
        {
            for (int level_index = 0; level_index < transform.childCount; level_index++)
            {
                if (transform.GetChild(level_index).gameObject.activeSelf == true && xml_file_data.Player_Saves[player_index].Level_Count[level_index].Completed == true)
                {
                    transform.GetChild(level_index).GetComponent<Renderer>().material.color = new Color(0, 0, 1);
                }
            }

        }
    }


    private void Unlock_Levels()
    {
        for (int player_index = 0; player_index < xml_file_data.Player_Saves.Count; player_index++)
        {
            for (int level_index = 0; level_index < transform.childCount; level_index++)
            {
                if (xml_file_data.Player_Saves[player_index].Level_Count[level_index].Unlocked == true)
                {
                    transform.GetChild(level_index).gameObject.SetActive(true);
                    transform.GetChild(level_index).GetComponent<Renderer>().material.color = new Color(0, 1, 0);
                }
                else
                {
                    transform.GetChild(level_index).gameObject.SetActive(true);
                }
            }
        }
    }





    #endregion


}