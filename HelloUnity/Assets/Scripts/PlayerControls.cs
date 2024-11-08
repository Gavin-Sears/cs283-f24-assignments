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

	// Character controller
	private CharacterController controller;

	private Transform tr;
	private PlayerMotionController PMC;
	private float yRotation = 0.0f;

	// layermask info
	private LayerMask obstacleMask;

	// jump controls. Note that all values are absolute
	[Header("Jump Parameters")]
	[SerializeField]
	private float gravity;
	[SerializeField]
	private float maxRiseSpeed;
	[SerializeField]
	private float maxFallSpeed;
	[SerializeField]
	private float jumpAcceleration;
	[SerializeField]
	private float coyoteTime;
	[SerializeField]
	private float groundedPadding = 0.0f;

	private float vAcceleration;
	private bool spacePressed = false;

	// moment in time when we were last grounded
	private float timeGrounded = 0.0f;
	// time since we were last grounded
	private float timeSinceGrounded = 0.0f;
	// so we can't jump twice due to coyoteTime
	private bool hasJumped = false;

	// Start is called before the first frame update
	void Start()
	{
		tr = GetComponent<Transform>();
		PMC = GetComponent<PlayerMotionController>();
		controller = GetComponent<CharacterController>();
        obstacleMask = LayerMask.GetMask("Obstacle");
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
        handleJump();
    }

    // Deals with non-vertical movement
    private void handleMovement()
	{
		// mouse input
		float inputX = Input.GetAxis("Horizontal");

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

		// apply movement
		if (Mathf.Abs(inputZ) > 0.0f)
		{
			PMC.walk();
			applyMovement(inputZ);
		}
		else
		{
			PMC.idle();
		}
	}

	private void handleJump()
	{
		// updating variables for coyoteTime
		updateGroundVars();

		// if we press space, and either:
		// - we are grounded
		// - time since we were last grounded is less than "coyoteTime"
		// AND finally is we have not jumped already
		// added nested if simply to collect space pressed
		if (Input.GetKeyDown("space"))
		{
			spacePressed = true;
            if ((isGrounded() || coyoteTime > timeSinceGrounded) && !hasJumped)
			{
				// we can jump
				vAcceleration = jumpAcceleration;
				hasJumped = true;
			}
		}
		// just so we can do mario jump
		else if (Input.GetKeyUp("space"))
			spacePressed = false;

		applyVertical();
	}

	// applies non-vertical movement
	private void applyMovement(float z)
	{
		Vector3 velocity = tr.forward * z * moveSpeed;

		// set position method
		// tr.position += velocity * Time.deltaTime;

		// character controller method
		controller.Move(velocity * Time.deltaTime);
	}

	private void applyVertical()
    {
        float testVelocity = vAcceleration * Time.deltaTime;
        // gravity is triple (in this case not 1/3)
        // if falling or not pressing space,
        // creating mario effect
        if (testVelocity < 0.000f || !spacePressed)
		{
			vAcceleration -= gravity;
		}
		else
		{
			vAcceleration -= gravity * 0.33f;
        }

		// calculating and clamping vertical velocity
		float velocity = vAcceleration * Time.deltaTime;
		velocity = Mathf.Clamp(velocity, -maxFallSpeed, maxRiseSpeed);

		controller.Move(tr.up * velocity);
	}

	// updates variables for coyoteTime
	private void updateGroundVars()
	{
		if (isGrounded())
		{
			timeGrounded = Time.realtimeSinceStartup;
			if (!hasJumped)
				vAcceleration = 0.0f;
            hasJumped = false;
		}
		timeSinceGrounded = Time.realtimeSinceStartup - timeGrounded;
	}

	// improved grounded function to not have jump inputs get randomly eaten
	// uses a raycast so that small bounces don't count as not being grounded
	// alongside old grounded method which gets slope collisions
	private bool isGrounded()
	{
		Bounds bounds = controller.bounds;
		Vector3 rayOrigin = bounds.center;
		bool rayGrounded = Physics.Raycast(rayOrigin,
			Vector3.down, (controller.height / 2.0f) + groundedPadding, obstacleMask);
		/*Debug.DrawRay(rayOrigin, Vector3.down * ((controller.height / 2.0f) + groundedPadding),
			new Color(1.0f, 0.0f, 0.0f));*/

		return rayGrounded || controller.isGrounded;
	}
}
