//*!----------------------------!*//
//*! Programmer: Alex Scicluna || Ryan Chung
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;


public class Line_Renderer_Container : MonoBehaviour
{

    //*!----------------------------!*//
    //*!    Private Variables
    //*!----------------------------!*//
    #region Private Variables

    //*! Line component
    [SerializeField]
    LineRenderer line_segment;

    #endregion


    //*!----------------------------!*//
    //*!    Public Variables
    //*!----------------------------!*//
    #region Public Variables

    ////*! Start Point
    //public GameObject start_point;
    ////*! Mid-Pivot Point
    //public GameObject mid_pivot_point;
    ////*! End Point
    //public GameObject end_point;



    //*! Array of points
    public Transform[] points;

    #endregion


    //*!----------------------------!*//
    //*!    Unity Functions
    //*!----------------------------!*//
    #region Unity Functions
    private void Start()
    {
        line_segment.positionCount = points.Length;
    }

    private void Update()
    {

        Update_Line_Segments();

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

    private void Update_Line_Segments()
    {
        for (int index = 0; index < points.Length; index++)
        {
            line_segment.SetPosition(index, points[index].position);
        }
    }

    #endregion



    //*! Protected Access
    #region Protected Functions

    #endregion



}