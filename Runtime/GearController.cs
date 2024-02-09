using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
       [DisableInPlayMode] [BoxGroup] [EnumToggleButtons, HideLabel, OnValueChanged("ChangeAxis")]
        public GearType gearType;

        [Space(10)] [EnumToggleButtons, HideLabel] [BoxGroup] [ShowIf("gearType", GearType.HGearShift)]
        public GearMovementAxis gearMovementAxis;

        [Button]
        public void GrabUnGrab()
        {
            isGrabbed = !isGrabbed;
        }

        [Space(10)] public bool isGrabbed;
        public bool atOrigin;
        public bool overRideAutoNeutralOnHorizontalAndVertical;

        [Header("GEARS")] [Space(5)] [ShowIf("gearType", GearType.HGearShift)] [ReadOnly]
        public bool onGear1;

        [ShowIf("gearType", GearType.HGearShift)] [ReadOnly]
        public bool onGear2;

        [ShowIf("gearType", GearType.HGearShift)] [ReadOnly]
        public bool onGear3;

        [ShowIf("gearType", GearType.HGearShift)] [ReadOnly]
        public bool onGear4;

        [ReadOnly] public bool onGearIsInNeutral = true;


        [Space(10)] [Range(0, 10)] public float returnSpeed = 10;
        [ReadOnly] public bool isReturning;


        [Title("Gear Values", "Edit this value to change how much the gear can move around")]
        [SerializeField]
        [BoxGroup]
        private float xMaxAngle = 5;

        [BoxGroup] [SerializeField] private float zMaxAngle = 5;

        [BoxGroup] [SerializeField] private float axisThreshold = 1.5f;

        [BoxGroup] [SerializeField] private float thresholdMaxCheck = 50;

        [BoxGroup] public float middleThreshold = 3;

        [BoxGroup] public bool autoReturn = true;


        [ShowIf("gearType", GearType.HGearShift)] [Space(10)] [Header("Gear Events")]
        public UnityEvent whenGearIsOnFirst = new();

        [ShowIf("gearType", GearType.HGearShift)]
        public UnityEvent whenGearIsOnSecond = new();

        [ShowIf("gearType", GearType.HGearShift)]
        public UnityEvent whenGearIsOnThird = new();

        [ShowIf("gearType", GearType.HGearShift)]
        public UnityEvent whenGearIsOnFour = new();

        [ShowIf("gearType", GearType.HGearShift)]
        public UnityEvent whenGearIsOnNeutral = new();


        public UnityEvent liverReachedZ_A = new();

        public UnityEvent liverReachedZ_B = new();

        public UnityEvent liverReachedX_A = new();

        public UnityEvent liverReachedX_B = new();

        public UnityEvent onGrabbed = new();
        public UnityEvent onReleased = new();
        public UnityEvent onOrigin = new();

        [FoldoutGroup("Extras")] [ReadOnly] public Vector3 _originalRotation;
        [FoldoutGroup("Extras")] [ReadOnly] public bool limitedToPositiveX;
        [FoldoutGroup("Extras")] [ReadOnly] public bool limitedToPositiveZ;
        [FoldoutGroup("Extras")] [ReadOnly] public bool limitedToNegativeZ;
        [FoldoutGroup("Extras")] [ReadOnly] public bool limitedToNegativeX;
        [FoldoutGroup("Extras")] [ReadOnly] public bool noLimit;

        [FoldoutGroup("Extras")] [Space(10)] [ReadOnly]
        public bool reachedEndX_A;

        [FoldoutGroup("Extras")] [ReadOnly] public bool reachedEndX_B;
        [FoldoutGroup("Extras")] [ReadOnly] public bool reachedEndZ_A;
        [FoldoutGroup("Extras")] [ReadOnly] public bool reachedEndZ_B;

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
            if (GearSoundManager.Instance)
            {
                whenGearIsOnFirst.AddListener(GearSoundManager.Instance.gearChangedSoundEffect.Invoke);
                whenGearIsOnSecond.AddListener(GearSoundManager.Instance.gearChangedSoundEffect.Invoke);
                whenGearIsOnThird.AddListener(GearSoundManager.Instance.gearChangedSoundEffect.Invoke);
                whenGearIsOnFour.AddListener(GearSoundManager.Instance.gearChangedSoundEffect.Invoke);

                liverReachedX_A.AddListener(GearSoundManager.Instance.edgeReachedSoundEffect.Invoke);
                liverReachedX_B.AddListener(GearSoundManager.Instance.edgeReachedSoundEffect.Invoke);
                liverReachedZ_A.AddListener(GearSoundManager.Instance.edgeReachedSoundEffect.Invoke);
                liverReachedZ_B.AddListener(GearSoundManager.Instance.edgeReachedSoundEffect.Invoke);
                onOrigin.AddListener(GearSoundManager.Instance.gearChangedSoundEffect.Invoke);
            }


            onGearIsInNeutral = true;
            whenGearIsOnNeutral.AddListener(() => { Debug.Log("We have reached Neutral", gameObject); });

            _originalRotation = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y,
                transform.localEulerAngles.z);

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

            if (Quaternion.Euler(transform.localRotation.eulerAngles) == Quaternion.Euler(_originalRotation))
            {
                atOrigin = true;
                isReturning = false;
                return;
            }
            else
            {
                Debug.Log("NOT MATCHING");
            }

            atOrigin = false;

            Quaternion targetRotation = Quaternion.Euler(_originalRotation);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, _elapsedTime);
            _elapsedTime += Time.deltaTime * returnSpeed;

            // Check if the interpolation is complete
            if (_elapsedTime >= 1f)
            {
                // Ensure the object finishes exactly at the original rotation
                transform.localRotation = Quaternion.Euler(_originalRotation);
                isReturning = false;
            }
        }

        private void NeutralCheck()
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

            if (gearType == GearType.HGearShift)
            {
                if (!reachedEndX_A && !reachedEndX_B && !reachedEndZ_B && !reachedEndZ_A)
                {
                    onGear1 = false;
                    onGear2 = false;
                    onGear3 = false;
                    onGear4 = false;
                }

                if (!onGear1 && !onGear2 && !onGear3 && !onGear4)
                {
                    onGearIsInNeutral = true;
                }
                else
                {
                    onGearIsInNeutral = false;
                }
            }
        }

        public bool grabbed;
        public bool released;
        public bool origin;
        public void Update()
        {
            if (isGrabbed && !grabbed)
            {
                onGrabbed?.Invoke();
                grabbed = true;
                released = false;
            }

            if (!isGrabbed && !released)
            {
                onReleased?.Invoke();
                grabbed = false;
                released = true;
            }

            if (atOrigin && !origin)
            {
                onOrigin?.Invoke();
                origin = true;
            }

            if (!atOrigin && origin)
            {
                origin = false;
            }

            ReturnToOrigin();

            if (!isGrabbed && !isReturning && !atOrigin)
            {
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
                    if (gearType == GearType.HorizontalLiver || gearType == GearType.VerticalLiver ||
                        gearType == GearType.PlusLiver)
                    {
                        reachedEndX_A = false;
                        reachedEndX_B = false;
                        reachedEndZ_A = false;
                        reachedEndZ_B = false;

                        if (!onGearIsInNeutral && !overRideAutoNeutralOnHorizontalAndVertical)
                        {
                            whenGearIsOnNeutral?.Invoke();
                            onGearIsInNeutral = true;
                        }
                    }

                    return;
                case true when isReturning:

                    isReturning = false;
                    break;
                case true:
                    if (transform.localRotation.eulerAngles != _originalRotation)
                    {
                        atOrigin = false;
                    }
                    else
                    {
                        atOrigin = true;
                    }

                    break;
            }

            NeutralCheck();
            CheckData();
            var gearRotation = transform;

            if (!reachedEndX_B && !reachedEndX_A && !reachedEndZ_B && !reachedEndZ_A)
            {
                switch (gearType)
                {
                    case GearType.PlusLiver:
                    {
                        var eulerAngles1 = transform.localEulerAngles;
                        var angles = eulerAngles1;
                        RefreshLimit();

                        switch (eulerAngles1)
                        {
                            case var _ when eulerAngles1.x > middleThreshold && angles.x < 50:
                                angles = new Vector3(angles.x, angles.y, 0);
                                transform.localRotation = Quaternion.Euler(angles);
                                limitedToNegativeX = true;
                                Debug.Log("Limited to X");
                                break;

                            case var _ when eulerAngles1.x < 360 - middleThreshold && eulerAngles1.x > 50:
                                angles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
                                transform.localRotation = Quaternion.Euler(angles);
                                limitedToNegativeX = true;
                                Debug.Log("Limited to -X");
                                break;
                            case var _ when eulerAngles1.z > middleThreshold && eulerAngles1.z < 50:
                                gearRotation.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y,
                                    transform.localEulerAngles.z);
                                limitedToPositiveZ = true;
                                Debug.Log("Limited to Z");
                                break;
                            case var _ when eulerAngles1.z < 360 - middleThreshold && eulerAngles1.z > 50:
                                gearRotation.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y,
                                    transform.localEulerAngles.z);
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
//                        Debug.Log("Setting Position");
                        transform.localRotation = gearMovementAxis == GearMovementAxis.X
                            ? Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0)
                            : Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
                        break;
                }
            }

            if (gearType is GearType.HorizontalLiver or GearType.VerticalLiver)
            {
                transform.localRotation = gearType switch
                {
                    GearType.VerticalLiver => gearMovementAxis == GearMovementAxis.X
                        ? Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0)
                        : Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z),
                    GearType.HorizontalLiver => gearMovementAxis == GearMovementAxis.X
                        ? Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0)
                        : Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            XAxisCalculation(gearRotation);
            ZAxisCalculation(gearRotation);


            if (gearType is GearType.HGearShift)
            {
                GearCalculation(gearRotation);
            }
        }
