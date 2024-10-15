using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeController : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    Transform[] followJoints;

    // Update is called once per frame
    void LateUpdate()
    {
        gazeTracking();
    }

    void gazeTracking()
    {
        for (int i = 0; i < followJoints.Length; ++i)
        {
            Transform tr = followJoints[i];
            IKRotate(ref tr, tr.forward, target);
        }
    }

    public static void IKRotate(ref Transform tr, Vector3 trForward, Transform target)
    {
        // e - target of rotation - difference from player (target - transform)
        Vector3 e = (target.position - tr.position);
        // r - limb we are rotating forward vector of transform
        Vector3 r = trForward;

        Vector3 cross = Vector3.Cross(r, e);

        // target is red
        Debug.DrawLine(tr.position, target.position, new Color(1.0f, 0.0f, 0.0f));
        // axis is blue
        Debug.DrawLine(tr.position, tr.position + cross, new Color(0.0f, 0.0f, 1.0f));

        // phi = atan2(||r x e||, (r dot r) + (r dot e))
        float theta = Mathf.Atan2(cross.magnitude, Mathf.Abs(Vector3.Dot(r, r) + Vector3.Dot(r, e)));
        // noramlize the cross product
        Vector3 axis = cross.normalized;

        tr.Rotate(axis, theta);
        // in order to fix backwards eye glitch, will need to uses cases like this
        // (could need to be for whole equation, though!)
        /*
        if (Vector3.Dot(r, r) + Vector3.Dot(r, e) > 0.0f)
            tr.Rotate(axis, theta);
        else
            tr.Rotate(axis, theta);
        */
    }

    // finds quaternion from an axis of rotation and a theta value
    // turns out this was not needed, but I'll keep it anyways
    public static Quaternion findQuaternion(float theta, Vector3 axis)
    {
        float halfAngle = theta / 2.0f;
        float sinHA = Mathf.Sin(halfAngle);
        return new Quaternion(sinHA * axis.x, sinHA * axis.y, sinHA * axis.z, Mathf.Cos(halfAngle));
    }
}