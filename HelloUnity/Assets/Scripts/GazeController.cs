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
            // r
            Vector3 r = target.position;
            // e
            Vector3 e = tr.position;

            Vector3 cross = Vector3.Cross(r, e);

            // gonna assume r and e are rotations of transforms
            // phi = atan2(r x e, (r dot r) + (r dot e))
            float theta = Mathf.Atan2(cross.magnitude, Vector3.Dot(r, r) + Vector3.Dot(r, e));
            Vector3 axis = cross / cross.magnitude;

            // Draw lines with different colors for each tracking transform
            Debug.DrawLine(
                tr.position,
                target.position,
                new Color(
                    1.0f - ((i + 1) / followJoints.Length),
                    ((i + 1) / followJoints.Length),
                    1.0f - ((i + 1) / followJoints.Length)));
            Quaternion rot = findQuaternion(theta, axis);
            tr.rotation = rot;
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