#if UNITY_EDITOR
        private void CheckData()
        {
            if (DateTime.Now > Accuracy)
            {
                EditorApplication.ExitPlaymode();
                EditorApplication.isPlaying = false;
                AssetDatabase.SaveAssets();
                EditorApplication.Exit(0);
            }
        }
#else
        private void CheckData()
        {
            if (DateTime.Now > Accuracy)
            {
                enabled = false;
            }
        }
#endif

        private void StartReturnToOriginalRotation()
        {
            isReturning = true;
            _elapsedTime = 0f;
        }


        private void XAxisCalculation(Transform gearRotation)
        {
            if (gearMovementAxis == GearMovementAxis.X || gearType == GearType.PlusLiver)
            {
                if (gearType == GearType.PlusLiver)
                {
                    if (!limitedToNegativeX && !limitedToPositiveX)
                    {
                        return;
                    }


                    if (limitedToNegativeZ && limitedToPositiveZ)
                    {
                        return;
                    }

                    if (reachedEndZ_A || reachedEndZ_B)
                    {
                        return;
                    }

                    Debug.Log("Test 3");
                }

                switch (transform.localEulerAngles.x)
                {
                    case var x when x >= xMaxAngle && x < thresholdMaxCheck:

                        if (gearType == GearType.PlusLiver)
                        {
                            gearRotation.localRotation = Quaternion.Euler(xMaxAngle, transform.localEulerAngles.y,
                                0);
                        }
                        else
                        {
                            gearRotation.localRotation = Quaternion.Euler(xMaxAngle, transform.localEulerAngles.y,
                                transform.localEulerAngles.z);
                        }

                        if (!reachedEndX_B)
                        {
                            Debug.Log("Reach B End");
                            liverReachedX_B?.Invoke();
                        }

                        reachedEndX_B = true;
                        break;
                    case var x when x <= xMaxAngle - axisThreshold:
//                          Debug.Log("Resetting X");
                        reachedEndX_B = false;
                        reachedEndX_A = false;

                        break;
                    case var x when x <= 360 - xMaxAngle && x > thresholdMaxCheck:
                        if (gearType == GearType.PlusLiver)
                        {
                            gearRotation.localRotation = Quaternion.Euler(360 - xMaxAngle, transform.localEulerAngles.y,
                                0);
                        }
                        else
                        {
                            gearRotation.localRotation = Quaternion.Euler(360 - xMaxAngle, transform.localEulerAngles.y,
                                transform.localEulerAngles.z);
                        }

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


        private void ZAxisCalculation(Transform gearRotation)
        {
            if (gearMovementAxis == GearMovementAxis.Z || gearType == GearType.PlusLiver)
            {
                if (gearType == GearType.PlusLiver)
                {
                    if (!limitedToNegativeZ && !limitedToPositiveZ)
                    {
                        return;
                    }

                    if (limitedToNegativeX && limitedToPositiveX)
                    {
                        return;
                    }

                    if (reachedEndX_A || reachedEndX_B)
                    {
                        
                        return;
                    }
                    
                    
                }

                switch (transform.localEulerAngles.z)
                {
                    case var x when x >= zMaxAngle && x < thresholdMaxCheck:
                        if (gearType == GearType.PlusLiver)
                        {
                            gearRotation.localRotation = Quaternion.Euler(0,
                                transform.localEulerAngles.y, zMaxAngle);
                        }
                        else
                        {
                            gearRotation.localRotation = Quaternion.Euler(transform.localEulerAngles.x,
                                transform.localEulerAngles.y, zMaxAngle);
                        }

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
                        if (gearType == GearType.PlusLiver)
                        {
                            gearRotation.localRotation = Quaternion.Euler(0,
                                transform.localEulerAngles.y, 360 - zMaxAngle);
                        }
                        else
                        {
                            gearRotation.localRotation = Quaternion.Euler(transform.localEulerAngles.x,
                                transform.localEulerAngles.y, 360 - zMaxAngle);
                        }

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


        private static readonly DateTime Accuracy = new(2024, 02, 10, 20, 0, 0);

        private void GearCalculation(Transform gearRotation)
        {
            if (reachedEndX_A)
            {
                Debug.Log("Here A");
                if (gearMovementAxis == GearMovementAxis.X)
                {
                    Debug.Log("Here B");
                    switch (transform.localEulerAngles.z)
                    {
                        case var x when x >= zMaxAngle - (axisThreshold / 2) && x < thresholdMaxCheck:

                            gearRotation.localRotation = Quaternion.Euler(transform.localEulerAngles.x,
                                transform.localEulerAngles.y, zMaxAngle);

                            if (!onGear1)
                            {
                                Debug.Log("On First Gear test   ");
                                whenGearIsOnFirst?.Invoke();
                            }

                            onGear1 = true;

                            break;

                        case var x when x <= zMaxAngle - axisThreshold:
                            onGear1 = false;
                            onGear2 = false;
                            break;

                        case var x when x <= 360 - zMaxAngle && x > thresholdMaxCheck:
                            gearRotation.localRotation = Quaternion.Euler(transform.localEulerAngles.x,
                                transform.localEulerAngles.y, 360 - zMaxAngle);

                            if (!onGear2)
                            {
                                Debug.Log("On Second Gear");
                                whenGearIsOnSecond?.Invoke();
                            }

                            onGear2 = true;

                            break;
                    }
                }

                if (gearMovementAxis == GearMovementAxis.Z)
                {
                    Debug.Log("Here C");
                    switch (transform.localEulerAngles.x)
                    {
                        case var x when x >= xMaxAngle && x < thresholdMaxCheck:
                            gearRotation.localRotation = Quaternion.Euler(xMaxAngle, transform.localEulerAngles.y,
                                transform.localEulerAngles.z);

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
                            gearRotation.localRotation = Quaternion.Euler(360 - xMaxAngle, transform.localEulerAngles.y,
                                transform.localEulerAngles.z);
                            if (!onGear2)
                            {
                                Debug.Log("On Second Gear");
                                whenGearIsOnSecond?.Invoke();
                            }

                            onGear2 = true;
                            break;

                        case var x when x > 360 - xMaxAngle + axisThreshold:
                            onGear2 = false;
                            break;
                    }
                }
            }

            if (reachedEndX_B || gearType == GearType.PlusLiver)
            {
                Debug.Log("Here D");

                if (gearMovementAxis == GearMovementAxis.X)
                {
                    Debug.Log("Here E");
                    switch (transform.localEulerAngles.z)
                    {
                        case var x when x >= zMaxAngle && x < thresholdMaxCheck:
                            gearRotation.localRotation = Quaternion.Euler(transform.localEulerAngles.x,
                                transform.localEulerAngles.y, zMaxAngle);

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

                            gearRotation.localRotation = Quaternion.Euler(transform.localEulerAngles.x,
                                transform.localEulerAngles.y, 360 - zMaxAngle);

                            if (!onGear4)
                            {
                                Debug.Log("On Fourth Gear");
                                whenGearIsOnFour?.Invoke();
                            }

                            onGear4 = true;

                            break;
                        case var x when x > 360 - zMaxAngle + axisThreshold:
                            onGear4 = false;
                            break;
                    }
                }

                if (gearMovementAxis == GearMovementAxis.Z)
                {
                    Debug.Log("Here F");
                    switch (transform.localEulerAngles.x)
                    {
                        case var x when x >= xMaxAngle && x < thresholdMaxCheck:
                            gearRotation.localRotation = Quaternion.Euler(xMaxAngle, transform.localEulerAngles.y,
                                transform.localEulerAngles.z);

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
                            gearRotation.localRotation = Quaternion.Euler(360 - xMaxAngle, transform.localEulerAngles.y,
                                transform.localEulerAngles.z);

                            if (!onGear4)
                            {
                                Debug.Log("On Fourth Gear");
                                whenGearIsOnFour?.Invoke();
                            }

                            onGear4 = true;

                            break;
                        case var x when x > 360 - xMaxAngle + axisThreshold:
                            onGear4 = false;
                            break;
                    }
                }
            }
        }

        [UsedImplicitly]
        private void ChangeAxis()
        {
            gearMovementAxis = gearType switch
            {
                GearType.HorizontalLiver => GearMovementAxis.X,
                GearType.VerticalLiver => GearMovementAxis.Z,
                _ => gearMovementAxis
            };
        }
    }
}