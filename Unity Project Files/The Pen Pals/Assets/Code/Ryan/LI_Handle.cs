//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for Assigning [Edge Type] to [Pencil Case]
//*!              [Edge Type] data.
//*!              This class in an experimental class to test using
//*!              editor with MonoBehavior.
//*!
//*! Last edit  : 09/09/2018
//*!--------------------------------------------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LI_Handle : MonoBehaviour
{
    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    public LI_Handle_Edge_Type edgeType;
    [HideInInspector]
    public Boarder_Type boarderType;
}


//*!----------------------------!*//
//*!    Custom Enum Classes
//*!----------------------------!*//

#region [Handle_Edge_Type] Enum class
public enum LI_Handle_Edge_Type
{
    NONE = 0,
    Black_Pen = 1,
    Boarder = 6
}
#endregion