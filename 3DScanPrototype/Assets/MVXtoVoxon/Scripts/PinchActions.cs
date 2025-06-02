using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FloatEvent : UnityEvent<float> { }

[Serializable]
public class FloatFloatEvent : UnityEvent<float, float> { }

public class PinchActions : MonoBehaviour
{

    public PinchDetector LPinchDetectorIndex;
    public PinchDetector RPinchDetectorIndex;
    public PinchDetector LPinchDetectorMiddle;
    public PinchDetector RPinchDetectorMiddle;
    public PinchDetector LPinchDetectorRing;
    public PinchDetector RPinchDetectorRing;
    public PinchDetector LPinchDetectorPinky;
    public PinchDetector RPinchDetectorPinky;

    [Header("Pinch Settings")]
    public float pinchActivateDistance = 0.025f;
    public float pinchDeactivateDistance = 0.03f;
    public float pinchCooldownTime = 0.5f;


    [Header("Rotation Settings")]
    public float rotationSpeed = 90f; // degrees per second

    // Tracking last frame a pinch start was detected for every finger.
    private int leftIndexLastPinchStartFrame = -1;
    private int rightIndexLastPinchStartFrame = -1;
    private int leftMiddleLastPinchStartFrame = -1;
    private int rightMiddleLastPinchStartFrame = -1;
    private int leftRingLastPinchStartFrame = -1;
    private int rightRingLastPinchStartFrame = -1;
    private int leftPinkyLastPinchStartFrame = -1;
    private int rightPinkyLastPinchStartFrame = -1;

    // Variables for cooldown: time when each finger is next allowed to register a pinch.
    private float leftIndexNextAvailable = 0f;
    private float rightIndexNextAvailable = 0f;
    private float leftMiddleNextAvailable = 0f;
    private float rightMiddleNextAvailable = 0f;
    private float leftRingNextAvailable = 0f;
    private float rightRingNextAvailable = 0f;
    private float leftPinkyNextAvailable = 0f;
    private float rightPinkyNextAvailable = 0f;

    // --- UnityEvents for each finger pinch (only start events now) ---
    public FloatEvent OnLeftIndexPinch = new FloatEvent();
    public FloatEvent OnRightIndexPinch = new FloatEvent();
    public FloatEvent OnLeftMiddlePinch = new FloatEvent();
    public FloatEvent OnRightMiddlePinch = new FloatEvent();
    public FloatEvent OnLeftRingPinch = new FloatEvent();
    public FloatEvent OnRightRingPinch = new FloatEvent();
    public FloatEvent OnLeftPinkyPinch = new FloatEvent();
    public FloatEvent OnRightPinkyPinch = new FloatEvent();

    // Special UnityEvent that fires when BOTH index fingers pinch in the same frame.
    public FloatFloatEvent OnBothIndexPinch = new FloatFloatEvent();

    private bool leftIndexPinching = false;
    private bool rightIndexPinching = false;

    private bool bothIndexPinching = false;

    // Add boolean flags to track the previous pinching state for each finger
    [SerializeField]
    private bool leftIndexWasPinching = false;
        [SerializeField]
    private bool rightIndexWasPinching = false;
    [SerializeField]
    private bool leftMiddleWasPinching = false;
    [SerializeField]
    private bool rightMiddleWasPinching = false;
    [SerializeField]
    private bool leftRingWasPinching = false;
    [SerializeField]
    private bool rightRingWasPinching = false;
    [SerializeField]
    private bool leftPinkyWasPinching = false;
    [SerializeField]
    private bool rightPinkyWasPinching = false;

    void Start()
    {
        // (Initialization as needed)
    }

    void Update()
    {
        SetActivateDeactivateDistances();

        CheckLeftIndexPinch();
        CheckRightIndexPinch();
        CheckLeftMiddlePinch();
        CheckRightMiddlePinch();
        CheckLeftRingPinch();
        CheckRightRingPinch();
        CheckLeftPinkyPinch();
        CheckRightPinkyPinch();


        if (leftIndexWasPinching && rightIndexWasPinching)
        {
            if (!bothIndexPinching)
            {
                bothIndexPinching = true;
                Debug.Log("Both index fingers pinching: Left Squish = " + LPinchDetectorIndex.SquishPercent + ", Right Squish = " + RPinchDetectorIndex.SquishPercent);
                OnBothIndexPinch.Invoke(LPinchDetectorIndex.SquishPercent, RPinchDetectorIndex.SquishPercent);
            }
        }
        else
        {
            bothIndexPinching = false;
        }
    }

