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

    private XML_SaveLoad xml_file_data;

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    public GameObject XML_Manager;

    public GameObject[] level_store;

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions
    private void Start()
    {
        xml_file_data = XML_Manager.GetComponent<XML_SaveLoad>();

        Unlock_Levels();

        Load_Star_Count();
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


    private void Load_Star_Count()
    {
        for (int player_index = 0; player_index < xml_file_data.Player_Saves.Count; player_index++)
        {
            for (int level_index = 0; level_index < level_store.Length; level_index++)
            {
                int score = 0;
                for (int sticker_index = 0; sticker_index < xml_file_data.Player_Saves[player_index].Level_Count[level_index].sticker_count.Length; sticker_index++)
                {
                    if (xml_file_data.Player_Saves[player_index].Level_Count[level_index].sticker_count[sticker_index] == true)
                    {
                        score++;
                    }
                }

                float percentage = 0.0f;
                float completion = 0.0f;
                if (score > 0)
                {
                    completion = 1.0f;
                    percentage = (xml_file_data.Player_Saves[player_index].Level_Count[level_index].sticker_count.Length / score);
                    percentage /= xml_file_data.Player_Saves[player_index].Level_Count[level_index].sticker_count.Length;
                    completion -= percentage;
                }


                int stars_to_show = 0;

                if (completion < 0.2f)
                {
                    stars_to_show = 0;
                }
                else if (completion >= 0.2f && completion <= 0.59f)
                {
                    stars_to_show = 1;
                }
                else if (completion >= 0.6f && completion <= 0.84f)
                {
                    stars_to_show = 2;
                }
                else if (completion >= 0.85f)
                {
                    stars_to_show = 3;
                }

                if (score == xml_file_data.Player_Saves[player_index].Level_Count[level_index].sticker_count.Length)
                {
                    stars_to_show = 3;
                }

                Debug.Log(completion);

                for (int star_index = 0; star_index < stars_to_show; star_index++)
                {
                    level_store[level_index].transform.GetChild(star_index + 1).gameObject.SetActive(true);
                    //if (xml_file_data.Player_Saves[player_index].Level_Count[level_index].star_count[star_index] == true)
                    //{
                    //}
                }
            }
        }
    }



    private void Unlock_Levels()
    {
        for (int player_index = 0; player_index < xml_file_data.Player_Saves.Count; player_index++)
        {
            for (int level_index = 0; level_index < level_store.Length; level_index++)
            {
                if (xml_file_data.Player_Saves[player_index].Level_Count[level_index].Unlocked == true)
                {
                    if (level_store[level_index].transform.Find("Lock") != null)
                    {
                        //level_store[level_index].gameObject.SetActive(true);
                        level_store[level_index].transform.Find("Lock").gameObject.SetActive(false);
                    }
                }
            }
        }
    }



    #endregion


}