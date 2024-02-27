using Sirenix.OdinInspector;
using UnityEngine;

namespace fourtyfourty.gearController
{
    public class GrabThis : MonoBehaviour
    {
        [DisplayAsString] [BoxGroup("Debug")] public string distanceMoved;

        [DisplayAsString]  [BoxGroup("Debug")]public string X;

        [DisplayAsString]  [BoxGroup("Debug")] public string Y;

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

        public bool makeKinmatic;

        private void Update()
        {
            distanceMoved = (transform.localPosition - handleDefaultPos.localPosition).ToString();
            var r = GetComponent<Rigidbody>();
            if (!gearController.isGrabbed)
            {
                if (!_meshRenderer.enabled)
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

            if (makeKinmatic)
            {
                if (r.isKinematic)
                {
                    r.isKinematic = false;
                    Debug.Log("Turning off");
                }
            }

            if (_meshRenderer.enabled)
            {
                _meshRenderer.enabled = false;
            }

            Vector3 moved = transform.localPosition - _previousPosition;


            // Check if the object is moving on the X-axis
            if (Mathf.Abs(moved.x) > 0.001f)
            {
                Y = ("Moving on X " + moved.x + gearController.reachedEndZ_A + gearController.reachedEndZ_B);
                if (gearController.reachedEndZ_B && moved.x < 0) return;
                if (gearController.reachedEndZ_A && moved.x > 0) return;
                float rotationAngleX = moved.x * multiplier;
                _objectToRotate.Rotate(Vector3.forward, rotationAngleX);
            }

            // Check if the object is moving on the Z-axis
            if (Mathf.Abs(moved.z) > 0.001f)
            {
                Y = ("Moving on Z " + moved.z + gearController.reachedEndX_A + gearController.reachedEndX_B);
                if (gearController.reachedEndX_B && moved.z > 0) return;
                if (gearController.reachedEndX_A && moved.z < 0) return;
                float rotationAngleZ = moved.z * multiplier;
                _objectToRotate.Rotate(Vector3.left, rotationAngleZ);
            }

            // Update the previous position for the next frame
            _previousPosition = transform.localPosition;
        }
    }
}