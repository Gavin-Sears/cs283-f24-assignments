using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using UnityStandardAssets.Vehicles.Aeroplane;

public class PlayerControls : MonoBehaviour
{
	[Header("Movement Parameters")]
	[SerializeField]
	private float moveSpeed = 1.0f;
	[SerializeField]
	private float turnSpeed = 1.0f;

    private Transform tr;
	private Animator animator;
	private float yRotation = 0.0f;

	// Start is called before the first frame update
	void Start()
    {
        tr = GetComponent<Transform>();
		animator = GetComponent<Animator>();
		Debug.Log(animator);
	}

	// Deals with non-vertical movement
    void handleMovement()
    {
		// mouse input
		float inputX = Input.GetAxis("Horizontal");
		Debug.Log(inputX);

		// apply rotation
		if (Mathf.Abs(inputX) > 0.0f)
		{
			yRotation += inputX * turnSpeed * Time.deltaTime;

			tr.rotation = Quaternion.Euler(new Vector3(0.0f, yRotation, 0.0f));
		}
		else
		{
			yRotation = tr.eulerAngles.y;
		}

		// keyboard input
		float inputZ = Input.GetAxis("Vertical");
		Debug.Log(inputZ);

		// apply movement
		if (Mathf.Abs(inputZ) > 0.0f)
		{
			animator.SetBool("Walking", true);
			applyMovement(inputZ);
		}
		else
		{
			animator.SetBool("Walking", false);
		}
	}

	// applies non-vertical movement
	void applyMovement(float z)
	{
		tr.position += Time.deltaTime * tr.forward * z * moveSpeed;
	}

	// Update is called once per frame
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.W))
		{
			Debug.Log("Update is running");
		}
		handleMovement();
	}
}
