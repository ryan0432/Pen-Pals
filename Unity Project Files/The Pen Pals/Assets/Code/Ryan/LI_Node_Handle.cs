//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: A class for Assigning [Node Type] to [Pencil Case]
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
public class LI_Node_Handle : MonoBehaviour
{
    public LI_Node_Handle_Type nodeType;
}

//*!----------------------------!*//
//*!    Custom Enum Classes
//*!----------------------------!*//

#region [LI_Node_Type] Enum class
public enum LI_Node_Handle_Type
{
    NONE = 0,
    Line_Red_Goal = 4
}
#endregion
