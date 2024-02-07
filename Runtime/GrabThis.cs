using Sirenix.OdinInspector;
using UnityEngine;

namespace fourtyfourty.gearController
{
    public class GrabThis : MonoBehaviour
    {
        [DisplayAsString][BoxGroup]
        public string distanceMoved;
        
        private Transform _objectToRotate; // Assign the object you want to rotate in the Inspector

        [SerializeField] private Transform handleDefaultPos; // max sure the pivot is correct for this

        private Vector3 _previousPosition;

        [ReadOnly] public GearController gearController;

        public float multiplier = -10;

        private MeshRenderer _meshRenderer;

        [Button]
        public void LetsGrabThis()
        {
            gearController.isGrabbed = true;
        }
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

            gearController = _objectToRotate.GetComponent<GearController>();
            _previousPosition = transform.localPosition;
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            this.distanceMoved = (transform.localPosition - handleDefaultPos.localPosition).ToString();
            var r = GetComponent<Rigidbody>();
            if (!gearController.isGrabbed)
            {
                if(!_meshRenderer.enabled)
                {
                    _meshRenderer.enabled = true;
                }
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

            if (_meshRenderer.enabled)
            {
                _meshRenderer.enabled = false;
            }   

            Vector3 moved = transform.localPosition - _previousPosition;

            // Check if the object is moving on the X-axis
            if (Mathf.Abs(moved.x) > 0.001f)
            {
                
                float rotationAngleX = moved.x*multiplier;
                _objectToRotate.Rotate(Vector3.forward, rotationAngleX);
            }

            // Check if the object is moving on the Z-axis
            if (Mathf.Abs(moved.z) > 0.001f)
            {
                float rotationAngleZ = moved.z*multiplier;
                _objectToRotate.Rotate(Vector3.left, rotationAngleZ);
            }

            // Update the previous position for the next frame
            _previousPosition = transform.localPosition;
        }
    }
}
