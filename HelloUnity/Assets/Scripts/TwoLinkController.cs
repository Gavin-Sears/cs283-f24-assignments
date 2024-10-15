using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoLinkController : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    // parent - middle hinge join
    // grandparent - ball joint
    [SerializeField]
    private Transform endEffector;
    private Transform middleHinge;
    private Transform ballJoint;

    private float l1;
    private float l2;
    private float maxLength;

    // Start is called before the first frame update
    void Start()
    {
        middleHinge = endEffector.parent;
        ballJoint = middleHinge.parent;

        // length 1st limb
        l1 = Mathf.Abs(middleHinge.position.y - ballJoint.position.y);
        // length 2nd limb
        l2 = Mathf.Abs(endEffector.position.y - middleHinge.position.y);
        // max length of arm
        maxLength = l1 + l2;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // find dist from base to target
        float r = (target.position - ballJoint.position).magnitude;

        // making sure we do not go farther than the arm can stretch
        if (r < maxLength)
        {
            // theta = arccos(term)
            float term = (Mathf.Pow(r, 2) - Mathf.Pow(l1, 2) - Mathf.Pow(l2, 2)) /
                (-2 * l1 * l2);

            float theta = Mathf.Acos(term);

            // hinge uses right axis to rotate
            middleHinge.localEulerAngles = new Vector3(theta * 180.0f/Mathf.PI,
                middleHinge.localEulerAngles.y,
                middleHinge.localEulerAngles.z);

            // rotate balljoint based on radius direction to land on end effector
            GazeController.IKRotate(ref ballJoint, ballJoint.up, target);

            // check against length for debug

        }
    }

    public static void TwoLinkRotate(ref Transform tr, Vector3 trForward, Transform target)
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
}
