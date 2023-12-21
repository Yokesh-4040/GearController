using UnityEngine;

public class AngleBasedRotation : MonoBehaviour
{
    public float rotationSpeed = 5f; // Adjust the rotation speed as needed
    public float minAngle = 45f; // Minimum angle for rotation to start
    public float maxAngle = 135f; // Maximum angle for rotation to stop
    
    public Transform target;

    void FixedUpdate()
    {
        // Assuming you want to rotate around the Y-axis
        float currentAngle = transform.eulerAngles.y;
        // Check if the current angle is between the specified range
        if (currentAngle > minAngle && currentAngle < maxAngle)
        {
            if (target.eulerAngles.y <= 180)
            {
                // Rotate the object around its up axis (Y-axis)
                target.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (target.localRotation.y > 0)
                target.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
    }
}