    void SetActivateDeactivateDistances()
    {
        if (LPinchDetectorIndex != null)
        {
            LPinchDetectorIndex.activateDistance = pinchActivateDistance;
            LPinchDetectorIndex.deactivateDistance = pinchDeactivateDistance;
        }
        if (RPinchDetectorIndex != null)
        {
            RPinchDetectorIndex.activateDistance = pinchActivateDistance;
            RPinchDetectorIndex.deactivateDistance = pinchDeactivateDistance;
        }
        if (LPinchDetectorMiddle != null)
        {
            LPinchDetectorMiddle.activateDistance = pinchActivateDistance;
            LPinchDetectorMiddle.deactivateDistance = pinchDeactivateDistance;
        }
        if (RPinchDetectorMiddle != null)
        {
            RPinchDetectorMiddle.activateDistance = pinchActivateDistance;
            RPinchDetectorMiddle.deactivateDistance = pinchDeactivateDistance;
        }
        if (LPinchDetectorRing != null)
        {
            LPinchDetectorRing.activateDistance = pinchActivateDistance;
            LPinchDetectorRing.deactivateDistance = pinchDeactivateDistance;
        }
        if (RPinchDetectorRing != null)
        {
            RPinchDetectorRing.activateDistance = pinchActivateDistance;
            RPinchDetectorRing.deactivateDistance = pinchDeactivateDistance;
        }
        if (LPinchDetectorPinky != null)
        {
            LPinchDetectorPinky.activateDistance = pinchActivateDistance;
            LPinchDetectorPinky.deactivateDistance = pinchDeactivateDistance;
        }
        if (RPinchDetectorPinky != null)
        {
            RPinchDetectorPinky.activateDistance = pinchActivateDistance;
            RPinchDetectorPinky.deactivateDistance = pinchDeactivateDistance;
        }
    }

    // --- Individual Finger Pinch Check Functions ---

    void CheckLeftIndexPinch()
    {
        if (LPinchDetectorIndex == null) return;

        if (LPinchDetectorIndex.IsPinching)
        {
            if (!leftIndexWasPinching)
            {
                OnLeftIndexPinch.Invoke(LPinchDetectorIndex.SquishPercent);
                Debug.Log("Left Index is pinching with squish percent: " + LPinchDetectorIndex.SquishPercent);
                leftIndexWasPinching = true;
            }
        }
        else
        {
            leftIndexWasPinching = false;
        }
    }

    void CheckRightIndexPinch()
    {
        if (RPinchDetectorIndex == null) return;

        if (RPinchDetectorIndex.IsPinching)
        {
            if (!rightIndexWasPinching)
            {
                OnRightIndexPinch.Invoke(RPinchDetectorIndex.SquishPercent);
                Debug.Log("Right Index is pinching with squish percent: " + RPinchDetectorIndex.SquishPercent);
                rightIndexWasPinching = true;
            }
        }
        else
        {
            rightIndexWasPinching = false;
        }
    }

    void CheckLeftMiddlePinch()
    {
        if (LPinchDetectorMiddle == null) return;

        if (LPinchDetectorMiddle.IsPinching)
        {
            if (!leftMiddleWasPinching)
            {
                OnLeftMiddlePinch.Invoke(LPinchDetectorMiddle.SquishPercent);
                Debug.Log("Left Middle is pinching with squish percent: " + LPinchDetectorMiddle.SquishPercent);
                leftMiddleWasPinching = true;
            }
        }
        else
        {
            leftMiddleWasPinching = false;
        }
    }

    void CheckRightMiddlePinch()
    {
        if (RPinchDetectorMiddle == null) return;

        if (RPinchDetectorMiddle.IsPinching)
        {
            if (!rightMiddleWasPinching)
            {
                OnRightMiddlePinch.Invoke(RPinchDetectorMiddle.SquishPercent);
                Debug.Log("Right Middle is pinching with squish percent: " + RPinchDetectorMiddle.SquishPercent);
                rightMiddleWasPinching = true;
            }
        }
        else
        {
            rightMiddleWasPinching = false;
        }
    }

    void CheckLeftRingPinch()
    {
        if (LPinchDetectorRing == null) return;

        if (LPinchDetectorRing.IsPinching)
        {
            if (!leftRingWasPinching)
            {
                OnLeftRingPinch.Invoke(LPinchDetectorRing.SquishPercent);
                Debug.Log("Left Ring is pinching with squish percent: " + LPinchDetectorRing.SquishPercent);
                leftRingWasPinching = true;
            }
        }
        else
        {
            leftRingWasPinching = false;
        }
    }

    void CheckRightRingPinch()
    {
        if (RPinchDetectorRing == null) return;

        if (RPinchDetectorRing.IsPinching)
        {
            if (!rightRingWasPinching)
            {
                OnRightRingPinch.Invoke(RPinchDetectorRing.SquishPercent);
                Debug.Log("Right Ring is pinching with squish percent: " + RPinchDetectorRing.SquishPercent);
                rightRingWasPinching = true;
            }
        }
        else
        {
            rightRingWasPinching = false;
        }
    }

    void CheckLeftPinkyPinch()
    {
        if (LPinchDetectorPinky == null) return;

        if (LPinchDetectorPinky.IsPinching)
        {
            if (!leftPinkyWasPinching)
            {
                OnLeftPinkyPinch.Invoke(LPinchDetectorPinky.SquishPercent);
                Debug.Log("Left Pinky is pinching with squish percent: " + LPinchDetectorPinky.SquishPercent);
                leftPinkyWasPinching = true;
            }
        }
        else
        {
            leftPinkyWasPinching = false;
        }
    }

    void CheckRightPinkyPinch()
    {
        if (RPinchDetectorPinky == null) return;

        if (RPinchDetectorPinky.IsPinching)
        {
            if (!rightPinkyWasPinching)
            {
                OnRightPinkyPinch.Invoke(RPinchDetectorPinky.SquishPercent);
                Debug.Log("Right Pinky is pinching with squish percent: " + RPinchDetectorPinky.SquishPercent);
                rightPinkyWasPinching = true;
            }
        }
        else
        {
            rightPinkyWasPinching = false;
        }
    }
}
