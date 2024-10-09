using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Stephen Gavin Sears
 * Tuesday, October 8
 * 
 * Script that allows a tranform to move
 * along a linear path described by 
 * an array of transforms. Pressing
 * space will allow movement one way,
 * and pressing space again will allow 
 * the transform to go back the way it
 * came.
 */

public class FollowPathLinear : MonoBehaviour
{
    [SerializeField]
    private Transform[] PathPoints;

    [SerializeField]
    private float duration;
    private float segment;

    private Transform tr;
    private bool inProgress = false;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();

        if (PathPoints.Length != 0)
            segment = 1.0f / (PathPoints.Length - 1);
        else
            segment = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !inProgress)
        {
            StartCoroutine(DoLerp());
            inProgress = true;
        }
    }

    IEnumerator DoLerp()
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            float u = (timer / duration) / segment;

            int uWhole = (int)Math.Floor((decimal)u);

            // progress within current segment
            float segProgress = u - uWhole;

            Transform start;
            Transform end;

            // ideally is the only branch that gets executed
            if (uWhole >= 0 && uWhole < PathPoints.Length)
            {
                start = PathPoints[uWhole];
                end = PathPoints[uWhole + 1];
            }
            // potential edge case
            // used when float rounds over array length
            else if (uWhole > PathPoints.Length)
            {
                start = PathPoints[PathPoints.Length - 2];
                end = PathPoints[PathPoints.Length - 1];
            }
            // potential edge case
            // used when float rounds below array length
            else if (uWhole < 0)
            {
                start = PathPoints[0];
                end = PathPoints[1];
            }
            // in case we somehow don't use the default
            else
            {
                start = tr;
                end = tr;
            }

            // update position and rotation based on segment progress
            tr.position = Vector3.Lerp(start.position, end.position, segProgress);
            tr.LookAt(end.position, tr.up);

            yield return null;
        }

        inProgress = false;
        // reverses so we can go back to beginning
        Array.Reverse(PathPoints);
        yield break;
    }
}
