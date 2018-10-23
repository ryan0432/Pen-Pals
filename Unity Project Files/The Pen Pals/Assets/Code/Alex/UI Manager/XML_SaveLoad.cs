//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//

//*! Using namespaces
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;



/// <summary>
/// Loading of Players in Awake please use Start for accessing player save information
/// </summary>
public class XML_SaveLoad : MonoBehaviour
{
    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    //*! Player Saves Container
    public List<Player_Save> Player_Saves;

    #endregion

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Public Variables

    //*! Player Saves File Path
    private string data_path = "./Player Save Files/";

    #endregion

    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions

    private void Awake()
    {
        Load_All_Players();
    }

    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions

    public void Set_Player_Red_Save(int selected_index)
    {
        Player_Saves[selected_index].active_save = true;
        Player_Saves[selected_index].player_type = Player_Save.Player_Type.RED;
    }

    public void Set_Player_Blue_Save(int selected_index)
    {
        Player_Saves[selected_index].active_save = true;
        Player_Saves[selected_index].player_type = Player_Save.Player_Type.BLUE;
    }

    /// <summary>
    /// Based on the player type returns the appropirate save back
    /// </summary>
    /// <param name="player_type"></param>
    /// <returns> Player_Save Information </returns>
    public Player_Save Get_Active_Save(int player_type)
    {
        for (int index = 0; index < Player_Saves.Count; index++)
        {
            if (Player_Saves[index].active_save == true && (int)Player_Saves[index].player_type == player_type)
            {
                return Player_Saves[index];
            }
        }

        Debug.LogError("NO SAVE DATA FOUND");
        return null;
    }


    /// <summary>
    /// Removes the player save from the list and file directory
    /// </summary>
    /// <param name="player_number"> Used to index into the array of player saves </param>
    public void Remove_Player(int player_number)
    {
        if (Player_Saves.Count == 0)
        {
            Debug.LogError("NO SAVE FILES IN THE DIRECTORY");
            //*! Don't Delete anything.
            return;
        }

        File.Delete("./Player Save Files/" + Player_Saves[player_number].Name + ".xml");
        Player_Saves.RemoveAt(player_number);
    }


    /// <summary>
    /// Adding a new player and writing it to file
    /// </summary>
    [ContextMenu("Add a new Player")]
    public void Add_Player()
    {
        if (Player_Saves.Count == 0)
        {
            Load_All_Players();

            //*! Create a new Player
            Player_Saves.Add(new Player_Save());

            //*! Save the last player added to the list
            Save_Player(Player_Saves.Count - 1);
        }
        else
        {
            //*! Create a new Player
            Player_Saves.Add(new Player_Save());

            //*! Save the last player added to the list
            Save_Player(Player_Saves.Count - 1);
        }

    }

    //*! Save all function
    #region Not Needed, but handy to have if need be.
    /// <summary>
    /// Saving all players to file from the Player_Saves List
    /// </summary>
    [ContextMenu("Save all Player")]
    public void Save_All_Players()
    {
        if (Player_Saves.Count == 0)
            return;

        //*! Save All Players
        for (int index = 0; index < Player_Saves.Count; index++)
        {

            //*! Blank Player Name
            if (Player_Saves[index].Name == "")
            {

                if (index + 1 <= 9)
                {
                    Player_Saves[index].Name = "Player_0" + (index + 1);
                }
                else
                {
                    Player_Saves[index].Name = "Player_" + (index + 1);
                }

                for (int inner_index = 0; inner_index < 15; inner_index++)
                {
                    Player_Saves[index].Level_Count.Add(new Level_Data());
                }
            }


            if (index + 1 <= 9)
            {
                //*! Construct the Player objects save file path
                Player_Saves[index].Data_Path = (data_path + "Player_0" + (index + 1) + ".xml");
                //*! Save each player
                Player_Saves[index].Save();
            }
            else
            {
                //*! Construct the Player objects save file path
                Player_Saves[index].Data_Path = (data_path + "Player_" + (index + 1) + ".xml");
                //*! Save each player
                Player_Saves[index].Save();
            }

        }
    }
    #endregion

    /// <summary>
    /// Load in all the save files and adding them to the Player_Saves List
    /// </summary>
    [ContextMenu("Load all Players")]
    public void Load_All_Players()
    {
        //*! Get some information on the directory 
        DirectoryInfo info = new DirectoryInfo(data_path);

        //*! Get the count of files in that directory
        List<FileInfo> save_files = new List<FileInfo>();

        save_files.AddRange(info.GetFiles());

        //*! No Save File Count
        if (save_files.Count == 0)
        {
            Debug.LogWarning("NO SAVE FILES IN THE DIRECTORY");
            //*! Don't load the save files.
            return;
        }

        //*! Add null objects to the player saves list of the count of files in the saves directory
        for (int index = 0; index < save_files.Count; index++)
        {
            ///Debug.Log("Added a new Player at INDEX : " + index + " : " + save_files[index].Name);
            Player_Saves.Add(null);
        }

        //*! Load All Players
        for (int index = 0; index < save_files.Count; index++)
        {
            Player_Saves[index] = Load<Player_Save>(data_path + save_files[index].Name);
        }

        //*! Remove any null objects in the list
        while (Player_Saves.Remove(null))
        {
            Debug.LogWarning("Removed a null object");
        };

    }


