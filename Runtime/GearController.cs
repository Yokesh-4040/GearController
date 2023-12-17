using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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
        public bool isGrabbed;
        public bool atOrigin;
        
        [Header("<b><u>GEARS</b></u>")] [Space(5)]
        public bool onGear1;

        public bool onGear2;
        public bool onGear3;
        public bool onGear4;
        public bool onGearIsInNeutral;

        [Space(10)] 
        public bool reachedEndX_A;
        public bool reachedEndX_B;
        public bool reachedEndZ_A;
        public bool reachedEndZ_B;


        [Space(10)] [Range(0, 10)] public float returnSpeed = 10;
        [FormerlySerializedAs("_isReturning")] public bool isReturning;

        private Quaternion _originalRotation;

        [Space(10)]
        [Header("<b><u>Gear Values</b></u>")]
        [Description("Edit this value to change how much the gear can move around")]
        [FormerlySerializedAs("XMaxAngle")]
        [SerializeField]
        private float xMaxAngle = 5;

        [FormerlySerializedAs("ZMaxAngle")] [SerializeField]
        private float zMaxAngle = 5;

        [FormerlySerializedAs("AxisThreshold")] [SerializeField]
        private float axisThreshold = 1.5f;

        [FormerlySerializedAs("ThresholdMaxCheck")] [SerializeField]
        private float thresholdMaxCheck = 50;

        [Space(20)] 
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


        private void RefreshLimit()
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


        private float _elapsedTime;
       
        private void ReturnToOrigin()
        {
            if (!isReturning) return;

            if (!autoReturn) return;
            
            if (transform.rotation == _originalRotation)
            {
                atOrigin = true;
                return;
            }

            atOrigin = false;
            transform.rotation = Quaternion.Slerp(transform.rotation, _originalRotation, _elapsedTime);
            _elapsedTime += Time.deltaTime * returnSpeed;

            // Check if the interpolation is complete
            if (_elapsedTime >= 1f)
            {
                // Ensure the object finishes exactly at the original rotation
                transform.rotation = _originalRotation;
                isReturning = false;
            }
        }

        public void NeutralCheck()
        {
            if (gearType is GearType.PlusLiver or GearType.VerticalLiver or GearType.HorizontalLiver)
            {
                if (!reachedEndX_A && !reachedEndX_B && !reachedEndZ_A && !reachedEndZ_B)
                {
                    onGearIsInNeutral = true;
                    whenGearIsOnNeutral?.Invoke();
                }
                else
                {
                    onGearIsInNeutral = false;
                }
            }
        }

        public void Update()
        {
            ReturnToOrigin();
           
            if (!isGrabbed && !isReturning && !atOrigin)
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

            switch (isGrabbed)
            {
                case false:
                    return;
                case true when isReturning:
                    isReturning = false;
                    break;
            }

            NeutralCheck();
            
            var thisTransform = transform;
            
            if (!reachedEndX_B && !reachedEndX_A && !reachedEndZ_B && !reachedEndZ_A)
            {
                switch (gearType)
                {
                    case GearType.PlusLiver:
                    {
                        var eulerAngles1 = transform.eulerAngles;
                        var angles = eulerAngles1;
                        RefreshLimit();

                        switch (eulerAngles1)
                        {
                            case var _ when eulerAngles1.x > middleThreshold && angles.x < 50:
                                angles = new Vector3(angles.x, 0, 0);
                                transform.eulerAngles = angles;
                                limitedToNegativeX = true;
                                Debug.Log("Limited to X");
                                break;

                            case var _ when eulerAngles1.x < 360 - middleThreshold && eulerAngles1.x > 50:
                                angles = new Vector3(transform.eulerAngles.x, 0, 0);
                                thisTransform.eulerAngles = angles;
                                limitedToNegativeX = true;
                                Debug.Log("Limited to -X");
                                break;
                            case var _ when eulerAngles1.z > middleThreshold && eulerAngles1.z < 50:
                                thisTransform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
                                limitedToPositiveZ = true;
                                Debug.Log("Limited to Z");
                                break;
                            case var _ when eulerAngles1.z < 360 - middleThreshold && eulerAngles1.z > 50:
                                thisTransform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
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

            thisTransform.eulerAngles = gearType switch
            {
                GearType.VerticalLiver => gearMovementAxis == GearMovementAxis.X
                    ? new Vector3(transform.eulerAngles.x, 0, 0)
                    : new Vector3(0, 0, transform.eulerAngles.z),
                GearType.HorizontalLiver => gearMovementAxis == GearMovementAxis.X
                    ? new Vector3(transform.eulerAngles.x, 0, 0)
                    : new Vector3(0, 0, transform.eulerAngles.z),
                _ => transform.eulerAngles
            };

            XAxisCalculation(thisTransform);
            ZAxisCalculation(thisTransform);


            if (gearType is GearType.HGearShift)
            {
                GearCalculation(thisTransform);
            }
        }

        private void StartReturnToOriginalRotation()
        {
            isReturning = true;
            _elapsedTime = 0f;
        }


        private void XAxisCalculation(Transform thisTransform)
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

                switch (transform.eulerAngles.x)
                {
                    case var x when x >= xMaxAngle && x < thresholdMaxCheck:
                        thisTransform.eulerAngles = new Vector3(xMaxAngle, 0, transform.eulerAngles.z);
                        if (!reachedEndX_B)
                        {
                            Debug.Log("Reach B End");
                            liverReachedX_B?.Invoke();
                        }

                        reachedEndX_B = true;
                        break;
                    case var x when x <= xMaxAngle - axisThreshold:
                        Debug.Log("Resetting X");
                        reachedEndX_B = false;
                        reachedEndX_A = false;

                        break;
                    case var x when x <= 360 - xMaxAngle && x > thresholdMaxCheck:
                        thisTransform.eulerAngles = new Vector3(360 - xMaxAngle, 0, transform.eulerAngles.z);
                        if (!reachedEndX_A)
                        {
                            Debug.Log("Reach A End");
                            liverReachedX_A?.Invoke();
                        }

                        reachedEndX_A = true;
                        break;

                    case var x when x > 360 - xMaxAngle + axisThreshold:
                        reachedEndX_A = false;
                        break;
                }
            }
        }

        private void ZAxisCalculation(Transform thisTransform)
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

                switch (transform.eulerAngles.z)
                {
                    case var x when x >= zMaxAngle && x < thresholdMaxCheck:
                        thisTransform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, zMaxAngle);
                        if (!reachedEndZ_B)
                        {
                            Debug.Log("Reach B End Z");
                            liverReachedZ_B?.Invoke();
                        }

                        reachedEndZ_B = true;

                        break;

                    case var x when x <= zMaxAngle - axisThreshold:
                        Debug.Log("Resetting Z");
                        reachedEndZ_B = false;
                        reachedEndZ_A = false;
                        break;

                    case var x when x <= 360 - zMaxAngle && x > thresholdMaxCheck:
                        thisTransform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 360 - xMaxAngle);
                        if (!reachedEndZ_A)
                        {
                            Debug.Log("Reach A End Z");
                            liverReachedZ_A?.Invoke();
                        }

                        reachedEndZ_A = true;
                        break;
                    case var x when x > 360 - xMaxAngle + axisThreshold:
                        reachedEndZ_A = false;
                        break;
                }
            }
        }

        private void GearCalculation(Transform thisTransform)
        {
            if (reachedEndX_A || gearType == GearType.PlusLiver)
            {
                Debug.Log("Here A");
                if (gearMovementAxis == GearMovementAxis.X || gearType == GearType.PlusLiver)
                {
                    Debug.Log("Here B");
                    switch (transform.eulerAngles.z)
                    {
                        case var x when x >= zMaxAngle && x < thresholdMaxCheck:
                            thisTransform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, zMaxAngle);

                            if (!onGear1)
                            {
                                Debug.Log("On First Gear");
                                whenGearIsOnFirst?.Invoke();
                            }

                            onGear1 = true;
                            break;

                        case var x when x <= zMaxAngle - axisThreshold:
                            onGear1 = false;
                            onGear2 = false;
                            break;

                        case var x when x <= 360 - zMaxAngle && x > thresholdMaxCheck:
                            thisTransform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 360 - zMaxAngle);

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
                        case var x when x >= xMaxAngle && x < thresholdMaxCheck:
                            thisTransform.eulerAngles = new Vector3(xMaxAngle, 0, transform.eulerAngles.z);

                            if (!onGear1)
                            {
                                Debug.Log("On First Gear");
                                whenGearIsOnFirst?.Invoke();
                            }

                            onGear1 = true;
                            break;

                        case var x when x <= xMaxAngle - axisThreshold:
                            onGear1 = false;
                            onGear2 = false;
                            break;

                        case var x when x <= 360 - xMaxAngle && x > thresholdMaxCheck:
                            thisTransform.eulerAngles = new Vector3(360 - xMaxAngle, 0, transform.eulerAngles.z);
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
                        case var x when x >= zMaxAngle && x < thresholdMaxCheck:
                            thisTransform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, zMaxAngle);

                            if (!onGear3)
                            {
                                Debug.Log("On Third Gear");
                                whenGearIsOnThird?.Invoke();
                            }

                            onGear3 = true;

                            break;
                        case var x when x <= zMaxAngle - axisThreshold:
                            Debug.Log("Third");
                            onGear3 = false;
                            onGear4 = false;
                            break;
                        case var x when x <= 360 - zMaxAngle && x > thresholdMaxCheck:

                            thisTransform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 360 - zMaxAngle);

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
                        case var x when x >= xMaxAngle && x < thresholdMaxCheck:
                            thisTransform.eulerAngles = new Vector3(xMaxAngle, 0, transform.eulerAngles.z);

                            if (!onGear3)
                            {
                                Debug.Log("On Third Gear");
                                whenGearIsOnThird?.Invoke();
                            }

                            onGear3 = true;
                            break;
                        case var x when x <= xMaxAngle - axisThreshold:
                            onGear3 = false;
                            onGear4 = false;
                            break;
                        case var x when x <= 360 - xMaxAngle && x > thresholdMaxCheck:
                            thisTransform.eulerAngles = new Vector3(360 - xMaxAngle, 0, transform.eulerAngles.z);

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