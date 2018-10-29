using UnityEngine;
using APS.SALLY;


public class Sally_Example : MonoBehaviour
{
    //*! Create the Sally Object - Save and Load - IMPORTANT
    SAL sally = new SAL();

    //*! Example Object to use
    //public Player_Save_SALLY player;

    public Lv_Data[] level_data;

    private string sal_location = "";

    [ContextMenu("Sally Save")]
    public void SALLY_SAVE()
    {
        sal_location = Application.dataPath + "/Sally/Data/";

        for (int index = 0; index < level_data.Length; index++)
        {
            if (index < 9)
            {
                sally.Save_JSON(level_data[index], sal_location + "level_0" + (index + 1) + "_JSON_DOC.json", false);
            }
            else if (index >= 10)
            {
                sally.Save_JSON(level_data[index], sal_location + "level_" + (index + 1) + "_JSON_DOC.json", false);
            }
        }

        SALLY_LOAD_JSON();



        //sally.Save_JSON_BINF(player, sal_location + "player_JSON_DOC_BINF.json");

        //sally.Save_XML(player, sal_location + "player_XML_DOC.xml");
    }



    [ContextMenu("Sally Load JSON")]
    public void SALLY_LOAD_JSON()
    {
        sal_location = Application.dataPath + "/Sally/Data/";

        for (int index = 0; index < level_data.Length; index++)
        {
            if (index < 9)
            {
                level_data[index] = new Lv_Data();
                level_data[index].name = "level_0" + (index + 1);
                level_data[index] = sally.Load_JSON(level_data[index], sal_location + "level_0" + (index + 1) + "_JSON_DOC.json");
            }
            else if (index >= 10)
            {
                level_data[index] = new Lv_Data();
                level_data[index].name = "level_" + (index + 1);
                level_data[index] = sally.Load_JSON(level_data[index], sal_location + "level_" + (index + 1) + "_JSON_DOC.json");
            }
        }
    }



    [ContextMenu("Sally Load JSON BINF")]
    public void SALLY_LOAD_JSON_BINF()
    {
        sal_location = Application.dataPath + "/Sally/Data/";
        //sally.Load_JSON_BINF(player, sal_location + "player_BINF.json");
    }



    [ContextMenu("Sally Load XML")]
    public void SALLY_LOAD_XML()
    {
        sal_location = Application.dataPath + "/Sally/Data/";
        //sally.Load_XML(player, sal_location + "player_plain.xml");
    }

}

[System.Serializable]
/// <summary>
/// Example Data to write to file.
/// </summary>
//[CreateAssetMenu(fileName = "new player data", menuName = "Player_Data_SO")]
public class Player_Save_SALLY //: ScriptableObject
{
    //*! Name of player
    public string Name = "";

    public enum Player_Type
    {
        NONE = 0,
        RED = 1,
        BLUE = 2
    }

    public Player_Type player_type;

    public bool active_save;

    [Range(0, 100)]
    public int health;

}








