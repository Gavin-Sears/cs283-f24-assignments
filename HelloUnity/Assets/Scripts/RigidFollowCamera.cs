using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using UnityStandardAssets.Effects;
using System;

public class RigidFollowCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float hDist = 0.0f;
    [SerializeField]
    private float vDist = 0.0f;

    transparencyToggle tToggle;

    private Transform tr;

	// Start is called before the first frame update
	void Start()
    {
        tr = GetComponent<Transform>();
        tToggle = GetComponent<transparencyToggle>();
	}

	// Update is called once per frame
	void LateUpdate()
    {
		// Camera position is offset from the target position
		Vector3 eye = target.position - target.forward * hDist + target.up * vDist;

		// The direction the camera should point is from the target to the camera position
		Vector3 cameraForward = target.position - eye;

        tToggle.seeThroughWalls(eye, cameraForward);

		// Set the camera's position and rotation with the new values
		// This code assumes that this code runs in a script attached to the camera
		tr.position = eye;
        tr.rotation = Quaternion.LookRotation(cameraForward);
    }
}
