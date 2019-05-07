using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraMovement : MonoBehaviour {

    public Camera playerCam;
    public float speed = 1;
    public float zoomSpeed = 1;
    private float vertical;
    private float horizontal;
    private float zoom;
	
	// Update is called once per frame
	void Update () {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        playerCam.transform.position = new Vector3(playerCam.transform.position.x + horizontal * speed, playerCam.transform.position.y + vertical * speed, -10);

        zoom = Input.mouseScrollDelta.y * zoomSpeed;
        if (2 < playerCam.orthographicSize && playerCam.orthographicSize < 10)
            playerCam.orthographicSize -= zoom;
        else if (playerCam.orthographicSize <= 2)
        {
            if (zoom < 0)
                playerCam.orthographicSize -= zoom;
        }
        else if (playerCam.orthographicSize >= 10)
        {
            if (zoom > 0)
                playerCam.orthographicSize -= zoom;
        }
	}
}
