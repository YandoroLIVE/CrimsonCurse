using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // The player character or object to follow
    public float smoothSpeed = 0.125f;  // Smooth speed of camera movement
    public Vector3 offset;  // Offset to maintain distance between the camera and player

    void LateUpdate()
    {
        // Calculate desired camera position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply the smoothed position to the camera
        transform.position = smoothedPosition;
    }
}
