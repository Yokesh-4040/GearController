using UnityEngine;

namespace fourtyfourty.gearController
{
    public class GrabThis : MonoBehaviour 
    {
        private Transform _objectToRotate; // Assign the object you want to rotate in the Inspector

        [SerializeField] private Transform handleDefaultPos; // max sure the pivot is correct for this

        [SerializeField]private float rotationSpeed = -20.0f;

        private Vector3 _previousPosition;

        public GearController _gearController;

        private void Start()
        {
            if (handleDefaultPos == null)
            {
                Debug.LogError("This cannot be empty please assign");
                enabled = false;
            }

            if (_objectToRotate == null)
            {
                _objectToRotate = transform.parent.transform;
            }

            _gearController = _objectToRotate.GetComponent<GearController>();
            _previousPosition = transform.position;
        }

        private void Update()
        {
            if (!_gearController.isGrabbed)
            {
                var transform1 = transform;
                transform1.position = handleDefaultPos.position;
                _previousPosition = transform1.position;
                return;
            }


            Vector3 distanceMoved = transform.position - _previousPosition;

            // Check if the object is moving on the X-axis
            if (Mathf.Abs(distanceMoved.x) > 0.001f)
            {
                float rotationAngleX = distanceMoved.x * rotationSpeed;
                _objectToRotate.Rotate(Vector3.forward, rotationAngleX);
            }

            // Check if the object is moving on the Z-axis
            if (Mathf.Abs(distanceMoved.z) > 0.001f)
            {
                float rotationAngleZ = distanceMoved.z * rotationSpeed;
                _objectToRotate.Rotate(Vector3.left, rotationAngleZ);
            }

            // Update the previous position for the next frame
            _previousPosition = transform.position;
        }
    }
}