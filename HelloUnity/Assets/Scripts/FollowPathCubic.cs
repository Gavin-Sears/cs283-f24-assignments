using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Stephen Gavin Sears
 * Tuesday, October 8
 * 
 * Script that allows a tranform to move
 * along a Cubic path described by 
 * an array of transforms. Pressing
 * space will allow movement one way,
 * and pressing space again will allow 
 * the transform to go back the way it
 * came.
 */

public class FollowPathCubic : MonoBehaviour
{
    [Header("Destinations and Timing")]
    [SerializeField]
    private Transform[] PathPoints;

    [SerializeField]
    private float duration;
    private float segment;

    [Header("Curve Parameters")]
    [SerializeField]
    private bool DeCasteljau = false;
    [SerializeField]
    private bool smoothB1 = false;
    [SerializeField]
    private bool smoothB2 = false;

    private Transform tr;
    private bool inProgress = false;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();

        // should be four transforms per curve
        if (PathPoints.Length != 0)
            segment = 1.0f / (PathPoints.Length / 4.0f);
        else
            segment = 1.0f;

        if (smoothB1 || smoothB2)
        {
            for (int i = 0; i < PathPoints.Length; i = i + 4)
            {
                Transform p0 = PathPoints[i];
                Transform p1 = PathPoints[i + 1];
                Transform p2 = PathPoints[i + 2];
                Transform p3 = PathPoints[i + 3];

                // recalculating control points for smooth curve
                // b1 = b0 + (1 / 6) * (b1 - b0)
                if (smoothB1)
                    p1.position = p0.position + (1.0f / 6.0f) * (p1.position - p0.position);

                // b2 = b3 - (1 / 6) * (b3 - b0)
                if (smoothB2)
                    p2.position = p3.position + (1.0f / 6.0f) * (p3.position - p0.position);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !inProgress)
        {
            StartCoroutine(CubicPath());
            inProgress = true;
        }
    }

    IEnumerator CubicPath()
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            float u = (timer / duration) / segment;

            int uWhole = (int)Math.Floor((decimal)u);

            // progress within current segment
            float segProgress = u - uWhole;

            // start, end, and two control points for curve
            Transform start;
            Transform p1;
            Transform p2;
            Transform end;

            // ideally is the only branch that gets executed
            if (uWhole >= 0 && uWhole < PathPoints.Length)
            {
                start = PathPoints[uWhole];
                p1 = PathPoints[uWhole + 1];
                p2 = PathPoints[uWhole + 1];
                end = PathPoints[uWhole + 3];
            }
            // potential edge case
            // used when float rounds over array length
            else if (uWhole > PathPoints.Length)
            {
                start = PathPoints[PathPoints.Length - 4];
                p1 = PathPoints[PathPoints.Length - 3];
                p2 = PathPoints[PathPoints.Length - 2];
                end = PathPoints[PathPoints.Length - 1];
            }
            // potential edge case
            // used when float rounds below array length
            else if (uWhole < 0)
            {
                start = PathPoints[0];
                p1 = PathPoints[1];
                p2 = PathPoints[2];
                end = PathPoints[3];
            }
            // in case we somehow don't use the default
            else
            {
                start = tr;
                p1 = tr;
                p2 = tr;
                end = tr;
            }

            // saving position to calculate lookDirection
            Vector3 lookDir = tr.position;

            if (DeCasteljau)
            {
                // recursively applies DeCasteljau, updates using reference to transform
                DCJ(ref tr, new Transform[] {start, p1, p2, end}, segProgress);
            }
            else
            {
                // update position and rotation based on segment progress
                // segProgress = t, start = b0, p1 = b1, p2 = b2, end = b3
                // p(t) = (1 - t)^3 * b0 + 3t(1 - t)^2 * b1 + 3t^2 * (1 - t)b2 + t^3 * b3
                tr.position =
                    ((float)Math.Pow((1.0f - segProgress), 3) * start.position) +
                    (3.0f * segProgress * (float)Math.Pow((1.0f - segProgress), 2) * p1.position) +
                    (3 * (float)Math.Pow(segProgress, 2) * (1.0f - segProgress) * p2.position) +
                    ((float)Math.Pow(segProgress, 3) * end.position);
            }


            tr.LookAt(
                2.0f * tr.position - lookDir,
                tr.up);

            yield return null;
        }

        inProgress = false;
        // reverses so we can go back to beginning
        Array.Reverse(PathPoints);
        yield break;
    }

    // applies DeCasteljau on Transform TR based on
    // spline defined by points and time t
    void DCJ(ref Transform TR, Transform[] controls, float t)
    {
        Vector3[] points = new Vector3[controls.Length];
        for (int i = 0; i < controls.Length; ++i)
            points[i] = controls[i].position;

        TR.position = DeCasteLerp(points, t)[0];
    }

    // Recursive function linearally interpolating between
    // n points based on DeCasteljau algorithm.
    // returns Vector3[] only containing result of
    // DeCasteljau
    Vector3[] DeCasteLerp(Vector3[] points, float t)
    {
        // base case, if we only have one point left
        if (points.Length == 1)
            return points;
        // in case we get unexpected behavior
        else if (points.Length < 1)
        {
            Debug.Log("Error occured - DeCasteljau resulted in no points");
            return new Vector3[] { new Vector3(0.0f, 0.0f, 0.0f) };
        }

        Vector3[] layer = new Vector3[points.Length - 1];
        for (int i = 0; i < layer.Length; ++i)
        {
            layer[i] = Vector3.Lerp(points[i], points[i + 1], t);
        }

        return DeCasteLerp(layer , t);
    }
}
