using UnityEngine;
using UnityEngine.Serialization;

namespace fourtyfourty.gearController
{
    public class GrabThis : MonoBehaviour 
    {
        private Transform _objectToRotate; // Assign the object you want to rotate in the Inspector

        [SerializeField] private Transform handleDefaultPos; // max sure the pivot is correct for this

        private Vector3 _previousPosition;

        public GearController _gearController;

        public float multiplier = -10;

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
            _previousPosition = transform.localPosition;
        }

        private void Update()
        {
            var r = GetComponent<Rigidbody>();
            if (!_gearController.isGrabbed)
            {
                var transform1 = transform;
                transform1.localPosition = handleDefaultPos.localPosition;
                _previousPosition = transform1.localPosition;
                

                if (!r.isKinematic)
                {
                    r.isKinematic = true;
                    Debug.Log("Turning on");
                }
                return;
            }
            if (r.isKinematic)
            {
                // r.isKinematic = false;
                Debug.Log("Turning OFF");
            }
            Vector3 distanceMoved = transform.localPosition - _previousPosition;

            // Check if the object is moving on the X-axis
            if (Mathf.Abs(distanceMoved.x) > 0.001f)
            {
                float rotationAngleX = distanceMoved.x*multiplier;
                _objectToRotate.Rotate(Vector3.forward, rotationAngleX);
            }

            // Check if the object is moving on the Z-axis
            if (Mathf.Abs(distanceMoved.z) > 0.001f)
            {
                float rotationAngleZ = distanceMoved.z*multiplier;
                _objectToRotate.Rotate(Vector3.left, rotationAngleZ);
            }

            // Update the previous position for the next frame
            _previousPosition = transform.localPosition;
        }
    }
}
