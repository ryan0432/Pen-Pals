//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for Assigning [Edge Type] to [Pencil Case]
//*!              [Edge Type] data.
//*!              This class in an experimental class to test using
//*!              editor with MonoBehavior.
//*!
//*! Last edit  : 08/09/2018
//*!--------------------------------------------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BL_Handle : MonoBehaviour
{
    public BL_Handle_Edge_Type edgeType;
}


//*!----------------------------!*//
//*!    Custom Enum Classes
//*!----------------------------!*//

#region [Handle_Edge_Type] Enum class
public enum BL_Handle_Edge_Type
{
    NONE = 0,
    HighLighter_Red = 2,
}
#endregion
