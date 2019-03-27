﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float turnSpeed = 4.0f;
    public Transform player;
    public Vector3 offset;

    Vector3 internalOffset;
    
    // Use this for initialization
	void Start ()
    {
        internalOffset = new Vector3(player.position.x, player.position.y + offset.y, player.position.z + offset.z);
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        internalOffset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * internalOffset;
        transform.position = player.position + internalOffset;
        transform.LookAt(player.position);
	}
}
