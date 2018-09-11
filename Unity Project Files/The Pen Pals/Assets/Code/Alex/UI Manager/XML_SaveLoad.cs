//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//

//*! Using namespaces
using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;


//*!----------------------------!*//
//*!    Commenting Style
//*!----------------------------!*//
#region Commenting style

//*! Single Line


/*--*/
/*-
* Multi-line
*  
*  
* -*/

/*!!*/
/*!
 * Multi-line
 * 
 * !*/

// Temporary comment
/// Side Notes, not really worth a proper single line comment, but something to take note of.

//*! If you can't explain the function in 1 short sentence. 
/// <summary>
/// Above complex funtions, otherwise use a single line comment.
/// </summary>
/// <param name="argument_name"> One space before and after the comment on the argument </param>


/// Seperated by 32 '-' Minus symbols
/// Title of the Region 1 tab space
//*!----------------------------!*//
//*!    Private Variables
//*!----------------------------!*//
#region Areas of code

#endregion

#endregion


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
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions

    private void Start()
    {
        //*! Load Player One
        ///player_saves[0] = Player_Save.Load("./Player Save Files/player_" + (index + 1) + ".xml");

        Load_All_Players();
    }

    #endregion

    //*!----------------------------!*//
    //*!    Custom Functions
    //*!----------------------------!*//

    //*! Public Access
    #region Public Functions

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
    public void Add_Player()
    {
        //*! Create a new Player
        Player_Saves.Add(new Player_Save());

        //*! Save the last player added to the list
        Save_Player(Player_Saves.Count - 1);
    }

    /// <summary>
    /// Saving all players to file from the Player_Saves List
    /// </summary>
    public void Save_All_Players()
    {
        //*! Save All Players
        for (int index = 0; index < Player_Saves.Count; index++)
        {

            //*! Blank Player Name
            if (Player_Saves[index].Name == "")
            {
                Player_Saves[index].Name = "Player_" + (index + 1);

                for (int inner_index = 0; inner_index < 15; inner_index++)
                {
                    Player_Saves[index].Level_Count.Add(new Level_Data());
                }
            }


            //*! Save each player
            Player_Saves[index].Save("./Player Save Files/player_" + (index + 1) + ".xml");
        }
    }


    /// <summary>
    /// Load in all the save files and adding them to the Player_Saves List
    /// </summary>
    public void Load_All_Players()
    {
        //*! Get some information on the directory 
        DirectoryInfo info = new DirectoryInfo("./Player Save Files/");

        //*! Get the count of files in that directory
        List<FileInfo> save_files = new List<FileInfo>();

        save_files.AddRange(info.GetFiles());

        //*! No Save File Count
        if (save_files.Count == 0)
        {
            Debug.LogError("NO SAVE FILES IN THE DIRECTORY");
            //*! Don't load the save files.
            return;
        }

        //*! Add null objects to the player saves list of the count of files in the saves directory
        for (int index = 0; index < save_files.Count; index++)
        {



            Debug.Log("INDEX : " + index + " : " + save_files[index].Name);



            Player_Saves.Add(null);
        }

        //*! Load All Players
        for (int index = 0; index < save_files.Count; index++)
        {
            Player_Saves[index] = Player_Save.Load("./Player Save Files/" + save_files[index].Name);
        }




        //*! Remove any null objects in the list
        while (Player_Saves.Remove(null))
        {
            Debug.LogWarning("Removed a null object");
        };

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

            if (player_id <= 9)
            {
                Player_Saves[player_id].Name = "Player_0" + (player_id + 1);
            }
            else
            {
                Player_Saves[player_id].Name = "Player_" + (player_id + 1);
            }


            for (int inner_index = 0; inner_index < 15; inner_index++)
            {
                Player_Saves[player_id].Level_Count.Add(new Level_Data());
            }
        }

        if (player_id <= 9)
        {
            //*! Save each player
            Player_Saves[player_id].Save("./Player Save Files/Player_0" + (player_id + 1) + ".xml");
        }
        else
        {
            //*! Save each player
            Player_Saves[player_id].Save("./Player Save Files/Player_" + (player_id + 1) + ".xml");
        }
    }

}

#endregion


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

    //*! Star Count
    [XmlArray("Star_Count"), XmlArrayItem("Star_Count")]
    public bool[] star_count = new bool[3];

}



[System.Serializable]
public class Player_Save
{
    //*! Name of player
    public string Name = "";

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

            //*! Was empty, puting in three stars
            if (level.star_count.Length == 0)
            {
                level.star_count = new bool[3];
            }

            //*! Increase the level count number
            count++;
        }
    }

    //*! Player Save 
    public void Save(string path)
    {
        //*! Check Against the level Defaults
        Check_Level_Defaults();

        //*! Create the XML Serializer
        XmlSerializer serializer = new XmlSerializer(typeof(Player_Save));
        //*! Create a file stream and write file
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            //*! Write the XML file
            serializer.Serialize(stream, this);
        }
    }

    //*! Player Load 
    public static Player_Save Load(string path)
    {
        //*! Create the XML Serializer
        XmlSerializer serializer = new XmlSerializer(typeof(Player_Save));
        //*! Create a file stream and open file
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            //*! Return the file information casted as a Player Save Class
            return serializer.Deserialize(stream) as Player_Save;
        }
    }
}