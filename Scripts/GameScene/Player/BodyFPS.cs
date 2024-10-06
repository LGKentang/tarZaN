using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyFPS : MonoBehaviour
{
    public Camera playerCamera;
    private Transform playerBody;

    public float rotationSpeed = 30.0f; 

    private void Start()
    {
        playerBody = transform;
    }

    private void Update()
    {
        Quaternion cameraRotation = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y, 0f);

        playerBody.rotation = Quaternion.Lerp(playerBody.rotation, cameraRotation, rotationSpeed * Time.deltaTime);
    }
}