    [ContextMenu("Unlock all levels for all players")]
    public void Unlock_All_For_All()
    {
        for (int player_index = 0; player_index < Player_Saves.Count; player_index++)
        {
            for (int level_index = 0; level_index < Player_Saves[player_index].Level_Count.Count; level_index++)
            {
                Player_Saves[player_index].Level_Count[level_index].Unlocked = true;
            }
        }
    }



    #endregion


    //*! Private Access
    #region Private Functions


    private void Save_Player(int player_id)
    {
        //*! Use Player_0# for 1-9 then 10 11 12 etc


        //*! Blank Player Name
        if (Player_Saves[player_id].Name == "")
        {

            if (player_id + 1 <= 9)
            {
                Player_Saves[player_id].Name = "Player_0" + (player_id + 1);
            }
            else
            {
                Player_Saves[player_id].Name = "Player_" + (player_id + 1);
            }


            for (int inner_index = 0; inner_index < 21; inner_index++)
            {
                Player_Saves[player_id].Level_Count.Add(new Level_Data());
            }
        }

        //*!----------------------------!*//
        //*!    Reminder - for each player the data path needs to be set.
        //*!    Then clear the data path stored for that player.
        //*!    Side note it allows for each player object to saved in different locations.
        //*!    Also allows for the player_save reference can just call player.Save(); and done.
        //*!----------------------------!*//


        if (player_id + 1 <= 9)
        {
            //*! Construct the Player objects save file path
            Player_Saves[player_id].Data_Path = (data_path + "Player_0" + (player_id + 1) + ".xml");
            //*! Save each player
            Player_Saves[player_id].Save();
        }
        else
        {
            //*! Construct the Player objects save file path
            Player_Saves[player_id].Data_Path = (data_path + "Player_" + (player_id + 1) + ".xml");
            //*! Save each player
            Player_Saves[player_id].Save();
        }

    }

    //*! Player Load 
    private static T Load<T>(string target_save_path)
    {
        //*! Create the XML Serializer
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        //*! Create a file stream and open file
        using (FileStream stream = new FileStream(target_save_path, FileMode.Open))
        {
            //*! Return the file information casted as a Player Save Class
            return (T)serializer.Deserialize(stream);
        }
    }

    #endregion
}



/// <summary>
/// For each Level this contains the information about each 
/// </summary>
[System.Serializable]
public class Level_Data
{

    //*! Name of Level
    [XmlArrayItem("Level Name")]
    public string Name = "";

    //*! Level Number
    [XmlArrayItem("Level ID")]
    public int Level_ID { get; set; }

    //*! Completed by Player
    [XmlArrayItem("Completed")]
    public bool Completed;

    //*! Unlocked For Player
    [XmlArrayItem("Unlocked")]
    public bool Unlocked;

    //*! Sticker Count
    [XmlArray("Sticker_Count"), XmlArrayItem("Sticker_Count")]
    public bool[] sticker_count = new bool[3];


}



[System.Serializable]
public class Player_Save
{
    //*! Private get for internal use, but the public set allows the information to dynamically set it.
    //*! Also not writable to file.
    public string Data_Path { get; set; }

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

    //*! Count of levels the game has.
    [XmlArray("Level_Count"), XmlArrayItem("Level_Data")]
    public List<Level_Data> Level_Count = new List<Level_Data>();

    /// <summary>
    /// Check Against the level Defaults - making sure the values are not blank
    /// </summary>
    private void Check_Level_Defaults()
    {
        //*! For each level in the array
        int count = 1;
        foreach (var level in Level_Count)
        {

            //*! Set the Level ID Number based on the count of levels
            level.Level_ID = count;

            //*! Set the default name of the level if it was blank
            if (level.Name == "")
            {
                level.Name = "Level Default_" + count;
            }

            //*! Was empty, puting in one sticker
            if (level.sticker_count.Length == 0)
            {
                level.sticker_count = new bool[1];
            }

            //*! Increase the level count number
            count++;
        }
    }


    //*! When Completed a level
    public void Completed_Level(int level_index)
    {
        Level_Count[level_index].Completed = true;

        if (level_index < Level_Count.Count - 1)
        {
            Level_Count[level_index + 1].Unlocked = true;
        }
    }


    //*! Player Save 
    public void Save()
    {
        //*! Check Against the level Defaults
        Check_Level_Defaults();

        //*! Create the XML Serializer
        XmlSerializer serializer = new XmlSerializer(typeof(Player_Save));
        //*! Create a file stream and write file
        using (FileStream stream = new FileStream(Data_Path, FileMode.Create))
        {
            //*! Write the XML file
            serializer.Serialize(stream, this);
        }
    }
}