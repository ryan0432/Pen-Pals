//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for Assigning [Edge Type] to [Pencil Case]
//*!              [Edge Type] data.
//*!              This class in an experimental class to test using
//*!              editor with MonoBehavior.
//*!
//*! Last edit  : 12/09/2018
//*!--------------------------------------------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BL_Node_Handle : MonoBehaviour
{
    public BL_Node_Handle_Type nodeType;
}

//*!----------------------------!*//
//*!    Custom Enum Classes
//*!----------------------------!*//

#region [BL_Node_Type] Enum class
public enum BL_Node_Handle_Type
{
    NONE = 0,
    Block_Blue_Goal = 1,
}
#endregion
