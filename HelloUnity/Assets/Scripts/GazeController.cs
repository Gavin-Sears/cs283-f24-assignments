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
    void Update()
    {
        gazeTracking();
    }

    void gazeTracking()
    {
        for (int i = 0; i < followJoints.Length; ++i)
        {
            //The formula on the website is totally wrong, this will need to be updated
            Transform tr = followJoints[i];
            // r - limb we are rotating
            Vector3 r = tr.position;
            // e - target of rotation - difference from player
            Vector3 e = target.position;

            Vector3 cross = Vector3.Cross(r, e);

            // gonna assume r and e are rotations of transforms
            // phi = atan2(||r x e||, (r dot r) + (r dot e))
            float theta = Mathf.Atan2(cross.magnitude, Vector3.Dot(r, r) + Vector3.Dot(r, e));
            // noramlize the cross product
            Vector3 axis = cross.normalized;

            // Draw lines with different colors for each tracking transform
            float ratio = (i + 1) / followJoints.Length;
            Debug.DrawLine(
                tr.position,
                target.position,
                new Color(
                    1.0f - ratio,
                    ratio,
                    1.0f - ratio
                    )
                );

            tr.Rotate(axis, theta);
            //Quaternion rot = findQuaternion(theta, axis);
            //tr.rotation = rot;
        }
    }

    // finds quaternion from an axis of rotation and a theta value
    Quaternion findQuaternion(float theta, Vector3 axis)
    {
        float halfAngle = theta / 2.0f;
        float sinHA = Mathf.Sin(halfAngle);
        return new Quaternion(sinHA * axis.x, sinHA * axis.y, sinHA * axis.z, Mathf.Cos(halfAngle));
    }
}
