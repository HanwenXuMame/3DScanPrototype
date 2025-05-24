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
            Debug.Log("Left Index pinch started this frame.");
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
            Debug.Log("Right Index pinch started this frame.");
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
            Debug.Log("Left Middle pinch started this frame.");
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
            Debug.Log("Right Middle pinch started this frame.");
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
            Debug.Log("Left Ring pinch started this frame.");
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
            Debug.Log("Right Ring pinch started this frame.");
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
            Debug.Log("Left Pinky pinch started this frame.");
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
            Debug.Log("Right Pinky pinch started this frame.");
    }
}
