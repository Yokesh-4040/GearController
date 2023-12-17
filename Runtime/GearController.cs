using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace fourtyfourty.gearController
{
    public enum GearMovementAxis
    {
        X,
        Z
    }

    public enum GearType
    {
        HGearShift,
        HorizontalLiver,
        VerticalLiver,
        PlusLiver, // moves in all four direction 
    }

    public class GearController : MonoBehaviour
    {
        public GearType gearType;
        public GearMovementAxis gearMovementAxis;
        public bool isGrabbed { get; set; }

        [Header("<b><u>GEARS</b></u>")] [Space(5)]
        public bool onGear1;

        public bool onGear2;
        public bool onGear3;
        public bool onGear4;

        [Space(10)] public bool reachedEndX_A;
        public bool reachedEndX_B;
        public bool reachedEndZ_A;
        public bool reachedEndZ_B;


        [Space(10)] [Range(0, 10)] public float returnSpeed = 10;
        public bool _isReturning;

        private Quaternion _originalRotation;


        [SerializeField] private float XMaxAngle = 5;
        [SerializeField] private float ZMaxAngle = 5;
        [SerializeField] private float AxisThreshold = 1.5f;
        [SerializeField] private float ThresholdMaxCheck = 50;

        public bool limitedToPositiveX;
        public bool limitedToPositiveZ;
        public bool limitedToNegativeZ;
        public bool limitedToNegativeX;
        public bool noLimit;

        public float middleThreshold = 3;

        public bool autoReturn = true;
        [Space(10)] [Header("Gear Events")] public UnityEvent whenGearIsOnFirst = new();
        public UnityEvent whenGearIsOnSecond = new();
        public UnityEvent whenGearIsOnThird = new();
        public UnityEvent whenGearIsOnFour = new();
        public UnityEvent whenGearIsOnNeutral = new();

        public UnityEvent liverReachedZ_A = new();
        public UnityEvent liverReachedZ_B = new();
        public UnityEvent liverReachedX_A = new();
        public UnityEvent liverReachedX_B = new();


        public void RefreshLimit()
        {
            limitedToPositiveX = false;
            limitedToNegativeX = false;
            limitedToPositiveZ = false;
            limitedToNegativeZ = false;
            noLimit = false;
        }

        private void Start()
        {
            _originalRotation = transform.rotation;

            gearMovementAxis = gearType switch
            {
                GearType.HorizontalLiver => GearMovementAxis.X,
                GearType.VerticalLiver => GearMovementAxis.Z,
                _ => gearMovementAxis
            };

            switch (gearType)
            {
                case GearType.HorizontalLiver:
                {
                    switch (gearMovementAxis)
                    {
                        case GearMovementAxis.X:
                            limitedToPositiveX = true;
                            limitedToNegativeX = true;
                            break;
                        case GearMovementAxis.Z:
                            limitedToPositiveZ = true;
                            limitedToNegativeZ = true;
                            break;
                    }

                    break;
                }
                case GearType.VerticalLiver:
                {
                    switch (gearMovementAxis)
                    {
                        case GearMovementAxis.X:
                            limitedToPositiveX = true;
                            limitedToNegativeX = true;
                            break;
                        case GearMovementAxis.Z:
                            limitedToPositiveZ = true;
                            limitedToNegativeZ = true;
                            break;
                    }

                    break;
                }
            }
        }


        private float _elapsedTime = 0f;

        public void ReturnToOrigin()
        {
            if (_isReturning)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _originalRotation, _elapsedTime);
                _elapsedTime += Time.deltaTime * returnSpeed;

                // Check if the interpolation is complete
                if (_elapsedTime >= 1f)
                {
                    // Ensure the object finishes exactly at the original rotation
                    transform.rotation = _originalRotation;
                    _isReturning = false;
                }
            }
        }

        public void Update()
        {
            ReturnToOrigin();

            if (!reachedEndX_B && !reachedEndX_A && !reachedEndZ_B && !reachedEndZ_A)
            {
                switch (gearType)
                {
                    case GearType.PlusLiver:
                    {
                        var eulerAngles1 = transform.eulerAngles;
                        var eulerAngles = eulerAngles1;
                        var t = new Vector2(eulerAngles.x, eulerAngles.z);

                        var angles = eulerAngles1;
                        RefreshLimit();

                        switch (eulerAngles)
                        {
                            case var _ when eulerAngles.x > middleThreshold && angles.x < 50:
                                angles = new Vector3(angles.x, 0, 0);
                                transform.eulerAngles = angles;
                                limitedToNegativeX = true;
                                Debug.Log("Limited to X");
                                break;

                            case var _ when eulerAngles.x < 360 - middleThreshold && eulerAngles.x > 50:
                                angles = new Vector3(transform.eulerAngles.x, 0, 0);
                                transform.eulerAngles = angles;
                                limitedToNegativeX = true;
                                Debug.Log("Limited to -X");
                                break;
                            case var _ when eulerAngles.z > middleThreshold && eulerAngles.z < 50:
                                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
                                limitedToPositiveZ = true;
                                Debug.Log("Limited to Z");
                                break;
                            case var _ when eulerAngles.z < 360 - middleThreshold && eulerAngles.z > 50:
                                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
                                limitedToNegativeZ = true;
                                Debug.Log("Limited to -Z");
                                break;
                            default:
                                Debug.Log("No Limit");
                                noLimit = true;
                                break;
                        }

                        break;
                    }
                    case GearType.HGearShift:
                        transform.eulerAngles = gearMovementAxis == GearMovementAxis.X
                            ? new Vector3(transform.eulerAngles.x, 0, 0)
                            : new Vector3(0, 0, transform.eulerAngles.z);
                        break;
                }
            }

            transform.eulerAngles = gearType switch
            {
                GearType.VerticalLiver => gearMovementAxis == GearMovementAxis.X
                    ? new Vector3(transform.eulerAngles.x, 0, 0)
                    : new Vector3(0, 0, transform.eulerAngles.z),
                GearType.HorizontalLiver => gearMovementAxis == GearMovementAxis.X
                    ? new Vector3(transform.eulerAngles.x, 0, 0)
                    : new Vector3(0, 0, transform.eulerAngles.z),
                _ => transform.eulerAngles
            };

            XAxisCalculation();
            ZAxisCalculation();


            if (gearType is GearType.HGearShift)
            {
                GearCalculation();
            }

            if (!autoReturn)
            {
                Debug.Log("No Auto rotate");
                return;
            }

            if (!isGrabbed && !_isReturning)
            {
                Debug.Log("Auto 1");
                if (!onGear3 && !onGear4 && !onGear1 && !onGear2)
                {
                    StartReturnToOriginalRotation();
                }

                if (gearType == GearType.PlusLiver)
                {
                    StartReturnToOriginalRotation();
                }
            }

            if (isGrabbed && _isReturning)
            {
                Debug.Log("Auto 2");
                _isReturning = false;
            }
        }

        private void StartReturnToOriginalRotation()
        {
            _isReturning = true;
            _elapsedTime = 0f;
        }


        private void XAxisCalculation()
        {
            if (gearMovementAxis == GearMovementAxis.X || gearType == GearType.PlusLiver)
            {
                if (gearType == GearType.PlusLiver)
                {
                    if (!limitedToNegativeX && !limitedToPositiveX)
                    {
                        return;
                    }
                }

                int t = 0;
                float tt = 0;
                switch (transform.eulerAngles.x)
                {
                    case var x when x >= XMaxAngle && x < ThresholdMaxCheck:
                        transform.eulerAngles = new Vector3(XMaxAngle, 0, transform.eulerAngles.z);
                        if (!reachedEndX_B)
                        {
                            Debug.Log("Reach B End");
                            liverReachedX_B?.Invoke();
                        }

                        reachedEndX_B = true;
                        break;
                    case var x when x <= XMaxAngle - AxisThreshold:
                        Debug.Log("Resetting X");
                        reachedEndX_B = false;
                        reachedEndX_A = false;
                        break;
                    case var x when x <= 360 - XMaxAngle && x > ThresholdMaxCheck:
                        transform.eulerAngles = new Vector3(360 - XMaxAngle, 0, transform.eulerAngles.z);
                        if (!reachedEndX_A)
                        {
                            Debug.Log("Reach A End");
                            liverReachedX_A?.Invoke();
                        }

                        reachedEndX_A = true;
                        break;
                }
            }
        }

        private void ZAxisCalculation()
        {
            if (gearMovementAxis == GearMovementAxis.Z || gearType == GearType.PlusLiver)
            {
                if (gearType == GearType.PlusLiver)
                {
                    if (!limitedToNegativeZ && !limitedToPositiveZ)
                    {
                        return;
                    }
                }

                Debug.Log("PLUS Test Z");
                switch (transform.eulerAngles.z)
                {
                    case var x when x >= ZMaxAngle && x < ThresholdMaxCheck:
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, ZMaxAngle);
                        if (!reachedEndZ_B)
                        {
                            Debug.Log("Reach B End Z");
                            liverReachedZ_B?.Invoke();
                        }

                        reachedEndZ_B = true;

                        break;

                    case var x when x <= ZMaxAngle - AxisThreshold:
                        Debug.Log("Resetting Z");
                        reachedEndZ_B = false;
                        reachedEndZ_A = false;
                        break;

                    case var x when x <= 360 - ZMaxAngle && x > ThresholdMaxCheck:
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 360 - XMaxAngle);
                        if (!reachedEndZ_A)
                        {
                            Debug.Log("Reach A End Z");
                            liverReachedZ_A?.Invoke();
                        }

                        reachedEndZ_A = true;
                        break;
                }
            }
        }

        private IEnumerator ReturnToOriginalRotation()
        {
            _isReturning = true;
            var elapsedTime = 0f;

            while (elapsedTime < 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _originalRotation, elapsedTime);
                elapsedTime += Time.deltaTime * returnSpeed;
                yield return null;
            }

            // Ensure the object finishes exactly at the original rotation
            transform.rotation = _originalRotation;
            _isReturning = false;
        }


        private void GearCalculation()
        {
            if (reachedEndX_A || gearType == GearType.PlusLiver)
            {
                Debug.Log("Here A");
                if (gearMovementAxis == GearMovementAxis.X || gearType == GearType.PlusLiver)
                {
                    Debug.Log("Here B");
                    switch (transform.eulerAngles.z)
                    {
                        case var x when x >= ZMaxAngle && x < ThresholdMaxCheck:
                            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, ZMaxAngle);

                            if (!onGear1)
                            {
                                Debug.Log("On First Gear");
                                whenGearIsOnFirst?.Invoke();
                            }

                            onGear1 = true;
                            break;

                        case var x when x <= ZMaxAngle - AxisThreshold:
                            onGear1 = false;
                            onGear2 = false;
                            break;

                        case var x when x <= 360 - ZMaxAngle && x > ThresholdMaxCheck:
                            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 360 - ZMaxAngle);

                            if (!onGear2)
                            {
                                Debug.Log("On Second Gear");
                                whenGearIsOnSecond?.Invoke();
                            }

                            onGear2 = true;
                            break;
                    }
                }

                if (gearMovementAxis == GearMovementAxis.Z || gearType == GearType.PlusLiver)
                {
                    Debug.Log("Here C");
                    switch (transform.eulerAngles.x)
                    {
                        case var x when x >= XMaxAngle && x < ThresholdMaxCheck:
                            transform.eulerAngles = new Vector3(XMaxAngle, 0, transform.eulerAngles.z);

                            if (!onGear1)
                            {
                                Debug.Log("On First Gear");
                                whenGearIsOnFirst?.Invoke();
                            }

                            onGear1 = true;
                            break;

                        case var x when x <= XMaxAngle - AxisThreshold:
                            onGear1 = false;
                            onGear2 = false;
                            break;

                        case var x when x <= 360 - XMaxAngle && x > ThresholdMaxCheck:
                            transform.eulerAngles = new Vector3(360 - XMaxAngle, 0, transform.eulerAngles.z);
                            if (!onGear2)
                            {
                                Debug.Log("On Second Gear");
                                whenGearIsOnSecond?.Invoke();
                            }

                            onGear2 = true;
                            break;
                    }
                }
            }

            if (reachedEndX_B || gearType == GearType.PlusLiver)
            {
                Debug.Log("Here D");

                if (gearMovementAxis == GearMovementAxis.X || gearType == GearType.PlusLiver)
                {
                    Debug.Log("Here E");
                    switch (transform.eulerAngles.z)
                    {
                        case var x when x >= ZMaxAngle && x < ThresholdMaxCheck:
                            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, ZMaxAngle);

                            if (!onGear3)
                            {
                                Debug.Log("On Third Gear");
                                whenGearIsOnThird?.Invoke();
                            }

                            onGear3 = true;

                            break;
                        case var x when x <= ZMaxAngle - AxisThreshold:
                            Debug.Log("Third");
                            onGear3 = false;
                            onGear4 = false;
                            break;
                        case var x when x <= 360 - ZMaxAngle && x > ThresholdMaxCheck:

                            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 360 - ZMaxAngle);

                            if (!onGear4)
                            {
                                Debug.Log("On Fourth Gear");
                                whenGearIsOnFour?.Invoke();
                            }

                            onGear4 = true;
                            break;
                    }
                }

                if (gearMovementAxis == GearMovementAxis.Z || gearType == GearType.PlusLiver)
                {
                    Debug.Log("Here F");
                    switch (transform.eulerAngles.x)
                    {
                        case var x when x >= XMaxAngle && x < ThresholdMaxCheck:
                            transform.eulerAngles = new Vector3(XMaxAngle, 0, transform.eulerAngles.z);

                            if (!onGear3)
                            {
                                Debug.Log("On Third Gear");
                                whenGearIsOnThird?.Invoke();
                            }

                            onGear3 = true;
                            break;
                        case var x when x <= XMaxAngle - AxisThreshold:
                            onGear3 = false;
                            onGear4 = false;
                            break;
                        case var x when x <= 360 - XMaxAngle && x > ThresholdMaxCheck:
                            transform.eulerAngles = new Vector3(360 - XMaxAngle, 0, transform.eulerAngles.z);

                            if (!onGear4)
                            {
                                Debug.Log("On Fourth Gear");
                                whenGearIsOnFour?.Invoke();
                            }

                            onGear4 = true;
                            break;
                    }
                }
            }
        }
    }
}