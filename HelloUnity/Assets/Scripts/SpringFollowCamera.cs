using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpringFollowCamera : MonoBehaviour
{
	[Header("Parameters")]
	[SerializeField]
	private Transform target;
	[SerializeField]
	private float hDist = 0.0f;
	[SerializeField]
	private float vDist = 0.0f;
	[SerializeField]
	private float dampConstant = 1.0f;
	[SerializeField]
	private float springConstant = 1.0f;

    transparencyToggle tToggle;

    private Transform tr;
	private Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
	private Vector3 actualPosition;

	// Start is called before the first frame update
	void Start()
	{
		tr = GetComponent<Transform>();
		actualPosition = tr.position;
		tToggle = GetComponent<transparencyToggle>();
	}

	// Update is called once per frame
	void LateUpdate()
	{
		// Camera position is offset from the target position
		Vector3 idealEye = target.position - target.forward * hDist + target.up * vDist;

		// The direction the camera should point is from the target to the
		Vector3 cameraForward = target.position - tr.position;

		// Compute the acceleration of the spring, and then integrate
		Vector3 displacement = tr.position - idealEye;
		Vector3 springAccel = (-springConstant * displacement) - (dampConstant * velocity);

		// Update the camera's velocity based on the spring acceleration
		velocity += springAccel * Time.deltaTime;
		actualPosition += velocity * Time.deltaTime;

		// Set the camera's position and rotation with the new values
		// This code assumes that this code runs in a script attached to the camera
		tr.position = actualPosition;
		tr.rotation = Quaternion.LookRotation(cameraForward);

		tToggle.seeThroughWalls(actualPosition, cameraForward);
	}
}
