using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;

public class PinchActions : MonoBehaviour
{
    public GameObject currentObject;
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

    [Header("Scaling Settings")]
    public float scaleSpeed = 1f;
    public float minScale = 0.1f;
    public float maxScale = 3f;

    private int scalingDirection = 0; // -1 = down, 1 = up, 0 = none
    private float scalingFactor = 0f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 90f; // degrees per second

    private Vector3 rotationAxis = Vector3.zero;
    private float rotationAmount = 0f;

    // Tracking last frame a pinch start was detected for every finger
    private int leftIndexLastPinchStartFrame = -1;
    private int rightIndexLastPinchStartFrame = -1;
    private int leftMiddleLastPinchStartFrame = -1;
    private int rightMiddleLastPinchStartFrame = -1;
    private int leftRingLastPinchStartFrame = -1;
    private int rightRingLastPinchStartFrame = -1;
    private int leftPinkyLastPinchStartFrame = -1;
    private int rightPinkyLastPinchStartFrame = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckLeftIndexPinch();
        CheckRightIndexPinch();
        CheckLeftMiddlePinch();
        CheckRightMiddlePinch();
        CheckLeftRingPinch();
        CheckRightRingPinch();
        CheckLeftPinkyPinch();
        CheckRightPinkyPinch();

        HandleScaling();
        HandleRotation();
        SetActivateDeactivateDistances();
    }

    void SetActivateDeactivateDistances()
    {
        LPinchDetectorIndex.activateDistance = pinchActivateDistance;
        LPinchDetectorIndex.deactivateDistance = pinchDeactivateDistance;
        RPinchDetectorIndex.activateDistance = pinchActivateDistance;
        RPinchDetectorIndex.deactivateDistance = pinchDeactivateDistance;
        LPinchDetectorMiddle.activateDistance = pinchActivateDistance;
        LPinchDetectorMiddle.deactivateDistance = pinchDeactivateDistance;
        RPinchDetectorMiddle.activateDistance = pinchActivateDistance;
        RPinchDetectorMiddle.deactivateDistance = pinchDeactivateDistance;
        LPinchDetectorRing.activateDistance = pinchActivateDistance;
        LPinchDetectorRing.deactivateDistance = pinchDeactivateDistance;
        RPinchDetectorRing.activateDistance = pinchActivateDistance;
        RPinchDetectorRing.deactivateDistance = pinchDeactivateDistance;
        LPinchDetectorPinky.activateDistance = pinchActivateDistance;
        LPinchDetectorPinky.deactivateDistance = pinchDeactivateDistance;
        RPinchDetectorPinky.activateDistance = pinchActivateDistance;
        RPinchDetectorPinky.deactivateDistance = pinchDeactivateDistance;
    }

    // --- Scaling ---
    void HandleScaling()
    {
        if (currentObject == null || scalingDirection == 0) return;

        float scaleChange = scalingDirection * scalingFactor * scaleSpeed * Time.deltaTime;
        Vector3 newScale = currentObject.transform.localScale + Vector3.one * scaleChange;

        float clamped = Mathf.Clamp(newScale.x, minScale, maxScale);
        newScale = new Vector3(clamped, clamped, clamped);

        currentObject.transform.localScale = newScale;
    }

    public void StartScaling(int direction, float factor)
    {
        scalingDirection = direction;
        scalingFactor = factor;
    }

    public void StopScaling()
    {
        scalingDirection = 0;
        scalingFactor = 0f;
    }

    // --- Rotation ---
    void HandleRotation()
    {
        if (currentObject == null || rotationAxis == Vector3.zero) return;

        float angle = rotationAmount * rotationSpeed * Time.deltaTime;
        currentObject.transform.Rotate(rotationAxis, angle, Space.Self);
    }

    public void StartRotation(Vector3 axis, float amount)
    {
        rotationAxis = axis;
        rotationAmount = amount;
    }

    public void StopRotation()
    {
        rotationAxis = Vector3.zero;
        rotationAmount = 0f;
    }

    // --- Pinch Checks ---

    void CheckLeftIndexPinch()
    {
        if (LPinchDetectorIndex == null) return;
        if (LPinchDetectorIndex.IsPinching)
        {
            StartScaling(-1, LPinchDetectorIndex.SquishPercent);
        }
        else if (scalingDirection == -1)
        {
            StopScaling();
        }
        if (LPinchDetectorIndex.PinchStartedThisFrame)
        {
            if (leftIndexLastPinchStartFrame == Time.frameCount)
            {
                Debug.Log("Consecutive Left Index pinch detected. Turning off scaling for Left Index.");
                StopScaling();
            }
            else
            {
                leftIndexLastPinchStartFrame = Time.frameCount;
                Debug.Log("Left Index pinch started this frame.");
            }
        }
    }

    void CheckRightIndexPinch()
    {
        if (RPinchDetectorIndex == null) return;
        if (RPinchDetectorIndex.IsPinching)
        {
            StartScaling(1, RPinchDetectorIndex.SquishPercent);
        }
        else if (scalingDirection == 1)
        {
            StopScaling();
        }
        if (RPinchDetectorIndex.PinchStartedThisFrame)
        {
            if (rightIndexLastPinchStartFrame == Time.frameCount)
            {
                Debug.Log("Consecutive Right Index pinch detected. Turning off scaling for Right Index.");
                StopScaling();
            }
            else
            {
                rightIndexLastPinchStartFrame = Time.frameCount;
                Debug.Log("Right Index pinch started this frame.");
            }
        }
    }

    void CheckLeftMiddlePinch()
    {
        if (LPinchDetectorMiddle == null) return;
        if (LPinchDetectorMiddle.IsPinching)
        {
            StartRotation(Vector3.right, LPinchDetectorMiddle.SquishPercent);
        }
        else if (rotationAxis == Vector3.right)
        {
            StopRotation();
        }
        if (LPinchDetectorMiddle.PinchStartedThisFrame)
        {
            if (leftMiddleLastPinchStartFrame == Time.frameCount)
            {
                Debug.Log("Consecutive Left Middle pinch detected. Turning off rotation for Left Middle.");
                StopRotation();
            }
            else
            {
                leftMiddleLastPinchStartFrame = Time.frameCount;
                Debug.Log("Left Middle pinch started this frame.");
            }
        }
    }

    void CheckRightMiddlePinch()
    {
        if (RPinchDetectorMiddle == null) return;
        if (RPinchDetectorMiddle.IsPinching)
        {
            StartRotation(Vector3.right, RPinchDetectorMiddle.SquishPercent);
        }
        else if (rotationAxis == Vector3.right)
        {
            StopRotation();
        }
        if (RPinchDetectorMiddle.PinchStartedThisFrame)
        {
            if (rightMiddleLastPinchStartFrame == Time.frameCount)
            {
                Debug.Log("Consecutive Right Middle pinch detected. Turning off rotation for Right Middle.");
                StopRotation();
            }
            else
            {
                rightMiddleLastPinchStartFrame = Time.frameCount;
                Debug.Log("Right Middle pinch started this frame.");
            }
        }
    }

    void CheckLeftRingPinch()
    {
        if (LPinchDetectorRing == null) return;
        if (LPinchDetectorRing.IsPinching)
        {
            StartRotation(Vector3.up, LPinchDetectorRing.SquishPercent);
        }
        else if (rotationAxis == Vector3.up)
        {
            StopRotation();
        }
        if (LPinchDetectorRing.PinchStartedThisFrame)
        {
            if (leftRingLastPinchStartFrame == Time.frameCount)
            {
                Debug.Log("Consecutive Left Ring pinch detected. Turning off rotation for Left Ring.");
                StopRotation();
            }
            else
            {
                leftRingLastPinchStartFrame = Time.frameCount;
                Debug.Log("Left Ring pinch started this frame.");
            }
        }
    }

    void CheckRightRingPinch()
    {
        if (RPinchDetectorRing == null) return;
        if (RPinchDetectorRing.IsPinching)
        {
            StartRotation(Vector3.up, RPinchDetectorRing.SquishPercent);
        }
        else if (rotationAxis == Vector3.up)
        {
            StopRotation();
        }
        if (RPinchDetectorRing.PinchStartedThisFrame)
        {
            if (rightRingLastPinchStartFrame == Time.frameCount)
            {
                Debug.Log("Consecutive Right Ring pinch detected. Turning off rotation for Right Ring.");
                StopRotation();
            }
            else
            {
                rightRingLastPinchStartFrame = Time.frameCount;
                Debug.Log("Right Ring pinch started this frame.");
            }
        }
    }

    void CheckLeftPinkyPinch()
    {
        if (LPinchDetectorPinky == null) return;
        if (LPinchDetectorPinky.IsPinching)
        {
            StartRotation(Vector3.forward, LPinchDetectorPinky.SquishPercent);
        }
        else if (rotationAxis == Vector3.forward)
        {
            StopRotation();
        }
        if (LPinchDetectorPinky.PinchStartedThisFrame)
        {
            if (leftPinkyLastPinchStartFrame == Time.frameCount)
            {
                Debug.Log("Consecutive Left Pinky pinch detected. Turning off rotation for Left Pinky.");
                StopRotation();
            }
            else
            {
                leftPinkyLastPinchStartFrame = Time.frameCount;
                Debug.Log("Left Pinky pinch started this frame.");
            }
        }
    }

    void CheckRightPinkyPinch()
    {
        if (RPinchDetectorPinky == null) return;
        if (RPinchDetectorPinky.IsPinching)
        {
            StartRotation(Vector3.forward, RPinchDetectorPinky.SquishPercent);
        }
        else if (rotationAxis == Vector3.forward)
        {
            StopRotation();
        }
        if (RPinchDetectorPinky.PinchStartedThisFrame)
        {
            if (rightPinkyLastPinchStartFrame == Time.frameCount)
            {
                Debug.Log("Consecutive Right Pinky pinch detected. Turning off rotation for Right Pinky.");
                StopRotation();
            }
            else
            {
                rightPinkyLastPinchStartFrame = Time.frameCount;
                Debug.Log("Right Pinky pinch started this frame.");
            }
        }
    }
}
