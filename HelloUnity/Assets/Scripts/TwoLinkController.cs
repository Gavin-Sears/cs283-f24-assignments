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

            float theta = Mathf.Acos(-term);

            Debug.DrawLine(middleHinge.position, (middleHinge.position + (middleHinge.right * 2.0f)), new Color(0.0f, 0.0f, 1.0f));
            //middleHinge.Rotate(new Vector3(1.0f, 0.0f, 0.0f), theta);

            middleHinge.localRotation = Quaternion.AngleAxis(theta * 180 / Mathf.PI, Vector3.right);

            // rotate balljoint based on radius direction to land on end effector
            GazeController.IKRotate(ref ballJoint, (endEffector.position - ballJoint.position), target);

            // check against length for debug
            Debug.Log("origin to target");
            Debug.Log(r);
            Debug.Log("origin to end effector");
            Debug.Log((endEffector.position - ballJoint.position).magnitude);
        }
    }
}
