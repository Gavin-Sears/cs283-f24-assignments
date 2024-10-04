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

	LayerMask obstacles;

    private Transform tr;

    [SerializeField]
    Material baseTransparent;

	// hold original material for object
	Dictionary<Renderer, Material> buffer;

    // Uses camera locations to make objects transparent based on layerMask
	void seeThroughWalls(Vector3 eye, Vector3 cameraForward)
	{
        RaycastHit[] hits;

        hits = Physics.RaycastAll(eye, cameraForward.normalized, cameraForward.magnitude, obstacles);
        Renderer[] currentRenderers = new Renderer[hits.Length];

        if (!(hits == null || hits.Length == 0))
        {
            for (int i = 0; i < hits.Length; ++i)
            {
                Renderer rend = hits[i].transform.gameObject.GetComponent<Renderer>();

                // adding objects we want to stay transparent to array
                currentRenderers[i] = rend;

                if (!buffer.ContainsKey(rend))
                {
                    // uses reference to original material
                    Material origMat = rend.sharedMaterial;
                    buffer.Add(rend, origMat);

                    // creates copy of material, sets parameters, and assigns
                    Material transparent = Instantiate(baseTransparent);
                    Texture texture = origMat.mainTexture;

                    // set color and texture to same as original
                    transparent.SetTexture("_BaseMap", texture);
                    if (origMat.HasProperty("_Color"))
                    {
                        // keeps alpha, but changes other colors 
                        transparent.color = new Color(origMat.color.r, origMat.color.g, origMat.color.g, transparent.color.a);
                    }

                    rend.material = transparent;
                }
            }
        }
        if (buffer.Count > 0)
        {
            // resets objects and removes entries from buffer
            foreach (Renderer r in new List<Renderer>(buffer.Keys))
            {
                // keep index of renderers we want to keep
                // allows some objects to return to normal, while others stay transparent
                int index = Array.IndexOf(currentRenderers, r);
                if (index == -1)
                {
                    // set orignal material to renderer
                    r.material = buffer[r];
                    buffer.Remove(r);
                }
            }
        }
    }

	// Start is called before the first frame update
	void Start()
    {
        Physics.queriesHitBackfaces = true;
        tr = GetComponent<Transform>();
		obstacles = LayerMask.GetMask("Obstacle");
		buffer = new Dictionary<Renderer, Material>();
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
