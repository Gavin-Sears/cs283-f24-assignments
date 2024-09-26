using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * To be attached to main camera
 */

// stuct defining a point of interest
[Serializable]
public class POI
{
    public Transform TR;
    public bool linear;
}

public class Tour : MonoBehaviour
{
    private Transform TR;

    [Header("Points of Interest")]
    [SerializeField]
    private POI[] Points;

    private int curPOINum = 0;
    private POI curPOI;

    [Header("Controls")]
    [SerializeField]
    private float speed = 1.0f;

    private bool isMoving = false;

    private float startTime = 0.0f;

    void moveToPOI()
    {

    }

    // increments current point of interest based on array
    void incrementPOI()
    {
        curPOI = (curPOI + 1) % Points.Length;
    }

    // Start is called before the first frame update
    void Start()
    {
        TR = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            incrementPOI();
            startTime = Time.realtimeSinceStartup;
            isMoving = true;
            curPOI = Points[curPOINum];
        }

        if (isMoving)
        {
        }
    }
}
