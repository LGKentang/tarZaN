using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Camera camera;
    public float rotationSpeed;

    void Update()
    {
        camera.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
