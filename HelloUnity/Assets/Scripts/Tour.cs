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
    public Transform tr;
    public bool linear;
}

public class Tour : MonoBehaviour
{
    private Transform tr;
    private Vector3 origPos;
    private Quaternion origRot;

    [Header("Points of Interest")]
    [SerializeField]
    private POI[] Points;

    private int curPOINum = -1;
    private POI curPOI;
    private bool curIsLinear;
    private Transform curTr;

    [Header("Controls")]
    [SerializeField]
    private float speed = 1.0f;

    private bool isMoving = false;

    private float elapsedTime = 0.0f;

    // Interpolate camera position to point position based on time
    void moveToPOI()
    {
        // uses time to lerp/slerp. Speed simply changes timescale
        float alpha = elapsedTime * speed;

        if (curIsLinear)
        {
            //lerp
            tr.position = Vector3.Lerp(origPos, curTr.position, alpha);
            tr.rotation = Quaternion.Lerp(origRot, curTr.rotation, alpha);
        }
        else
        {
            //slerp
            tr.position = Vector3.Slerp(origPos, curTr.position, alpha);
            tr.rotation = Quaternion.Slerp(origRot, curTr.rotation, alpha);

        }
        // reset if ended
        if (alpha >= 1.0f)
        {
            isMoving = false;
        }
    }

    // executes the various functions needed to begin moving the camera
    void beginMovement()
    {
        incrementPOI();
        isMoving = true;
        origPos = tr.position;
        origRot = tr.rotation;
        recordPOI();
    }

    // increments current point of interest based on array
    void incrementPOI()
    {
        curPOINum = (curPOINum + 1) % Points.Length;
    }

    // simply records info for current point of interest
    void recordPOI()
    {
        curPOI = Points[curPOINum];
        curTr = curPOI.tr;
        curIsLinear = curPOI.linear;
    }

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        origPos = tr.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) && !isMoving)
        {
            beginMovement();
        }

        if (isMoving)
        {
            elapsedTime += Time.deltaTime;
            moveToPOI();
        }
        else
        {
            elapsedTime = 0.0f;
        }
    }
}
