using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOverlay : MonoBehaviour
{
    [Header("Camera we want the overlay to affect")]
    [SerializeField]
    private GameObject camera;

    private Camera cam;
    private Transform camTr;

    [Header("Adjustments for overlay on screen")]
	[SerializeField]
	private float scaleX = 1.0f;
	[SerializeField]
	private float scaleY = 1.0f;
    [SerializeField]
    private float adjustX = 0.0f;
    [SerializeField]
    private float adjustY = 0.0f;
	[SerializeField]
	private float adjustZ = 0.0f;

    // Overaly transform
    private Transform tr;

    // Dimensions of camera in pixels
    private float halfHeight;
    private float halfWidth;

    // World size of screen
    private float xSize;
    private float ySize;

    // World coord corners of screen
    private Vector3 topRight;
	private Vector3 topLeft;
	private Vector3 bottomLeft;


	void updateScreenDimensions()
	{
		halfHeight = (float)Screen.height / 2.0f;
		halfWidth = (float)Screen.width / 2.0f;
	}

    void updateScreenWorldSize()
	{
		topRight = cam.ScreenToWorldPoint(new Vector3(halfWidth * 2.0f, 0.0f, (cam.nearClipPlane + 0.01f)));
		topLeft = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, (cam.nearClipPlane + 0.01f)));
		bottomLeft = cam.ScreenToWorldPoint(new Vector3(0.0f, halfHeight * 2.0f, (cam.nearClipPlane + 0.01f)));

        xSize = (topLeft - topRight).magnitude;
        ySize = (topLeft - bottomLeft).magnitude;
	}

	// Start is called before the first frame update
	void Start()
    {
        cam = camera.GetComponent<Camera>();
        camTr = camera.GetComponent<Transform>();
        tr = GetComponent<Transform>();
        tr.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        updateScreenDimensions();
        updateScreenWorldSize();
    }

    // Update is called once per frame
    void Update()
    {
        updateScreenDimensions();
		updateScreenWorldSize();
        tr.localScale = new Vector3(xSize * scaleX, ySize * scaleY, 1.0f);
		tr.position = cam.ScreenToWorldPoint(new Vector3(halfWidth + adjustX, halfHeight + adjustY, (cam.nearClipPlane + 0.01f + adjustZ)));
	}
}
