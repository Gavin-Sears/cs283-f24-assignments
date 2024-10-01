using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private GameObject camGO;
    private Camera mainCam;
    private Transform camTR;


    // Start is called before the first frame update
    void Start()
    {
        camTR = GetComponent<Transform>().GetChild(0);
        camGO = camTR.gameObject;
        mainCam = camGO.GetComponent<Camera>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
