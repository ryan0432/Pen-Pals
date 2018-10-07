//*!--------------------------------------------------------------!*//
//*! Programmer : Ryan Chung
//*!
//*! Description: This is the [Camera Control] class.
//*!              This class in an experimental class to test using
//*!              camera behaviors the suits the game.
//*!              
//*! Last edit  : 01/10/2018
//*!--------------------------------------------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Control : MonoBehaviour
{
    public Camera cam;
    public Vector3[] frustumCorners;
    public GameObject[] players;
    public Rect[] triggerAreas;
    public Texture recTex;

    // Use this for initialization
    void Start ()
    {
        cam = GetComponent<Camera>();
        frustumCorners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
        triggerAreas = new Rect[4];
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update ()
    {
        Print_Corners();
        Get_Trigger_Areas();
    }

    private void Print_Corners()
    {
        for (int i = 0; i < frustumCorners.Length; ++i)
        {
            Debug.Log("Corner " + i + ": " + frustumCorners[i]);
        }
    }

    private void Get_Trigger_Areas()
    {
        Vector2 camPos = transform.position;
        
        //triggerAreas[0] = new Rect(camPos + new Vector2(frustumCorners[0].x * 0.2f, 0), new Vector2(2.0f, frustumCorners[1].y * 2));
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawGUITexture(triggerAreas[0], recTex);
    }
}
