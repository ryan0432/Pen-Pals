//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;


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
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.gameObject.GetComponent<Renderer>().material.color = (xml_file_data.Player_Saves[0].Level_Count[0].Unlocked) ? new Color(0, 1, 0) : new Color(1, 0, 0);
        }

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


    #endregion



    //*! Protected Access
    #region Protected Functions

    #endregion


}