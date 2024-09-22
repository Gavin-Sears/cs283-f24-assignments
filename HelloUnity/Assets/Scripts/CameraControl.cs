using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("control parameters")]
    [SerializeField]
    private float rotateSpeed;
	[SerializeField]
	private float speed;

    private Transform tr;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
		Vector3 normMovement = (new Vector3(inputX, 0.0f, inputZ)).normalized;

		float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yRotation += mouseX * rotateSpeed * Time.deltaTime;
        xRotation += -Mathf.Clamp(mouseY, -90f, 90) * rotateSpeed * Time.deltaTime;

        tr.rotation = Quaternion.Euler(new Vector3(xRotation, yRotation, 0.0f));

        tr.position += 
            speed * 
            Time.deltaTime * 
            ((tr.forward * normMovement.z) + (tr.right * normMovement.x));
    }
}
