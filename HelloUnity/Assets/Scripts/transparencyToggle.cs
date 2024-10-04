using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Stephen Gavin Sears
 * Firday, October 4, 2024
 * 
 * Unity component that causes objects between
 * the player and camera to be rendered transparent
 * 
 * To use:
 * 
 * - Add as component to camera object
 * - Set script parameters as instructed
 * 
 * - In follow camera script:
 * 
 * transparencyToggle tToggle;
 * 
 * void Start()
 * {
 *      tToggle = GetComponent<transparencyToggle>();
 * }
 * 
 * void LateUpdate()
 * {
 *      // eye is CURRENT camera location, cameraForward is 
 *      // vector going from camera to player
 *      tToggle.seeThroughWalls(eye, cameraForward);
 * }
 * 
 * NOTE: This script uses unity URP.
 * The only changes you should need to make if you are not using URP 
 * is to alter material property names if things are not working. 
 * Otherwise, this should still work.
 */

public class transparencyToggle : MonoBehaviour
{
    [SerializeField]
    private Material transparencyToggleMat;

    [Header("layers that can become transparent")]
    [SerializeField]
    private string[] transparentLayers;
    private LayerMask obstacles;

    // hold original material for objects
    Dictionary<Renderer, Material> buffer;

    // Start is called before the first frame update
    void Start()
    {
        // necessary for double-sided objects
        Physics.queriesHitBackfaces = true;

        // find layer if available, init buffer
        obstacles = LayerMask.GetMask(transparentLayers);
        buffer = new Dictionary<Renderer, Material>();
    }

    // To be called in Update method of camera controls for best efficiency
    // toggles transparency for items in between camera and player
    public void seeThroughWalls(Vector3 eye, Vector3 cameraForward)
    {
        RaycastHit[] hits;

        hits = Physics.RaycastAll(eye, cameraForward.normalized, cameraForward.magnitude, obstacles);
        Renderer[] currentRenderers = new Renderer[hits.Length];

        if (!(hits == null || hits.Length == 0))
        {
            for (int i = 0; i < hits.Length; ++i)
            {
                Renderer rend = hits[i].transform.gameObject.GetComponent<Renderer>();

                if (rend != null)
                {
                    // adding objects we want to stay transparent to array
                    currentRenderers[i] = rend;

                    if (!buffer.ContainsKey(rend))
                    {
                        // uses reference to original material
                        Material origMat = rend.sharedMaterial;
                        buffer.Add(rend, origMat);

                        // creates copy of material, sets parameters, and assigns
                        Material transparent = Instantiate(transparencyToggleMat);
                        Texture texture = origMat.mainTexture;

                        // IF NOT USING URP, MAY NEED TO CHANGE THESE LINES
                        // set color and texture to same as original
                        // "_MainTex" is potential non-URP alternative
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
}
