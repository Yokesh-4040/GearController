using UnityEngine;

public class SeatTurnController : MonoBehaviour
{

    public Transform targetObject;
    public float minLocalRotationAngle = 45f;
    public float maxLocalRotationAngle = 135f;
    public float timeThreshold = 5f;

    private float timeWithinRange = 0f;
    private bool hasRotated = false;

    private void Update()
    {
        // Get the current local rotation angle around the Z-axis
        float currentLocalRotation = transform.localRotation.eulerAngles.y;

        // Check if the current local rotation angle is between the specified range
        if (currentLocalRotation > minLocalRotationAngle && currentLocalRotation < maxLocalRotationAngle)
        {
            // Increment the timer
            timeWithinRange += Time.deltaTime;

            // Check if the user has been within the range for the specified duration
            if (timeWithinRange >= timeThreshold && !hasRotated)
            {
                // Rotate the target object locally by 180 degrees instantly
                targetObject.localRotation *= Quaternion.Euler(0, 0, 180);
                hasRotated = true;
            }
        }
        else
        {
            // Reset the timer and rotation status if the user is not within the specified range
            timeWithinRange = 0f;
            hasRotated = false;
        }
    }
}

