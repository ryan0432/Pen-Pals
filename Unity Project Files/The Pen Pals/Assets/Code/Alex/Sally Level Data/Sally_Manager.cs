//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//

//*! Using namespaces
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//*! Initial namespace
using APS.SALLY;

/// <summary>
/// Custom Level Save and Loading
/// </summary>
public class Sally_Manager : MonoBehaviour
{
    private SAL sally = new SAL();
    private Lv_Data level_data_template;

    private string data_path = "";


    public List<Lv_Data> Custom_Level = new List<Lv_Data>();


    private void Awake()
    {
        data_path = Application.dataPath + "/Custom/";
    }


    /// <summary>
    /// Load in all custom levels in the file directory
    /// </summary>
    [ContextMenu("Custom Level Load")]
    public void Load_Custom_Levels()
    {
        data_path = Application.dataPath + "/Custom/";

        //*! Get some information on the directory 
        DirectoryInfo dir_info = new DirectoryInfo(data_path);

        //*! Get the count of files in that directory
        List<FileInfo> c_level_files = new List<FileInfo>();
        //*! Set the range based on the amount of files in the dir
        c_level_files.AddRange(dir_info.GetFiles());

        //*! Fill in the list of null objects
        for (int index = 0; index < c_level_files.Count / 2; index++)
        {
            Custom_Level.Add((Lv_Data)ScriptableObject.CreateInstance(typeof(Lv_Data)));
        }

        //*!Assign the file data into the custom level list
        for (int index = 0; index < c_level_files.Count / 2; index++)
        {
            Custom_Level[index] = sally.Load_JSON(Custom_Level[index], data_path + (index + 1) + "_Custom_Level.json");
            Custom_Level[index].name = (index + 1) + "_Custom_Level";
        }



    }

    [ContextMenu("Custom Level Create")]
    public void Create_Custom_Level()
    {
        data_path = Application.dataPath + "/Custom/";
        level_data_template = (Lv_Data)ScriptableObject.CreateInstance(typeof(Lv_Data));
        sally.Save_JSON(level_data_template, data_path + (Custom_Level.Count + 1) + "_Custom_Level.json", true);
        level_data_template.name = (Custom_Level.Count + 1) + "_Custom_Level";
        Custom_Level.Add(level_data_template);
        level_data_template = null;
    }



    [ContextMenu("Custom Level Save All")]
    public void Save_Custom_List()
    {
        data_path = Application.dataPath + "/Custom/";
        for (int index = 0; index < Custom_Level.Count; index++)
        {
            sally.Save_JSON(Custom_Level[index], data_path + (index + 1) + "_Custom_Level.json", true);
        }
    }




}