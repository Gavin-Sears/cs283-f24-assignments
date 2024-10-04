using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.Effects;

public class RigidFollowCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float hDist = 0.0f;
    [SerializeField]
    private float vDist = 0.0f;

	LayerMask obstacles;

    private Transform tr;

	List<Material> buffer;

	// Start is called before the first frame update
	void Start()
	{
		tr = GetComponent<Transform>();
		obstacles = LayerMask.GetMask("Obstacle");
		buffer = new List<Material>(); 
	}

	// Update is called once per frame
	void LateUpdate()
    {
		// Camera position is offset from the target position
		Vector3 eye = target.position - target.forward * hDist + target.up * vDist;

		// The direction the camera should point is from the target to the camera position
		Vector3 cameraForward = target.position - eye;

		RaycastHit[] hits;
		hits = Physics.RaycastAll(eye, cameraForward.normalized, cameraForward.magnitude, obstacles);

		if (!(hits == null || hits.Length == 0))
		{
			foreach (RaycastHit hit in hits)
			{
				Debug.Log(hit.transform);
				var col = hit.transform.gameObject.GetComponent<Renderer>().material.color;
				col.a = 0.0f;
			}
		}

		// Set the camera's position and rotation with the new values
		// This code assumes that this code runs in a script attached to the camera
		tr.position = eye;
        tr.rotation = Quaternion.LookRotation(cameraForward);
    }
}
